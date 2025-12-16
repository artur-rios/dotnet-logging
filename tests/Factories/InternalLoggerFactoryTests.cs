using ArturRios.Logging.Configuration;
using ArturRios.Logging.Factories;
using ArturRios.Logging.Loggers;

namespace ArturRios.Logging.Tests.Factories;

public class InternalLoggerFactoryTests
{
    [Fact]
    public void Should_ThrowArgumentNullException_WhenConfigurationIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => InternalLoggerFactory.Create(null!));
    }

    [Fact]
    public void Should_CreateConsoleLogger_WhenConsoleLoggerConfigurationIsProvided()
    {
        var config = new ConsoleLoggerConfiguration
        {
            UseColors = true
        };

        var logger = InternalLoggerFactory.Create(config);

        Assert.NotNull(logger);
        Assert.IsType<ConsoleLogger>(logger);
    }

    [Fact]
    public void Should_CreateConsoleLogger_WhenConsoleLoggerConfigurationWithColorsDisabled()
    {
        var config = new ConsoleLoggerConfiguration
        {
            UseColors = false
        };

        var logger = InternalLoggerFactory.Create(config);

        Assert.NotNull(logger);
        Assert.IsType<ConsoleLogger>(logger);
    }

    [Fact]
    public void Should_CreateFileLogger_WhenFileLoggerConfigurationIsProvided()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FolderScheme = LogFolderScheme.ByMonth,
            FileSplitLevel = LogSplitLevel.Day
        };

        var logger = InternalLoggerFactory.Create(config);

        Assert.NotNull(logger);
        Assert.IsType<FileLogger>(logger);
    }

    [Fact]
    public void Should_CreateFileLogger_WhenFileLoggerConfigurationWithCustomPath()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp",
            FilePath = "C:\\Logs",
            FolderScheme = LogFolderScheme.ByYear,
            FileSplitLevel = LogSplitLevel.Month
        };

        var logger = InternalLoggerFactory.Create(config);

        Assert.NotNull(logger);
        Assert.IsType<FileLogger>(logger);
    }

    private sealed class UnknownConfig : LoggerConfiguration;

    [Fact]
    public void Should_ThrowArgumentException_WhenUnsupportedConfigurationTypeIsProvided()
    {
        var config = new UnknownConfig();

        var exception = Assert.Throws<ArgumentException>(() => InternalLoggerFactory.Create(config));

        Assert.Contains("Unsupported logger configuration type", exception.Message);
        Assert.Contains(nameof(UnknownConfig), exception.Message);
    }
}
