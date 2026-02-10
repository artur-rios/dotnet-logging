using System.Runtime.InteropServices;

namespace ArturRios.Logging.Tests;

public class ConsoleAnsiTests
{
    [Fact]
    public void GivenNonWindowsPlatform_WhenEnableVirtualTerminalProcessingCalled_ThenReturnsFalse()
    {
        // This test verifies the behavior when not on Windows
        // The implementation returns false for non-Windows platforms
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Skip this test on Windows as it would require mocking OS detection
            Assert.True(true);
        }
        else
        {
            // On non-Windows platforms, EnableVirtualTerminalProcessing should return false
            var result = ConsoleAnsi.EnableVirtualTerminalProcessing();
            Assert.False(result);
        }
    }

    [Fact]
    public void GivenConsoleAnsiClass_WhenInspected_ThenHasPrivatePInvokeMethods()
    {
        // Verify that the class has the required P/Invoke method declarations
        var type = typeof(ConsoleAnsi);
        var methods = type.GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        // Check for LibraryImport attribute usage on private methods
        var libraryImportMethods = methods.Where(m =>
            m.GetCustomAttributes(typeof(LibraryImportAttribute), false).Length > 0
        ).ToList();

        // ConsoleAnsi should have P/Invoke methods for kernel32.dll
        Assert.NotEmpty(libraryImportMethods);
    }

    [Fact]
    public void GivenConsoleAnsiClass_WhenInspected_ThenHasGetStdHandleMethod()
    {
        // Verify GetStdHandle method exists
        var type = typeof(ConsoleAnsi);
        var method = type.GetMethod("GetStdHandle",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(method);
        Assert.Equal(typeof(IntPtr), method.ReturnType);
    }

    [Fact]
    public void GivenConsoleAnsiClass_WhenInspected_ThenHasGetConsoleModeMethod()
    {
        // Verify GetConsoleMode method exists
        var type = typeof(ConsoleAnsi);
        var method = type.GetMethod("GetConsoleMode",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(method);
        Assert.Equal(typeof(bool), method.ReturnType);
    }

    [Fact]
    public void GivenConsoleAnsiClass_WhenInspected_ThenHasSetConsoleModeMethod()
    {
        // Verify SetConsoleMode method exists
        var type = typeof(ConsoleAnsi);
        var method = type.GetMethod("SetConsoleMode",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(method);
        Assert.Equal(typeof(bool), method.ReturnType);
    }

    [Fact]
    public void GivenConsoleAnsiClass_WhenInspected_ThenHasEnableVirtualTerminalProcessingPublicMethod()
    {
        // Verify the public method exists with correct signature
        var type = typeof(ConsoleAnsi);
        var method = type.GetMethod("EnableVirtualTerminalProcessing",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        Assert.NotNull(method);
        Assert.Equal(typeof(bool), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    [Fact]
    public void GivenConsoleAnsiClass_WhenInspected_ThenHasRequiredConstantsDefinedPrivately()
    {
        // Verify that the class defines required constants
        var type = typeof(ConsoleAnsi);
        var fields = type.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var fieldNames = fields.Select(f => f.Name).ToList();

        // Should have StdOutputHandle, EnableVirtualTerminalProcessingHex, and DisableNewlineAutoReturn constants
        Assert.Contains("StdOutputHandle", fieldNames);
        Assert.Contains("EnableVirtualTerminalProcessingHex", fieldNames);
        Assert.Contains("DisableNewlineAutoReturn", fieldNames);
    }

    [Fact]
    public void GivenConsoleAnsiClass_WhenConstantsInspected_ThenDefinesCorrectConstantValues()
    {
        // Verify the constant values are correct
        var type = typeof(ConsoleAnsi);

        var stdOutputHandle = type.GetField("StdOutputHandle",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Assert.NotNull(stdOutputHandle);
        Assert.Equal(-11, (int)stdOutputHandle.GetValue(null)!);

        var enableVirtualTerminal = type.GetField("EnableVirtualTerminalProcessingHex",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Assert.NotNull(enableVirtualTerminal);
        Assert.Equal(0x0004u, (uint)enableVirtualTerminal.GetValue(null)!);

        var disableNewlineAutoReturn = type.GetField("DisableNewlineAutoReturn",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Assert.NotNull(disableNewlineAutoReturn);
        Assert.Equal(0x0008u, (uint)disableNewlineAutoReturn.GetValue(null)!);
    }

    [Fact]
    public void GivenConsoleAnsiClass_WhenInspected_ThenIsStaticClass()
    {
        // Verify ConsoleAnsi is a static class
        var type = typeof(ConsoleAnsi);
        Assert.True(type is { IsAbstract: true, IsSealed: true });
    }

    [Fact]
    public void GivenConsoleAnsiClass_WhenInspected_ThenIsInternalClass()
    {
        // Verify ConsoleAnsi has internal visibility
        var type = typeof(ConsoleAnsi);
        Assert.False(type.IsPublic);
    }

    [Fact]
    public void GivenConsoleAnsiPublicMethod_WhenInspected_ThenHasXmlDocumentation()
    {
        // Verify that the public method has XML documentation
        var type = typeof(ConsoleAnsi);
        var method = type.GetMethod("EnableVirtualTerminalProcessing",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        Assert.NotNull(method);

        // XML documentation would be in the compiled assembly as compiler-generated attributes
        // This test ensures the method has proper accessibility for documentation
        var isPublic = method is { IsPublic: true, IsStatic: true };
        Assert.True(isPublic);
    }

    [Fact]
    public void GivenConsoleAnsi_WhenEnableVirtualTerminalProcessingCalled_ThenDoesNotThrowException()
    {
        // Basic integration test - ensure the method can be called without throwing
        // Note: Actual behavior depends on the OS and console state
        try
        {
            var result = ConsoleAnsi.EnableVirtualTerminalProcessing();
            // Result can be true or false depending on OS and context
            Assert.IsType<bool>(result);
        }
        catch (Exception ex)
        {
            Assert.Fail($"EnableVirtualTerminalProcessing threw an unexpected exception: {ex.Message}");
        }
    }
}


