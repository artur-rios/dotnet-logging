using System.Globalization;
using ArturRios.Logging.Factories;
using ArturRios.Logging.Tests.Helpers;

namespace ArturRios.Logging.Tests.Factories;

[Trait("Category", "Unit")]
public class LogEntryFactoryTests
{
    [Fact]
    public void GivenLogEntryFactory_WhenCreateCalled_ThenFormatsMessageWithExpectedParts()
    {
        const CustomLogLevel level = CustomLogLevel.Debug;
        const string filePath = "C:/src/MyClass.cs";
        const string method = "DoWork";
        const string message = "Hello";

        var result = LogEntryFactory.Create(level, filePath, method, message);

        Assert.Contains("DEBUG:", result);
        Assert.Contains("MyClass", result);
        Assert.Contains("DoWork", result);
        Assert.Contains("Hello", result);
        Assert.EndsWith(Environment.NewLine, result);
    }

    [Fact]
    public void GivenATimestamp_WhenCreateCalledWithIt_ThenTheEntryCarriesExactlyThatInstant()
    {
        var timestamp = new DateTime(2025, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);

        var result = LogEntryFactory.Create(CustomLogLevel.Information, "A.cs", "M", "msg", timestamp);

        Assert.Equal($"INFO: A | M | {timestamp:o} | msg{Environment.NewLine}", result);
    }

    [Theory]
    [InlineData("pt-BR")]
    [InlineData("th-TH")] // a Buddhist calendar: its year is 2568 where the Gregorian one is 2025
    [InlineData("")]
    public void GivenAnyCulture_WhenAnEntryIsCreated_ThenItsTimestampIsGregorianRoundTripText(string culture)
    {
        var timestamp = new DateTime(2025, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
        using var scope = new CultureScope(culture);

        var result = LogEntryFactory.Create(CustomLogLevel.Information, "A.cs", "M", "msg", timestamp);

        Assert.Equal($"INFO: A | M | 2025-12-31T23:59:59.9990000Z | msg{Environment.NewLine}", result);
    }

    [Fact]
    public void GivenNoTimestamp_WhenAnEntryIsCreated_ThenItIsStampedWithTheCurrentUtcTime()
    {
        var before = DateTime.UtcNow;

        var result = LogEntryFactory.Create(CustomLogLevel.Warning, "A.cs", "M", "msg");

        var after = DateTime.UtcNow;
        var stamp = DateTime.Parse(result.Split(" | ")[2], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        Assert.Equal(DateTimeKind.Utc, stamp.Kind);
        Assert.InRange(stamp, before, after);
    }

    [Theory]
    [InlineData("C:/src/Orders/OrderService.cs", "OrderService")]
    [InlineData("OrderService.cs", "OrderService")]
    [InlineData("OrderService", "OrderService")]
    [InlineData("Order.Service.cs", "Order.Service")]
    [InlineData("unknown", "unknown")]
    [InlineData("", "")]
    public void GivenACallerFilePath_WhenAnEntryIsCreated_ThenItNamesTheFileWithoutItsFolderOrExtension(
        string filePath, string className)
    {
        var result = LogEntryFactory.Create(CustomLogLevel.Information, filePath, "M", "msg", DateTime.UnixEpoch);

        Assert.StartsWith($"INFO: {className} | M | ", result);
    }
}
