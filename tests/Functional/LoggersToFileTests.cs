using ArturRios.Logging.Configuration;

namespace ArturRios.Logging.Tests.Functional;

/// <summary>
/// <see cref="StandaloneLogger"/> and <see cref="StateLogger"/> built from a real file configuration, as an application
/// builds them, with what they write read back from the file.
/// </summary>
[Trait("Category", "Functional")]
public sealed class LoggersToFileTests : IDisposable
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

    private List<LoggerConfiguration> Configurations(string applicationName) =>
    [
        new FileLoggerConfiguration
        {
            ApplicationName = applicationName,
            FilePath = _root,
            FolderScheme = LogFolderScheme.AllInOne,
            FileSplitLevel = LogSplitLevel.Request
        }
    ];

    [Fact]
    public void GivenAStandaloneLoggerWithAFileConfiguration_WhenLogging_ThenTheFileGetsTheEntryWithItsCallerAndTraceId()
    {
        var logger = new StandaloneLogger(Configurations("Standalone")) { TraceId = "trace-1" };

        logger.Warn("stock is low");

        var line = Assert.Single(File.ReadAllLines(Path.Combine(_root, "Standalone.log")));
        Assert.StartsWith(
            $"WARN: LoggersToFileTests | {nameof(GivenAStandaloneLoggerWithAFileConfiguration_WhenLogging_ThenTheFileGetsTheEntryWithItsCallerAndTraceId)} | ",
            line);
        Assert.EndsWith(" | [TraceId] trace-1 | stock is low", line);
    }

    [Fact]
    public void GivenAStateLoggerWithTwoFileConfigurations_WhenLogging_ThenEachFileGetsTheEntry()
    {
        var logger = new StateLogger([.. Configurations("First"), .. Configurations("Second")]);

        logger.Error("payment failed", new[]
        {
            new KeyValuePair<string, object>("CallerFilePath", "src/Payments/PaymentService.cs"),
            new KeyValuePair<string, object>("CallerMemberName", "Charge")
        });

        Assert.All(new[] { "First.log", "Second.log" }, file =>
        {
            var line = Assert.Single(File.ReadAllLines(Path.Combine(_root, file)));
            Assert.StartsWith("ERROR: PaymentService | Charge | ", line);
            Assert.EndsWith(" | payment failed", line);
        });
    }
}
