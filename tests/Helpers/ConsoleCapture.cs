namespace ArturRios.Logging.Tests.Helpers;

/// <summary>
/// The test classes that replace the process-wide <see cref="Console.Out"/>. The collection runs on its own, after
/// every other one, so nothing else writes to the console while a test reads what it captured.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ConsoleOutputCollection
{
    /// <summary>The collection's name, for <see cref="CollectionAttribute"/>.</summary>
    public const string Name = "Console output";
}

/// <summary>Sends <see cref="Console.Out"/> to a <see cref="StringWriter"/> until disposed, then puts it back.</summary>
public sealed class ConsoleCapture : IDisposable
{
    private readonly TextWriter _original = Console.Out;
    private readonly StringWriter _writer = new();

    public ConsoleCapture() => Console.SetOut(_writer);

    /// <summary>Everything written to the console since the capture began.</summary>
    public string Output => _writer.ToString();

    public void Dispose()
    {
        Console.SetOut(_original);
        _writer.Dispose();
    }
}
