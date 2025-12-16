using ArturRios.Logging.Configuration;
using ArturRios.Logging.Factories;
using ArturRios.Logging.Interfaces;

namespace ArturRios.Logging;

/// <summary>
/// A logger that extracts caller information from state objects passed with log messages.
/// Useful for integration with dependency injection frameworks and structured logging.
/// </summary>
public class StateLogger : IStateLogger
{
    private readonly List<IInternalLogger> _loggers = [];

    /// <summary>
    /// Gets or sets the trace ID for correlating log entries across related operations.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StateLogger"/> class with the specified configurations.
    /// </summary>
    /// <param name="configurations">The list of logger configurations to use.</param>
    public StateLogger(List<LoggerConfiguration> configurations)
    {
        foreach (var config in configurations)
        {
            _loggers.Add(InternalLoggerFactory.Create(config));
        }
    }

    /// <summary>
    /// Logs a trace-level message with caller information extracted from state.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    public void Trace(string message, object? state = null)
    {
        var (fp, mn) = ResolveCallerInfo(state);
        foreach (var logger in _loggers)
        {
            logger.Trace(FormatMessageWithTraceId(message), fp, mn);
        }
    }

    /// <summary>
    /// Logs a debug-level message with caller information extracted from state.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    public void Debug(string message, object? state = null)
    {
        var (fp, mn) = ResolveCallerInfo(state);
        foreach (var logger in _loggers)
        {
            logger.Debug(FormatMessageWithTraceId(message), fp, mn);
        }
    }

    /// <summary>
    /// Logs an information-level message with caller information extracted from state.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    public void Info(string message, object? state = null)
    {
        var (fp, mn) = ResolveCallerInfo(state);
        foreach (var logger in _loggers)
        {
            logger.Info(FormatMessageWithTraceId(message), fp, mn);
        }
    }

    /// <summary>
    /// Logs a warning-level message with caller information extracted from state.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    public void Warn(string message, object? state = null)
    {
        var (fp, mn) = ResolveCallerInfo(state);
        foreach (var logger in _loggers)
        {
            logger.Warn(FormatMessageWithTraceId(message), fp, mn);
        }
    }

    /// <summary>
    /// Logs an error-level message with caller information extracted from state.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    public void Error(string message, object? state = null)
    {
        var (fp, mn) = ResolveCallerInfo(state);
        foreach (var logger in _loggers)
        {
            logger.Error(FormatMessageWithTraceId(message), fp, mn);
        }
    }

    /// <summary>
    /// Logs an exception with caller information extracted from state.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    public void Exception(Exception exception, object? state = null)
    {
        var (fp, mn) = ResolveCallerInfo(state);
        var msg = FormatMessageWithTraceId(exception.ToString() ?? exception.Message);
        foreach (var logger in _loggers)
        {
            logger.Exception(msg, fp, mn);
        }
    }

    /// <summary>
    /// Logs a critical-level message with caller information extracted from state.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    public void Critical(string message, object? state = null)
    {
        var (fp, mn) = ResolveCallerInfo(state);
        foreach (var logger in _loggers)
        {
            logger.Critical(FormatMessageWithTraceId(message), fp, mn);
        }
    }

    /// <summary>
    /// Logs a fatal-level message with caller information extracted from state.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    public void Fatal(string message, object? state = null)
    {
        var (fp, mn) = ResolveCallerInfo(state);
        foreach (var logger in _loggers)
        {
            logger.Fatal(FormatMessageWithTraceId(message), fp, mn);
        }
    }

    private static (string filePath, string methodName) ResolveCallerInfo(object? state)
    {
        string? filePath = null;
        string? methodName = null;

        if (state is not IEnumerable<KeyValuePair<string, object>> pairs)
        {
            return (filePath ?? "unknown", methodName ?? "unknown");
        }

        foreach (var (key, value) in pairs)
        {
            if (value is null)
            {
                continue;
            }

            if (filePath == null && (string.Equals(key, "CallerFilePath", StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(key, "FilePath", StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(key, "callerFilePath", StringComparison.OrdinalIgnoreCase)))
            {
                filePath = value.ToString();
            }

            if (methodName == null && (string.Equals(key, "CallerMemberName", StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(key, "MemberName", StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(key, "Method", StringComparison.OrdinalIgnoreCase) ||
                                       string.Equals(key, "callerMemberName", StringComparison.OrdinalIgnoreCase)))
            {
                methodName = value.ToString();
            }

            if (filePath != null && methodName != null) break;
        }

        return (filePath ?? "unknown", methodName ?? "unknown");
    }

    private string FormatMessageWithTraceId(string message)
    {
        return !string.IsNullOrEmpty(TraceId) ? $"[{nameof(TraceId)}] {TraceId} | {message}" : message;
    }
}
