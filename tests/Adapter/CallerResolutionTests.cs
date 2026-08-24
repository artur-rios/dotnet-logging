using ArturRios.Logging.Adapter;
using ArturRios.Logging.Interfaces;
using ArturRios.Logging.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArturRios.Logging.Tests.Adapter;

/// <summary>
/// When the state carries no caller information, the adapter walks the stack to find one. It has to skip
/// its own frames to get past itself, and the namespace it used to skip — ArturRios.Common.Logging — has
/// not existed for some time, so it reported itself as the caller of every entry.
/// </summary>
[Trait("Category", "Unit")]
public class CallerResolutionTests
{
    private static ServiceProvider BuildProvider(IStateLogger stateLogger)
    {
        var services = new ServiceCollection();

        services.AddSingleton(stateLogger);
        services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor());

        return services.BuildServiceProvider();
    }

    private static (string FilePath, string MemberName) CallerFrom(CapturingStateLogger capturing)
    {
        var state = (IEnumerable<KeyValuePair<string, object>>)capturing.Calls.First().State!;
        var pairs = state.ToArray();

        return (
            pairs.Single(pair => pair.Key == "CallerFilePath").Value.ToString()!,
            pairs.Single(pair => pair.Key == "CallerMemberName").Value.ToString()!);
    }

    [Fact]
    public void GivenStateWithoutCallerInfo_WhenLogging_ThenTheAdapterIsNotReportedAsTheCaller()
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));

        logger.Log(LogLevel.Information, new EventId(0), Array.Empty<KeyValuePair<string, object>>(), null,
            (_, _) => "hello");

        var (filePath, memberName) = CallerFrom(capturing);

        Assert.DoesNotContain("MicrosoftLoggerAdapter", filePath);
        Assert.NotEqual("Log", memberName);
    }

    [Fact]
    public void GivenStateWithoutCallerInfo_WhenLogging_ThenNoFrameFromThisLibraryIsReported()
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));

        logger.Log(LogLevel.Information, new EventId(0), Array.Empty<KeyValuePair<string, object>>(), null,
            (_, _) => "hello");

        var (filePath, _) = CallerFrom(capturing);

        Assert.DoesNotContain("ArturRios.Logging", filePath);
        Assert.NotEqual("unknown", filePath);
    }

    [Fact]
    public void GivenStateThatAlreadyCarriesCallerInfo_WhenLogging_ThenItIsUsedUntouched()
    {
        var capturing = new CapturingStateLogger();
        var logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));

        var state = new[]
        {
            new KeyValuePair<string, object>("CallerFilePath", "Supplied.cs"),
            new KeyValuePair<string, object>("CallerMemberName", "SuppliedMember")
        };

        logger.Log(LogLevel.Information, new EventId(0), state, null, (_, _) => "hello");

        var (filePath, memberName) = CallerFrom(capturing);

        Assert.Equal("Supplied.cs", filePath);
        Assert.Equal("SuppliedMember", memberName);
    }

    [Fact]
    public void GivenTheCallerExtensions_WhenLoggingThroughTheAdapter_ThenTheExtensionIsNotReportedAsTheCaller()
    {
        var capturing = new CapturingStateLogger();
        ILogger logger = new MicrosoftLoggerAdapter(BuildProvider(capturing));

        logger.LogInformationWithCaller("hello");

        var (_, memberName) = CallerFrom(capturing);

        Assert.Equal(nameof(GivenTheCallerExtensions_WhenLoggingThroughTheAdapter_ThenTheExtensionIsNotReportedAsTheCaller), memberName);
    }
}
