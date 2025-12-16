namespace ArturRios.Logging.Configuration;

/// <summary>
/// Defines the level at which log files should be split into separate files.
/// </summary>
public enum LogSplitLevel
{
    /// <summary>
    /// Split log files per request.
    /// </summary>
    Request = 0,

    /// <summary>
    /// Split log files per hour.
    /// </summary>
    Hour,

    /// <summary>
    /// Split log files per day.
    /// </summary>
    Day,

    /// <summary>
    /// Split log files per month.
    /// </summary>
    Month,

    /// <summary>
    /// Split log files per year.
    /// </summary>
    Year
}
