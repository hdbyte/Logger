using System;

namespace HDByte.Logger.Listeners
{
    public class ConsoleListener : IListener
    {
        public string Name { get; set; }
        public LoggingLevel MinimumImportance { set; get; }

        private string _messageFormat = "$$[shorttimestamp]$$|$$[level]$$|$$[message]$$";

        public ConsoleListener(string format = null)
        {
            if (!String.IsNullOrEmpty(format))
                _messageFormat = format;
        }

        public void Start()
        {

        }

        public void End()
        {

        }

        public void LogAction(LogMessage message)
        {
            if (message.Importance >= MinimumImportance)
            {
                string formattedMessage = ListenerService.FormatMessage(_messageFormat, message);

                Console.WriteLine(formattedMessage);
            }
        }
    }
}
