using HDByte.Logger.Listeners;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HDByte.Logger
{
    public class LoggerManager
    {
        private readonly ConcurrentDictionary<string, LoggerService> _loggerList;
        private readonly BlockingCollection<LogMessage> _pendingMessages;
        private Thread myThread;
        private static readonly object _padLock = new object();
        private readonly object _padLockDefaultLogger = new object();
        private static LoggerManager _instance = null;
        public static bool DefaultTraceLoggerEnabled = false;

        public static LoggerManager GetLoggerManager()
        {
            if (_instance == null)
            {
                lock (_padLock)
                {
                    if (_instance == null)
                    {
                        _instance = new LoggerManager();
                    }
                }
            }

            return _instance;
        }

        public LoggerService GetDefaultLogger()
        {
            LoggerService logger;

            lock (_padLockDefaultLogger)
            {
                var loggerName = "DefaultLogger";
                if (IsLoggerActive(loggerName))
                {
                    logger = GetLogger(loggerName);
                } else
                {
                    logger = CreateLogger(loggerName)
                        .AttachListener(LoggingLevel.Debug, new FileListener(@"C:\Logs\$$[processname]$$\$$[launchtimestamp=yyyy-MM-dd HH_mm_ss]$$\debug.txt"));

                    if (DefaultTraceLoggerEnabled)
                        logger.AttachListener(LoggingLevel.Trace, new FileListener(@"C:\Logs\$$[processname]$$\$$[launchtimestamp=yyyy-MM-dd HH_mm_ss]$$\trace.txt"));
                }
            }

            return logger;
        }

        public LoggerManager EnableTraceLogger()
        {
            DefaultTraceLoggerEnabled = true;

            return this;
        }

        public LoggerManager()
        {
            _loggerList = new ConcurrentDictionary<string, LoggerService>();
            _pendingMessages = new BlockingCollection<LogMessage>();

            myThread = new Thread(Loop);
            myThread.IsBackground = true;
            myThread.Start();
        }

        public bool IsLoggerActive(string name)
        {
            return _loggerList.ContainsKey(name);
        }

        public bool RemoveLogger(string name)
        {
            if (IsLoggerActive(name))
            {
                if (_loggerList.TryRemove(name, out LoggerService end))
                {
                    end.Stop();
                    return true;
                }
            }

            return false;
        }

        public LoggerService GetLogger(string name)
        {
            if (IsLoggerActive(name))
            {
                return _loggerList[name];
            } else
            {
                throw new Exceptions.LoggerNotFoundException($"Logger with a name of {name} does not exist.");
            }
        }

        public LoggerService CreateLogger(string name = null)
        {
            lock (_padLock)
            {
                if (!IsLoggerActive(name))
                {
                    var logger = new LoggerService(name);
                    logger._pendingMessages = _pendingMessages;
                    _loggerList.TryAdd(name, logger);

                    return logger;
                } else
                {
                    throw new Exceptions.LoggerAlreadyExistsException($"Logger with name '{name}' already exists.");
                }
            }
        }

        private void Loop()
        {
            var tokenSource = new CancellationTokenSource();
            LogMessage message;
            while (_pendingMessages.TryTake(out message, Timeout.Infinite, tokenSource.Token))
            {
                var target = message.Target;
                if (_loggerList.ContainsKey(target))
                    _loggerList[target].PerformLogAction(message);
            }
        }
    }
}
