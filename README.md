# Dotnet Logging

[![Docs](https://img.shields.io/badge/docs-website-blue)](https://artur-rios.github.io/dotnet-logging)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/artur-rios/dotnet-logging/blob/main/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/ArturRios.Logging.svg)](https://www.nuget.org/packages/ArturRios.Logging)

A flexible and feature-rich logging library for .NET applications. This library provides multiple logger implementations (Console and File), automatic caller information capture, custom log levels, and seamless integration with Microsoft.Extensions.Logging.

## Features

- **Multiple Logger Implementations**: Console and File loggers with customizable configurations
- **Automatic Caller Information**: Capture file path and method name automatically using compiler attributes
- **Custom Log Levels**: 8 severity levels - Trace, Debug, Information, Warning, Error, Exception, Critical, and Fatal
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

// Every message is written to both the console and the file; there is no minimum level filter
logger.Warn("This warning appears in both console and file");
```

### Microsoft.Extensions.Logging Integration

Integrate with ASP.NET Core or other frameworks using the standard logging abstractions:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ArturRios.Logging;
using ArturRios.Logging.Adapter;
using ArturRios.Logging.Configuration;
using ArturRios.Logging.Interfaces;

var services = new ServiceCollection();

// The adapter forwards every entry to an IStateLogger resolved from a fresh scope;
// without this registration nothing is written.
services.AddScoped<IStateLogger>(_ => new StateLogger(new List<LoggerConfiguration>
{
    new ConsoleLoggerConfiguration { UseColors = true }
}));

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

`ByRequest` names the folder once per logger instance, so resolving the logger from a **scoped** registration
within a request gives one folder per request; a singleton gives one folder per process. Entries forwarded by
`MicrosoftLoggerAdapter` resolve the `IStateLogger` from a fresh scope per entry, so through the adapter a scoped
registration gives one folder per entry.

| `LogSplitLevel` | File name |
|---|---|
| `Request` | `MyApp.log` |
| `Year` | `MyApp_2026.log` |
| `Month` | `MyApp_2026_08.log` |
| `Day` | `MyApp_2026_08_24.log` |
| `Hour` | `MyApp_2026_08_24_13.log` |

## State Logger

`StateLogger` takes the caller's file and method from a state object instead of compiler attributes — the
shape Microsoft.Extensions.Logging passes along, which is how `MicrosoftLoggerAdapter` uses it. The keys
`CallerFilePath` / `FilePath` and `CallerMemberName` / `MemberName` / `Method` are recognized regardless of case;
anything missing is logged as `unknown`.

```csharp
using ArturRios.Logging;

var stateLogger = new StateLogger(configurations);
stateLogger.TraceId = "trace-123";

stateLogger.Info("Operation started", new Dictionary<string, object>
{
    ["CallerFilePath"] = "OrderService.cs",
    ["CallerMemberName"] = "PlaceOrder"
});

stateLogger.TraceId = null;
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

The adapter hands that id to the `IStateLogger` it resolves for the one entry being forwarded and then puts
the state logger's own `TraceId` back, so a singleton `IStateLogger` can be shared by concurrent work without
one entry's id landing on another entry's line or on a later entry that has no id of its own.

Under `ArturRios.Util.WebApi`, `TraceActivityMiddleware` starts a W3C activity per request and derives its
own trace id from exactly that activity — so the id logged here is the one the middleware publishes on the
`traceparent` response header, with no wiring and no ASP.NET Core dependency on this side.

## Upgrading

Releases that need changes in consuming code carry an upgrade guide in the changelog:

- From 1.x to 2.0: [Upgrading from 1.x to 2.0](https://github.com/artur-rios/dotnet-logging/blob/main/CHANGELOG.md#upgrading-from-1x-to-20)

## Changelog

Notable changes in each release are recorded in [CHANGELOG.md](https://github.com/artur-rios/dotnet-logging/blob/main/CHANGELOG.md). Releases follow
[Semantic Versioning](https://semver.org/).

## Contributing

Contributions are welcome! Building from source, running the tests, the branching model and the release process are
described in [CONTRIBUTING.md](https://github.com/artur-rios/dotnet-logging/blob/main/CONTRIBUTING.md).

## Legal Details

This project is licensed under the [MIT License](https://en.wikipedia.org/wiki/MIT_License). A copy of the license is available at [LICENSE](https://github.com/artur-rios/dotnet-logging/blob/main/LICENSE) in the repository.
