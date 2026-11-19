using System.Diagnostics;
using ArturRios.Logging.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArturRios.Logging.Adapter;

/// <summary>
/// Bridges <see cref="ILogger"/> onto <see cref="IStateLogger"/>, so code written against
/// Microsoft.Extensions.Logging reaches this library's loggers.
/// </summary>
/// <remarks>
/// <para>
/// Each call resolves <see cref="IStateLogger"/> from a fresh dependency injection scope, so a scoped
/// dependency of the state logger is isolated per log entry. The correlation id therefore does not travel
/// through the scope; it is read from the ambient <see cref="Activity"/> instead.
/// </para>
/// <para>
/// <see cref="Activity"/> is the correlation primitive of the platform, not of any one hosting model: an
/// ASP.NET Core request has one, and so does a worker, a console app or a test. Reading the id from there
/// is what lets this library log correlated entries without depending on ASP.NET Core.
/// </para>
/// </remarks>
/// <param name="services">The provider used to resolve the state logger.</param>
/// <exception cref="ArgumentNullException"><paramref name="services"/> is <c>null</c>.</exception>
public class MicrosoftLoggerAdapter(IServiceProvider services) : ILogger
{
    /// <summary>
    /// A correlation id set explicitly by a caller, overriding the ambient activity's. Held in an
    /// <see cref="AsyncLocal{T}"/> so it flows with the execution context — per request under a server,
    /// per logical operation anywhere else — rather than leaking across concurrent work.
    /// </summary>
    private static readonly AsyncLocal<string?> Override = new();

    private readonly IServiceProvider _services = services ?? throw new ArgumentNullException(nameof(services));

    /// <summary>
    /// Gets or sets the correlation id stamped on log entries.
    /// </summary>
    /// <value>
    /// The id set explicitly on this property, or the trace id of <see cref="Activity.Current"/> when none
    /// was set, or <c>null</c> when there is no ambient activity either.
    /// </value>
    /// <remarks>
    /// Setting it establishes the id for the current execution context and everything that flows from it.
    /// <c>ArturRios.Util.WebApi</c>'s <c>TraceActivityMiddleware</c> starts a W3C activity per request and
    /// derives its own trace id from exactly this activity, so under that middleware the value read here is
    /// the same one the middleware publishes — with no ASP.NET Core dependency on this side.
    /// </remarks>
    public string? TraceId
    {
        get => Override.Value ?? Activity.Current?.TraceId.ToString();
        set => Override.Value = value;
    }

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    /// <inheritdoc />
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string>? formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter != null ? formatter(state!, exception) : state?.ToString() ?? string.Empty;

        // Resolve the scoped IStateLogger (if not registered you'll get null)
        using var scope = _services.CreateScope();
        var stateLogger = scope.ServiceProvider.GetService<IStateLogger>();

        if (stateLogger is null)
        {
            // No IStateLogger registered — nothing to forward to
            return;
        }

        // Enrich state with caller info if not present so StateLogger.ResolveCallerInfo can pick it up.
        // A message template's state (ILogger.LogInformation("HTTP {Method} {Path}", …) and LoggerMessage
        // alike) carries the template under {OriginalFormat}, and its other keys are the placeholders' values:
        // a {Method} or {FilePath} there is data, not the caller. For such a state only CallerFilePath and
        // CallerMemberName count as caller information, and the caller's keys go first, since StateLogger takes
        // the first key that names each part. A state built by hand names the caller under any key StateLogger
        // accepts, and keeps its order.
        object? enrichedState = state;
        var isTemplate = IsMessageTemplateState(state);
        if (!StateContainsCallerInfo(state, isTemplate))
        {
            var (filePath, memberName) = FindCallerFromStack();
            KeyValuePair<string, object>[] caller =
            [
                new("CallerFilePath", filePath),
                new("CallerMemberName", memberName)
            ];
            var kvList = new List<KeyValuePair<string, object>>();

            if (isTemplate)
            {
                kvList.AddRange(caller);
            }

            if (state is IEnumerable<KeyValuePair<string, object>> existingPairs)
            {
                foreach (var kv in existingPairs)
                {
                    kvList.Add(kv);
                }
            }

            if (!isTemplate)
            {
                kvList.AddRange(caller);
            }

            kvList.Add(new KeyValuePair<string, object>("OriginalMessage", message));

            enrichedState = kvList.ToArray();
        }

