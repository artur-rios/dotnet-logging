using System.Runtime.CompilerServices;
using ArturRios.Logging.Configuration;
using ArturRios.Logging.Factories;
using ArturRios.Logging.Interfaces;

namespace ArturRios.Logging;

/// <summary>
/// A standalone logger that automatically captures caller information using compiler attributes.
/// Supports multiple logger configurations and optional trace ID tracking.
/// </summary>
public class StandaloneLogger : IStandaloneLogger
{
    private readonly List<IInternalLogger> _loggers = [];

    /// <summary>
    /// Gets or sets the trace ID for correlating log entries across related operations.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StandaloneLogger"/> class with the specified configurations.
    /// </summary>
    /// <param name="configurations">The list of logger configurations to use.</param>
    public StandaloneLogger(List<LoggerConfiguration> configurations)
    {
        foreach (var config in configurations)
        {
            _loggers.Add(InternalLoggerFactory.Create(config));
        }
    }

    /// <summary>
    /// Logs a trace-level message with automatic caller information capture.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    public void Trace(string message, [CallerFilePath] string filePath = "unknown",
        [CallerMemberName] string methodName = "unknown")
    {
        foreach (var logger in _loggers)
        {
            logger.Trace(FormatMessageWithTraceId(message), filePath, methodName);
        }
    }

    /// <summary>
    /// Logs a debug-level message with automatic caller information capture.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    public void Debug(string message, [CallerFilePath] string filePath = "unknown",
        [CallerMemberName] string methodName = "unknown")
    {
        foreach (var logger in _loggers)
        {
            logger.Debug(FormatMessageWithTraceId(message), filePath, methodName);
        }
    }

    /// <summary>
    /// Logs an information-level message with automatic caller information capture.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    public void Info(string message, [CallerFilePath] string filePath = "unknown",
        [CallerMemberName] string methodName = "unknown")
    {
        foreach (var logger in _loggers)
        {
            logger.Info(FormatMessageWithTraceId(message), filePath, methodName);
        }
    }

    /// <summary>
    /// Logs a warning-level message with automatic caller information capture.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    public void Warn(string message, [CallerFilePath] string filePath = "unknown",
        [CallerMemberName] string methodName = "unknown")
    {
        foreach (var logger in _loggers)
        {
            logger.Warn(FormatMessageWithTraceId(message), filePath, methodName);
        }
    }

    /// <summary>
    /// Logs an error-level message with automatic caller information capture.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    public void Error(string message, [CallerFilePath] string filePath = "unknown",
        [CallerMemberName] string methodName = "unknown")
    {
        foreach (var logger in _loggers)
        {
            logger.Error(FormatMessageWithTraceId(message), filePath, methodName);
        }
    }

    /// <summary>
    /// Logs an exception with automatic caller information capture.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    public void Exception(Exception exception, [CallerFilePath] string filePath = "unknown",
        [CallerMemberName] string methodName = "unknown")
    {
        foreach (var logger in _loggers)
        {
            logger.Exception(FormatMessageWithTraceId(exception.Message), filePath, methodName);
        }
    }

    /// <summary>
    /// Logs a critical-level message with automatic caller information capture.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    public void Critical(string message, [CallerFilePath] string filePath = "unknown",
        [CallerMemberName] string methodName = "unknown")
    {
        foreach (var logger in _loggers)
        {
            logger.Critical(FormatMessageWithTraceId(message), filePath, methodName);
        }
    }

    /// <summary>
    /// Logs a fatal-level message with automatic caller information capture.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="filePath">The source file path (automatically captured).</param>
    /// <param name="methodName">The calling method name (automatically captured).</param>
    public void Fatal(string message, [CallerFilePath] string filePath = "unknown",
        [CallerMemberName] string methodName = "unknown")
    {
        foreach (var logger in _loggers)
        {
            logger.Fatal(FormatMessageWithTraceId(message), filePath, methodName);
        }
    }

    private string FormatMessageWithTraceId(string message)
    {
        return !string.IsNullOrEmpty(TraceId) ? $"[{nameof(TraceId)}] {TraceId} | {message}" : message;
    }
}
