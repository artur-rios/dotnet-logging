using ArturRios.Logging.Configuration;
using ArturRios.Logging.Interfaces;
using ArturRios.Logging.Loggers;

namespace ArturRios.Logging.Factories;

/// <summary>
/// Factory for creating internal logger instances based on configuration.
/// </summary>
public static class InternalLoggerFactory
{
    /// <summary>
    /// Creates an internal logger instance based on the provided configuration.
    /// </summary>
    /// <param name="loggerConfiguration">The logger configuration specifying the type and settings.</param>
    /// <returns>An instance of <see cref="IInternalLogger"/> configured according to the provided settings.</returns>
    /// <exception cref="ArgumentNullException">Thrown when loggerConfiguration is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the configuration type is not supported.</exception>
    public static IInternalLogger Create(LoggerConfiguration loggerConfiguration)
    {
        ArgumentNullException.ThrowIfNull(loggerConfiguration);

        return loggerConfiguration switch
        {
            ConsoleLoggerConfiguration consoleConfig => new ConsoleLogger(consoleConfig),
            FileLoggerConfiguration fileConfig => new FileLogger(fileConfig),
            _ => throw new ArgumentException($"Unsupported logger configuration type: {loggerConfiguration.GetType().FullName}", nameof(loggerConfiguration))
        };
    }
}
