using HDByte.Logger.Listeners;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace HDByte.Logger
{
    public class LoggerService
    {
        public string Name { get; set; }
        public BlockingCollection<IListener> _listeners;
        public BlockingCollection<LogMessage> _pendingMessages { get; set; }

        private bool isActive;

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
            isActive = true;

            return this;
        }

        public void Push (LoggingLevel importance, string message)
        {
            var timestamp = DateTime.Now;

            _pendingMessages.Add(LogMessage.Create(Name, timestamp, importance, message));
        }

        public void PerformLogAction(LogMessage message)
        {
            if (!isActive)
                return;

            foreach(IListener listener in _listeners)
            {
                listener.LogAction(message);
            }
        }

        public void Stop()
        {
            isActive = false;
            foreach (IListener listener in _listeners)
            {
                listener.End();

                var sw = new Stopwatch();
                sw.Start();
                bool completed = false;
                while (!completed)
                {
                    if (!listener.IsRunning)
                        completed = true;

                    if (sw.Elapsed.TotalMilliseconds >= 500)
                        break;
                }

                if (!completed)
                    throw new Exception($"Unable to end Listener process {listener.GetType().ToString()}");
            }
        }

        public void Log(LoggingLevel level, string message)
        {
            Push(level, message);
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

        public void Info(string message)
        {
            Push(LoggingLevel.Info, message);
        }
    }
}
