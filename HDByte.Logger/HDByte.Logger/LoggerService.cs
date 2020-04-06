using HDByte.Logger.Listeners;
using System;
using System.Collections.Concurrent;

namespace HDByte.Logger
{
    public class LoggerService
    {
        public string Name { get; set; }
        public BlockingCollection<IListener> _listeners;
        public BlockingCollection<LogMessage> _pendingMessages { get; set; }

        public LoggerService(string name)
        {
            Name = name;
            _listeners = new BlockingCollection<IListener>();
        }

        public LoggerService AttachListener(LoggingLevel importance, IListener listener)
        {
            listener.Name = Name;
            listener.MinimumImportance = importance;
            _listeners.Add(listener);
            listener.Start();

            return this;
        }

        public void Push (LoggingLevel importance, string message)
        {
            var timestamp = DateTime.Now;

            _pendingMessages.Add(LogMessage.Create(Name, timestamp, importance, message));
        }

        public void PerformLogAction(LogMessage message)
        {
            foreach(IListener listener in _listeners)
            {
                listener.LogAction(message);
            }
        }

        public void Trace(string message)
        {
            Push(LoggingLevel.Trace, message);
        }

        public void Debug(string message)
        {
            Push(LoggingLevel.Debug, message);
        }

        public void Warning(string message)
        {
            Push(LoggingLevel.Warning, message);
        }

        public void Error(string message)
        {
            Push(LoggingLevel.Error, message);
        }

        public void Fatal(string message)
        {
            Push(LoggingLevel.Fatal, message);
        }

        public void Information(string message)
        {
            Push(LoggingLevel.Information, message);
        }
    }
}
