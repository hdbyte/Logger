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

        public LoggerManager()
        {
            _loggerList = new ConcurrentDictionary<string, LoggerService>();
            _pendingMessages = new BlockingCollection<LogMessage>();

            myThread = new Thread(Loop);
            myThread.IsBackground = true;
            myThread.Start();
        }

        public LoggerService CreateLogger(string name = null)
        {
            var logger = new LoggerService(name);
            logger._pendingMessages = _pendingMessages;

            _loggerList.TryAdd(name, logger);

            return logger;
        }

        private void Loop()
        {
            var tokenSource = new CancellationTokenSource();
            LogMessage message;
            while (_pendingMessages.TryTake(out message, Timeout.Infinite, tokenSource.Token))
            {
                var target = message.Target;
                _loggerList[target].Log(message);
            }
        }
    }
}
