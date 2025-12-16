using System.Diagnostics.CodeAnalysis;
using ArturRios.Logging.Interfaces;

namespace ArturRios.Logging.Tests;

internal class DummyInternalLogger : IInternalLogger
{
    public readonly List<(CustomLogLevel Level, string Message, string File, string Method)> Calls = new();

    public void Trace(string message, string filePath, string methodName) =>
        Calls.Add((CustomLogLevel.Trace, message, filePath, methodName));

    public void Debug(string message, string filePath, string methodName) =>
        Calls.Add((CustomLogLevel.Debug, message, filePath, methodName));

    public void Info(string message, string filePath, string methodName) =>
        Calls.Add((CustomLogLevel.Information, message, filePath, methodName));

    public void Warn(string message, string filePath, string methodName) =>
        Calls.Add((CustomLogLevel.Warning, message, filePath, methodName));

    public void Error(string message, string filePath, string methodName) =>
        Calls.Add((CustomLogLevel.Error, message, filePath, methodName));

    public void Exception(string message, string filePath, string methodName) =>
        Calls.Add((CustomLogLevel.Exception, message, filePath, methodName));

    public void Critical(string message, string filePath, string methodName) =>
        Calls.Add((CustomLogLevel.Critical, message, filePath, methodName));

    public void Fatal(string message, string filePath, string methodName) =>
        Calls.Add((CustomLogLevel.Fatal, message, filePath, methodName));
}

