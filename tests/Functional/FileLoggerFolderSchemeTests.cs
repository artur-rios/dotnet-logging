using ArturRios.Logging.Configuration;
using ArturRios.Logging.Loggers;

namespace ArturRios.Logging.Tests.Functional;

/// <summary>
/// Drives <see cref="FileLogger"/> against a real directory on disk and inspects what actually lands there,
/// for every folder scheme and split level.
/// </summary>
[Trait("Category", "Functional")]
public sealed class FileLoggerFolderSchemeTests : IDisposable
{
    private readonly string _root =
        Path.Combine(Path.GetTempPath(), "ArturRios.Logging.Functional_" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }

    private FileLoggerConfiguration Configuration(LogFolderScheme scheme, LogSplitLevel split = LogSplitLevel.Day) =>
        new()
        {
            ApplicationName = "TestApp",
            FilePath = _root,
            FolderScheme = scheme,
            FileSplitLevel = split
        };

    private string[] LogFiles() =>
        Directory.Exists(_root) ? Directory.GetFiles(_root, "*.log", SearchOption.AllDirectories) : [];

    [Fact]
    public void GivenTheByRequestScheme_WhenOneLoggerWritesManyEntries_ThenTheyAllShareOneFolder()
    {
        var logger = new FileLogger(Configuration(LogFolderScheme.ByRequest));

        logger.Info("first", "Caller.cs", "Method");
        logger.Info("second", "Caller.cs", "Method");
        logger.Warn("third", "Caller.cs", "Method");

        var folders = LogFiles().Select(Path.GetDirectoryName).Distinct().ToArray();

        Assert.Single(folders);
        Assert.Single(LogFiles());

        var contents = File.ReadAllText(LogFiles().Single());

        Assert.Contains("first", contents);
        Assert.Contains("second", contents);
        Assert.Contains("third", contents);
    }

    [Fact]
    public void GivenTheByRequestScheme_WhenTwoLoggersWrite_ThenEachGetsItsOwnFolder()
    {
        new FileLogger(Configuration(LogFolderScheme.ByRequest)).Info("one", "Caller.cs", "Method");
        new FileLogger(Configuration(LogFolderScheme.ByRequest)).Info("two", "Caller.cs", "Method");

        var folders = LogFiles().Select(Path.GetDirectoryName).Distinct().ToArray();

        Assert.Equal(2, folders.Length);
    }

    [Fact]
    public void GivenTheAllInOneScheme_WhenWriting_ThenTheFileSitsDirectlyUnderTheBasePath()
    {
        new FileLogger(Configuration(LogFolderScheme.AllInOne)).Info("entry", "Caller.cs", "Method");

        var file = Assert.Single(LogFiles());

        Assert.Equal(Path.GetFullPath(_root), Path.GetFullPath(Path.GetDirectoryName(file)!));
    }

    [Theory]
    [InlineData(LogFolderScheme.ByYear, 1)]
    [InlineData(LogFolderScheme.ByMonth, 2)]
    [InlineData(LogFolderScheme.ByDay, 3)]
    [InlineData(LogFolderScheme.ByHour, 4)]
    public void GivenACalendarScheme_WhenWriting_ThenTheFileIsNestedThatManyLevelsDeep(
        LogFolderScheme scheme,
        int expectedDepth)
    {
        new FileLogger(Configuration(scheme)).Info("entry", "Caller.cs", "Method");

        var file = Assert.Single(LogFiles());
        var relative = Path.GetRelativePath(_root, file);
        var depth = relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length - 1;

        Assert.Equal(expectedDepth, depth);
    }

    [Theory]
    [InlineData(LogSplitLevel.Request, "TestApp.log")]
    [InlineData(LogSplitLevel.Year, "TestApp_{0:yyyy}.log")]
    [InlineData(LogSplitLevel.Month, "TestApp_{0:yyyy_MM}.log")]
    [InlineData(LogSplitLevel.Day, "TestApp_{0:yyyy_MM_dd}.log")]
    [InlineData(LogSplitLevel.Hour, "TestApp_{0:yyyy_MM_dd_HH}.log")]
    public void GivenASplitLevel_WhenWriting_ThenTheFileIsNamedForThatPeriod(LogSplitLevel split, string expected)
    {
        new FileLogger(Configuration(LogFolderScheme.AllInOne, split)).Info("entry", "Caller.cs", "Method");

        var file = Assert.Single(LogFiles());

        Assert.Equal(string.Format(expected, DateTime.UtcNow), Path.GetFileName(file));
    }

    [Fact]
    public void GivenNoDirectoryYet_WhenWriting_ThenItIsCreated()
    {
        Assert.False(Directory.Exists(_root));

        new FileLogger(Configuration(LogFolderScheme.ByMonth)).Info("entry", "Caller.cs", "Method");

        Assert.True(Directory.Exists(_root));
    }

    [Fact]
    public void GivenEveryLevel_WhenWriting_ThenEachEntryCarriesItsLevelAndCallerOnItsOwnLine()
    {
        var logger = new FileLogger(Configuration(LogFolderScheme.AllInOne));

        logger.Trace("t", "Caller.cs", "Method");
        logger.Debug("d", "Caller.cs", "Method");
        logger.Info("i", "Caller.cs", "Method");
        logger.Warn("w", "Caller.cs", "Method");
        logger.Error("e", "Caller.cs", "Method");
        logger.Exception("x", "Caller.cs", "Method");
        logger.Critical("c", "Caller.cs", "Method");
        logger.Fatal("f", "Caller.cs", "Method");

        var lines = File.ReadAllLines(Assert.Single(LogFiles()));

        Assert.Equal(8, lines.Length);
        Assert.All(lines, line => Assert.Contains("Caller | Method", line));
        Assert.Equal(
            new[] { "TRACE", "DEBUG", "INFO", "WARN", "ERROR", "EXCEPTION", "CRITICAL", "FATAL" },
            lines.Select(line => line[..line.IndexOf(':')]));
    }
}
