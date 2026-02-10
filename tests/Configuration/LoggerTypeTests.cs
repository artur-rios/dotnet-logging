using ArturRios.Logging.Configuration;

namespace ArturRios.Logging.Tests.Configuration;

public class LoggerTypeTests
{
    [Fact]
    public void GivenLoggerTypeEnum_WhenInspected_ThenHasExpectedNumericValues()
    {
        Assert.Equal(0, (int)LoggerType.Console);
        Assert.Equal(1, (int)LoggerType.File);
    }

    [Fact]
    public void GivenLoggerTypeEnum_WhenInspected_ThenHasExpectedNames()
    {
        Assert.Equal("Console", nameof(LoggerType.Console));
        Assert.Equal("File", nameof(LoggerType.File));
    }
}


