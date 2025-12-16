namespace ArturRios.Logging.Interfaces;

/// <summary>
/// Defines the internal contract for logging implementations that handle formatted log entries.
/// </summary>
public interface IInternalLogger
{
    /// <summary>
    /// Logs a trace-level message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="methodName">The calling method name.</param>
    void Trace(string message, string filePath, string methodName);

    /// <summary>
    /// Logs a debug-level message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="methodName">The calling method name.</param>
    void Debug(string message, string filePath, string methodName);

    /// <summary>
    /// Logs an information-level message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="methodName">The calling method name.</param>
    void Info(string message, string filePath, string methodName);

    /// <summary>
    /// Logs a warning-level message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="methodName">The calling method name.</param>
    void Warn(string message, string filePath, string methodName);

    /// <summary>
    /// Logs an error-level message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="methodName">The calling method name.</param>
    void Error(string message, string filePath, string methodName);

    /// <summary>
    /// Logs an exception message.
    /// </summary>
    /// <param name="message">The exception message to log.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="methodName">The calling method name.</param>
    void Exception(string message, string filePath, string methodName);

    /// <summary>
    /// Logs a critical-level message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="methodName">The calling method name.</param>
    void Critical(string message, string filePath, string methodName);

    /// <summary>
    /// Logs a fatal-level message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path.</param>
    /// <param name="methodName">The calling method name.</param>
    void Fatal(string message, string filePath, string methodName);
}
