using ArturRios.Extensions;

namespace ArturRios.Logging.Factories;

/// <summary>
/// Factory for creating formatted log entry strings.
/// </summary>
public static class LogEntryFactory
{
    /// <summary>
    /// Creates a formatted log entry string with timestamp, log level, class name, method name, and message.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="methodName">The calling method name.</param>
    /// <param name="message">The log message.</param>
    /// <returns>A formatted log entry string.</returns>
    public static string Create(CustomLogLevel level, string filePath, string methodName, string message)
        => Create(level, filePath, methodName, message, DateTime.UtcNow);

    /// <summary>
    /// Creates a formatted log entry stamped with <paramref name="timestamp"/>, so a caller that also derives a file
    /// name from the time can use the same instant for both.
    /// </summary>
    internal static string Create(CustomLogLevel level, string filePath, string methodName, string message,
        DateTime timestamp)
    {
        var logLevel = level.GetDescription()!;
        var className = Path.GetFileNameWithoutExtension(filePath);

        return $"{logLevel}: {className} | {methodName} | {timestamp:o} | {message}{Environment.NewLine}";
    }
}
