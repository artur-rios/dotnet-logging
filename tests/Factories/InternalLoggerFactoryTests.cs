using ArturRios.Logging.Configuration;
using ArturRios.Logging.Factories;

namespace ArturRios.Logging.Tests.Factories;

public class InternalLoggerFactoryTests
{
    [Fact]
    public void Should_ThrowOnNullConfig()
    {
        Assert.Throws<ArgumentNullException>(() => InternalLoggerFactory.Create(null!));
    }

    private sealed class UnknownConfig : LoggerConfiguration { }

    [Fact]
    public void Should_ThrowOnUnsupportedConfig()
    {
        Assert.Throws<ArgumentException>(() => InternalLoggerFactory.Create(new UnknownConfig()));
    }
}