        // The ambient correlation id, whether set explicitly or carried by the current activity, reaches the
        // output through IStateLogger.TraceId — a plain property. A singleton state logger is shared by every
        // entry, so the id is set, used and put back under a lock on the instance: otherwise a concurrent entry
        // could be written with this entry's id, and a later entry without an ambient id would inherit it.
        // A scoped state logger is a fresh instance per entry here, so the lock is never contended.
        lock (stateLogger)
        {
            var previousTraceId = stateLogger.TraceId;

            if (TraceId is { } ambientTraceId)
            {
                stateLogger.TraceId = ambientTraceId;
            }

            try
            {
                Forward(stateLogger, logLevel, message, exception, enrichedState);
            }
            finally
            {
                stateLogger.TraceId = previousTraceId;
            }
        }
    }

    private static void Forward(IStateLogger stateLogger, LogLevel logLevel, string message, Exception? exception,
        object? enrichedState)
    {
        if (exception is not null)
        {
            stateLogger.Exception(exception, enrichedState);
        }

        switch (logLevel)
        {
            case LogLevel.Trace:
                stateLogger.Trace(message, enrichedState);
                break;
            case LogLevel.Debug:
                stateLogger.Debug(message, enrichedState);
                break;
            case LogLevel.Information:
                stateLogger.Info(message, enrichedState);
                break;
            case LogLevel.Warning:
                stateLogger.Warn(message, enrichedState);
                break;
            case LogLevel.Error:
                stateLogger.Error(message, enrichedState);
                break;
            case LogLevel.Critical:
                stateLogger.Critical(message, enrichedState);
                break;
            case LogLevel.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }
    }

    private const string OriginalFormatKey = "{OriginalFormat}";

    private static bool IsMessageTemplateState(object? state) =>
        state is IEnumerable<KeyValuePair<string, object?>> pairs &&
        pairs.Any(pair => string.Equals(pair.Key, OriginalFormatKey, StringComparison.Ordinal));

    private static bool StateContainsCallerInfo(object? state, bool isTemplate)
    {
        if (state is not IEnumerable<KeyValuePair<string, object?>> pairs)
        {
            return false;
        }

        bool hasFile = false, hasMember = false;

        foreach (var (k, v) in pairs)
        {
            if (v is null)
            {
                continue;
            }

            if (!hasFile && (string.Equals(k, "CallerFilePath", StringComparison.OrdinalIgnoreCase) ||
                             (!isTemplate && string.Equals(k, "FilePath", StringComparison.OrdinalIgnoreCase))))
            {
                hasFile = true;
            }

            if (!hasMember && (string.Equals(k, "CallerMemberName", StringComparison.OrdinalIgnoreCase) ||
                               (!isTemplate && (string.Equals(k, "MemberName", StringComparison.OrdinalIgnoreCase) ||
                                                string.Equals(k, "Method", StringComparison.OrdinalIgnoreCase)))))
            {
                hasMember = true;
            }

            if (hasFile && hasMember)
                return true;
        }

        return false;
    }

    private static (string filePath, string memberName) FindCallerFromStack()
    {
        try
        {
            var st = new StackTrace(skipFrames: 1, fNeedFileInfo: false);

            for (var i = 0; i < st.FrameCount; i++)
            {
                var frame = st.GetFrame(i);
                var method = frame?.GetMethod();
                if (method == null)
                    continue;
                var declaring = method.DeclaringType;
                if (declaring == null)
                    continue;

                var ns = declaring.Namespace ?? string.Empty;

                // Skip known logging infrastructure namespaces so we find the real caller
                // ArturRios.Logging is this library's own namespace. The predecessor of this check named
                // ArturRios.Common.Logging, which no longer exists, so nothing here was ever skipped and the
                // frame reported as the caller was MicrosoftLoggerAdapter.Log itself.
                if (ns.StartsWith("Microsoft.Extensions.Logging", StringComparison.Ordinal) ||
                    ns.Equals("ArturRios.Logging", StringComparison.Ordinal) ||
                    ns.StartsWith("ArturRios.Logging.", StringComparison.Ordinal) ||
                    ns.StartsWith("System.", StringComparison.Ordinal))
                {
                    continue;
                }

                return SourceCaller(declaring, method.Name);
            }
        }
        catch
        {
            // swallow any errors and fall through to unknowns
        }

        return ("unknown", "unknown");
    }

    /// <summary>
    /// The class and method a frame belongs to in the source, in the shape <c>[CallerFilePath]</c> and
    /// <c>[CallerMemberName]</c> give them. The entry's class is the file path's name without its extension,
    /// so the class goes in as a bare name: a full name such as <c>MyApp.Orders.OrderService</c> would lose
    /// its last segment, the class, as an "extension".
    /// </summary>
    /// <remarks>
    /// The compiler moves an async method or iterator into a nested state machine (<c>&lt;PlaceAsync&gt;d__3</c>,
    /// whose method is <c>MoveNext</c>), a lambda into a closure class or a method named
    /// <c>&lt;Place&gt;b__0_0</c>, and a local function into <c>&lt;Place&gt;g__Local|0_0</c>. The source method
    /// is the name between the angle brackets, and the class is the first type out from them that the source
    /// declared.
    /// </remarks>
    private static (string filePath, string memberName) SourceCaller(Type declaring, string memberName)
    {
        var type = declaring;

        while (IsCompilerGenerated(type) && type.DeclaringType is { } outer)
        {
            if (memberName is "MoveNext" or "SetStateMachine" or "MoveNextAsync" or "DisposeAsync" &&
                SourceName(type.Name) is { } stateMachineOwner)
            {
                memberName = stateMachineOwner;
            }

            type = outer;
        }

        memberName = SourceName(memberName) ?? memberName;

        var className = type.Name;
        var arity = className.IndexOf('`');

        return (arity > 0 ? className[..arity] : className, memberName);
    }

    private static bool IsCompilerGenerated(Type type) => type.Name.StartsWith('<');

    /// <summary>The name inside a compiler-generated <c>&lt;Name&gt;…</c>, or <c>null</c> for any other name.</summary>
    private static string? SourceName(string name)
    {
        if (!name.StartsWith('<'))
            return null;

        var close = name.IndexOf('>');

        return close > 1 ? name[1..close] : null;
    }

    private class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();
        public void Dispose() { }
    }
}
