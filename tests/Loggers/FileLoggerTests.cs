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
    public void Should_ImplementIInternalLogger()
    {
        var logger = new FileLogger(_configuration);

        Assert.IsType<IInternalLogger>(logger, exactMatch: false);
    }

    [Fact]
    public void Should_AcceptFileLoggerConfigurationInConstructor()
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
    public void Should_HaveTraceMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Trace("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_HaveDebugMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Debug("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_HaveInfoMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Info("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_HaveWarnMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Warn("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_HaveErrorMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Error("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_HaveExceptionMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Exception("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_HaveCriticalMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Critical("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_HaveFatalMethod()
    {
        var logger = new FileLogger(_configuration);

        logger.Fatal("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_WriteTraceToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Trace("Trace message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Trace message", fileContent);
    }

    [Fact]
    public void Should_WriteDebugToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Debug("Debug message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Debug message", fileContent);
    }

    [Fact]
    public void Should_WriteInfoToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Info("Info message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Info message", fileContent);
    }

    [Fact]
    public void Should_WriteWarnToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Warn("Warning message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Warning message", fileContent);
    }

    [Fact]
    public void Should_WriteErrorToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Error("Error message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Error message", fileContent);
    }

    [Fact]
    public void Should_WriteExceptionToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Exception("Exception message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Exception message", fileContent);
    }

    [Fact]
    public void Should_WriteCriticalToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Critical("Critical message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Critical message", fileContent);
    }

    [Fact]
    public void Should_WriteFatalToFile()
    {
        var logger = new FileLogger(_configuration);
        logger.Fatal("Fatal message", "Program.cs", "Main");

        var logFiles = Directory.GetFiles(_testLogDirectory, "*.log", SearchOption.AllDirectories);
        Assert.NotEmpty(logFiles);

        var fileContent = File.ReadAllText(logFiles[0]);
        Assert.Contains("Fatal message", fileContent);
    }

    [Fact]
    public void Should_CreateLogDirectoryIfNotExists()
    {
        Assert.False(Directory.Exists(_testLogDirectory));

        var logger = new FileLogger(_configuration);
        logger.Info("Test message", "Program.cs", "Main");

        Assert.True(Directory.Exists(_testLogDirectory));
    }

    [Fact]
    public void Should_AppendToExistingLogFile()
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
    public void Should_SupportAllInOneFolderScheme()
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
    public void Should_SupportByYearFolderScheme()
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
    public void Should_SupportByMonthFolderScheme()
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
    public void Should_SupportByDayFolderScheme()
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
    public void Should_SupportByHourFolderScheme()
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
    public void Should_SupportByRequestFolderScheme()
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
    public void Should_SupportRequestFileSplitLevel()
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
    public void Should_SupportYearFileSplitLevel()
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
    public void Should_SupportMonthFileSplitLevel()
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
    public void Should_SupportDayFileSplitLevel()
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
    public void Should_SupportHourFileSplitLevel()
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
    public void Should_UseDefaultLogFolderWhenFilePathIsNull()
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
    public void Should_HandleMultipleConsecutiveLoggingCalls()
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
    public void Should_HandleEmptyMessage()
    {
        var logger = new FileLogger(_configuration);

        var exception = Record.Exception(() =>
            logger.Info(string.Empty, "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_AllowLoggingWithSpecialCharacters()
    {
        var logger = new FileLogger(_configuration);

        var exception = Record.Exception(() =>
            logger.Info("Message with special chars: !@#$%^&*()_+-=[]{}|;:',.<>?", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_AllowLoggingWithUnicodeCharacters()
    {
        var logger = new FileLogger(_configuration);

        var exception = Record.Exception(() =>
            logger.Info("Message with unicode: 你好世界 مرحبا العالم 🎉", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_AllowLoggingWithLongMessage()
    {
        var logger = new FileLogger(_configuration);
        var longMessage = string.Concat(Enumerable.Repeat("This is a very long message. ", 100));

        var exception = Record.Exception(() =>
            logger.Info(longMessage, "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_BeThreadSafe()
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


