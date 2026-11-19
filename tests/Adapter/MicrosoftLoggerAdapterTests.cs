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
        Assert.Equal("trace-123", capturing.LastTraceId);

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

    private static KeyValuePair<string, object>[] Pairs(object? state) =>
        ((IEnumerable<KeyValuePair<string, object>>)state!).ToArray();

    [Theory]
    [InlineData(LogLevel.Trace, nameof(IStateLogger.Trace))]
    [InlineData(LogLevel.Debug, nameof(IStateLogger.Debug))]
    [InlineData(LogLevel.Information, nameof(IStateLogger.Info))]
    [InlineData(LogLevel.Warning, nameof(IStateLogger.Warn))]
    [InlineData(LogLevel.Error, nameof(IStateLogger.Error))]
    [InlineData(LogLevel.Critical, nameof(IStateLogger.Critical))]
    public void GivenEachLevel_WhenLogged_ThenTheMatchingStateLoggerMethodGetsTheFormattedMessage(LogLevel level, string method)
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));

        Assert.True(logger.IsEnabled(level));

        logger.Log(level, new EventId(1), "state", null, (state, _) => $"formatted {state}");

        var call = Assert.Single(capturing.Calls);
        Assert.Equal(method, call.Method);
        Assert.Equal("formatted state", call.Message);
    }

    [Fact]
    public void GivenTheNoneLevel_WhenLogged_ThenItIsNotEnabledAndNothingIsForwarded()
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));

        logger.Log(LogLevel.None, new EventId(), "state", new InvalidOperationException("ignored"), (s, _) => s);

        Assert.False(logger.IsEnabled(LogLevel.None));
        Assert.Empty(capturing.Calls);
    }

    [Fact]
    public void GivenALevelOutsideTheEnum_WhenLogged_ThenItThrowsAfterForwardingTheException()
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));
        var exception = new InvalidOperationException("boom");

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            logger.Log((LogLevel)42, new EventId(), "state", exception, (s, _) => s));

        Assert.Equal([nameof(IStateLogger.Exception)], capturing.Calls.Select(call => call.Method));
    }

    [Fact]
    public void GivenAnExceptionAtANonErrorLevel_WhenLogged_ThenTheExceptionAndThenTheLevelAreForwarded()
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));
        var exception = new InvalidOperationException("boom");

        logger.Log(LogLevel.Warning, new EventId(), "state", exception, (_, _) => "careful");

        Assert.Equal([nameof(IStateLogger.Exception), nameof(IStateLogger.Warn)], capturing.Calls.Select(call => call.Method));
        Assert.Same(exception, Assert.Single(capturing.Exceptions));
    }

    [Fact]
    public void GivenNoFormatter_WhenLogged_ThenTheStatesTextIsTheMessage()
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));

        logger.Log(LogLevel.Information, new EventId(), 42, null, null!);

        Assert.Equal("42", Assert.Single(capturing.Calls).Message);
    }

    [Fact]
    public void GivenStatePairsWithoutCallerInfo_WhenLogged_ThenTheyAreKeptAndTheCallerAndMessageAreAppended()
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));

        logger.Log(LogLevel.Information, new EventId(), new[] { new KeyValuePair<string, object>("OrderId", 7) }, null,
            (_, _) => "placed");

        var pairs = Pairs(Assert.Single(capturing.Calls).State);
        Assert.Equal(["OrderId", "CallerFilePath", "CallerMemberName", "OriginalMessage"], pairs.Select(pair => pair.Key));
        Assert.Equal(7, pairs[0].Value);
        Assert.Equal("placed", pairs[3].Value);
    }

    [Fact]
    public void GivenStateThatIsNotAPairList_WhenLogged_ThenItIsReplacedByTheCallerAndTheMessage()
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));

        logger.Log(LogLevel.Information, new EventId(), "plain state", null, (_, _) => "message");

        var pairs = Pairs(Assert.Single(capturing.Calls).State);
        Assert.Equal(["CallerFilePath", "CallerMemberName", "OriginalMessage"], pairs.Select(pair => pair.Key));
    }

    [Theory]
    [InlineData("CallerFilePath", "CallerMemberName")]
    [InlineData("FilePath", "MemberName")]
    [InlineData("callerfilepath", "Method")]
    public void GivenStateNamingTheCallerUnderAnyAcceptedKey_WhenLogged_ThenItIsForwardedUntouched(string fileKey, string memberKey)
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));
        var state = new[]
        {
            new KeyValuePair<string, object>(fileKey, "Caller.cs"),
            new KeyValuePair<string, object>(memberKey, "Run")
        };

        logger.Log(LogLevel.Information, new EventId(), state, null, (_, _) => "m");

        Assert.Same(state, Assert.Single(capturing.Calls).State);
    }

    [Fact]
    public void GivenStateWithOnlyAFilePathOrANullMember_WhenLogged_ThenTheCallerIsLookedUpAndAppended()
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));
        var state = new[]
        {
            new KeyValuePair<string, object>("CallerFilePath", "Caller.cs"),
            new KeyValuePair<string, object>("CallerMemberName", null!)
        };

        logger.Log(LogLevel.Information, new EventId(), state, null, (_, _) => "m");

        var pairs = Pairs(Assert.Single(capturing.Calls).State);
        Assert.Equal(5, pairs.Length);
        Assert.Equal("CallerMemberName", pairs[3].Key);
        Assert.NotNull(pairs[3].Value);
    }

    [Fact]
    public void GivenAScopedStateLogger_WhenTwoEntriesAreLogged_ThenEachIsWrittenByAFreshInstance()
    {
        var created = new List<CapturingStateLogger>();
        var services = new ServiceCollection();
        services.AddScoped<IStateLogger>(_ =>
        {
            var instance = new CapturingStateLogger();
            created.Add(instance);
            return instance;
        });
        var logger = new MicrosoftLoggerAdapter(services.BuildServiceProvider());

        logger.Log(LogLevel.Information, new EventId(), "one", null, (s, _) => s);
        logger.Log(LogLevel.Information, new EventId(), "two", null, (s, _) => s);

        Assert.Equal(2, created.Count);
        Assert.Equal("one", Assert.Single(created[0].Calls).Message);
        Assert.Equal("two", Assert.Single(created[1].Calls).Message);
    }

    [Fact]
    public void GivenAnyState_WhenAScopeIsBegun_ThenTheSameDisposableThatDoesNothingComesBack()
    {
        var logger = new MicrosoftLoggerAdapter(BuildProvider(new CapturingStateLogger()));

        var first = logger.BeginScope("one");
        var second = logger.BeginScope(new { Id = 2 });

        Assert.Same(first, second);
        first.Dispose();
        first.Dispose();
    }

    [Fact]
    public void GivenNoServiceProvider_WhenTheAdapterIsCreated_ThenItIsRefusedAsAnArgument()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new MicrosoftLoggerAdapter(null!));

        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void GivenAProvider_WhenALoggerIsCreated_ThenItForwardsToTheProvidersStateLogger()
    {
        var capturing = new CapturingStateLogger();
        using var provider = new MicrosoftLoggerProvider(BuildProvider(capturing));

        var logger = provider.CreateLogger("Orders");
        logger.Log(LogLevel.Information, new EventId(), "state", null, (_, _) => "from the provider");

        Assert.IsType<MicrosoftLoggerAdapter>(logger);
        Assert.Equal("from the provider", Assert.Single(capturing.Calls).Message);
    }

    [Fact]
    public void GivenAProvider_WhenDisposedTwice_ThenNothingThrows()
    {
        var provider = new MicrosoftLoggerProvider(BuildProvider(new CapturingStateLogger()));

        provider.Dispose();

        Assert.Null(Record.Exception(provider.Dispose));
    }

    [Fact]
    public void GivenNoServiceProvider_WhenTheProviderIsCreated_ThenItIsRefusedAsAnArgument()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new MicrosoftLoggerProvider(null!));

        Assert.Equal("serviceProvider", exception.ParamName);
    }
}
