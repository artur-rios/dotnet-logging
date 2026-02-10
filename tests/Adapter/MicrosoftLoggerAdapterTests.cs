using System.Diagnostics.CodeAnalysis;
using ArturRios.Logging.Adapter;
using ArturRios.Logging.Interfaces;
using ArturRios.Logging.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArturRios.Logging.Tests.Adapter;

[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")] // Reason: test purposes
public class MicrosoftLoggerAdapterTests
{
    private static ServiceProvider BuildProvider(IStateLogger stateLogger, IHttpContextAccessor accessor)
    {
        var services = new ServiceCollection();

        services.AddSingleton(stateLogger);
        services.AddSingleton(accessor);

        return services.BuildServiceProvider();
    }

    [Fact]
    public void GivenMicrosoftLoggerAdapterWithTraceId_WhenLogCalled_ThenForwardsToStateLoggerWithEnrichedState()
    {
        var capturing = new CapturingStateLogger();
        var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
        accessor.HttpContext!.Items["TraceId"] = "trace-123";

        var sp = BuildProvider(capturing, accessor);
        var logger = new MicrosoftLoggerAdapter(sp);

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
        var sp = BuildProvider(capturing, new HttpContextAccessor());
        var logger = new MicrosoftLoggerAdapter(sp);

        var ex = new InvalidOperationException("oops");

        logger.Log(LogLevel.Error, new EventId(0), Array.Empty<KeyValuePair<string, object>>(), ex, (_, e) => e!.Message);

        Assert.NotEmpty(capturing.Exceptions);
        Assert.Same(ex, capturing.Exceptions.First());
        Assert.Contains(capturing.Calls, c => c.Method == nameof(IStateLogger.Exception));
        Assert.Contains(capturing.Calls, c => c.Method == nameof(IStateLogger.Error));
    }

    [Fact]
    public void GivenMicrosoftLoggerAdapterWithNoHttpContext_WhenTraceIdAccessed_ThenReturnsNull()
    {
        var capturing = new CapturingStateLogger();
        var accessor = new HttpContextAccessor { HttpContext = null };
        var sp = BuildProvider(capturing, accessor);
        var logger = new MicrosoftLoggerAdapter(sp);

        var traceId = logger.TraceId;

        Assert.Null(traceId);
    }

    [Fact]
    public void GivenMicrosoftLoggerAdapterWithHttpContextButNoTraceId_WhenTraceIdAccessed_ThenReturnsNull()
    {
        var capturing = new CapturingStateLogger();
        var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
        var sp = BuildProvider(capturing, accessor);
        var logger = new MicrosoftLoggerAdapter(sp);

        var traceId = logger.TraceId;

        Assert.Null(traceId);
    }

    [Fact]
    public void GivenMicrosoftLoggerAdapterWithTraceIdInHttpContext_WhenTraceIdAccessed_ThenReturnsTraceId()
    {
        var capturing = new CapturingStateLogger();
        var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
        accessor.HttpContext!.Items["TraceId"] = "trace-456";
        var sp = BuildProvider(capturing, accessor);
        var logger = new MicrosoftLoggerAdapter(sp);

        var traceId = logger.TraceId;

        Assert.Equal("trace-456", traceId);
    }

    [Fact]
    public void GivenMicrosoftLoggerAdapter_WhenTraceIdSet_ThenSetsTraceIdInHttpContext()
    {
        var capturing = new CapturingStateLogger();
        var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
        var sp = BuildProvider(capturing, accessor);
        _ = new MicrosoftLoggerAdapter(sp) { TraceId = "trace-789" };

        Assert.Equal("trace-789", accessor.HttpContext!.Items["TraceId"]);
    }

    [Fact]
    public void GivenMicrosoftLoggerAdapterWithNoHttpContext_WhenTraceIdSet_ThenDoesNotThrow()
    {
        var capturing = new CapturingStateLogger();
        var accessor = new HttpContextAccessor { HttpContext = null };
        var sp = BuildProvider(capturing, accessor);
        var logger = new MicrosoftLoggerAdapter(sp);

        var exception = Record.Exception(() => logger.TraceId = "trace-xyz");

        Assert.Null(exception);
    }

    [Fact]
    public void GivenMicrosoftLoggerAdapter_WhenTraceIdSetAndAccessed_ThenSetsAndGetsTraceId()
    {
        var capturing = new CapturingStateLogger();
        var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
        var sp = BuildProvider(capturing, accessor);
        var logger = new MicrosoftLoggerAdapter(sp) { TraceId = "trace-roundtrip" };

        var retrievedTraceId = logger.TraceId;

        Assert.Equal("trace-roundtrip", retrievedTraceId);
    }
}
