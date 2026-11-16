namespace ArturRios.Logging.Tests;

[Trait("Category", "Unit")]
public class ConsoleAnsiTests
{
    [Fact]
    public void GivenAPlatformOtherThanWindows_WhenEnablingVirtualTerminalProcessing_ThenItReportsFalse()
    {
        // On Windows the answer depends on the console the process has, which a test run does not control.
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        Assert.False(ConsoleAnsi.EnableVirtualTerminalProcessing());
    }

    [Fact]
    public void GivenAnOutputThatIsNotAConsole_WhenEnablingVirtualTerminalProcessing_ThenItReportsFalse()
    {
        // The test host's standard output is a pipe, on Windows too, so there is no console mode to read or set.
        if (!Console.IsOutputRedirected)
        {
            return;
        }

        Assert.False(ConsoleAnsi.EnableVirtualTerminalProcessing());
    }
}
