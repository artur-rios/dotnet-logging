using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly:InternalsVisibleTo("ArturRios.Logging.Tests")]

namespace ArturRios.Logging;

internal static partial class ConsoleAnsi
{
    private const int StdOutputHandle = -11;
    private const uint EnableVirtualTerminalProcessingHex = 0x0004;
    private const uint DisableNewlineAutoReturn = 0x0008;

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr GetStdHandle(int nStdHandle);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    /// <summary>
    /// Enables virtual terminal processing for the console to support ANSI escape sequences.
    /// </summary>
    /// <returns>True if virtual terminal processing was successfully enabled or already enabled; otherwise, false.</returns>
    public static bool EnableVirtualTerminalProcessing()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return false;
        }

        var handle = GetStdHandle(StdOutputHandle);
        if (handle == IntPtr.Zero)
        {
            return false;
        }

        if (!GetConsoleMode(handle, out var mode))
        {
            return false;
        }

        if ((mode & EnableVirtualTerminalProcessingHex) != 0)
        {
            return true;
        }

        var newMode = mode | EnableVirtualTerminalProcessingHex | DisableNewlineAutoReturn;

        return SetConsoleMode(handle, newMode);
    }
}
