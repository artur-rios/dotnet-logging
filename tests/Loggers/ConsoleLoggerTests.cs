﻿using ArturRios.Logging.Configuration;
using ArturRios.Logging.Interfaces;
using ArturRios.Logging.Loggers;

namespace ArturRios.Logging.Tests.Loggers;

public class ConsoleLoggerTests
{
    private readonly ConsoleLoggerConfiguration _configuration = new() { UseColors = false };

    [Fact]
    public void GivenConsoleLogger_WhenCreated_ThenImplementsIInternalLogger()
    {
        var logger = new ConsoleLogger(_configuration);
        Assert.IsType<IInternalLogger>(logger, exactMatch: false);
    }

    [Fact]
    public void GivenConsoleLoggerConfiguration_WhenConsoleLoggerCreated_ThenAcceptsConfiguration()
    {
        var config = new ConsoleLoggerConfiguration { UseColors = true };
        var logger = new ConsoleLogger(config);

        Assert.NotNull(logger);
    }

    [Fact]
    public void GivenConsoleLogger_WhenInspected_ThenHasTraceMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Trace("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenConsoleLogger_WhenInspected_ThenHasDebugMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Debug("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenConsoleLogger_WhenInspected_ThenHasInfoMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Info("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenConsoleLogger_WhenInspected_ThenHasWarnMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Warn("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenConsoleLogger_WhenInspected_ThenHasErrorMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Error("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenConsoleLogger_WhenInspected_ThenHasExceptionMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Exception("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenConsoleLogger_WhenInspected_ThenHasCriticalMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Critical("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenConsoleLogger_WhenInspected_ThenHasFatalMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Fatal("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void GivenConsoleLogger_WhenTraceLogged_ThenWritesWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Trace("Trace message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLogger_WhenDebugLogged_ThenWritesWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Debug("Debug message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLogger_WhenInfoLogged_ThenWritesWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Info("Info message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLogger_WhenWarnLogged_ThenWritesWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Warn("Warning message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLogger_WhenErrorLogged_ThenWritesWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Error("Error message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLogger_WhenExceptionLogged_ThenWritesWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Exception("Exception message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLogger_WhenCriticalLogged_ThenWritesWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Critical("Critical message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLogger_WhenFatalLogged_ThenWritesWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Fatal("Fatal message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLoggerWithColorsEnabled_WhenInfoLogged_ThenSupportsColoredOutput()
    {
        var configWithColors = new ConsoleLoggerConfiguration { UseColors = true };
        var loggerWithColors = new ConsoleLogger(configWithColors);

        var exception = Record.Exception(() =>
            loggerWithColors.Info("Colored message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLoggerWithColorsDisabled_WhenInfoLogged_ThenSupportsNonColoredOutput()
    {
        var configWithoutColors = new ConsoleLoggerConfiguration { UseColors = false };
        var loggerWithoutColors = new ConsoleLogger(configWithoutColors);

        var exception = Record.Exception(() =>
            loggerWithoutColors.Info("Plain message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLoggerWithEmptyMessage_WhenInfoLogged_ThenHandlesEmptyMessage()
    {
        var logger = new ConsoleLogger(_configuration);

        var exception = Record.Exception(() =>
            logger.Info(string.Empty, "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLoggerWithNullMessage_WhenInfoLogged_ThenHandlesNullMessage()
    {
        var logger = new ConsoleLogger(_configuration);

        try
        {
            logger.Info(null!, "Program.cs", "Main");
        }
        catch (ArgumentNullException)
        {
            Assert.True(true);
        }
    }

    [Fact]
    public void GivenConsoleLogger_WhenMultipleConsecutiveCallsMade_ThenHandlesMultipleConsecutiveLoggingCalls()
    {
        var logger = new ConsoleLogger(_configuration);

        var exception = Record.Exception(() =>
        {
            logger.Trace("First message", "File1.cs", "Method1");
            logger.Debug("Second message", "File2.cs", "Method2");
            logger.Info("Third message", "File3.cs", "Method3");
            logger.Error("Fourth message", "File4.cs", "Method4");
        });

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLoggerWithSpecialCharacters_WhenInfoLogged_ThenAllowsLoggingWithSpecialCharacters()
    {
        var logger = new ConsoleLogger(_configuration);

        var exception = Record.Exception(() =>
            logger.Info("Message with special chars: !@#$%^&*()_+-=[]{}|;:',.<>?", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLoggerWithUnicodeCharacters_WhenInfoLogged_ThenAllowsLoggingWithUnicodeCharacters()
    {
        var logger = new ConsoleLogger(_configuration);

        var exception = Record.Exception(() =>
            logger.Info("Message with unicode: 你好世界 مرحبا العالم 🎉", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLoggerWithLongMessage_WhenInfoLogged_ThenAllowsLoggingWithLongMessage()
    {
        var logger = new ConsoleLogger(_configuration);
        var longMessage = string.Concat(Enumerable.Repeat("This is a very long message. ", 100));

        var exception = Record.Exception(() =>
            logger.Info(longMessage, "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void GivenConsoleLoggerWithConcurrentCalls_WhenMultipleThreadsLog_ThenIsThreadSafe()
    {
        var logger = new ConsoleLogger(_configuration);
        var tasks = new List<Task>();

        for (int i = 0; i < 10; i++)
        {
            int index = i;
            tasks.Add(Task.Run(() =>
                logger.Info($"Message {index}", "Program.cs", "Main")
            ));
        }

        var exception = Record.Exception(() => Task.WaitAll(tasks.ToArray()));
        Assert.Null(exception);
    }
}


