using ArturRios.Logging.Configuration;
using ArturRios.Logging.Interfaces;

namespace ArturRios.Logging.Tests;

internal class DummyInternalLogger2 : IInternalLogger
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

public class StateLoggerTests
{
    private class TestStateLogger : StateLogger
    {
        public TestStateLogger() : base(new List<LoggerConfiguration>())
        {
            var field = typeof(StateLogger).GetField("_loggers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            var list = (List<IInternalLogger>)field.GetValue(this)!;
            list.Clear();
            Dummy = new DummyInternalLogger2();
            list.Add(Dummy);
        }
        public DummyInternalLogger2 Dummy { get; }
    }

    [Fact]
    public void Trace_ShouldUseUnknown_WhenStateIsNull()
    {
        var logger = new TestStateLogger();
        logger.Trace("hello");
        var call = Assert.Single(logger.Dummy.Calls);
        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void Debug_ShouldReadCallerInfo_FromStatePairs()
    {
        var logger = new TestStateLogger();
        var state = new[]
        {
            new KeyValuePair<string, object>("CallerFilePath", "fp"),
            new KeyValuePair<string, object>("CallerMemberName", "mn")
        };
        logger.Debug("m", state);
        var call = Assert.Single(logger.Dummy.Calls);
        Assert.Equal("fp", call.File);
        Assert.Equal("mn", call.Method);
    }

    [Fact]
    public void Exception_ShouldLogException_ToStringAndPrefixTraceId()
    {
        var logger = new TestStateLogger { TraceId = "t1" };
        var ex = new InvalidOperationException("boom");
        logger.Exception(ex, new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn")});
        var call = logger.Dummy.Calls.Last();
        Assert.Equal(CustomLogLevel.Exception, call.Level);
        Assert.Contains("[TraceId] t1 | ", call.Message);
        Assert.Contains("System.InvalidOperationException: boom", call.Message);
    }
}
