namespace ArturRios.Logging.Configuration;

/// <summary>
/// Configuration settings for console logger.
/// </summary>
public class ConsoleLoggerConfiguration : LoggerConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether to use ANSI colors for log output.
    /// </summary>
    public bool UseColors { get; set; } = true;
}
