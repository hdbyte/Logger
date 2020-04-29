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
        public bool IsRunning { get; private set; }

        public FileListener(string fileName, string format = null)
        {
            if (!String.IsNullOrEmpty(format))
                _format = format;

            // Replace 'launchtimestamp' with the timestamp format of when the LoggerManager() class was created
            var ltsExpression = @"(?<=\$\$\[launchtimestamp=)(.*?)(?=\]\$\$)";
            MatchCollection ltsCollection = Regex.Matches(fileName, ltsExpression);
            foreach(Match ltsMatch in ltsCollection)
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
            IsRunning = true;

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
                IsRunning = false;
            } catch(Exception ex)
            {
                throw; // lol baller
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