[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")] // Reason: testing purposes
public class StandaloneLoggerTests
{
    private class TestStandaloneLogger : StandaloneLogger
    {
        public TestStandaloneLogger() : base([])
        {
            var field = typeof(StandaloneLogger).GetField("_loggers",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            var list = (List<IInternalLogger>)field.GetValue(this)!;

            list.Clear();

            Dummy = new DummyInternalLogger();

            list.Add(Dummy);
        }

        public DummyInternalLogger Dummy { get; }
    }

    [Fact]
    public void Should_ForwardToInternalLoggerAndPreserveCallerInfo()
    {
        var logger = new TestStandaloneLogger();

        logger.Trace("msg", filePath: "a/b/c.cs", methodName: "M");

        Assert.Single(logger.Dummy.Calls);

        var call = logger.Dummy.Calls.Single();

        Assert.Equal(CustomLogLevel.Trace, call.Level);
        Assert.Equal("msg", call.Message);
        Assert.Equal("a/b/c.cs", call.File);
        Assert.Equal("M", call.Method);
    }

    [Fact]
    public void Should_PrefixTraceId_WhenPresent()
    {
        var logger = new TestStandaloneLogger { TraceId = "abc" };

        logger.Info("hello", filePath: "fp", methodName: "mn");

        var call = logger.Dummy.Calls.Single();

        Assert.StartsWith("[TraceId] abc | ", call.Message);
        Assert.EndsWith("hello", call.Message);
    }

    [Fact]
    public void Should_Debug_ForwardToInternalLogger()
    {
        var logger = new TestStandaloneLogger();

        logger.Debug("debug msg", filePath: "path/file.cs", methodName: "DebugMethod");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal(CustomLogLevel.Debug, call.Level);
        Assert.Equal("debug msg", call.Message);
        Assert.Equal("path/file.cs", call.File);
        Assert.Equal("DebugMethod", call.Method);
    }

    [Fact]
    public void Should_Debug_PrefixTraceId_WhenPresent()
    {
        var logger = new TestStandaloneLogger { TraceId = "trace123" };

        logger.Debug("debug msg", filePath: "fp", methodName: "mn");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] trace123 | ", call.Message);
    }

    [Fact]
    public void Should_Warn_ForwardToInternalLogger()
    {
        var logger = new TestStandaloneLogger();

        logger.Warn("warning msg", filePath: "path/warn.cs", methodName: "WarnMethod");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal(CustomLogLevel.Warning, call.Level);
        Assert.Equal("warning msg", call.Message);
        Assert.Equal("path/warn.cs", call.File);
        Assert.Equal("WarnMethod", call.Method);
    }

    [Fact]
    public void Should_Warn_PrefixTraceId_WhenPresent()
    {
        var logger = new TestStandaloneLogger { TraceId = "warn-trace" };

        logger.Warn("warn msg", filePath: "fp", methodName: "mn");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] warn-trace | ", call.Message);
    }

    [Fact]
    public void Should_Error_ForwardToInternalLogger()
    {
        var logger = new TestStandaloneLogger();

        logger.Error("error msg", filePath: "path/error.cs", methodName: "ErrorMethod");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal(CustomLogLevel.Error, call.Level);
        Assert.Equal("error msg", call.Message);
        Assert.Equal("path/error.cs", call.File);
        Assert.Equal("ErrorMethod", call.Method);
    }

    [Fact]
    public void Should_Error_PrefixTraceId_WhenPresent()
    {
        var logger = new TestStandaloneLogger { TraceId = "err-trace" };

        logger.Error("error msg", filePath: "fp", methodName: "mn");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] err-trace | ", call.Message);
    }

    [Fact]
    public void Should_Exception_LogExceptionMessage()
    {
        var logger = new TestStandaloneLogger();
        var ex = new InvalidOperationException("something went wrong");

        logger.Exception(ex, filePath: "path/exception.cs", methodName: "ExceptionMethod");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal(CustomLogLevel.Exception, call.Level);
        Assert.Equal("something went wrong", call.Message);
        Assert.Equal("path/exception.cs", call.File);
        Assert.Equal("ExceptionMethod", call.Method);
    }

    [Fact]
    public void Should_Exception_PrefixTraceId_WhenPresent()
    {
        var logger = new TestStandaloneLogger { TraceId = "exc-trace" };
        var ex = new ArgumentException("invalid arg");

        logger.Exception(ex, filePath: "fp", methodName: "mn");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] exc-trace | ", call.Message);
    }

    [Fact]
    public void Should_Critical_ForwardToInternalLogger()
    {
        var logger = new TestStandaloneLogger();

        logger.Critical("critical msg", filePath: "path/critical.cs", methodName: "CriticalMethod");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal(CustomLogLevel.Critical, call.Level);
        Assert.Equal("critical msg", call.Message);
        Assert.Equal("path/critical.cs", call.File);
        Assert.Equal("CriticalMethod", call.Method);
    }

    [Fact]
    public void Should_Critical_PrefixTraceId_WhenPresent()
    {
        var logger = new TestStandaloneLogger { TraceId = "crit-trace" };

        logger.Critical("critical msg", filePath: "fp", methodName: "mn");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] crit-trace | ", call.Message);
    }

    [Fact]
    public void Should_Fatal_ForwardToInternalLogger()
    {
        var logger = new TestStandaloneLogger();

        logger.Fatal("fatal msg", filePath: "path/fatal.cs", methodName: "FatalMethod");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal(CustomLogLevel.Fatal, call.Level);
        Assert.Equal("fatal msg", call.Message);
        Assert.Equal("path/fatal.cs", call.File);
        Assert.Equal("FatalMethod", call.Method);
    }

    [Fact]
    public void Should_Fatal_PrefixTraceId_WhenPresent()
    {
        var logger = new TestStandaloneLogger { TraceId = "fatal-trace" };

        logger.Fatal("fatal msg", filePath: "fp", methodName: "mn");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] fatal-trace | ", call.Message);
    }

    [Fact]
    public void Should_Trace_NotPrefixTraceId_WhenNull()
    {
        var logger = new TestStandaloneLogger { TraceId = null };

        logger.Trace("msg", filePath: "fp", methodName: "mn");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("msg", call.Message);
        Assert.DoesNotContain("[TraceId]", call.Message);
    }

    [Fact]
    public void Should_Trace_NotPrefixTraceId_WhenEmpty()
    {
        var logger = new TestStandaloneLogger { TraceId = "" };

        logger.Trace("msg", filePath: "fp", methodName: "mn");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("msg", call.Message);
        Assert.DoesNotContain("[TraceId]", call.Message);
    }

    [Fact]
    public void Should_ForwardToAllConfiguredLoggers()
    {
        // Need to create a logger with multiple internal loggers
        var logger = new TestStandaloneLogger();
        var dummy2 = new DummyInternalLogger();

        var field = typeof(StandaloneLogger).GetField("_loggers",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var list = (List<IInternalLogger>)field.GetValue(logger)!;
        list.Add(dummy2);

        logger.Info("test", filePath: "fp", methodName: "mn");

        Assert.Single(logger.Dummy.Calls);
        Assert.Single(dummy2.Calls);
    }

    [Fact]
    public void Should_UseCallerInfo()
    {
        var logger = new TestStandaloneLogger();

        logger.Trace("msg");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.EndsWith("StandaloneLoggerTests.cs", call.File);
        Assert.Equal("Should_UseCallerInfo", call.Method);
    }
}
