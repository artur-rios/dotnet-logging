using ArturRios.Logging.Interfaces;

namespace ArturRios.Logging.Tests.Helpers;

/// <summary>An <see cref="IInternalLogger"/> that records every call, for the standalone and state logger tests.</summary>
internal sealed class RecordingInternalLogger : IInternalLogger
{
    public readonly List<(CustomLogLevel Level, string Message, string File, string Method)> Calls = [];

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
