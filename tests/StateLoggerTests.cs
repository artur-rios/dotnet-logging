using ArturRios.Logging.Configuration;
using ArturRios.Logging.Interfaces;
using ArturRios.Logging.Tests.Helpers;

namespace ArturRios.Logging.Tests;

[Trait("Category", "Unit")]
public class StateLoggerTests
{
    private class TestStateLogger : StateLogger
    {
        public TestStateLogger() : base(new List<LoggerConfiguration>())
        {
            var field = typeof(StateLogger).GetField("_loggers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            var list = (List<IInternalLogger>)field.GetValue(this)!;
            list.Clear();
            Dummy = new RecordingInternalLogger();
            list.Add(Dummy);
        }

        public RecordingInternalLogger Dummy { get; }
    }

    [Fact]
    public void GivenStateLoggerWithNullState_WhenTraceLogged_ThenUsesUnknownCallerInfo()
    {
        var logger = new TestStateLogger();
        logger.Trace("hello");
        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void GivenStateLoggerWithCallerStatePairs_WhenDebugLogged_ThenReadsCallerInfo()
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
    public void GivenStateLoggerWithTraceId_WhenExceptionLogged_ThenLogsToStringAndPrefixesTraceId()
    {
        var logger = new TestStateLogger { TraceId = "t1" };
        var ex = new InvalidOperationException("boom");
        logger.Exception(ex, new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn") });
        var call = logger.Dummy.Calls.Last();

        Assert.Equal(CustomLogLevel.Exception, call.Level);
        Assert.Contains("[TraceId] t1 | ", call.Message);
        Assert.Contains("System.InvalidOperationException: boom", call.Message);
    }

    [Fact]
    public void GivenStateLoggerWithCallerState_WhenInfoLogged_ThenReadsCallerInfo()
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
    public void GivenStateLoggerWithNullState_WhenInfoLogged_ThenUsesUnknownCallerInfo()
    {
        var logger = new TestStateLogger();

        logger.Info("msg");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void GivenStateLoggerWithTraceId_WhenInfoLogged_ThenPrefixesTraceId()
    {
        var logger = new TestStateLogger { TraceId = "info-trace" };

        logger.Info("info", new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn") });

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] info-trace | ", call.Message);
    }

    [Fact]
    public void GivenStateLoggerWithCallerState_WhenWarnLogged_ThenReadsCallerInfo()
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
    public void GivenStateLoggerWithNullState_WhenWarnLogged_ThenUsesUnknownCallerInfo()
    {
        var logger = new TestStateLogger();

        logger.Warn("msg");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void GivenStateLoggerWithTraceId_WhenWarnLogged_ThenPrefixesTraceId()
    {
        var logger = new TestStateLogger { TraceId = "warn-trace" };

        logger.Warn("warn", new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn") });

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] warn-trace | ", call.Message);
    }

    [Fact]
    public void GivenStateLoggerWithCallerState_WhenErrorLogged_ThenReadsCallerInfo()
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
    public void GivenStateLoggerWithNullState_WhenErrorLogged_ThenUsesUnknownCallerInfo()
    {
        var logger = new TestStateLogger();

        logger.Error("msg");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void GivenStateLoggerWithTraceId_WhenErrorLogged_ThenPrefixesTraceId()
    {
        var logger = new TestStateLogger { TraceId = "error-trace" };

        logger.Error("error", new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn") });

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] error-trace | ", call.Message);
    }

    [Fact]
    public void GivenStateLoggerWithCallerState_WhenCriticalLogged_ThenReadsCallerInfo()
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
    public void GivenStateLoggerWithNullState_WhenCriticalLogged_ThenUsesUnknownCallerInfo()
    {
        var logger = new TestStateLogger();

        logger.Critical("msg");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void GivenStateLoggerWithTraceId_WhenCriticalLogged_ThenPrefixesTraceId()
    {
        var logger = new TestStateLogger { TraceId = "critical-trace" };

        logger.Critical("critical", new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn") });

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] critical-trace | ", call.Message);
    }

    [Fact]
    public void GivenStateLoggerWithCallerState_WhenFatalLogged_ThenReadsCallerInfo()
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
    public void GivenStateLoggerWithNullState_WhenFatalLogged_ThenUsesUnknownCallerInfo()
    {
        var logger = new TestStateLogger();

        logger.Fatal("msg");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void GivenStateLoggerWithTraceId_WhenFatalLogged_ThenPrefixesTraceId()
    {
        var logger = new TestStateLogger { TraceId = "fatal-trace" };

        logger.Fatal("fatal", new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn") });

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.StartsWith("[TraceId] fatal-trace | ", call.Message);
    }

    [Fact]
    public void GivenStateLoggerWithNonEnumerableState_WhenTraceLogged_ThenUsesUnknownCallerInfo()
    {
        var logger = new TestStateLogger();

        logger.Trace("msg", "not enumerable state");

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Equal("unknown", call.File);
        Assert.Equal("unknown", call.Method);
    }

    [Fact]
    public void GivenStateLoggerWithFilePathAlternateProperty_WhenDebugLogged_ThenReadsCallerInfo()
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
    public void GivenStateLoggerWithLowercasePropertyNames_WhenDebugLogged_ThenReadsCallerInfo()
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
    public void GivenStateLoggerWithMethodProperty_WhenDebugLogged_ThenReadsMethodFromAlternateProperty()
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
    public void GivenStateLoggerWithNullPropertyValues_WhenTraceLogged_ThenIgnoresNullValues()
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
    public void GivenStateLoggerWithDuplicateProperties_WhenInfoLogged_ThenStopsProcessingWhenBothFound()
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
    public void GivenStateLoggerWithException_WhenExceptionLogged_ThenUsesExceptionToString()
    {
        var logger = new TestStateLogger();
        var ex = new InvalidOperationException("message");

        logger.Exception(ex, new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn") });

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.Contains("System.InvalidOperationException", call.Message);
        Assert.Contains("message", call.Message);
    }

    [Fact]
    public void GivenStateLoggerWithNullTraceId_WhenExceptionLogged_ThenDoesNotPrefixTraceId()
    {
        var logger = new TestStateLogger { TraceId = null };
        var ex = new Exception("test");

        logger.Exception(ex, new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn") });

        var call = Assert.Single(logger.Dummy.Calls);

        Assert.DoesNotContain("[TraceId]", call.Message);
    }

    [Fact]
    public void GivenStateLoggerWithMultipleInternalLoggers_WhenInfoLogged_ThenForwardsToAllConfiguredLoggers()
    {
        var logger = new TestStateLogger();
        var dummy2 = new RecordingInternalLogger();

        var field = typeof(StateLogger).GetField("_loggers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var list = (List<IInternalLogger>)field.GetValue(logger)!;
        list.Add(dummy2);

        logger.Info("test", new[] { new KeyValuePair<string, object>("CallerFilePath", "fp"), new KeyValuePair<string, object>("CallerMemberName", "mn") });

        Assert.Single(logger.Dummy.Calls);
        Assert.Single(dummy2.Calls);
    }

    [Fact]
    public void GivenNullConfigurations_WhenStateLoggerConstructed_ThenArgumentNullExceptionIsThrown()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new StateLogger(null!));

        Assert.Equal("configurations", exception.ParamName);
    }

    [Fact]
    public void GivenNoConfigurations_WhenEveryLevelIsLogged_ThenNothingThrows()
    {
        var logger = new StateLogger([]) { TraceId = "t" };

        var exception = Record.Exception(() =>
        {
            logger.Trace("m");
            logger.Debug("m");
            logger.Info("m");
            logger.Warn("m");
            logger.Error("m");
            logger.Exception(new InvalidOperationException("e"));
            logger.Critical("m");
            logger.Fatal("m");
        });

        Assert.Null(exception);
    }

    [Theory]
    [InlineData("CallerFilePath", "Caller.cs", "Caller.cs", "unknown")]
    [InlineData("CallerMemberName", "Run", "unknown", "Run")]
    public void GivenStateWithOnlyOneHalfOfTheCaller_WhenLogged_ThenTheOtherHalfIsUnknown(
        string key, string value, string expectedFile, string expectedMethod)
    {
        var logger = new TestStateLogger();

        logger.Info("m", new[] { new KeyValuePair<string, object>(key, value) });

        var call = Assert.Single(logger.Dummy.Calls);
        Assert.Equal(expectedFile, call.File);
        Assert.Equal(expectedMethod, call.Method);
    }
}
