using System;
using System.Collections.Generic;
using System.Text;

namespace HDByte.Logger
{
    public class LogMessage
    {
        public string Target { get; set; }
        public DateTime Timestamp { get; set; }
        public LoggingLevel Importance { get; set; }
        public string Message { get; set; }
        public static LogMessage Create(string target, DateTime timestamp, LoggingLevel importance, string message)
        {
            return new LogMessage() { Target = target, Timestamp = timestamp, Importance = importance, Message = message };
        }
    }
}
