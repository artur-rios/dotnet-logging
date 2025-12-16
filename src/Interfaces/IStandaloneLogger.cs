using System.Runtime.CompilerServices;

namespace ArturRios.Logging.Interfaces;

/// <summary>
/// Defines the contract for a standalone logger with automatic caller information capture.
/// </summary>
public interface IStandaloneLogger
{
    /// <summary>
    /// Gets or sets the trace ID for correlating log entries.
    /// </summary>
    string? TraceId { get; set; }

    /// <summary>
    /// Logs a trace-level message with automatic caller information capture.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    void Trace(string message, [CallerFilePath] string filePath = "unknown", [CallerMemberName] string methodName = "unknown");

    /// <summary>
    /// Logs a debug-level message with automatic caller information capture.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    void Debug(string message, [CallerFilePath] string filePath = "unknown", [CallerMemberName] string methodName = "unknown");

    /// <summary>
    /// Logs an information-level message with automatic caller information capture.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    void Info(string message, [CallerFilePath] string filePath = "unknown", [CallerMemberName] string methodName = "unknown");

    /// <summary>
    /// Logs a warning-level message with automatic caller information capture.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    void Warn(string message, [CallerFilePath] string filePath = "unknown", [CallerMemberName] string methodName = "unknown");

    /// <summary>
    /// Logs an error-level message with automatic caller information capture.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    void Error(string message, [CallerFilePath] string filePath = "unknown", [CallerMemberName] string methodName = "unknown");

    /// <summary>
    /// Logs an exception with automatic caller information capture.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    void Exception(Exception exception, [CallerFilePath] string filePath = "unknown", [CallerMemberName] string methodName = "unknown");

    /// <summary>
    /// Logs a critical-level message with automatic caller information capture.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    void Critical(string message, [CallerFilePath] string filePath = "unknown", [CallerMemberName] string methodName = "unknown");

    /// <summary>
    /// Logs a fatal-level message with automatic caller information capture.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    void Fatal(string message, [CallerFilePath] string filePath = "unknown", [CallerMemberName] string methodName = "unknown");
}
