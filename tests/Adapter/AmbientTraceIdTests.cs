using System.Diagnostics;
using ArturRios.Logging.Adapter;
using ArturRios.Logging.Interfaces;
using ArturRios.Logging.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArturRios.Logging.Tests.Adapter;

/// <summary>
/// The correlation id comes from the ambient <see cref="Activity"/>, which is the platform's own
/// correlation primitive rather than any one hosting model's. These pin the behaviour that replaced
/// reading <c>HttpContext.Items["TraceId"]</c>, including the guarantee that a host starting a W3C activity
/// — as <c>ArturRios.Util.WebApi</c>'s <c>TraceActivityMiddleware</c> does — still yields the same id.
/// </summary>
[Trait("Category", "Unit")]
public class AmbientTraceIdTests
{
    private static ServiceProvider BuildProvider(IStateLogger stateLogger)
    {
        var services = new ServiceCollection();

        services.AddSingleton(stateLogger);

        return services.BuildServiceProvider();
    }

    private static void LogOnce(ILogger logger) =>
        logger.Log(LogLevel.Information, new EventId(0), Array.Empty<KeyValuePair<string, object>>(), null,
            (_, _) => "hello");

    [Fact]
    public void GivenAW3CActivityStartedTheWayTheMiddlewareDoes_WhenLogging_ThenTheStateLoggerGetsThatTraceId()
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));

        // Exactly what TraceActivityMiddleware does before invoking the rest of the pipeline.
        using var activity = new Activity("ServerReceive").SetIdFormat(ActivityIdFormat.W3C).Start();

        LogOnce(logger);

        Assert.Equal(activity.TraceId.ToString(), capturing.TraceId);
    }

    [Fact]
    public void GivenNoActivityAndNoOverride_WhenLogging_ThenTheStateLoggerIsLeftAlone()
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));

        Assert.Null(Activity.Current);

        LogOnce(logger);

        Assert.Null(capturing.TraceId);
    }

    [Fact]
    public void GivenAnExplicitOverride_WhenLogging_ThenTheStateLoggerGetsTheOverride()
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing)) { TraceId = "explicit-id" };

        using var activity = new Activity("ServerReceive").SetIdFormat(ActivityIdFormat.W3C).Start();

        LogOnce(logger);

        Assert.Equal("explicit-id", capturing.TraceId);
        Assert.NotEqual(activity.TraceId.ToString(), capturing.TraceId);
    }

    [Fact]
    public void GivenNestedActivities_WhenLogging_ThenTheTraceIdIsSharedAcrossThem()
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));

        using var outer = new Activity("outer").SetIdFormat(ActivityIdFormat.W3C).Start();

        var outerTraceId = outer.TraceId.ToString();

        using (var inner = new Activity("inner").SetIdFormat(ActivityIdFormat.W3C).Start())
        {
            LogOnce(logger);

            Assert.Equal(outerTraceId, inner.TraceId.ToString());
            Assert.Equal(outerTraceId, capturing.TraceId);
        }
    }

    [Fact]
    public async Task GivenAnOverrideSetInsideOneFlow_WhenReadFromAnother_ThenItDoesNotLeakAcrossThem()
    {
        var sp = BuildProvider(new CapturingStateLogger());

        await Task.Run(() => new MicrosoftLoggerAdapter(sp).TraceId = "inside-a-branch");

        Assert.Null(new MicrosoftLoggerAdapter(sp).TraceId);
    }

    [Fact]
    public void GivenTheAdapterIsUsedWithoutAnyStateLogger_WhenLogging_ThenNothingThrows()
    {
        var logger = new MicrosoftLoggerAdapter(new ServiceCollection().BuildServiceProvider());

        using var activity = new Activity("ServerReceive").SetIdFormat(ActivityIdFormat.W3C).Start();

        Assert.Null(Record.Exception(() => LogOnce(logger)));
    }
}
