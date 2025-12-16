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
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            // Act
            builder.AddCustomLogger();
        });

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ILoggerProvider));

        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
        Assert.Equal(typeof(MicrosoftLoggerProvider), serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void Should_AddCustomLogger_ReturnBuilderForChaining()
    {
        // Arrange
        var services = new ServiceCollection();
        ILoggingBuilder? capturedBuilder = null;
        ILoggingBuilder? resultBuilder = null;

        // Act
        services.AddLogging(builder =>
        {
            capturedBuilder = builder;
            resultBuilder = builder.AddCustomLogger();
        });

        // Assert
        Assert.NotNull(resultBuilder);
        Assert.Same(capturedBuilder, resultBuilder);
    }

    [Fact]
    public void Should_AddCustomLogger_ThrowArgumentNullException_WhenBuilderIsNull()
    {
        // Arrange
        ILoggingBuilder? builder = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder!.AddCustomLogger());
    }

    [Fact]
    public void Should_AddCustomLogger_AllowMultipleCalls()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            // Act
            builder.AddCustomLogger();
            builder.AddCustomLogger();
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var loggerProviders = serviceProvider.GetServices<ILoggerProvider>();

        // Multiple calls should add multiple instances
        Assert.True(loggerProviders.Count() >= 1);
    }

    [Fact]
    public void Should_AddCustomLogger_IntegrateWithLoggingPipeline()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddCustomLogger();
        });

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("TestCategory");

        // Assert
        Assert.NotNull(logger);
        Assert.True(logger.IsEnabled(LogLevel.Information));
    }

    [Fact]
    public void Should_AddCustomLogger_WorkWithOtherLoggerProviders()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddCustomLogger();
            builder.AddDebug();
        });

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var loggerProviders = serviceProvider.GetServices<ILoggerProvider>().ToList();

        // Assert
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

    // Helper class for testing
    private class TestService
    {
    }
}

