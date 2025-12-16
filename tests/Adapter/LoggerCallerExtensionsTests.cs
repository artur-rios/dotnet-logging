using System.Diagnostics.CodeAnalysis;
using ArturRios.Logging.Adapter;
using Microsoft.Extensions.Logging;
using Moq;

namespace ArturRios.Logging.Tests.Adapter;

[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public class LoggerCallerExtensionsTests
{
    [Fact]
    public void Should_LogTraceWithCaller_LogAtTraceLevel()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test trace message";
        const string filePath = "test.cs";
        const string memberName = "TestMethod";

        mockLogger.Object.LogTraceWithCaller(message, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Trace),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogDebugWithCaller_LogAtDebugLevel()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test debug message";
        const string filePath = "test.cs";
        const string memberName = "TestMethod";

        mockLogger.Object.LogDebugWithCaller(message, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Debug),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogInformationWithCaller_LogAtInformationLevel()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test information message";
        const string filePath = "test.cs";
        const string memberName = "TestMethod";

        mockLogger.Object.LogInformationWithCaller(message, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogWarningWithCaller_LogAtWarningLevel()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test warning message";
        const string filePath = "test.cs";
        const string memberName = "TestMethod";

        mockLogger.Object.LogWarningWithCaller(message, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogErrorWithCaller_LogAtErrorLevel()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test error message";
        const string filePath = "test.cs";
        const string memberName = "TestMethod";

        mockLogger.Object.LogErrorWithCaller(message, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogCriticalWithCaller_LogAtCriticalLevel()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test critical message";
        const string filePath = "test.cs";
        const string memberName = "TestMethod";

        mockLogger.Object.LogCriticalWithCaller(message, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Critical),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogExceptionWithCaller_LogExceptionWithErrorLevel()
    {
        var mockLogger = new Mock<ILogger>();
        var exception = new InvalidOperationException("Test exception");
        const string filePath = "test.cs";
        const string memberName = "TestMethod";

        mockLogger.Object.LogExceptionWithCaller(exception, null, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            exception,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogExceptionWithCaller_LogWithCustomMessage()
    {
        var mockLogger = new Mock<ILogger>();
        var exception = new InvalidOperationException("Original exception message");
        const string customMessage = "Custom error message";
        const string filePath = "test.cs";
        const string memberName = "TestMethod";

        mockLogger.Object.LogExceptionWithCaller(exception, customMessage, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            exception,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogTraceWithCaller_CaptureCallerInformation()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test message";
        const string filePath = @"D:\Project\File.cs";
        const string memberName = "MethodName";

        mockLogger.Object.LogTraceWithCaller(message, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Trace),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);

        var invocations = mockLogger.Invocations;
        Assert.NotEmpty(invocations);
        var state = invocations[^1].Arguments[2];
        Assert.True(ValidateState(state, filePath, memberName, message));
    }

    [Fact]
    public void Should_LogDebugWithCaller_CaptureCallerInformation()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test message";
        const string filePath = @"D:\Project\File.cs";
        const string memberName = "MethodName";

        mockLogger.Object.LogDebugWithCaller(message, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Debug),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);

        var invocations = mockLogger.Invocations;
        Assert.NotEmpty(invocations);
        var state = invocations[^1].Arguments[2];
        Assert.True(ValidateState(state, filePath, memberName, message));
    }

    [Fact]
    public void Should_LogInformationWithCaller_CaptureCallerInformation()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test message";
        const string filePath = @"D:\Project\File.cs";
        const string memberName = "MethodName";

        mockLogger.Object.LogInformationWithCaller(message, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);

        var invocations = mockLogger.Invocations;
        Assert.NotEmpty(invocations);
        var state = invocations[^1].Arguments[2];
        Assert.True(ValidateState(state, filePath, memberName, message));
    }

    [Fact]
    public void Should_LogWarningWithCaller_CaptureCallerInformation()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test message";
        const string filePath = @"D:\Project\File.cs";
        const string memberName = "MethodName";

        mockLogger.Object.LogWarningWithCaller(message, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);

        var invocations = mockLogger.Invocations;
        Assert.NotEmpty(invocations);
        var state = invocations[^1].Arguments[2];
        Assert.True(ValidateState(state, filePath, memberName, message));
    }

    [Fact]
    public void Should_LogErrorWithCaller_CaptureCallerInformation()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test message";
        const string filePath = @"D:\Project\File.cs";
        const string memberName = "MethodName";

        mockLogger.Object.LogErrorWithCaller(message, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);

        var invocations = mockLogger.Invocations;
        Assert.NotEmpty(invocations);
        var state = invocations[^1].Arguments[2];
        Assert.True(ValidateState(state, filePath, memberName, message));
    }

    [Fact]
    public void Should_LogCriticalWithCaller_CaptureCallerInformation()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test message";
        const string filePath = @"D:\Project\File.cs";
        const string memberName = "MethodName";

        mockLogger.Object.LogCriticalWithCaller(message, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Critical),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);

        var invocations = mockLogger.Invocations;
        Assert.NotEmpty(invocations);
        var state = invocations[^1].Arguments[2];
        Assert.True(ValidateState(state, filePath, memberName, message));
    }

    [Fact]
    public void Should_LogExceptionWithCaller_CaptureCallerInformationWithException()
    {
        var mockLogger = new Mock<ILogger>();
        var exception = new InvalidOperationException("Test exception");
        const string message = "Error occurred";
        const string filePath = @"D:\Project\File.cs";
        const string memberName = "MethodName";

        mockLogger.Object.LogExceptionWithCaller(exception, message, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            exception,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);

        var invocations = mockLogger.Invocations;
        Assert.NotEmpty(invocations);
        var state = invocations[^1].Arguments[2];
        Assert.True(ValidateState(state, filePath, memberName, message));
    }

    [Fact]
    public void Should_LogExceptionWithCaller_UseExceptionMessageWhenCustomMessageIsNull()
    {
        var mockLogger = new Mock<ILogger>();
        var exception = new InvalidOperationException("Exception message");
        const string filePath = @"D:\Project\File.cs";
        const string memberName = "MethodName";

        mockLogger.Object.LogExceptionWithCaller(exception, null, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            exception,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);

        var invocations = mockLogger.Invocations;
        Assert.NotEmpty(invocations);
        var state = invocations[^1].Arguments[2];
        Assert.True(ValidateState(state, filePath, memberName, "Exception message"));
    }

    [Fact]
    public void Should_LogTraceWithCaller_HandleDefaultCallerValues()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test message";

        mockLogger.Object.LogTraceWithCaller(message);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Trace),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogDebugWithCaller_HandleDefaultCallerValues()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test message";

        mockLogger.Object.LogDebugWithCaller(message);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Debug),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogInformationWithCaller_HandleDefaultCallerValues()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test message";

        mockLogger.Object.LogInformationWithCaller(message);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogWarningWithCaller_HandleDefaultCallerValues()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test message";

        mockLogger.Object.LogWarningWithCaller(message);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Warning),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogErrorWithCaller_HandleDefaultCallerValues()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test message";

        mockLogger.Object.LogErrorWithCaller(message);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogCriticalWithCaller_HandleDefaultCallerValues()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "Test message";

        mockLogger.Object.LogCriticalWithCaller(message);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Critical),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogTraceWithCaller_HandleEmptyMessage()
    {
        var mockLogger = new Mock<ILogger>();
        const string message = "";
        const string filePath = "test.cs";
        const string memberName = "TestMethod";

        mockLogger.Object.LogTraceWithCaller(message, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Trace),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            null,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public void Should_LogExceptionWithCaller_HandleExceptionWithoutMessage()
    {
        var mockLogger = new Mock<ILogger>();
        var exception = new InvalidOperationException();
        const string filePath = "test.cs";
        const string memberName = "TestMethod";

        mockLogger.Object.LogExceptionWithCaller(exception, null, filePath, memberName);

        mockLogger.Verify(l => l.Log(
            It.Is<LogLevel>(level => level == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            exception,
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    private static bool ValidateState(object state, string expectedFilePath, string expectedMemberName, string expectedMessage)
    {
        if (state is not KeyValuePair<string, object>[] kvpArray)
        {
            return false;
        }

        var hasFilePath = kvpArray.Any(kvp => kvp.Key == "CallerFilePath" && (string)kvp.Value == expectedFilePath);
        var hasMemberName = kvpArray.Any(kvp => kvp.Key == "CallerMemberName" && (string)kvp.Value == expectedMemberName);
        var hasMessage = kvpArray.Any(kvp => kvp.Key == "OriginalMessage" && (string)kvp.Value == expectedMessage);

        return hasFilePath && hasMemberName && hasMessage;
    }
}

