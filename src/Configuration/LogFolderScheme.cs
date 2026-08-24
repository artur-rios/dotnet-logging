namespace ArturRios.Logging.Configuration;

/// <summary>
/// Defines the folder structure scheme for organizing log files.
/// </summary>
public enum LogFolderScheme
{
    /// <summary>
    /// All log files in a single folder.
    /// </summary>
    AllInOne = 0,

    /// <summary>
    /// Organize log files by year.
    /// </summary>
    ByYear = 1,

    /// <summary>
    /// Organize log files by year and month.
    /// </summary>
    ByMonth = 2,

    /// <summary>
    /// Organize log files by year, month, and day.
    /// </summary>
    ByDay = 3,

    /// <summary>
    /// Organize log files by year, month, day, and hour.
    /// </summary>
    ByHour = 4,

    /// <summary>
    /// Organize log files into one folder per logger instance.
    /// </summary>
    /// <remarks>
    /// The folder name is generated once per <c>FileLogger</c>, so registering the logger with a scoped
    /// lifetime gives one folder per request. A singleton logger gives one folder per process.
    /// </remarks>
    ByRequest = 5
}
