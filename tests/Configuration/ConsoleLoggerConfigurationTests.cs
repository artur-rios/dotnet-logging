using ArturRios.Logging.Configuration;

namespace ArturRios.Logging.Tests.Configuration;

[Trait("Category", "Unit")]
public class ConsoleLoggerConfigurationTests
{
    [Fact]
    public void GivenConsoleLoggerConfiguration_WhenCreated_ThenUsesColorsAsDefault()
    {
        var config = new ConsoleLoggerConfiguration();

        Assert.True(config.UseColors);
    }
}
