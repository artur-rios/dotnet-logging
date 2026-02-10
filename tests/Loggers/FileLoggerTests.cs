using ArturRios.Logging.Configuration;
using ArturRios.Logging.Interfaces;
using ArturRios.Logging.Loggers;

namespace ArturRios.Logging.Tests.Loggers;

public class FileLoggerTests : IDisposable
{
    private readonly string _testLogDirectory;
    private readonly FileLoggerConfiguration _configuration;

    public FileLoggerTests()
    {
        _testLogDirectory = Path.Combine(Path.GetTempPath(), $"ArturRios.Logging.Tests_{Guid.NewGuid()}");
        _configuration = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = _testLogDirectory,
            FolderScheme = LogFolderScheme.AllInOne,
            FileSplitLevel = LogSplitLevel.Day
        };
    }

    public void Dispose()
    {
        if (Directory.Exists(_testLogDirectory))
        {
            Directory.Delete(_testLogDirectory, true);
        }

        GC.SuppressFinalize(this);
    }

    [Fact]
    public void GivenFileLogger_WhenCreated_ThenImplementsIInternalLogger()
    {
        var logger = new FileLogger(_configuration);

        Assert.IsType<IInternalLogger>(logger, exactMatch: false);
    }

    [Fact]
    public void GivenFileLoggerConfiguration_WhenFileLoggerCreated_ThenAcceptsConfiguration()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = _testLogDirectory
        };
        var logger = new FileLogger(config);

        Assert.NotNull(logger);
    }

    [Fact]
    public void GivenFileLogger_WhenInspected_ThenHasTraceMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Trace("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenFileLogger_WhenInspected_ThenHasDebugMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Debug("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenFileLogger_WhenInspected_ThenHasInfoMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Info("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenFileLogger_WhenInspected_ThenHasWarnMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Warn("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenFileLogger_WhenInspected_ThenHasErrorMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Error("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenFileLogger_WhenInspected_ThenHasExceptionMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Exception("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenFileLogger_WhenInspected_ThenHasCriticalMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Critical("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenFileLogger_WhenInspected_ThenHasFatalMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Fatal("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenFileLogger_WhenTraceLogged_ThenWritesToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Trace("Trace message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Trace message", fileContent);
    }

    [Fact]
    public void GivenFileLogger_WhenDebugLogged_ThenWritesToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Debug("Debug message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Debug message", fileContent);
    }

    [Fact]
    public void GivenFileLogger_WhenInfoLogged_ThenWritesToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Info("Info message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Info message", fileContent);
    }

    [Fact]
    public void GivenFileLogger_WhenWarnLogged_ThenWritesToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Warn("Warning message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Warning message", fileContent);
    }

    [Fact]
    public void GivenFileLogger_WhenErrorLogged_ThenWritesToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Error("Error message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Error message", fileContent);
    }

    [Fact]
    public void GivenFileLogger_WhenExceptionLogged_ThenWritesToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Exception("Exception message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Exception message", fileContent);
    }

    [Fact]
    public void GivenFileLogger_WhenCriticalLogged_ThenWritesToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Critical("Critical message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Critical message", fileContent);
    }

    [Fact]
    public void GivenFileLogger_WhenFatalLogged_ThenWritesToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Fatal("Fatal message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Fatal message", fileContent);
    }

    [Fact]
    public void GivenFileLoggerWithNonExistentDirectory_WhenInfoLogged_ThenCreatesLogDirectory()
    {
        Assert.False(Directory.Exists(_testLogDirectory));

        var logger = new FileLogger(_configuration);
        logger.Info("Test message", "Program.cs", "Main");

        Assert.True(Directory.Exists(_testLogDirectory));
    }

    [Fact]
    public void GivenFileLoggerWithExistingLog_WhenMultipleMessagesLogged_ThenAppendsToExistingLogFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Info("First message", "Program.cs", "Main");
        logger.Info("Second message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("First message", fileContent);
        Assert.Contains("Second message", fileContent);
    }

    [Fact]
    public void GivenFileLoggerWithAllInOneFolderScheme_WhenInfoLogged_ThenSupportsAllInOneFolderScheme()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = _testLogDirectory,
            FolderScheme = LogFolderScheme.AllInOne,
            FileSplitLevel = LogSplitLevel.Day
        };
        var logger = new FileLogger(config);

        logger.Info("Test message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);

        Assert.NotEmpty(logFiles);

        var hasSubdirectories = Directory.GetDirectories(_testLogDirectory).Length > 0;

        Assert.False(hasSubdirectories);
    }

    [Fact]
    public void GivenFileLoggerWithByYearFolderScheme_WhenInfoLogged_ThenSupportsByYearFolderScheme()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = _testLogDirectory,
            FolderScheme = LogFolderScheme.ByYear,
            FileSplitLevel = LogSplitLevel.Day
        };
        var logger = new FileLogger(config);

        logger.Info("Test message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var yearDir = Directory.GetDirectories(_testLogDirectory);
        Assert.NotEmpty(yearDir);
    }

    [Fact]
    public void GivenFileLoggerWithByMonthFolderScheme_WhenInfoLogged_ThenSupportsByMonthFolderScheme()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = _testLogDirectory,
            FolderScheme = LogFolderScheme.ByMonth,
            FileSplitLevel = LogSplitLevel.Day
        };
        var logger = new FileLogger(config);

        logger.Info("Test message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var subDirs = Directory.GetDirectories(_testLogDirectory, "*", SearchOption.AllDirectories);
        Assert.NotEmpty(subDirs);
    }

    [Fact]
    public void GivenFileLoggerWithByDayFolderScheme_WhenInfoLogged_ThenSupportsByDayFolderScheme()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = _testLogDirectory,
            FolderScheme = LogFolderScheme.ByDay,
            FileSplitLevel = LogSplitLevel.Day
        };
        var logger = new FileLogger(config);

        logger.Info("Test message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var subDirs = Directory.GetDirectories(_testLogDirectory, "*", SearchOption.AllDirectories);
        Assert.NotEmpty(subDirs);
    }

    [Fact]
    public void GivenFileLoggerWithByHourFolderScheme_WhenInfoLogged_ThenSupportsByHourFolderScheme()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = _testLogDirectory,
            FolderScheme = LogFolderScheme.ByHour,
            FileSplitLevel = LogSplitLevel.Day
        };
        var logger = new FileLogger(config);

        logger.Info("Test message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var subDirs = Directory.GetDirectories(_testLogDirectory, "*", SearchOption.AllDirectories);
        Assert.NotEmpty(subDirs);
    }

    [Fact]
    public void GivenFileLoggerWithByRequestFolderScheme_WhenMultipleMessagesLogged_ThenSupportsByRequestFolderScheme()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = _testLogDirectory,
            FolderScheme = LogFolderScheme.ByRequest,
            FileSplitLevel = LogSplitLevel.Day
        };
        var logger = new FileLogger(config);

        logger.Info("Test message 1", "Program.cs", "Main");
        logger.Info("Test message 2", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);
    }

    [Fact]
    public void GivenFileLoggerWithRequestFileSplitLevel_WhenInfoLogged_ThenSupportsRequestFileSplitLevel()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = _testLogDirectory,
            FolderScheme = LogFolderScheme.AllInOne,
            FileSplitLevel = LogSplitLevel.Request
        };
        var logger = new FileLogger(config);

        logger.Info("Test message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);

        Assert.NotEmpty(logFiles);
        Assert.EndsWith("TestApp.log", logFiles[0]);
    }

    [Fact]
    public void GivenFileLoggerWithYearFileSplitLevel_WhenInfoLogged_ThenSupportsYearFileSplitLevel()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = _testLogDirectory,
            FolderScheme = LogFolderScheme.AllInOne,
            FileSplitLevel = LogSplitLevel.Year
        };
        var logger = new FileLogger(config);

        logger.Info("Test message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var filename = Path.GetFileName(logFiles[0]);
        Assert.Contains("TestApp_", filename);
    }

    [Fact]
    public void GivenFileLoggerWithMonthFileSplitLevel_WhenInfoLogged_ThenSupportsMonthFileSplitLevel()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = _testLogDirectory,
            FolderScheme = LogFolderScheme.AllInOne,
            FileSplitLevel = LogSplitLevel.Month
        };
        var logger = new FileLogger(config);

        logger.Info("Test message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var filename = Path.GetFileName(logFiles[0]);
        Assert.Contains("TestApp_", filename);
    }

    [Fact]
    public void GivenFileLoggerWithDayFileSplitLevel_WhenInfoLogged_ThenSupportsDayFileSplitLevel()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = _testLogDirectory,
            FolderScheme = LogFolderScheme.AllInOne,
            FileSplitLevel = LogSplitLevel.Day
        };
        var logger = new FileLogger(config);

        logger.Info("Test message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var filename = Path.GetFileName(logFiles[0]);
        Assert.Contains("TestApp_", filename);
    }

    [Fact]
    public void GivenFileLoggerWithHourFileSplitLevel_WhenInfoLogged_ThenSupportsHourFileSplitLevel()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = _testLogDirectory,
            FolderScheme = LogFolderScheme.AllInOne,
            FileSplitLevel = LogSplitLevel.Hour
        };
        var logger = new FileLogger(config);

        logger.Info("Test message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var filename = Path.GetFileName(logFiles[0]);
        Assert.Contains("TestApp_", filename);
    }

    [Fact]
    public void GivenFileLoggerWithNullFilePath_WhenInfoLogged_ThenUsesDefaultLogFolder()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = null,
            FolderScheme = LogFolderScheme.AllInOne,
            FileSplitLevel = LogSplitLevel.Day
        };
        var logger = new FileLogger(config);

        logger.Info("Test message", "Program.cs", "Main");

        var defaultLogPath = Path.Combine(AppContext.BaseDirectory, "log");
        var logFiles = Directory.GetFiles(defaultLogPath, "*.log", SearchOption.AllDirectories);

        Assert.NotEmpty(logFiles);

        if (Directory.Exists(defaultLogPath))
        {
            Directory.Delete(defaultLogPath, true);
        }
    }

    [Fact]
    public void GivenFileLogger_WhenMultipleConsecutiveCallsMade_ThenHandlesMultipleConsecutiveLoggingCalls()
    {
        var logger = new FileLogger(_configuration);

        var exception = Record.Exception(() =>
        {
            logger.Trace("First message", "File1.cs", "Method1");
            logger.Debug("Second message", "File2.cs", "Method2");
            logger.Info("Third message", "File3.cs", "Method3");
            logger.Error("Fourth message", "File4.cs", "Method4");
        });

        Assert.Null(exception);
    }

    [Fact]
    public void GivenFileLoggerWithEmptyMessage_WhenInfoLogged_ThenHandlesEmptyMessage()
    {
        var logger = new FileLogger(_configuration);

        var exception = Record.Exception(() =>
            logger.Info(string.Empty, "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenFileLoggerWithSpecialCharacters_WhenInfoLogged_ThenAllowsLoggingWithSpecialCharacters()
    {
        var logger = new FileLogger(_configuration);

        var exception = Record.Exception(() =>
            logger.Info("Message with special chars: !@#$%^&*()_+-=[]{}|;:',.<>?", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenFileLoggerWithUnicodeCharacters_WhenInfoLogged_ThenAllowsLoggingWithUnicodeCharacters()
    {
        var logger = new FileLogger(_configuration);

        var exception = Record.Exception(() =>
            logger.Info("Message with unicode: 你好世界 مرحبا العالم 🎉", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenFileLoggerWithLongMessage_WhenInfoLogged_ThenAllowsLoggingWithLongMessage()
    {
        var logger = new FileLogger(_configuration);
        var longMessage = string.Concat(Enumerable.Repeat("This is a very long message. ", 100));

        var exception = Record.Exception(() =>
            logger.Info(longMessage, "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenFileLoggerWithConcurrentCalls_WhenMultipleThreadsLog_ThenIsThreadSafe()
    {
        var logger = new FileLogger(_configuration);
        var tasks = new List<Task>();

        for (var i = 0; i < 10; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() =>
                logger.Info($"Message {index}", "Program.cs", "Main")
            ));
        }

        var exception = Record.Exception(() => Task.WaitAll(tasks.ToArray()));
        Assert.Null(exception);
    }
}


