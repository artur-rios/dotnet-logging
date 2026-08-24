using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ArturRios.Logging.Adapter;
using ArturRios.Logging.Interfaces;
using ArturRios.Logging.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArturRios.Logging.Tests.Adapter;

[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")] // Reason: test purposes
[Trait("Category", "Unit")]
public class MicrosoftLoggerAdapterTests
{
    private static ServiceProvider BuildProvider(IStateLogger stateLogger)
    {
        var services = new ServiceCollection();

        services.AddSingleton(stateLogger);

        return services.BuildServiceProvider();
    }

    [Fact]
    public void GivenMicrosoftLoggerAdapterWithTraceId_WhenLogCalled_ThenForwardsToStateLoggerWithEnrichedState()
    {
        var capturing = new CapturingStateLogger();

        var sp = BuildProvider(capturing);
        var logger = new MicrosoftLoggerAdapter(sp) { TraceId = "trace-123" };

        Assert.True(logger.IsEnabled(LogLevel.Information));

        logger.Log(LogLevel.Information,
            new EventId(10,
                "name"),
            new[]
            {
                new KeyValuePair<string, object>("k",
                    "v")
            },
            null,
            (_,
                _) => "hello");

        Assert.NotEmpty(capturing.Calls);

        var call = capturing.Calls.First();

        Assert.Equal("Info", call.Method);
        Assert.Equal("trace-123", capturing.TraceId);

        var pairs = (IEnumerable<KeyValuePair<string, object>>)call.State!;

        var keyValuePairs = pairs as KeyValuePair<string, object>[] ?? pairs.ToArray();

        Assert.Contains(keyValuePairs, kv => kv.Key == "CallerFilePath");
        Assert.Contains(keyValuePairs, kv => kv.Key == "CallerMemberName");
    }

    [Fact]
    public void GivenMicrosoftLoggerAdapterWithException_WhenLogCalled_ThenCallsExceptionThenLevel()
    {
        var capturing = new CapturingStateLogger();
        var sp = BuildProvider(capturing);
        var logger = new MicrosoftLoggerAdapter(sp);

        var ex = new InvalidOperationException("oops");

        logger.Log(LogLevel.Error, new EventId(0), Array.Empty<KeyValuePair<string, object>>(), ex, (_, e) => e!.Message);

        Assert.NotEmpty(capturing.Exceptions);
        Assert.Same(ex, capturing.Exceptions.First());
        Assert.Contains(capturing.Calls, c => c.Method == nameof(IStateLogger.Exception));
        Assert.Contains(capturing.Calls, c => c.Method == nameof(IStateLogger.Error));
    }

    [Fact]
    public void GivenNoAmbientActivityAndNoOverride_WhenTraceIdAccessed_ThenNullComesBack()
    {
        var sp = BuildProvider(new CapturingStateLogger());
        var logger = new MicrosoftLoggerAdapter(sp);

        Assert.Null(Activity.Current);
        Assert.Null(logger.TraceId);
    }

    [Fact]
    public void GivenAnAmbientActivity_WhenTraceIdAccessed_ThenItsTraceIdComesBack()
    {
        var sp = BuildProvider(new CapturingStateLogger());
        var logger = new MicrosoftLoggerAdapter(sp);

        using var activity = new Activity("test").SetIdFormat(ActivityIdFormat.W3C).Start();

        Assert.Equal(activity.TraceId.ToString(), logger.TraceId);
    }

    [Fact]
    public void GivenATraceIdWasSet_WhenTraceIdAccessed_ThenItComesBack()
    {
        var sp = BuildProvider(new CapturingStateLogger());
        var logger = new MicrosoftLoggerAdapter(sp) { TraceId = "trace-456" };

        Assert.Equal("trace-456", logger.TraceId);
    }

    [Fact]
    public void GivenATraceIdIsSetOnOneAdapter_WhenReadFromAnother_ThenTheSameContextSeesIt()
    {
        var sp = BuildProvider(new CapturingStateLogger());

        _ = new MicrosoftLoggerAdapter(sp) { TraceId = "trace-789" };

        Assert.Equal("trace-789", new MicrosoftLoggerAdapter(sp).TraceId);
    }

    [Fact]
    public void GivenNoAmbientActivity_WhenTraceIdSet_ThenDoesNotThrow()
    {
        var sp = BuildProvider(new CapturingStateLogger());
        var logger = new MicrosoftLoggerAdapter(sp);

        var exception = Record.Exception(() => logger.TraceId = "trace-xyz");

        Assert.Null(exception);
    }

    [Fact]
    public void GivenAnOverrideAndAnAmbientActivity_WhenTraceIdAccessed_ThenTheOverrideWins()
    {
        var sp = BuildProvider(new CapturingStateLogger());

        using var activity = new Activity("test").SetIdFormat(ActivityIdFormat.W3C).Start();
        var logger = new MicrosoftLoggerAdapter(sp) { TraceId = "trace-roundtrip" };

        var retrievedTraceId = logger.TraceId;

        Assert.Equal("trace-roundtrip", retrievedTraceId);
    }
}
