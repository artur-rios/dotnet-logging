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
    public void Should_Trace_UseUnknown_WhenStateIsNull()
    {
        var logger = new TestStateLogger();
        logger.Trace("hello");
        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void Should_Debug_ReadCallerInfo_FromStatePairs()
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
    public void Should_Exception_LogException_ToStringAndPrefixTraceId()
    {
        var logger = new TestStateLogger { TraceId = "t1" };
        var ex = new InvalidOperationException("boom");
        logger.Exception(ex, new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn")});
        var call = logger.Dummy.Calls.Last();

        Assert.Equal(CustomLogLevel.Exception, call.Level);
        Assert.Contains("[TraceId] t1 | ", call.Message);
        Assert.Contains("System.InvalidOperationException: boom", call.Message);
    }

    [Fact]
    public void Should_Info_ReadCallerInfo_FromState()
    {
        var logger = new TestStateLogger();
        var state = new[]
        {
            new KeyValuePair<string, object>("CallerFilePath", "path/info.cs"),
            new KeyValuePair<string, object>("CallerMemberName", "InfoMethod")
        };

        logger.Info("info message", state);

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal(CustomLogLevel.Information, call.Level);
        Assert.Equal("info message", call.Message);
        Assert.Equal("path/info.cs", call.File);
        Assert.Equal("InfoMethod", call.Method);
    }

    [Fact]
    public void Should_Info_UseUnknown_WhenStateIsNull()
    {
        var logger = new TestStateLogger();

        logger.Info("msg");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void Should_Info_PrefixTraceId_WhenPresent()
    {
        var logger = new TestStateLogger { TraceId = "info-trace" };

        logger.Info("info", new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn") });

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] info-trace | ", call.Message);
    }

    [Fact]
    public void Should_Warn_ReadCallerInfo_FromState()
    {
        var logger = new TestStateLogger();
        var state = new[]
        {
            new KeyValuePair<string, object>("CallerFilePath", "path/warn.cs"),
            new KeyValuePair<string, object>("CallerMemberName", "WarnMethod")
        };

        logger.Warn("warn message", state);

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal(CustomLogLevel.Warning, call.Level);
        Assert.Equal("warn message", call.Message);
        Assert.Equal("path/warn.cs", call.File);
        Assert.Equal("WarnMethod", call.Method);
    }

    [Fact]
    public void Should_Warn_UseUnknown_WhenStateIsNull()
    {
        var logger = new TestStateLogger();

        logger.Warn("msg");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void Should_Warn_PrefixTraceId_WhenPresent()
    {
        var logger = new TestStateLogger { TraceId = "warn-trace" };

        logger.Warn("warn", new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn") });

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] warn-trace | ", call.Message);
    }

    [Fact]
    public void Should_Error_ReadCallerInfo_FromState()
    {
        var logger = new TestStateLogger();
        var state = new[]
        {
            new KeyValuePair<string, object>("CallerFilePath", "path/error.cs"),
            new KeyValuePair<string, object>("CallerMemberName", "ErrorMethod")
        };

        logger.Error("error message", state);

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal(CustomLogLevel.Error, call.Level);
        Assert.Equal("error message", call.Message);
        Assert.Equal("path/error.cs", call.File);
        Assert.Equal("ErrorMethod", call.Method);
    }

    [Fact]
    public void Should_Error_UseUnknown_WhenStateIsNull()
    {
        var logger = new TestStateLogger();

        logger.Error("msg");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void Should_Error_PrefixTraceId_WhenPresent()
    {
        var logger = new TestStateLogger { TraceId = "error-trace" };

        logger.Error("error", new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn") });

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] error-trace | ", call.Message);
    }

    [Fact]
    public void Should_Critical_ReadCallerInfo_FromState()
    {
        var logger = new TestStateLogger();
        var state = new[]
        {
            new KeyValuePair<string, object>("CallerFilePath", "path/critical.cs"),
            new KeyValuePair<string, object>("CallerMemberName", "CriticalMethod")
        };

        logger.Critical("critical message", state);

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal(CustomLogLevel.Critical, call.Level);
        Assert.Equal("critical message", call.Message);
        Assert.Equal("path/critical.cs", call.File);
        Assert.Equal("CriticalMethod", call.Method);
    }

    [Fact]
    public void Should_Critical_UseUnknown_WhenStateIsNull()
    {
        var logger = new TestStateLogger();

        logger.Critical("msg");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void Should_Critical_PrefixTraceId_WhenPresent()
    {
        var logger = new TestStateLogger { TraceId = "critical-trace" };

        logger.Critical("critical", new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn") });

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] critical-trace | ", call.Message);
    }

    [Fact]
    public void Should_Fatal_ReadCallerInfo_FromState()
    {
        var logger = new TestStateLogger();
        var state = new[]
        {
            new KeyValuePair<string, object>("CallerFilePath", "path/fatal.cs"),
            new KeyValuePair<string, object>("CallerMemberName", "FatalMethod")
        };

        logger.Fatal("fatal message", state);

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal(CustomLogLevel.Fatal, call.Level);
        Assert.Equal("fatal message", call.Message);
        Assert.Equal("path/fatal.cs", call.File);
        Assert.Equal("FatalMethod", call.Method);
    }

    [Fact]
    public void Should_Fatal_UseUnknown_WhenStateIsNull()
    {
        var logger = new TestStateLogger();

        logger.Fatal("msg");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void Should_Fatal_PrefixTraceId_WhenPresent()
    {
        var logger = new TestStateLogger { TraceId = "fatal-trace" };

        logger.Fatal("fatal", new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn") });

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] fatal-trace | ", call.Message);
    }

    [Fact]
    public void Should_Trace_UseUnknown_WhenStateIsNotEnumerable()
    {
        var logger = new TestStateLogger();

        logger.Trace("msg", "not enumerable state");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void Should_Debug_ReadCallerInfo_UsingFilePath()
    {
        var logger = new TestStateLogger();
        var state = new[]
        {
            new KeyValuePair<string, object>("FilePath", "alt/file.cs"),
            new KeyValuePair<string, object>("MemberName", "AltMethod")
        };

        logger.Debug("msg", state);

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("alt/file.cs", call.File);
        Assert.Equal("AltMethod", call.Method);
    }

    [Fact]
    public void Should_Debug_ReadCallerInfo_UsingLowercaseNames()
    {
        var logger = new TestStateLogger();
        var state = new[]
        {
            new KeyValuePair<string, object>("callerFilePath", "lower/file.cs"),
            new KeyValuePair<string, object>("callerMemberName", "lowerMethod")
        };

        logger.Debug("msg", state);

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("lower/file.cs", call.File);
        Assert.Equal("lowerMethod", call.Method);
    }

    [Fact]
    public void Should_Debug_ReadCallerInfo_UsingMethodProperty()
    {
        var logger = new TestStateLogger();
        var state = new[]
        {
            new KeyValuePair<string, object>("CallerFilePath", "fp.cs"),
            new KeyValuePair<string, object>("Method", "MethodNameVariation")
        };

        logger.Debug("msg", state);

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("MethodNameVariation", call.Method);
    }

    [Fact]
    public void Should_Trace_IgnoreNullValues_InState()
    {
        var logger = new TestStateLogger();
        var state = new[]
        {
            new KeyValuePair<string, object>("CallerFilePath", null!),
            new KeyValuePair<string, object>("CallerMemberName", "validMethod")
        };

        logger.Trace("msg", state);

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("validMethod", call.Method);
    }

    [Fact]
    public void Should_Info_StopProcessing_WhenBothPropertiesFound()
    {
        var logger = new TestStateLogger();
        var state = new[]
        {
            new KeyValuePair<string, object>("CallerFilePath", "first.cs"),
            new KeyValuePair<string, object>("CallerMemberName", "firstMethod"),
            new KeyValuePair<string, object>("FilePath", "second.cs"),
            new KeyValuePair<string, object>("MemberName", "secondMethod")
        };

        logger.Info("msg", state);

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("first.cs", call.File);
        Assert.Equal("firstMethod", call.Method);
    }

    [Fact]
    public void Should_Exception_UseExceptionToString_WhenAvailable()
    {
        var logger = new TestStateLogger();
        var ex = new InvalidOperationException("message");

        logger.Exception(ex, new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn")});

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Contains("System.InvalidOperationException", call.Message);
        Assert.Contains("message", call.Message);
    }

    [Fact]
    public void Should_Exception_NotPrefixTraceId_WhenNull()
    {
        var logger = new TestStateLogger { TraceId = null };
        var ex = new Exception("test");

        logger.Exception(ex, new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn")});

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.DoesNotContain("[TraceId]", call.Message);
    }

    [Fact]
    public void Should_ForwardToAllConfiguredLoggers()
    {
        var logger = new TestStateLogger();
        var dummy2 = new DummyInternalLogger2();

        var field = typeof(StateLogger).GetField("_loggers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var list = (List<IInternalLogger>)field.GetValue(logger)!;
        list.Add(dummy2);

        logger.Info("test", new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn")});

        Assert.Single(logger.Dummy.Calls);
        Assert.Single(dummy2.Calls);
    }
}
