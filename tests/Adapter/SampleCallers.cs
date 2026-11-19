using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

// Callers outside the ArturRios.Logging namespaces, which the adapter skips while walking the stack: a test
// class of this project lives under ArturRios.Logging.Tests, so its own frames are never the reported caller.
namespace SampleApp.Orders;

public class OrderService(ILogger logger)
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void PlaceOrder() => logger.LogInformation("placing");

    [MethodImpl(MethodImplOptions.NoInlining)]
    public async Task PlaceOrderAsync()
    {
        await Task.Yield();
        logger.LogInformation("placing");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void PlaceOrderFromLambda()
    {
        var items = new[] { "a" };
        Array.ForEach(items, item => logger.LogInformation("placing {Item}", item));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void LogRequest() => logger.LogInformation("HTTP {Method} {Path}", "GET", "/orders");

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void LogFile() => logger.LogInformation("Read {FilePath} in {Method}", "/data/in.csv", "bulk");
}

public class Repository<T>(ILogger logger)
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Save() => logger.LogInformation("saving {Type}", typeof(T).Name);
}
