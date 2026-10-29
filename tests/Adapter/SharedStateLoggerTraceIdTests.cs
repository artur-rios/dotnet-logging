using System.Diagnostics;
using ArturRios.Logging.Adapter;
using ArturRios.Logging.Interfaces;
using ArturRios.Logging.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArturRios.Logging.Tests.Adapter;

/// <summary>
/// The adapter's trace id lives in an <see cref="AsyncLocal{T}"/>, but it reaches the output through
/// <see cref="IStateLogger.TraceId"/>, a plain property. When that state logger is a singleton it is shared by every
/// entry, so the adapter must not let one entry's id land on another entry's line or outlive the entry it belongs to.
/// </summary>
[Trait("Category", "Unit")]
public class SharedStateLoggerTraceIdTests
{
    private static ServiceProvider BuildProvider(IStateLogger stateLogger)
    {
        var services = new ServiceCollection();

        services.AddSingleton(stateLogger);

        return services.BuildServiceProvider();
    }

    private static void Log(ILogger logger, string message) =>
        logger.Log(LogLevel.Information, new EventId(0), Array.Empty<KeyValuePair<string, object>>(), null,
            (_, _) => message);

    [Fact]
    public async Task GivenASingletonStateLogger_WhenAnEntryWithoutATraceIdFollowsOneWithIt_ThenTheEarlierIdDoesNotLeak()
    {
        var capturing = new CapturingStateLogger();
        var sp = BuildProvider(capturing);

        await Task.Run(() =>
        {
            var adapter = new MicrosoftLoggerAdapter(sp) { TraceId = "earlier-request" };
            Log(adapter, "first");
        });

        Assert.Null(Activity.Current);
        Log(new MicrosoftLoggerAdapter(sp), "second");

        var calls = capturing.Calls.ToArray();
        Assert.Equal("earlier-request", calls[0].TraceId);
        Assert.Null(calls[1].TraceId);
    }

    [Fact]
    public void GivenAStateLoggerWithItsOwnTraceId_WhenAnEntryCarriesAnAmbientOne_ThenItsOwnIsRestoredAfterwards()
    {
        var capturing = new CapturingStateLogger { TraceId = "configured" };
        var adapter = new MicrosoftLoggerAdapter(BuildProvider(capturing));

        using (var activity = new Activity("op").SetIdFormat(ActivityIdFormat.W3C).Start())
        {
            Log(adapter, "inside");

            Assert.Equal(activity.TraceId.ToString(), capturing.LastTraceId);
        }

        Log(adapter, "outside");

        Assert.Equal("configured", capturing.LastTraceId);
        Assert.Equal("configured", capturing.TraceId);
    }

    [Fact]
    public async Task GivenASingletonStateLogger_WhenEntriesAreLoggedConcurrently_ThenEachCarriesItsOwnTraceId()
    {
        var capturing = new CapturingStateLogger();
        var sp = BuildProvider(capturing);

        const int workers = 8;
        const int entriesPerWorker = 500;

        await Task.WhenAll(Enumerable.Range(0, workers).Select(worker => Task.Run(() =>
        {
            for (var entry = 0; entry < entriesPerWorker; entry++)
            {
                var id = $"{worker}-{entry}";
                var adapter = new MicrosoftLoggerAdapter(sp) { TraceId = id };

                Log(adapter, id);
            }
        })));

        var calls = capturing.Calls.ToArray();

        Assert.Equal(workers * entriesPerWorker, calls.Length);
        Assert.All(calls, call => Assert.Equal(call.Message, call.TraceId));
    }
}
