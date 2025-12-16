namespace ArturRios.Logging.Configuration;

/// <summary>
/// Configuration settings for file-based logger.
/// </summary>
public class FileLoggerConfiguration : LoggerConfiguration
{
    /// <summary>
    /// Gets or sets the application name used in log file names.
    /// </summary>
    public required string ApplicationName { get; set; }

    /// <summary>
    /// Gets or sets the folder scheme for organizing log files.
    /// </summary>
    public LogFolderScheme FolderScheme { get; set; } = LogFolderScheme.ByMonth;

    /// <summary>
    /// Gets or sets the level at which to split log files.
    /// </summary>
    public LogSplitLevel FileSplitLevel { get; set; } = LogSplitLevel.Day;

    /// <summary>
    /// Gets or sets the base file path for log files. If null, defaults to a 'log' folder in the application directory.
    /// </summary>
    public string? FilePath { get; set; }
}
