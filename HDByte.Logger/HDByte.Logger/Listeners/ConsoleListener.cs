using System;

namespace HDByte.Logger.Listeners
{
    public class ConsoleListener : IListener
    {
        public string Name { get; set; }
        public LoggingLevel MinimumImportance { set; get; }

        private string _messageFormat = "$$[shorttimestamp]$$|$$[level]$$|$$[message]$$";

        public bool IsRunning { get; private set; }
        public ConsoleListener(string format = null)
        {
            if (!String.IsNullOrEmpty(format))
                _messageFormat = format;
        }

        public void Start()
        {
            IsRunning = true;
        }

        public void End()
        {
            IsRunning = false;
        }

        public void LogAction(LogMessage message)
        {
            if (!IsRunning)
                return;

            if (message.Importance >= MinimumImportance)
            {
                string formattedMessage = ListenerService.FormatMessage(_messageFormat, message);

                Console.WriteLine(formattedMessage);
            }
        }
    }
}
