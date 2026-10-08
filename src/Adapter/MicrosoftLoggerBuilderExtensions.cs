using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace ArturRios.Logging.Adapter;

/// <summary>
/// Extension methods for configuring the custom logger with Microsoft.Extensions.Logging.
/// </summary>
public static class MicrosoftLoggerBuilderExtensions
{
    /// <summary>
    /// Adds the custom logger provider to the logging builder.
    /// </summary>
    /// <param name="builder">The logging builder to configure.</param>
    /// <returns>The logging builder for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when builder is null.</exception>
    public static ILoggingBuilder AddCustomLogger(this ILoggingBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // TryAddEnumerable: a second call must not register a second provider, which would write every entry twice.
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, MicrosoftLoggerProvider>());

        return builder;
    }
}
