using ArturRios.Logging.Factories;

namespace ArturRios.Logging.Tests.Factories;

public class LogEntryFactoryTests
{
    [Fact]
    public void Should_FormatMessageWithExpectedParts()
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
}
