using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

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

        public static string FormatFileName(string fileName)
        {
            // Replace 'launchtimestamp' with the timestamp format of when the LoggerManager() class was created
            var ltsExpression = @"(?<=\$\$\[launchtimestamp=)(.*?)(?=\]\$\$)";
            MatchCollection ltsCollection = Regex.Matches(fileName, ltsExpression);
            foreach (Match ltsMatch in ltsCollection)
            {
                fileName = fileName.Replace($"$$[launchtimestamp={ltsMatch.Value}]$$", LoggerConfig.LaunchDateTime.ToString(ltsMatch.Value));
            }

            // Replace 'timestamp' with provided timestamp format
            var tsExpression = @"(?<=\$\$\[timestamp=)(.*?)(?=\]\$\$)";
            var tsCollection = Regex.Matches(fileName, tsExpression);
            foreach (Match tsMatch in tsCollection)
            {
                fileName = fileName.Replace($"$$[timestamp={tsMatch.Value}]$$", DateTime.Now.ToString(tsMatch.Value));
            }

            // Replace 'custom=XXXXX' with the value that's stored in LoggerConfig
            var lcExpression = @"(?<=\$\$\[custom=)(.*?)(?=\]\$\$)";
            var lcCollection = Regex.Matches(fileName, lcExpression);
            foreach (Match lcMatch in lcCollection)
            {
                fileName = fileName.Replace($"$$[custom={lcMatch.Value}]$$", LoggerConfig.GetCustomVariable(lcMatch.Value));
            }

            // Replace 'processname' with the name of the running Application
            var pnExpression = @"(?<=\$\$\[processname\]\$\$)";
            var pnCollection = Regex.Matches(fileName, pnExpression);
            var currentProc = Process.GetCurrentProcess();
            var splitProcessName = currentProc.ProcessName.Split('.');
            if (pnCollection.Count > 0)
                fileName = fileName.Replace($"$$[processname]$$", splitProcessName[splitProcessName.Length - 1]);

            return fileName;
        }
    }
}
