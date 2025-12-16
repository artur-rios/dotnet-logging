using Microsoft.Extensions.Logging;

namespace ArturRios.Logging.Adapter;

/// <summary>
/// Logger provider that creates instances of <see cref="MicrosoftLoggerAdapter"/>.
/// </summary>
public class MicrosoftLoggerProvider(IServiceProvider serviceProvider) : ILoggerProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName) => new MicrosoftLoggerAdapter(_serviceProvider);

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
