using ArturRios.Logging.Adapter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArturRios.Logging.Tests.Adapter;

public class MicrosoftLoggerBuilderExtensionsTests
{
    [Fact]
    public void Should_AddCustomLogger_RegisterLoggerProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddCustomLogger();
        });

        var serviceProvider = services.BuildServiceProvider();
        var loggerProvider = serviceProvider.GetService<ILoggerProvider>();

        Assert.NotNull(loggerProvider);
        Assert.IsType<MicrosoftLoggerProvider>(loggerProvider);
    }

    [Fact]
    public void Should_AddCustomLogger_RegisterAsSingleton()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddCustomLogger();
        });

        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ILoggerProvider));

        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
        Assert.Equal(typeof(MicrosoftLoggerProvider), serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void Should_AddCustomLogger_ReturnBuilderForChaining()
    {
        var services = new ServiceCollection();
        ILoggingBuilder? capturedBuilder = null;
        ILoggingBuilder? resultBuilder = null;

        services.AddLogging(builder =>
        {
            capturedBuilder = builder;
            resultBuilder = builder.AddCustomLogger();
        });

        Assert.NotNull(resultBuilder);
        Assert.Same(capturedBuilder, resultBuilder);
    }

    [Fact]
    public void Should_AddCustomLogger_ThrowArgumentNullException_WhenBuilderIsNull()
    {
        ILoggingBuilder? builder = null;

        Assert.Throws<ArgumentNullException>(() => builder!.AddCustomLogger());
    }

    [Fact]
    public void Should_AddCustomLogger_AllowMultipleCalls()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddCustomLogger();
            builder.AddCustomLogger();
        });

        var serviceProvider = services.BuildServiceProvider();
        var loggerProviders = serviceProvider.GetServices<ILoggerProvider>();

        Assert.True(loggerProviders.Any());
    }

    [Fact]
    public void Should_AddCustomLogger_IntegrateWithLoggingPipeline()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddCustomLogger();
        });

        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("TestCategory");

        Assert.NotNull(logger);
        Assert.True(logger.IsEnabled(LogLevel.Information));
    }

    [Fact]
    public void Should_AddCustomLogger_WorkWithOtherLoggerProviders()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddCustomLogger();
            builder.AddDebug();
        });

        var serviceProvider = services.BuildServiceProvider();
        var loggerProviders = serviceProvider.GetServices<ILoggerProvider>().ToList();

        Assert.NotEmpty(loggerProviders);
        Assert.Contains(loggerProviders, provider => provider is MicrosoftLoggerProvider);
    }

    [Fact]
    public void Should_AddCustomLogger_PreserveExistingServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<TestService>();

        services.AddLogging(builder =>
        {
            builder.AddCustomLogger();
        });

        var serviceProvider = services.BuildServiceProvider();
        var testService = serviceProvider.GetService<TestService>();

        Assert.NotNull(testService);
    }

    private class TestService;
}

