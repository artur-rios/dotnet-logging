using ArturRios.Logging.Configuration;

namespace ArturRios.Logging.Tests.Configuration;

public class LogFolderSchemeTests
{
    [Fact]
    public void Should_HaveExpectedNumericValues()
    {
        Assert.Equal(0, (int)LogFolderScheme.AllInOne);
        Assert.Equal(1, (int)LogFolderScheme.ByYear);
        Assert.Equal(2, (int)LogFolderScheme.ByMonth);
        Assert.Equal(3, (int)LogFolderScheme.ByDay);
        Assert.Equal(4, (int)LogFolderScheme.ByHour);
        Assert.Equal(5, (int)LogFolderScheme.ByRequest);
    }

    [Fact]
    public void Should_HaveExpectedNames()
    {
        Assert.Equal("AllInOne", nameof(LogFolderScheme.AllInOne));
        Assert.Equal("ByYear", nameof(LogFolderScheme.ByYear));
        Assert.Equal("ByMonth", nameof(LogFolderScheme.ByMonth));
        Assert.Equal("ByDay", nameof(LogFolderScheme.ByDay));
        Assert.Equal("ByHour", nameof(LogFolderScheme.ByHour));
        Assert.Equal("ByRequest", nameof(LogFolderScheme.ByRequest));
    }
}
