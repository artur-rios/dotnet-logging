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
    /// Organize log files by request (creates a unique folder per request).
    /// </summary>
    ByRequest = 5
}
