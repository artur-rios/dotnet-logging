using ArturRios.Logging.Factories;

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
}
