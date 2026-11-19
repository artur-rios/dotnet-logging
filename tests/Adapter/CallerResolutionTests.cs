using ArturRios.Logging.Adapter;
using ArturRios.Logging.Interfaces;
using ArturRios.Logging.Tests.Helpers;
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

    // What the entry line shows: LogEntryFactory takes the class from the file path's name.
    private static (string ClassName, string MemberName) EntryCallerFrom(CapturingStateLogger capturing)
    {
        var (filePath, memberName) = CallerFrom(capturing);

        return (Path.GetFileNameWithoutExtension(filePath), memberName);
    }

    // StateLogger takes the first key that names the caller, so the order of the forwarded pairs decides it.
    private static (string FilePath, string MemberName) FirstCallerFrom(CapturingStateLogger capturing)
    {
        var pairs = ((IEnumerable<KeyValuePair<string, object>>)capturing.Calls.First().State!).ToArray();

        static bool Is(string key, params string[] names) =>
            names.Any(name => string.Equals(key, name, StringComparison.OrdinalIgnoreCase));

        return (
            pairs.First(pair => Is(pair.Key, "CallerFilePath", "FilePath")).Value.ToString()!,
            pairs.First(pair => Is(pair.Key, "CallerMemberName", "MemberName", "Method")).Value.ToString()!);
    }

    [Fact]
    public void GivenACallerInANamespace_WhenLoggingThroughTheAdapter_ThenItsClassAndMethodAreReported()
    {
        var capturing = new CapturingStateLogger();

        new SampleApp.Orders.OrderService(new MicrosoftLoggerAdapter(BuildProvider(capturing))).PlaceOrder();

        Assert.Equal(("OrderService", "PlaceOrder"), EntryCallerFrom(capturing));
    }

    [Fact]
    public async Task GivenAnAsyncCaller_WhenLoggingThroughTheAdapter_ThenTheAsyncMethodIsReportedNotMoveNext()
    {
        var capturing = new CapturingStateLogger();

        await new SampleApp.Orders.OrderService(new MicrosoftLoggerAdapter(BuildProvider(capturing)))
            .PlaceOrderAsync();

        Assert.Equal(("OrderService", "PlaceOrderAsync"), EntryCallerFrom(capturing));
    }

    [Fact]
    public void GivenALambdaCaller_WhenLoggingThroughTheAdapter_ThenTheEnclosingClassAndMethodAreReported()
    {
        var capturing = new CapturingStateLogger();

        new SampleApp.Orders.OrderService(new MicrosoftLoggerAdapter(BuildProvider(capturing)))
            .PlaceOrderFromLambda();

        Assert.Equal(("OrderService", "PlaceOrderFromLambda"), EntryCallerFrom(capturing));
    }

    [Fact]
    public void GivenAGenericCaller_WhenLoggingThroughTheAdapter_ThenItsClassIsReportedWithoutItsArity()
    {
        var capturing = new CapturingStateLogger();

        new SampleApp.Orders.Repository<int>(new MicrosoftLoggerAdapter(BuildProvider(capturing))).Save();

        Assert.Equal(("Repository", "Save"), EntryCallerFrom(capturing));
    }

    [Fact]
    public void GivenATemplateWithAMethodPlaceholder_WhenLoggingThroughTheAdapter_ThenTheCallerIsStillReported()
    {
        var capturing = new CapturingStateLogger();

        new SampleApp.Orders.OrderService(new MicrosoftLoggerAdapter(BuildProvider(capturing))).LogRequest();

        var (filePath, memberName) = FirstCallerFrom(capturing);

        Assert.Equal("OrderService", Path.GetFileNameWithoutExtension(filePath));
        Assert.Equal("LogRequest", memberName);
    }

    [Fact]
    public void GivenATemplateWithFilePathAndMethodPlaceholders_WhenLoggingThroughTheAdapter_ThenTheCallerIsStillReported()
    {
        var capturing = new CapturingStateLogger();

        new SampleApp.Orders.OrderService(new MicrosoftLoggerAdapter(BuildProvider(capturing))).LogFile();

        var (filePath, memberName) = FirstCallerFrom(capturing);

        Assert.Equal("OrderService", Path.GetFileNameWithoutExtension(filePath));
        Assert.Equal("LogFile", memberName);
    }

    [Fact]
    public void GivenATemplateWithAMethodPlaceholder_WhenLoggingThroughTheAdapter_ThenThePlaceholderValueIsKept()
    {
        var capturing = new CapturingStateLogger();

        new SampleApp.Orders.OrderService(new MicrosoftLoggerAdapter(BuildProvider(capturing))).LogRequest();

        var pairs = ((IEnumerable<KeyValuePair<string, object>>)capturing.Calls.First().State!).ToArray();

        Assert.Contains(pairs, pair => pair is { Key: "Method", Value: "GET" });
        Assert.Contains(pairs, pair => pair is { Key: "Path", Value: "/orders" });
    }
}
