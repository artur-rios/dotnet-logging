using ArturRios.Logging.Configuration;

namespace ArturRios.Logging.Tests.Configuration;

public class LoggerTypeTests
{
    [Fact]
    public void Should_HaveExpectedNumericValues()
    {
        Assert.Equal(0, (int)LoggerType.Console);
        Assert.Equal(1, (int)LoggerType.File);
    }

    [Fact]
    public void Should_HaveExpectedNames()
    {
        Assert.Equal("Console", nameof(LoggerType.Console));
        Assert.Equal("File", nameof(LoggerType.File));
    }
}


