# Logger

## Getting Started
This project is still in the early stages of development. Semantic versision will start with 2.0.0 release. For the 1.x.x release, expect backwards compatiblity changes with minor updates, but not patch updates.

### Notes
Add EnableTraceLogger() to enable the DefaultTraceLogger to create a seperate file for trace level logging.
Must use EnableTraceLogger() before GetDefaultLogger() is ever called

### 1.4.0 -> 2.0.0
Added LoggerManager unit tests.
Add RemoveLogger(string name)
Changed LoggerService.Information to LoggerService.Info (bug from v1.4)
Changed 'timestamp' in FileListener to use the current Datetime.Now instead of launch time 
Added 'launchtimestamp' in FileListener that uses the DateTime of when the app was first loaded
Fixed bug in FileListener which prevented $$[processname]$$ from being evaluated if there was no valid $$[timestamp=xxxxx]$$ to be evaluated


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
    .AttachListener(LoggingLevel.Debug, new FileListener(@"C:\Logs\$$[processname]$$\$$[timestamp=yyyy-MM-dd HH_mm_ss]$$.txt", "$$[timestamp]$$  -  $$[level]$$ - $$[message]$$"));
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

If EnableTraceLogger() is performed then Default Logger will also create a trace.txt file which logs all Log.Trace level and above messages.

```csharp
var Log = LoggerManager.GetLoggerManager().GetDefaultLogger();
Log.Info("information only.......");
```

```csharp
var Log = LoggerManager.GetLoggerManager().EnableTraceLogger().GetDefaultLogger(;
Log.Info("information only.......");
Log.Trace("trace test log");
