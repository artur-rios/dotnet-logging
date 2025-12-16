using ArturRios.Logging.Configuration;
using ArturRios.Logging.Factories;
using ArturRios.Logging.Interfaces;

namespace ArturRios.Logging.Loggers;

public class FileLogger(FileLoggerConfiguration configuration) : IInternalLogger
{
    private const string DefaultLogFolder = "log";
    private const string FileExtension = ".log";
    private static readonly Lock s_fileLock = new();

    /// <inheritdoc />
    public void Trace(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Trace, filePath, methodName, message);
    }

    /// <inheritdoc />
    public void Debug(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Debug, filePath, methodName, message);
    }

    /// <inheritdoc />
    public void Info(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Information, filePath, methodName, message);
    }

    /// <inheritdoc />
    public void Warn(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Warning, filePath, methodName, message);
    }

    /// <inheritdoc />
    public void Error(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Error, filePath, methodName, message);
    }

    /// <inheritdoc />
    public void Exception(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Exception, filePath, methodName, message);
    }

    /// <inheritdoc />
    public void Critical(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Critical, filePath, methodName, message);
    }

    /// <inheritdoc />
    public void Fatal(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Fatal, filePath, methodName, message);
    }

    private void Write(CustomLogLevel level, string filePath, string methodName, string message)
    {
        var path = BuildFullPath();

        CreateDirectoryIfNotExists(path);

        lock (s_fileLock)
        {
            File.AppendAllText(path, LogEntryFactory.Create(level, filePath, methodName, message));
        }
    }

    private static void CreateDirectoryIfNotExists(string path)
    {
        var directory = Path.GetDirectoryName(path);

        if (directory is not null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private string BuildFullPath()
    {
        var timestamp = DateTime.UtcNow;
        var folderPath = BuildFolderPath(timestamp);
        var fileName = BuildFileName(timestamp);

        return Path.Combine(folderPath, $"{fileName}{FileExtension}");
    }

    private string BuildFolderPath(DateTime timestamp)
    {
        var basePath = configuration.FilePath ?? Path.Combine(AppContext.BaseDirectory, DefaultLogFolder);
        var path = basePath;

        switch (configuration.FolderScheme)
        {
            case LogFolderScheme.AllInOne:
                break;
            case LogFolderScheme.ByYear:
                path = Path.Combine(path, timestamp.Year.ToString());
                break;
            case LogFolderScheme.ByMonth:
                path = Path.Combine(path, timestamp.Year.ToString(), timestamp.Month.ToString("D2"));
                break;
            case LogFolderScheme.ByDay:
                path = Path.Combine(path, timestamp.Year.ToString(), timestamp.Month.ToString("D2"), timestamp.Day.ToString("D2"));
                break;
            case LogFolderScheme.ByHour:
                path = Path.Combine(path, timestamp.Year.ToString(), timestamp.Month.ToString("D2"), timestamp.Day.ToString("D2"), timestamp.Hour.ToString("D2"));
                break;
            case LogFolderScheme.ByRequest:
                var requestId = Guid.NewGuid().ToString();
                path = Path.Combine(path, requestId);
                break;
            default:
                throw new ArgumentOutOfRangeException(configuration.FolderScheme.ToString());
        }

        return path;
    }

    private string BuildFileName(DateTime timestamp)
    {
        return configuration.FileSplitLevel switch
        {
            LogSplitLevel.Request => configuration.ApplicationName,
            LogSplitLevel.Year => $"{configuration.ApplicationName}_{timestamp:yyyy}",
            LogSplitLevel.Month => $"{configuration.ApplicationName}_{timestamp:yyyy_MM}",
            LogSplitLevel.Day => $"{configuration.ApplicationName}_{timestamp:yyyy_MM_dd}",
            LogSplitLevel.Hour => $"{configuration.ApplicationName}_{timestamp:yyyy_MM_dd_HH}",
            _ => throw new ArgumentOutOfRangeException(configuration.FileSplitLevel.ToString())
        };
    }
}
