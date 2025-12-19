+++
title = 'Dotnet Logging'
+++

# Documentation

A flexible and feature-rich logging library for .NET applications. This library provides multiple logger implementations (Console and File), automatic caller information capture, custom log levels, and seamless integration with Microsoft.Extensions.Logging.

## Features

- **Multiple Logger Implementations**: Console and File loggers with customizable configurations
- **Automatic Caller Information**: Capture file path and method name automatically using compiler attributes
- **Custom Log Levels**: 7 severity levels - Trace, Debug, Information, Warning, Error, Exception, and Critical
- **Color-Coded Console Output**: ANSI color support for better console readability on Windows and Unix-like systems
- **Flexible File Logging**: Configurable log file output with options for splitting logs
- **Trace ID Support**: Built-in correlation ID tracking for distributed tracing
- **Microsoft.Extensions.Logging Integration**: Seamless integration with ASP.NET Core and other frameworks using the standard logging abstractions
- **Standalone and State Loggers**: Use independently or as part of a larger logging state management system

## Installation

### NuGet Package

Install the package from NuGet:

```bash
dotnet add package ArturRios.Logging
```

Or via the Package Manager:

```
Install-Package ArturRios.Logging
```

### GitHub Submodule

You can also include this repository as a submodule in your project:

```bash
git submodule add https://github.com/artur-rios/dotnet-logging.git dotnet-logging
```

## Quick Start

### Standalone Logger

The simplest way to get started is using the `StandaloneLogger`:

```csharp
using ArturRios.Logging;
using ArturRios.Logging.Configuration;

// Create console logger configuration
var consoleConfig = new ConsoleLoggerConfiguration
{
    // Controls ANSI color usage in console output
    UseColors = true
};

// Initialize standalone logger
var logger = new StandaloneLogger(new List<LoggerConfiguration> { consoleConfig });

// Log messages
logger.Info("Application started");
logger.Debug("Debug information");
logger.Error("An error occurred");
```

### File Logger

Log to files with configurable output:

```csharp
var fileConfig = new FileLoggerConfiguration
{
    ApplicationName = "MyApp",                  // Required: used in log file names
    FolderScheme = LogFolderScheme.ByMonth,      // Organize logs by month
    FileSplitLevel = LogSplitLevel.Day,          // Split logs by day
    FilePath = "./logs"                         // Base path for log files (optional)
};

var logger = new StandaloneLogger(new List<LoggerConfiguration> { fileConfig });

logger.Info("This will be written to a file");
```

### Multiple Loggers

Use multiple loggers simultaneously:

```csharp
var configurations = new List<LoggerConfiguration>
{
    new ConsoleLoggerConfiguration
    {
        // Controls ANSI color usage in console output
        UseColors = true
    },
    new FileLoggerConfiguration
    {
        ApplicationName = "MyApp",              // Required
        FolderScheme = LogFolderScheme.ByMonth,
        FileSplitLevel = LogSplitLevel.Day,
        FilePath = "./logs"
    }
};

var logger = new StandaloneLogger(configurations);

// Messages will be logged to both console and file based on their log level
logger.Warn("This warning appears in both console and file");
```

### Microsoft.Extensions.Logging Integration

Integrate with ASP.NET Core or other frameworks using the standard logging abstractions:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ArturRios.Logging.Adapter;

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddCustomLogger();
});

var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILogger<MyClass>>();

logger.LogInformation("Integrated with ASP.NET Core");
```

## Configuration

### ConsoleLoggerConfiguration

```csharp
var config = new ConsoleLoggerConfiguration
{
    // Enable or disable ANSI color output in the console
    UseColors = true
};
```

### FileLoggerConfiguration

```csharp
var config = new FileLoggerConfiguration
{
    ApplicationName = "MyApp",                  // Required: used in log file names
    FolderScheme = LogFolderScheme.ByMonth,      // Organize logs by month
    FileSplitLevel = LogSplitLevel.Day,          // Split logs by day
    FilePath = "./logs"                         // Base path for log files (optional)
};
```

### Log Levels

The library supports the following log levels in order of severity:

1. **Trace** - Most detailed, diagnostic-level logging
2. **Debug** - Debug and development information
3. **Information** - General informational messages
4. **Warning** - Potential issues or unexpected behavior
5. **Error** - Errors that occurred during execution
6. **Exception** - Exceptions that were thrown
7. **Critical** - Critical errors requiring immediate attention

## State Logger

The `StateLogger` class allows you to manage logging state across your application:

```csharp
using ArturRios.Logging;

var stateLogger = new StateLogger();
stateLogger.SetTraceId("trace-123");
stateLogger.Log("Operation started");
stateLogger.ClearTraceId();
```

## Automatic Caller Information

The library automatically captures the calling file and method name without any additional configuration:

```csharp
logger.Info("User logged in");
// Automatically captures: filename and method name where this call was made
```

## Trace ID Support

Track related log entries across operations with trace IDs:

```csharp
var logger = new StandaloneLogger(configurations);
logger.TraceId = "request-uuid-12345";

logger.Info("Processing request");  // Will include trace ID in output
logger.Debug("Step 1 complete");    // Will include trace ID in output
```

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests to improve this project.

## Versioning

Semantic Versioning (SemVer). Breaking changes result in a new major version. New methods or non-breaking behavior
changes increment the minor version; fixes or tweaks increment the patch.

## Build, test and publish

Use the official [.NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/) to build, test and publish the project and Git for source control.
If you want, optional helper toolsets I built to facilitate these tasks are available:

- [Dotnet Tools](https://github.com/artur-rios/dotnet-tools)
- [Python Dotnet Tools](https://github.com/artur-rios/python-dotnet-tools)

## Legal Details

This project is licensed under the [MIT License](https://en.wikipedia.org/wiki/MIT_License). A copy of the license is available at [LICENSE](./LICENSE) in the repository.
