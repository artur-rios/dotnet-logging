using System.Collections.Concurrent;
using ArturRios.Logging.Interfaces;

namespace ArturRios.Logging.Tests.Helpers;

public class CapturingStateLogger : IStateLogger
{
    public string? TraceId { get; set; }

    public readonly ConcurrentQueue<(string Method, string Message, object? State)> Calls = new();
    public readonly ConcurrentQueue<Exception> Exceptions = new();

    public void Trace(string message, object? state = null) => Calls.Enqueue((nameof(Trace), message, state));
    public void Debug(string message, object? state = null) => Calls.Enqueue((nameof(Debug), message, state));
    public void Info(string message, object? state = null) => Calls.Enqueue((nameof(Info), message, state));
    public void Warn(string message, object? state = null) => Calls.Enqueue((nameof(Warn), message, state));
    public void Error(string message, object? state = null) => Calls.Enqueue((nameof(Error), message, state));
    public void Exception(Exception exception, object? state = null)
    {
        Exceptions.Enqueue(exception);
        Calls.Enqueue((nameof(Exception), exception.Message, state));
    }
    void IStateLogger.Exception(Exception exception, object? state) => Exception(exception, state);
    public void Critical(string message, object? state = null) => Calls.Enqueue((nameof(Critical), message, state));
    public void Fatal(string message, object? state = null) => Calls.Enqueue((nameof(Fatal), message, state));
}
