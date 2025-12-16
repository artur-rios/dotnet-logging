using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace ArturRios.Logging.Adapter;

/// <summary>
/// Extension methods for Microsoft.Extensions.Logging.ILogger that capture caller information.
/// </summary>
public static class LoggerCallerExtensions
{
    /// <param name="logger">The logger instance.</param>
    extension(ILogger logger)
    {
        /// <summary>
        /// Logs a trace-level message with automatic caller information capture.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="callerFilePath">The source file path (automatically captured).</param>
        /// <param name="callerMemberName">The calling method name (automatically captured).</param>
        public void LogTraceWithCaller(string message,
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerMemberName] string callerMemberName = "unknown")
            => LogWithCaller(logger, LogLevel.Trace, message, null, callerFilePath, callerMemberName);

        /// <summary>
        /// Logs a debug-level message with automatic caller information capture.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="callerFilePath">The source file path (automatically captured).</param>
        /// <param name="callerMemberName">The calling method name (automatically captured).</param>
        public void LogDebugWithCaller(string message,
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerMemberName] string callerMemberName = "unknown")
            => LogWithCaller(logger, LogLevel.Debug, message, null, callerFilePath, callerMemberName);

        /// <summary>
        /// Logs an information-level message with automatic caller information capture.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="callerFilePath">The source file path (automatically captured).</param>
        /// <param name="callerMemberName">The calling method name (automatically captured).</param>
        public void LogInformationWithCaller(string message,
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerMemberName] string callerMemberName = "unknown")
            => LogWithCaller(logger, LogLevel.Information, message, null, callerFilePath, callerMemberName);

        /// <summary>
        /// Logs a warning-level message with automatic caller information capture.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="callerFilePath">The source file path (automatically captured).</param>
        /// <param name="callerMemberName">The calling method name (automatically captured).</param>
        public void LogWarningWithCaller(string message,
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerMemberName] string callerMemberName = "unknown")
            => LogWithCaller(logger, LogLevel.Warning, message, null, callerFilePath, callerMemberName);

        /// <summary>
        /// Logs an error-level message with automatic caller information capture.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="callerFilePath">The source file path (automatically captured).</param>
        /// <param name="callerMemberName">The calling method name (automatically captured).</param>
        public void LogErrorWithCaller(string message,
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerMemberName] string callerMemberName = "unknown")
            => LogWithCaller(logger, LogLevel.Error, message, null, callerFilePath, callerMemberName);

        /// <summary>
        /// Logs a critical-level message with automatic caller information capture.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="callerFilePath">The source file path (automatically captured).</param>
        /// <param name="callerMemberName">The calling method name (automatically captured).</param>
        public void LogCriticalWithCaller(string message,
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerMemberName] string callerMemberName = "unknown")
            => LogWithCaller(logger, LogLevel.Critical, message, null, callerFilePath, callerMemberName);

        /// <summary>
        /// Logs an exception with automatic caller information capture.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Optional message to log (defaults to exception message).</param>
        /// <param name="callerFilePath">The source file path (automatically captured).</param>
        /// <param name="callerMemberName">The calling method name (automatically captured).</param>
        public void LogExceptionWithCaller(Exception exception, string? message = null,
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerMemberName] string callerMemberName = "unknown")
            => LogWithCaller(logger, LogLevel.Error, message ?? exception.Message, exception, callerFilePath, callerMemberName);
    }

    private static void LogWithCaller(ILogger logger, LogLevel level, string? message, Exception? exception,
        string callerFilePath, string callerMemberName)
    {
        var state = new[]
        {
            new KeyValuePair<string, object>("CallerFilePath", callerFilePath),
            new KeyValuePair<string, object>("CallerMemberName", callerMemberName),
            new KeyValuePair<string, object>("OriginalMessage", message ?? string.Empty)
        };

        logger.Log(level, new EventId(), state, exception, (_, ex) => message ?? ex?.Message ?? string.Empty);
    }
}
