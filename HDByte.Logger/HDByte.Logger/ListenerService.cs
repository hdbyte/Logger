using System;
using System.Collections.Generic;
using System.Text;

namespace HDByte.Logger
{
    public static class ListenerService
    {
        public static string FormatMessage(string format, LogMessage logMessage)
        {
            format = format.Replace("$$[shorttimestamp]$$", logMessage.Timestamp.ToString("HH:mm:ss.ffff"));
            format = format.Replace("$$[timestamp]$$", logMessage.Timestamp.ToString("MM/dd/yyyy HH:mm:ss.ffff"));
            format = format.Replace("$$[level]$$", logMessage.Importance.ToString().ToUpper());
            format = format.Replace("$$[message]$$", logMessage.Message);

            return format;
        }
    }
}
