using ArturRios.Logging.Configuration;

namespace ArturRios.Logging.Tests.Configuration;

public class ConsoleLoggerConfigurationTests
{
    [Fact]
    public void Should_UseColorsAsDefault()
    {
        var config = new ConsoleLoggerConfiguration();

        Assert.True(config.UseColors);
    }
}
