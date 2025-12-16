using ArturRios.Logging.Configuration;
using ArturRios.Logging.Factories;
using ArturRios.Logging.Interfaces;
using ArturRios.Util.Collections;

namespace ArturRios.Logging.Loggers;

public class ConsoleLogger(ConsoleLoggerConfiguration configuration) : IInternalLogger
{
    private static readonly Lock s_writeLock = new();

    /// <inheritdoc />
    public void Trace(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Trace, filePath, methodName, message);
    }

    /// <inheritdoc />
    public void Debug(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Debug, filePath, methodName, message);
    }

    /// <inheritdoc />
    public void Info(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Information, filePath, methodName, message);
    }

    /// <inheritdoc />
    public void Warn(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Warning, filePath, methodName, message);
    }

    /// <inheritdoc />
    public void Error(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Error, filePath, methodName, message);
    }

    /// <inheritdoc />
    public void Exception(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Exception, filePath, methodName, message);
    }

    /// <inheritdoc />
    public void Critical(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Critical, filePath, methodName, message);
    }

    /// <inheritdoc />
    public void Fatal(string message, string filePath, string methodName)
    {
        Write(CustomLogLevel.Fatal, filePath, methodName, message);
    }

    private static string GetAnsiColorSequence(CustomLogLevel level) => level switch
    {
        CustomLogLevel.Trace => AnsiColors.DarkGray,
        CustomLogLevel.Debug => AnsiColors.Cyan,
        CustomLogLevel.Information => AnsiColors.Green,
        CustomLogLevel.Warning => AnsiColors.Yellow,
        CustomLogLevel.Error => AnsiColors.Red,
        CustomLogLevel.Exception => AnsiColors.Magenta,
        CustomLogLevel.Critical or CustomLogLevel.Fatal => AnsiColors.BrightRed,
        _ => AnsiColors.White
    };

    private void Write(CustomLogLevel level, string filePath, string methodName, string message)
    {
        var entry = LogEntryFactory.Create(level, filePath, methodName, message);

        if (configuration.UseColors)
        {
            _ = ConsoleAnsi.EnableVirtualTerminalProcessing();

            var ansiColor = GetAnsiColorSequence(level);

            const string colorReset = "\e[0m";

            lock (s_writeLock)
            {
                Console.Write(ansiColor + entry + colorReset);
            }
        }
        else
        {
            lock (s_writeLock)
            {
                Console.Write(entry);
            }
        }
    }
}
