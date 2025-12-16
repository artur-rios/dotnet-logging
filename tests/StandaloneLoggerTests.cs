using System.Diagnostics.CodeAnalysis;
using ArturRios.Logging.Configuration;
using ArturRios.Logging.Interfaces;

namespace ArturRios.Logging.Tests;

internal class DummyInternalLogger : IInternalLogger
{
    public readonly List<(CustomLogLevel Level, string Message, string File, string Method)> Calls = new();
    public void Trace(string message, string filePath, string methodName) => Calls.Add((CustomLogLevel.Trace, message, filePath, methodName));
    public void Debug(string message, string filePath, string methodName) => Calls.Add((CustomLogLevel.Debug, message, filePath, methodName));
    public void Info(string message, string filePath, string methodName) => Calls.Add((CustomLogLevel.Information, message, filePath, methodName));
    public void Warn(string message, string filePath, string methodName) => Calls.Add((CustomLogLevel.Warning, message, filePath, methodName));
    public void Error(string message, string filePath, string methodName) => Calls.Add((CustomLogLevel.Error, message, filePath, methodName));
    public void Exception(string message, string filePath, string methodName) => Calls.Add((CustomLogLevel.Exception, message, filePath, methodName));
    public void Critical(string message, string filePath, string methodName) => Calls.Add((CustomLogLevel.Critical, message, filePath, methodName));
    public void Fatal(string message, string filePath, string methodName) => Calls.Add((CustomLogLevel.Fatal, message, filePath, methodName));
}

[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")] // Reason: testing purposes
public class StandaloneLoggerTests
{
    private class TestStandaloneLogger : StandaloneLogger
    {
        public TestStandaloneLogger() : base(new List<LoggerConfiguration>())
        {
            var field = typeof(StandaloneLogger).GetField("_loggers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            var list = (List<IInternalLogger>)field.GetValue(this)!;

            list.Clear();

            Dummy = new DummyInternalLogger();

            list.Add(Dummy);
        }
        public DummyInternalLogger Dummy { get; }
    }

    [Fact]
    public void Trace_ShouldForward_ToInternalLogger_AndPreserveCallerInfo()
    {
        var logger = new TestStandaloneLogger();

        // ReSharper disable once ExplicitCallerInfoArgument
        // Reason: testing purposes
        logger.Trace("msg", filePath: "a/b/c.cs", methodName: "M");

        Assert.Single(logger.Dummy.Calls);

        var call = logger.Dummy.Calls.Single();

        Assert.Equal(CustomLogLevel.Trace, call.Level);
        Assert.Equal("msg", call.Message);
        Assert.Equal("a/b/c.cs", call.File);
        Assert.Equal("M", call.Method);
    }

    [Fact]
    public void Trace_ShouldPrefixTraceId_WhenPresent()
    {
        var logger = new TestStandaloneLogger { TraceId = "abc" };

        logger.Info("hello", filePath: "fp", methodName: "mn");

        var call = logger.Dummy.Calls.Single();

        Assert.StartsWith("[TraceId] abc | ", call.Message);
        Assert.EndsWith("hello", call.Message);
    }
}
