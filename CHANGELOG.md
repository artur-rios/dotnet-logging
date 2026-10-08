# Changelog

All notable changes to `ArturRios.Logging` are recorded in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- `StandaloneLogger.Exception` logs the exception's `ToString()` — type, message, inner exceptions and stack trace —
  as `StateLogger.Exception` already did, instead of only its message.
- `StandaloneLogger.Exception` and `StateLogger.Exception` throw `ArgumentNullException` for a null exception instead
  of a `NullReferenceException`.
- The `StandaloneLogger` and `StateLogger` constructors throw `ArgumentNullException` for a null configuration list
  instead of a `NullReferenceException`.

### Fixed

- `MicrosoftLoggerAdapter` puts the resolved `IStateLogger`'s own `TraceId` back after forwarding an entry, and sets,
  uses and restores it under a lock on that instance. With a singleton `IStateLogger`, concurrent entries could be
  written with each other's trace id, and an entry with no ambient id inherited the id of the previous one.
- `FileLogger` stamps an entry and chooses its file from the same instant, so an entry written across the turn of an
  hour, day, month or year no longer lands in the previous period's file with the next period's timestamp.
- `AddCustomLogger` registers its provider once however many times it is called; a second call registered a second
  provider, and every entry was written twice.

## [2.0.0] - 2026-08-24

See [Upgrading from 1.x to 2.0](#upgrading-from-1x-to-20) for the one case that needs a code change.

### Changed

- **Breaking:** `MicrosoftLoggerAdapter.TraceId` returns an explicitly set id, else the trace id of `Activity.Current`,
  instead of reading `HttpContext.Items["TraceId"]`. Setting it holds the value in an `AsyncLocal` for the current
  execution context rather than writing into `HttpContext.Items`. Code behind `ArturRios.Util.WebApi`'s
  `TraceActivityMiddleware` logs the same id as before.
- `MicrosoftLoggerAdapter.Log` passes that same ambient trace id to the resolved `IStateLogger`.
- `FileLogger` reports an unknown folder scheme with the parameter name, the offending value and a message.
- `ArturRios.Extensions` updated from 1.3.0 to 1.4.0 and `ArturRios.Util` from 2.0.0 to 2.1.0.

### Removed

- **Breaking:** the `Microsoft.AspNetCore.Http` dependency, so the package is usable from a console app or a worker
  without pulling in an ASP.NET Core 2.x compatibility package. `IHttpContextAccessor` no longer needs to be registered.

### Fixed

- Entries logged through `MicrosoftLoggerAdapter` without caller information are attributed to the calling code rather
  than to the logger itself.
- `LogFolderScheme.ByRequest` groups a logger's entries in one folder instead of creating a folder per entry.
- `MicrosoftLoggerAdapter`, `ConsoleLogger` and `FileLogger` carry XML documentation, so consuming projects that
  generate documentation build without CS1591 warnings.

### Upgrading from 1.x to 2.0

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

## [1.1.0] - 2026-08-19

### Changed

- `ArturRios.Extensions` updated from 1.0.2 to 1.3.0 and `ArturRios.Util` from 1.1.0 to 2.0.0, so consumers no longer
  resolve outdated transitive dependencies.
- `Microsoft.Extensions.Logging.Abstractions` updated from 10.0.1 to 10.0.11, which the newer `ArturRios.Util` requires.

## [1.0.0] - 2025-12-21

### Added

- `ConsoleLogger` with ANSI color-coded output and `FileLogger` with configurable folder schemes and log splitting.
- Automatic caller information capture, trace id support and custom log levels.
- `StandaloneLogger` and `StateLogger` to use the loggers on their own or as part of a logging state.
- `MicrosoftLoggerAdapter` and `MicrosoftLoggerProvider` to plug the loggers into `Microsoft.Extensions.Logging`.

[Unreleased]: https://github.com/artur-rios/dotnet-logging/compare/2.0.0...HEAD
[2.0.0]: https://github.com/artur-rios/dotnet-logging/compare/v1.1.0...2.0.0
[1.1.0]: https://github.com/artur-rios/dotnet-logging/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/artur-rios/dotnet-logging/releases/tag/v1.0.0
