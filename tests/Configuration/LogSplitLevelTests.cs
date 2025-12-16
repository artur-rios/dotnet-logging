using ArturRios.Logging.Configuration;

namespace ArturRios.Logging.Tests.Configuration;

public class LogSplitLevelTests
{
    [Fact]
    public void Should_HaveExpectedNumericValues()
    {
        Assert.Equal(0, (int)LogSplitLevel.Request);
        Assert.Equal(1, (int)LogSplitLevel.Hour);
        Assert.Equal(2, (int)LogSplitLevel.Day);
        Assert.Equal(3, (int)LogSplitLevel.Month);
        Assert.Equal(4, (int)LogSplitLevel.Year);
    }

    [Fact]
    public void Should_HaveExpectedNames()
    {
        Assert.Equal("Request", nameof(LogSplitLevel.Request));
        Assert.Equal("Hour", nameof(LogSplitLevel.Hour));
        Assert.Equal("Day", nameof(LogSplitLevel.Day));
        Assert.Equal("Month", nameof(LogSplitLevel.Month));
        Assert.Equal("Year", nameof(LogSplitLevel.Year));
    }
}

