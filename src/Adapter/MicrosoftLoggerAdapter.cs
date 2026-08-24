using System.Diagnostics;
using ArturRios.Logging.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArturRios.Logging.Adapter;

/// <summary>
/// Bridges <see cref="ILogger"/> onto <see cref="IStateLogger"/>, so code written against
/// Microsoft.Extensions.Logging reaches this library's loggers.
/// </summary>
/// <remarks>
/// Each call resolves <see cref="IStateLogger"/> from a fresh dependency injection scope, so a scoped
/// dependency of the state logger is isolated per log entry. The correlation id therefore does not travel
/// through the scope; it is read from the current <see cref="HttpContext"/> instead, where the tracing
/// middleware puts it.
/// </remarks>
/// <param name="services">The provider used to resolve the state logger and the HTTP context accessor.</param>
/// <exception cref="ArgumentNullException"><paramref name="services"/> is <c>null</c>.</exception>
public class MicrosoftLoggerAdapter(IServiceProvider services) : ILogger
{
    private readonly IServiceProvider _services = services ?? throw new ArgumentNullException(nameof(services));

    /// <summary>
    /// Gets or sets the correlation id carried by the current <see cref="HttpContext"/>, or <c>null</c> when
    /// there is no HTTP context — outside a request, or when no <see cref="IHttpContextAccessor"/> is
    /// registered. Setting it outside a request is a no-op.
    /// </summary>
    public string? TraceId
    {
        get
        {
            var accessor = _services.GetService<IHttpContextAccessor>();
            return accessor?.HttpContext?.Items["TraceId"] as string;
        }
        set
        {
            var accessor = _services.GetService<IHttpContextAccessor>();

            accessor?.HttpContext?.Items["TraceId"] = value;
        }
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
        if (!IsEnabled(logLevel)) return;

        var message = formatter != null ? formatter(state!, exception) : state?.ToString() ?? string.Empty;

        // Resolve the scoped IStateLogger (if not registered you'll get null)
        using var scope = _services.CreateScope();
        var stateLogger = scope.ServiceProvider.GetService<IStateLogger>();
        var httpAccessor = scope.ServiceProvider.GetService<IHttpContextAccessor>();

        // Propagate TraceId from HttpContext (middleware should set this per request)
        if (stateLogger is not null && httpAccessor?.HttpContext?.Items["TraceId"] is string httpTrace)
        {
            stateLogger.TraceId = httpTrace;
        }

        if (stateLogger is null)
        {
            // No IStateLogger registered — nothing to forward to
            return;
        }

        // Enrich state with caller info if not present so StateLogger.ResolveCallerInfo can pick it up.
        object? enrichedState = state;
        if (!StateContainsCallerInfo(state))
        {
            var (filePath, memberName) = FindCallerFromStack();
            var kvList = new List<KeyValuePair<string, object>>();

            if (state is IEnumerable<KeyValuePair<string, object>> existingPairs)
            {
                foreach (var kv in existingPairs)
                {
                    kvList.Add(kv);
                }
            }

            kvList.Add(new KeyValuePair<string, object>("CallerFilePath", filePath));
            kvList.Add(new KeyValuePair<string, object>("CallerMemberName", memberName));
            kvList.Add(new KeyValuePair<string, object>("OriginalMessage", message));

            enrichedState = kvList.ToArray();
        }

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

    private static bool StateContainsCallerInfo(object? state)
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
                             string.Equals(k, "FilePath", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(k, "callerFilePath", StringComparison.OrdinalIgnoreCase)))
            {
                hasFile = true;
            }

            if (!hasMember && (string.Equals(k, "CallerMemberName", StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(k, "MemberName", StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(k, "Method", StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(k, "callerMemberName", StringComparison.OrdinalIgnoreCase)))
            {
                hasMember = true;
            }

            if (hasFile && hasMember) return true;
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
                if (method == null) continue;
                var declaring = method.DeclaringType;
                if (declaring == null) continue;

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

                var memberName = method.Name;
                var filePath = declaring.FullName ?? declaring.Name;

                return (filePath, memberName);
            }
        }
        catch
        {
            // swallow any errors and fall through to unknowns
        }

        return ("unknown", "unknown");
    }

    private class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();
        public void Dispose() { }
    }
}
