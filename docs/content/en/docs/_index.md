---
title: Documentation
linkTitle: Documentation
weight: 20
description: >-
  A flexible and feature-rich logging library for .NET applications. This library provides multiple logger implementations (Console and File), automatic caller...
---

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

```bash
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
8. **Fatal** - Unrecoverable errors that terminate the application

### Folder schemes and split levels

`FolderScheme` decides the directory a log file is written to, and `FileSplitLevel` decides its name.

| `LogFolderScheme` | Directory under the base path |
|---|---|
| `AllInOne` | the base path itself |
| `ByYear` | `2026/` |
| `ByMonth` | `2026/08/` |
| `ByDay` | `2026/08/24/` |
| `ByHour` | `2026/08/24/13/` |
| `ByRequest` | one folder per `FileLogger` instance |

`ByRequest` names the folder once per logger instance, so registering the logger with a **scoped** lifetime
gives one folder per request; a singleton gives one folder per process.

| `LogSplitLevel` | File name |
|---|---|
| `Request` | `MyApp.log` |
| `Year` | `MyApp_2026.log` |
| `Month` | `MyApp_2026_08.log` |
| `Day` | `MyApp_2026_08_24.log` |
| `Hour` | `MyApp_2026_08_24_13.log` |

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

### Through Microsoft.Extensions.Logging

`MicrosoftLoggerAdapter` stamps a correlation id on everything it forwards, taken from the ambient
[`Activity`](https://learn.microsoft.com/dotnet/api/system.diagnostics.activity):

```csharp
using var activity = new Activity("ProcessOrder").SetIdFormat(ActivityIdFormat.W3C).Start();

logger.LogInformation("Processing order");  // carries activity.TraceId
```

`Activity` is the platform's correlation primitive, not any one hosting model's, so this works the same in
a web application, a worker, a console app and a test. Nested activities share a trace id, so every entry
from one logical operation correlates.

To override it — a message id from a queue, a correlation id from an upstream header — assign it:

```csharp
adapter.TraceId = messageId;
```

The value is held in an `AsyncLocal<string?>`, so it applies to the current execution context and
everything that flows from it, and does not leak into concurrent work. It takes precedence over the
activity's id until it is cleared.

Under `ArturRios.Util.WebApi`, `TraceActivityMiddleware` starts a W3C activity per request and derives its
own trace id from exactly that activity — so the id logged here is the one the middleware publishes on the
`traceparent` response header, with no wiring and no ASP.NET Core dependency on this side.

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests to improve this project.

## Upgrading to 2.0

**`Microsoft.AspNetCore.Http` is gone.** The package no longer depends on it, so `ArturRios.Logging` is
now usable from a console app or a worker without dragging in an ASP.NET Core 2.x compatibility package
and its transitive closure.

**`MicrosoftLoggerAdapter.TraceId` reads the ambient `Activity` instead of `HttpContext.Items["TraceId"]`.**

For anything running behind `ArturRios.Util.WebApi`'s `TraceActivityMiddleware`, **nothing changes**: that
middleware starts a W3C activity per request and sets `HttpContext.Items["TraceId"]` to
`activity.TraceId.ToString()`, so the value read from the activity is the same string that used to be read
from the items dictionary.

Two cases do change:

- Code that wrote `HttpContext.Items["TraceId"]` **by hand**, without an activity, is no longer seen.
  Start an activity, or assign `adapter.TraceId` directly.
- Setting `adapter.TraceId` no longer writes into `HttpContext.Items`. It sets an `AsyncLocal` that this
  library reads; anything else reading that dictionary entry must be given the value explicitly.

`IHttpContextAccessor` no longer needs to be registered for correlation to work, and registering it has no
effect on this library.

## Testing

The test suite is xUnit, and every test is named with the Given / When / Then pattern. Every test class
carries a `Category` trait, so the two kinds can be run — and reported — separately:

```bash
dotnet test src/ArturRios.Logging.sln --filter "Category=Unit"
dotnet test src/ArturRios.Logging.sln --filter "Category=Functional"
```

Unit tests exercise the code in isolation against test doubles.
Functional tests write real log files to a temporary directory and inspect the folder layout, file names and contents that land there.
CI runs the two as separate jobs, and both must pass before a pull request can be merged.

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
