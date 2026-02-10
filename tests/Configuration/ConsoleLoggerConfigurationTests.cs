using ArturRios.Logging.Configuration;

namespace ArturRios.Logging.Tests.Configuration;

public class ConsoleLoggerConfigurationTests
{
    [Fact]
    public void GivenConsoleLoggerConfiguration_WhenCreated_ThenUsesColorsAsDefault()
    {
        var config = new ConsoleLoggerConfiguration();

        Assert.True(config.UseColors);
    }
}
