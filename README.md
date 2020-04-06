# Logger

## Getting Started
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
Log.Information
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

