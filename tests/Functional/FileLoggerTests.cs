using ArturRios.Logging.Configuration;
using ArturRios.Logging.Loggers;

namespace ArturRios.Logging.Tests.Functional;

/// <summary>
/// What <see cref="FileLogger"/> writes into its file, read back from a real directory. Where the file goes, for each
/// folder scheme and split level, is <see cref="FileLoggerFolderSchemeTests"/>.
/// </summary>
[Trait("Category", "Functional")]
public sealed class FileLoggerTests : IDisposable
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

    private FileLoggerConfiguration Configuration() =>
        new()
        {
            ApplicationName = "TestApp",
            FilePath = _root,
            FolderScheme = LogFolderScheme.AllInOne,
            FileSplitLevel = LogSplitLevel.Request
        };

    private string LogFile => Path.Combine(_root, "TestApp.log");

    [Theory]
    [InlineData("")]
    [InlineData("special: !@#$%^&*()_+-=[]{}|;:',.<>?")]
    [InlineData("unicode: 你好世界 مرحبا العالم 🎉")]
    public void GivenAnyMessage_WhenWritten_ThenTheEntryEndsWithItAsItIs(string message)
    {
        new FileLogger(Configuration()).Info(message, "Caller.cs", "Method");

        var line = Assert.Single(File.ReadAllLines(LogFile));
        Assert.StartsWith("INFO: Caller | Method | ", line);
        Assert.EndsWith(" | " + message, line);
    }

    [Fact]
    public void GivenALongMessage_WhenWritten_ThenAllOfItIsWritten()
    {
        var message = string.Concat(Enumerable.Repeat("This is a very long message. ", 1000));

        new FileLogger(Configuration()).Info(message, "Caller.cs", "Method");

        Assert.EndsWith(message, Assert.Single(File.ReadAllLines(LogFile)));
    }

    [Fact]
    public void GivenAFileWrittenBefore_WhenAnotherLoggerWritesToIt_ThenItsEntryIsAppended()
    {
        new FileLogger(Configuration()).Info("first", "Caller.cs", "Method");
        new FileLogger(Configuration()).Warn("second", "Caller.cs", "Method");

        var lines = File.ReadAllLines(LogFile);

        Assert.Equal(2, lines.Length);
        Assert.EndsWith("first", lines[0]);
        Assert.EndsWith("second", lines[1]);
    }

    [Fact]
    public void GivenSeveralThreads_WhenEachWritesManyEntries_ThenEveryEntryIsWrittenWholeOnItsOwnLine()
    {
        var logger = new FileLogger(Configuration());

        Parallel.For(0, 8, thread =>
        {
            for (var i = 0; i < 25; i++)
            {
                logger.Info($"thread {thread} entry {i}", "Caller.cs", "Method");
            }
        });

        var lines = File.ReadAllLines(LogFile);

        Assert.Equal(200, lines.Length);
        Assert.All(lines, line => Assert.Matches(@"^INFO: Caller \| Method \| \S+ \| thread \d entry \d+$", line));
        Assert.Equal(200, lines.Distinct().Count());
    }

    [Fact]
    public void GivenNoFilePath_WhenWritten_ThenTheFileGoesToALogFolderBesideTheApplication()
    {
        var defaultFolder = Path.Combine(AppContext.BaseDirectory, "log");
        var configuration = Configuration();
        configuration.FilePath = null;

        try
        {
            new FileLogger(configuration).Info("entry", "Caller.cs", "Method");

            Assert.True(File.Exists(Path.Combine(defaultFolder, "TestApp.log")));
        }
        finally
        {
            if (Directory.Exists(defaultFolder))
            {
                Directory.Delete(defaultFolder, recursive: true);
            }
        }
    }

    [Fact]
    public void GivenAnEmptyFilePath_WhenWritten_ThenTheFileGoesToTheCurrentDirectory()
    {
        // A name of its own, so the file in the shared current directory cannot be another test's.
        var configuration = Configuration();
        configuration.ApplicationName = "TestApp_" + Guid.NewGuid().ToString("N");
        configuration.FilePath = "";
        var file = Path.Combine(Environment.CurrentDirectory, configuration.ApplicationName + ".log");

        try
        {
            new FileLogger(configuration).Info("entry", "Caller.cs", "Method");

            var line = Assert.Single(File.ReadAllLines(file));
            Assert.EndsWith(" | entry", line);
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Fact]
    public void GivenAFolderSchemeOutsideTheEnum_WhenWritten_ThenItThrowsAndWritesNothing()
    {
        var configuration = Configuration();
        configuration.FolderScheme = (LogFolderScheme)42;

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new FileLogger(configuration).Info("entry", "Caller.cs", "Method"));

        Assert.Equal((LogFolderScheme)42, exception.ActualValue);
        Assert.False(Directory.Exists(_root));
    }

    [Fact]
    public void GivenASplitLevelOutsideTheEnum_WhenWritten_ThenItThrowsAndWritesNothing()
    {
        var configuration = Configuration();
        configuration.FileSplitLevel = (LogSplitLevel)42;

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new FileLogger(configuration).Info("entry", "Caller.cs", "Method"));

        Assert.Equal((LogSplitLevel)42, exception.ActualValue);
        Assert.False(Directory.Exists(_root));
    }
}
