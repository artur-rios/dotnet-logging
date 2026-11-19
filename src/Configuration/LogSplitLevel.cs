namespace ArturRios.Logging.Configuration;

/// <summary>
/// Defines the level at which log files should be split into separate files.
/// </summary>
public enum LogSplitLevel
{
    /// <summary>
    /// Do not split by time: every entry goes to one file named after the application, such as
    /// <c>MyApp.log</c>. Combined with <see cref="LogFolderScheme.ByRequest"/>, the folder, not the file, changes
    /// per logger instance.
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
