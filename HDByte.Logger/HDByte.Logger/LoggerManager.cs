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
        private readonly object _padLockDebugging = new object();
        private static LoggerManager _instance = null;

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

            lock (_padLockDebugging)
            {
                var loggerName = "DefaultLogger";
                if (IsLoggerActive(loggerName))
                {
                    logger = GetLogger(loggerName);
                } else
                {
                    logger = CreateLogger(loggerName)
                        .AttachListener(LoggingLevel.Trace, new FileListener(@"C:\Logs\$$[processname]$$\$$[timestamp=yyyy-MM-dd HH_mm_ss]$$.txt"));
                }
            }

            return logger;
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

        public LoggerService GetLogger(string name)
        {
            if (IsLoggerActive(name))
            {
                return _loggerList[name];
            } else
            {
                throw new Exception($"Logger with a name of {name} does not exist.");
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
                    throw new Exception($"Logger with name '{name}' already exists.");
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
                _loggerList[target].PerformLogAction(message);
            }
        }
    }
}
