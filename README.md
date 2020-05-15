# Logger

[![Nuget Version](https://img.shields.io/nuget/v/HDByte.Logger.svg?style=flat-square)](https://www.nuget.org/packages/HDByte.Logger/)
![Downloads](https://img.shields.io/nuget/dt/HDByte.Logger)
![Lines Of Code](https://tokei.rs/b1/github/hdbyte/logger)

![GitHub issues](https://img.shields.io/github/issues/hdbyte/logger?style=flat-square)

## Quick Introduction
This project is still in the early stages of development. Changelog.md contains a list of changes between versions.

## Examples
```csharp
var manager = LoggerManager.GetLoggerManager();
LoggerService Log = manager.CreateLogger("TestLogger")
    .AttachListener(LoggingLevel.Debug, new ConsoleListener())
    .AttachListener(LoggingLevel.Debug, new FileListener(@"C:\LogFiles\$$[processname]$$\$$[timestamp=yyyy-MM-dd HH_mm_ss]$$.txt"));
```

```csharp
Log.Debug("Button 1 Clicked!");
Log.Fatal("fatal error, blah blah happened");
```

### Logging Levels
```csharp
Log.Trace
Log.Debug
Log.Info
Log.Warn
Log.Error
Log.Fatal
```

## Advanced Examples (Custom Message Formats)
```csharp
var manager = LoggerManager.GetLoggerManager();
LoggerService Log = manager.CreateLogger("TestLogger")
    .AttachListener(LoggingLevel.Debug, new ConsoleListener(@"$$[shorttimestamp]$$ - $$[level]$$ - $$[message]$$"))
    .AttachListener(LoggingLevel.Debug, new FileListener(@"C:\LogFiles\$$[processname]$$\$$[timestamp=yyyy-MM-dd HH_mm_ss]$$.txt", "$$[timestamp]$$  -  $$[level]$$ - $$[message]$$"));
```

### Custom Filename Formats for FileListener
Acceptable Variables
* processname           Name of process currently being ran.
* launchtimestamp=      Uses the DateTime.Now of when LoggerConfig was first initialized
* timestamp=HH_mm_ss    This uses Datetime.ToString() formatting, customize it to your hearts content.
* custom=NAMEHERE       Replace NAMEHERE with the variable name used by using LoggerConfig.AddCustomVariable("NAMEHERE", "value");

### Custom Message Formats
Acceptable Variables
* timestamp = 04/05/2020 20:38:01.1312
* shorttimestamp = 20:38:01.1312
* level - FATAL
* message - The actual log message to be logged.

## Default Debugging Logger
This logger stores files in C:\Logs\{processname}\{timestamp}\debug.txt which logs all Log.Debug level and above messages.

```csharp
var Log = LoggerManager.GetLoggerManager().GetDefaultLogger();
Log.Info("information only.......");
```

Default Logger can also log trace level messages if enabled. These logs are logged to the same folder as debug.txt but is named trace.txt
There are two ways to enable the default Trace Level logger, shown below

```csharp
var Log = LoggerManager.GetLoggerManager().GetDefaultLogger(true);
Log.Info("information only.......");
Log.Trace("trace test log");
```

```csharp
var manager = LoggerManager.GetLoggerManager();
var Log = manager.GetDefaultLogger();
manager.EnableTraceLogger();

Log.Info("information only.......");
Log.Trace("trace test log");
```