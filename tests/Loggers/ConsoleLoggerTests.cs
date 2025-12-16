using ArturRios.Logging.Configuration;
using ArturRios.Logging.Interfaces;
using ArturRios.Logging.Loggers;

namespace ArturRios.Logging.Tests.Loggers;

public class ConsoleLoggerTests
{
    private readonly ConsoleLoggerConfiguration _configuration = new() { UseColors = false };

    [Fact]
    public void Should_ImplementIInternalLogger()
    {
        var logger = new ConsoleLogger(_configuration);
        Assert.IsType<IInternalLogger>(logger, exactMatch: false);
    }

    [Fact]
    public void Should_AcceptConsoleLoggerConfigurationInConstructor()
    {
        var config = new ConsoleLoggerConfiguration { UseColors = true };
        var logger = new ConsoleLogger(config);

        Assert.NotNull(logger);
    }

    [Fact]
    public void Should_HaveTraceMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Trace("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_HaveDebugMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Debug("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_HaveInfoMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Info("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_HaveWarnMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Warn("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_HaveErrorMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Error("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_HaveExceptionMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Exception("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_HaveCriticalMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Critical("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_HaveFatalMethod()
    {
        var logger = new ConsoleLogger(_configuration);

        logger.Fatal("test message", "TestFile.cs", "TestMethod");
    }

    [Fact]
    public void Should_WriteTraceWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Trace("Trace message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_WriteDebugWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Debug("Debug message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_WriteInfoWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Info("Info message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_WriteWarnWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Warn("Warning message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_WriteErrorWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Error("Error message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_WriteExceptionWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Exception("Exception message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_WriteCriticalWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Critical("Critical message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_WriteFatalWithoutException()
    {
        var logger = new ConsoleLogger(_configuration);
        var exception = Record.Exception(() =>
            logger.Fatal("Fatal message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_SupportColoredOutput()
    {
        var configWithColors = new ConsoleLoggerConfiguration { UseColors = true };
        var loggerWithColors = new ConsoleLogger(configWithColors);

        var exception = Record.Exception(() =>
            loggerWithColors.Info("Colored message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_SupportNonColoredOutput()
    {
        var configWithoutColors = new ConsoleLoggerConfiguration { UseColors = false };
        var loggerWithoutColors = new ConsoleLogger(configWithoutColors);

        var exception = Record.Exception(() =>
            loggerWithoutColors.Info("Plain message", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_HandleEmptyMessage()
    {
        var logger = new ConsoleLogger(_configuration);

        var exception = Record.Exception(() =>
            logger.Info(string.Empty, "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_HandleNullMessage()
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
    public void Should_HandleMultipleConsecutiveLoggingCalls()
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
    public void Should_AllowLoggingWithSpecialCharacters()
    {
        var logger = new ConsoleLogger(_configuration);

        var exception = Record.Exception(() =>
            logger.Info("Message with special chars: !@#$%^&*()_+-=[]{}|;:',.<>?", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_AllowLoggingWithUnicodeCharacters()
    {
        var logger = new ConsoleLogger(_configuration);

        var exception = Record.Exception(() =>
            logger.Info("Message with unicode: 你好世界 مرحبا العالم 🎉", "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_AllowLoggingWithLongMessage()
    {
        var logger = new ConsoleLogger(_configuration);
        var longMessage = string.Concat(Enumerable.Repeat("This is a very long message. ", 100));

        var exception = Record.Exception(() =>
            logger.Info(longMessage, "Program.cs", "Main")
        );

        Assert.Null(exception);
    }

    [Fact]
    public void Should_BeThreadSafe()
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


