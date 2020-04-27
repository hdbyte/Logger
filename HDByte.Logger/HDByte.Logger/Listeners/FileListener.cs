using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace HDByte.Logger.Listeners
{
    public sealed class FileListener : IListener, IDisposable
    {
        public string Name { get; set; }
        public LoggingLevel MinimumImportance { set; get; }
        private StreamWriter _stream;
        private string _format = "$$[timestamp]$$|$$[level]$$|$$[message]$$";
        private string _fileName;

        private object PadLock = new object();
        public StringBuilder Buffer = new StringBuilder();
        private bool IsCompleted;

        // This was added as static because sometimes if two FileListener's are used, two different folders can be created because the ms portion of the time changed between creation of the listeners
        private static DateTime LaunchDateTime = DateTime.Now;

        public FileListener(string fileName, string format = null)
        {
            if (!String.IsNullOrEmpty(format))
                _format = format;

            //Read the timestamp format used and setup filename with correct format
            var tsExpression = @"(?<=\$\$\[timestamp=)(.*?)(?=\]\$\$)";
            MatchCollection tsCollection = Regex.Matches(fileName, tsExpression);
            foreach(Match tsMatch in tsCollection)
            {
                fileName = fileName.Replace($"$$[timestamp={tsMatch.Value}]$$", LaunchDateTime.ToString(tsMatch.Value));
            }

            // Check the filename for any custom variables and replace them with what's in the LoggerConfig static Dictionary
            var tsExpression2 = @"(?<=\$\$\[custom=)(.*?)(?=\]\$\$)";
            MatchCollection tsCollection2 = Regex.Matches(fileName, tsExpression2);
            foreach (Match tsMatch in tsCollection2)
            {
                fileName = fileName.Replace($"$$[custom={tsMatch.Value}]$$", LoggerConfig.GetCustomVariable(tsMatch.Value));
            }

            var pnExpression = @"\b\$\$\[processname\]\$\$\b";
            MatchCollection pnCollection = Regex.Matches(fileName, pnExpression);
            var currentProc = Process.GetCurrentProcess();
            var splitProcessName = currentProc.ProcessName.Split('.');

            foreach (Match pnMatch in tsCollection)
            {
                fileName = fileName.Replace($"$$[processname]$$", splitProcessName[splitProcessName.Length - 1]);
            }

            _fileName = fileName;
        }

        public void Start()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_fileName));
            _stream = new StreamWriter(_fileName, true);

            IsCompleted = false;

            var thread = new Thread(() => LogThreadConsumer());
            thread.IsBackground = true;
            thread.Start();
        }

        public void End()
        {
            IsCompleted = true;
        }

        private void LogThreadConsumer()
        {
            while (!IsCompleted)
            {
                string toWrite;
                lock (PadLock)
                {
                    toWrite = Buffer.ToString();
                    Buffer.Clear();
                }
                
                _stream.Write(toWrite);
                _stream.Flush();

                Thread.Sleep(100);
            }

            try
            {
                _stream.Close();
            } catch(Exception ex)
            {

            }
            
        }

        public void LogAction(LogMessage message)
        {
            if (message.Importance >= MinimumImportance)
            {
                string formattedMessage = ListenerService.FormatMessage(_format, message);

                lock (PadLock)
                {
                    Buffer.AppendLine(formattedMessage);
                }
            }
        }

        public void Dispose()
        {
            if (_stream != null)
            {
                End();
                _stream.Dispose();
            }
        }
    }
}
