using System.Collections.Concurrent;
using ArturRios.Logging.Interfaces;

namespace ArturRios.Logging.Tests.Helpers;

public class CapturingStateLogger : IStateLogger
{
    public string? TraceId { get; set; }

    /// <summary>Every call, with the <see cref="TraceId"/> the logger carried when it was made.</summary>
    public readonly ConcurrentQueue<(string Method, string Message, object? State, string? TraceId)> Calls = new();
    public readonly ConcurrentQueue<Exception> Exceptions = new();

    /// <summary>The trace id the most recent call was made with.</summary>
    public string? LastTraceId => Calls.LastOrDefault().TraceId;

    public void Trace(string message, object? state = null) => Calls.Enqueue((nameof(Trace), message, state, TraceId));
    public void Debug(string message, object? state = null) => Calls.Enqueue((nameof(Debug), message, state, TraceId));
    public void Info(string message, object? state = null) => Calls.Enqueue((nameof(Info), message, state, TraceId));
    public void Warn(string message, object? state = null) => Calls.Enqueue((nameof(Warn), message, state, TraceId));
    public void Error(string message, object? state = null) => Calls.Enqueue((nameof(Error), message, state, TraceId));
    public void Exception(Exception exception, object? state = null)
    {
        Exceptions.Enqueue(exception);
        Calls.Enqueue((nameof(Exception), exception.Message, state, TraceId));
    }
    void IStateLogger.Exception(Exception exception, object? state) => Exception(exception, state);
    public void Critical(string message, object? state = null) => Calls.Enqueue((nameof(Critical), message, state, TraceId));
    public void Fatal(string message, object? state = null) => Calls.Enqueue((nameof(Fatal), message, state, TraceId));
}
