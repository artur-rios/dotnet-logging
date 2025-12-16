using ArturRios.Logging.Configuration;

namespace ArturRios.Logging.Tests.Configuration;

public class FileLoggerConfigurationTests
{
    [Fact]
    public void Should_SetDefaultValues()
    {
        var config = new FileLoggerConfiguration
        {
            ApplicationName = "TestApp"
        };

        Assert.Equal(LogFolderScheme.ByMonth, config.FolderScheme);
        Assert.Equal(LogSplitLevel.Day, config.FileSplitLevel);
        Assert.Null(config.FilePath);
    }
}
