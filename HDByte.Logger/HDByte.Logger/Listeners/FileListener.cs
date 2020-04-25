using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace HDByte.Logger.Listeners
{
    public sealed class FileListener : IListener, IDisposable
    {
        public string Name { get; set; }
        public LoggingLevel MinimumImportance { set; get; }
        private StreamWriter _stream;
        private string _format = "$$[timestamp]$$|$$[level]$$|$$[message]$$";
        private string _fileName;

        // This was added as static because sometimes if two FileListener's are used, two different folders are created because the ms portion of the time changed between creation of the listeners
        private static DateTime LaunchDateTime = DateTime.Now;

        public FileListener(string fileName, string format = null)
        {
            if (!String.IsNullOrEmpty(format))
                _format = format;

            var tsExpression = @"(?<=\$\$\[timestamp=)(.*?)(?=\]\$\$)";
            MatchCollection tsCollection = Regex.Matches(fileName, tsExpression);
            foreach(Match tsMatch in tsCollection)
            {
                fileName = fileName.Replace($"$$[timestamp={tsMatch.Value}]$$", LaunchDateTime.ToString(tsMatch.Value));
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
        }

        public void End()
        {
            _stream.Close();
        }

        public void LogAction(LogMessage message)
        {
            if (message.Importance >= MinimumImportance)
            {
                string formattedMessage = ListenerService.FormatMessage(_format, message);

                _stream.WriteLine(formattedMessage);
                _stream.Flush();
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
