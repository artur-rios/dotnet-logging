using System.ComponentModel;

namespace ArturRios.Logging;

/// <summary>
/// Defines the severity levels for logging messages.
/// </summary>
public enum CustomLogLevel
{
    /// <summary>
    /// Trace level - most detailed logging for diagnostic purposes.
    /// </summary>
    [Description("TRACE")]
    Trace = 0,

    /// <summary>
    /// Debug level - used for debugging and development purposes.
    /// </summary>
    [Description("DEBUG")]
    Debug = 1,

    /// <summary>
    /// Information level - general informational messages.
    /// </summary>
    [Description("INFO")]
    Information = 2,

    /// <summary>
    /// Warning level - indicates a potential issue or unexpected behavior.
    /// </summary>
    [Description("WARN")]
    Warning = 3,

    /// <summary>
    /// Error level - indicates an error that occurred during execution.
    /// </summary>
    [Description("ERROR")]
    Error = 4,

    /// <summary>
    /// Exception level - indicates an exception was thrown.
    /// </summary>
    [Description("EXCEPTION")]
    Exception = 5,

    /// <summary>
    /// Critical level - indicates a critical error that requires immediate attention.
    /// </summary>
    [Description("CRITICAL")]
    Critical = 6,

    /// <summary>
    /// Fatal level - indicates a fatal error that terminates the application.
    /// </summary>
    [Description("FATAL")]
    Fatal = 7
}
