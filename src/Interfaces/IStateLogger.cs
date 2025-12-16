namespace ArturRios.Logging.Interfaces;

/// <summary>
/// Defines the contract for a logger that extracts caller information from state objects.
/// </summary>
public interface IStateLogger
{
    /// <summary>
    /// Gets or sets the trace ID for correlating log entries.
    /// </summary>
    string? TraceId { get; set; }

    /// <summary>
    /// Logs a trace-level message with caller information from state.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    void Trace(string message, object? state = null);

    /// <summary>
    /// Logs a debug-level message with caller information from state.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    void Debug(string message, object? state = null);

    /// <summary>
    /// Logs an information-level message with caller information from state.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    void Info(string message, object? state = null);

    /// <summary>
    /// Logs a warning-level message with caller information from state.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    void Warn(string message, object? state = null);

    /// <summary>
    /// Logs an error-level message with caller information from state.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    void Error(string message, object? state = null);

    /// <summary>
    /// Logs an exception with caller information from state.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    void Exception(Exception exception, object? state = null);

    /// <summary>
    /// Logs a critical-level message with caller information from state.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    void Critical(string message, object? state = null);

    /// <summary>
    /// Logs a fatal-level message with caller information from state.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="state">Optional state object containing caller information.</param>
    void Fatal(string message, object? state = null);
}
