using System.Text.RegularExpressions;
using ArturRios.Logging.Configuration;
using ArturRios.Logging.Interfaces;
using ArturRios.Logging.Loggers;
using ArturRios.Logging.Tests.Helpers;
using ArturRios.Util.Collections;

namespace ArturRios.Logging.Tests.Loggers;

/// <summary>
/// What <see cref="ConsoleLogger"/> writes to the console, read back from <see cref="Console.Out"/>. The class is in
/// <see cref="ConsoleOutputCollection"/>, since the console is the process's.
/// </summary>
[Trait("Category", "Unit")]
[Collection(ConsoleOutputCollection.Name)]
public class ConsoleLoggerTests
{
    private const string Reset = "\e[0m";

    private static readonly ConsoleLoggerConfiguration Plain = new() { UseColors = false };
    private static readonly ConsoleLoggerConfiguration Colored = new() { UseColors = true };

    /// <summary>Each level: the method that logs it, the name the entry carries and the color it is written in.</summary>
    public static TheoryData<string, string, string> Levels => new()
    {
        { nameof(IInternalLogger.Trace), "TRACE", AnsiColors.DarkGray },
        { nameof(IInternalLogger.Debug), "DEBUG", AnsiColors.Cyan },
        { nameof(IInternalLogger.Info), "INFO", AnsiColors.Green },
        { nameof(IInternalLogger.Warn), "WARN", AnsiColors.Yellow },
        { nameof(IInternalLogger.Error), "ERROR", AnsiColors.Red },
        { nameof(IInternalLogger.Exception), "EXCEPTION", AnsiColors.Magenta },
        { nameof(IInternalLogger.Critical), "CRITICAL", AnsiColors.BrightRed },
        { nameof(IInternalLogger.Fatal), "FATAL", AnsiColors.BrightRed }
    };

    private static void Log(IInternalLogger logger, string method, string message, string filePath = "src/Caller.cs",
        string methodName = "Run")
    {
        switch (method)
        {
            case nameof(IInternalLogger.Trace):
                logger.Trace(message, filePath, methodName);
                break;
            case nameof(IInternalLogger.Debug):
                logger.Debug(message, filePath, methodName);
                break;
            case nameof(IInternalLogger.Info):
                logger.Info(message, filePath, methodName);
                break;
            case nameof(IInternalLogger.Warn):
                logger.Warn(message, filePath, methodName);
                break;
            case nameof(IInternalLogger.Error):
                logger.Error(message, filePath, methodName);
                break;
            case nameof(IInternalLogger.Exception):
                logger.Exception(message, filePath, methodName);
                break;
            case nameof(IInternalLogger.Critical):
                logger.Critical(message, filePath, methodName);
                break;
            case nameof(IInternalLogger.Fatal):
                logger.Fatal(message, filePath, methodName);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(method), method, null);
        }
    }

    /// <summary>One entry, as <c>LogEntryFactory</c> formats it, with any timestamp.</summary>
    private static Regex Entry(string level, string message, string className = "Caller", string methodName = "Run") =>
        new($"^{level}: {Regex.Escape(className)} \\| {Regex.Escape(methodName)} \\| " +
            $@"\d{{4}}-\d{{2}}-\d{{2}}T\d{{2}}:\d{{2}}:\d{{2}}\.\d{{7}}Z \| {Regex.Escape(message)}" +
            $"{Regex.Escape(Environment.NewLine)}$");

    [Theory]
    [MemberData(nameof(Levels))]
    public void GivenColorsDisabled_WhenEachLevelIsLogged_ThenOnePlainEntryIsWritten(string method, string level, string _)
    {
        using var console = new ConsoleCapture();

        Log(new ConsoleLogger(Plain), method, "the message");

        Assert.Matches(Entry(level, "the message"), console.Output);
    }

    [Theory]
    [MemberData(nameof(Levels))]
    public void GivenColorsEnabled_WhenEachLevelIsLogged_ThenTheEntryIsWrittenInItsColorFollowedByAReset(
        string method, string level, string color)
    {
        using var console = new ConsoleCapture();

        Log(new ConsoleLogger(Colored), method, "the message");

        var output = console.Output;
        Assert.StartsWith(color, output);
        Assert.EndsWith(Reset, output);
        Assert.Matches(Entry(level, "the message"), output[color.Length..^Reset.Length]);
    }

    [Fact]
    public void GivenColorsDisabled_WhenLogged_ThenNoEscapeSequenceIsWritten()
    {
        using var console = new ConsoleCapture();

        new ConsoleLogger(Plain).Error("plain", "Caller.cs", "Run");

        Assert.DoesNotContain('\e', console.Output);
    }

    [Fact]
    public void GivenADefaultConfiguration_WhenLogged_ThenTheEntryIsColored()
    {
        using var console = new ConsoleCapture();

        new ConsoleLogger(new ConsoleLoggerConfiguration()).Info("colored", "Caller.cs", "Run");

        Assert.StartsWith(AnsiColors.Green, console.Output);
    }

    [Theory]
    [InlineData("C:\\src\\Orders\\OrderService.cs", "OrderService")]
    [InlineData("/home/ci/src/OrderService.cs", "OrderService")]
    [InlineData("unknown", "unknown")]
    public void GivenACallerFilePath_WhenLogged_ThenTheEntryNamesTheFileWithoutItsFolderOrExtension(
        string filePath, string className)
    {
        if (filePath.Contains('\\') && !OperatingSystem.IsWindows())
        {
            // A Windows path is one file name elsewhere: there, only '/' separates folders.
            className = Path.GetFileNameWithoutExtension(filePath);
        }

        using var console = new ConsoleCapture();

        new ConsoleLogger(Plain).Info("m", filePath, "Run");

        Assert.Matches(Entry("INFO", "m", className), console.Output);
    }

    [Theory]
    [InlineData("")]
    [InlineData("tab\there | pipes, ünïcödé ✓ and \"quotes\"")]
    [InlineData("two\nlines")]
    public void GivenAnyMessage_WhenLogged_ThenItIsWrittenAsItIs(string message)
    {
        using var console = new ConsoleCapture();

        new ConsoleLogger(Plain).Info(message, "Caller.cs", "Run");

        Assert.Matches(Entry("INFO", message), console.Output);
    }

    [Fact]
    public void GivenANullMessage_WhenLogged_ThenTheEntryHasAnEmptyMessage()
    {
        using var console = new ConsoleCapture();

        new ConsoleLogger(Plain).Info(null!, "Caller.cs", "Run");

        Assert.Matches(Entry("INFO", string.Empty), console.Output);
    }

    [Fact]
    public void GivenSeveralThreads_WhenEachLogsManyEntries_ThenEveryEntryIsWrittenWholeOnItsOwnLine()
    {
        using var console = new ConsoleCapture();
        var logger = new ConsoleLogger(Colored);

        Parallel.For(0, 8, thread =>
        {
            for (var i = 0; i < 25; i++)
            {
                logger.Info($"thread {thread} entry {i}", "Caller.cs", "Run");
            }
        });

        var entries = console.Output.Split(Reset, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(200, entries.Length);
        Assert.All(entries, entry =>
        {
            Assert.StartsWith(AnsiColors.Green, entry);
            Assert.Matches(new Regex($@"^INFO: Caller \| Run \| \S+ \| thread \d entry \d+{Regex.Escape(Environment.NewLine)}$"),
                entry[AnsiColors.Green.Length..]);
        });
    }
}
