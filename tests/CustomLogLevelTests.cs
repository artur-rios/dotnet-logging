using ArturRios.Extensions;

namespace ArturRios.Logging.Tests;

public class CustomLogLevelTests
{
    [Fact]
    public void Should_HaveExpectedNumericValues()
    {
        Assert.Equal(0, (int)CustomLogLevel.Trace);
        Assert.Equal(1, (int)CustomLogLevel.Debug);
        Assert.Equal(2, (int)CustomLogLevel.Information);
        Assert.Equal(3, (int)CustomLogLevel.Warning);
        Assert.Equal(4, (int)CustomLogLevel.Error);
        Assert.Equal(5, (int)CustomLogLevel.Exception);
        Assert.Equal(6, (int)CustomLogLevel.Critical);
        Assert.Equal(7, (int)CustomLogLevel.Fatal);
    }

    [Fact]
    public void Should_HaveExpectedNames()
    {
        Assert.Equal("Trace", nameof(CustomLogLevel.Trace));
        Assert.Equal("Debug", nameof(CustomLogLevel.Debug));
        Assert.Equal("Information", nameof(CustomLogLevel.Information));
        Assert.Equal("Warning", nameof(CustomLogLevel.Warning));
        Assert.Equal("Error", nameof(CustomLogLevel.Error));
        Assert.Equal("Exception", nameof(CustomLogLevel.Exception));
        Assert.Equal("Critical", nameof(CustomLogLevel.Critical));
        Assert.Equal("Fatal", nameof(CustomLogLevel.Fatal));
    }

    [Fact]
    public void Should_HaveExpectedDescriptions()
    {
        Assert.Equal("TRACE", CustomLogLevel.Trace.GetDescription());
        Assert.Equal("DEBUG", CustomLogLevel.Debug.GetDescription());
        Assert.Equal("INFO", CustomLogLevel.Information.GetDescription());
        Assert.Equal("WARN", CustomLogLevel.Warning.GetDescription());
        Assert.Equal("ERROR", CustomLogLevel.Error.GetDescription());
        Assert.Equal("EXCEPTION", CustomLogLevel.Exception.GetDescription());
        Assert.Equal("CRITICAL", CustomLogLevel.Critical.GetDescription());
        Assert.Equal("FATAL", CustomLogLevel.Fatal.GetDescription());
    }
}
