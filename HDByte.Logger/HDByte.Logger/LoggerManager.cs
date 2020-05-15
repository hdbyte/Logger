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

        private bool defaultTraceLoggerEnabled;
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

        /// <summary>
        /// Added to null out the instance. Needs to be able to remove all loggers before setting _instance to null.
        /// </summary>
        public static void Nullify()
        {
            _instance = null;
        }

        public LoggerService GetDefaultLogger(bool enableTraceLogger = false)
        {
            LoggerService logger;
            var loggerName = "DefaultLogger";

            if (!IsLoggerActive(loggerName))
            {
                lock (_padLockDefaultLogger)
                {
                    if (!IsLoggerActive(loggerName))
                    {
                        var debugListener = new FileListener(LoggerConfig.DefaultLoggerPath + "debug.txt");

                        CreateLogger(loggerName)
                        .AttachListener(LoggingLevel.Debug, debugListener);
                    }
                }

                if (enableTraceLogger)
                {
                    EnableTraceLogger();
                }
                    
                
            } else if (!defaultTraceLoggerEnabled & enableTraceLogger)
            {
                EnableTraceLogger();
            }

            logger = GetLogger(loggerName);
            return logger;
        }

        public LoggerManager EnableTraceLogger()
        {
            if (!IsLoggerActive("DefaultLogger"))
            {
                GetDefaultLogger();
            }

            if (!defaultTraceLoggerEnabled)
            {
                lock (_padLockDefaultLogger)
                {
                    defaultTraceLoggerEnabled = true;
                    var traceListener = new FileListener(LoggerConfig.DefaultLoggerPath + "trace.txt");

                    var log = GetLogger("DefaultLogger");
                    log.AttachListener(LoggingLevel.Trace, traceListener);
                }
            }

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
