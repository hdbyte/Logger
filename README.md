# Logger

## Getting Started
This project is still in the early stages of development. Semantic versision will start with 2.0.0 release. For the 1.x.x release, expect backwards compatiblity changes with minor updates, but not patch updates.

### Differences between 1.2.0 and 1.3.0
Add EnableTraceLogger() to enable the DefaultTraceLogger to create a seperate file for trace level logging.
Changed save file locations of DefaultLogger
Removed LogLevel.Information and replaced it with LogLevel.Info

```csharp
LoggerService Log;
var manager = LoggerManager.GetLoggerManager();
Log = manager.CreateLogger("TestLogger")
    .AttachListener(LoggingLevel.Debug, new ConsoleListener())
    .AttachListener(LoggingLevel.Debug, new FileListener(@"C:\Logs\$$[processname]$$\$$[timestamp=yyyy-MM-dd HH_mm_ss]$$.txt"));
```

### Examples
```csharp
Log.Debug("Button 1 Clicked!");
Log.Fatal("fatal error, blah blah happened");
```

### Logging Levels
```csharp
Log.Trace
Log.Debug
Log.Info
Log.Warning
Log.Error
Log.Fatal
```

## Advanced Examples (Custom Message Formats)
```csharp
LoggerService Log;
var manager = LoggerManager.GetLoggerManager();
Log = manager.CreateLogger("TestLogger")
    .AttachListener(LoggingLevel.Debug, new ConsoleListener(@"$$[shorttimestamp]$$ - $$[level]$$ - $$[message]$$"))
    .AttachListener(LoggingLevel.Debug, new FileListener(@"C:\Logs\$$[processname]$$\$$[timestamp=yyyy-MM-dd HH_mm_ss]$$.txt", "$$[timestamp]$$    $$[level]$$   $$[message]$$"));
```

### Custom Filename Formats for FileListener
Acceptable Variables
* processname = Name of process currently being ran.
* timestamp= - This uses Datetime.ToString() formatting.

### Custom Message Formats
Acceptable Variables
* timestamp = 04/05/2020 20:38:01.1312
* shorttimestamp = 20:38:01.1312
* level - FATAL
* message - The actual log message to be logged.

## Default Debugging Logger
This logger stores files in C:\Logs\{processname}\{timestamp}\debug.txt which logs all Log.Debug level and above messages.

If EnableTraceLogger() is performed then Default Logger will also create a trace.txt file which logs all Log.Trace level and above messages.

```csharp
var Log = LoggerManager.GetLoggerManager().GetDefaultLogger();
Log.Info("information only.......");
```

```csharp
var Log = LoggerManager.GetLoggerManager().EnableTraceLogger();
Log.Info("information only.......");
Log.Trace("trace test log");
