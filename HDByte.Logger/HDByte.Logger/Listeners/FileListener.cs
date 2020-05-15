using HDByte.Logger.Core;
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

        public object PadLock = new object();
        public StringBuilder Buffer = new StringBuilder();
        public bool IsRunning { get; private set; }

        public FileListener(string fileName, string format = null)
        {
            if (!String.IsNullOrEmpty(format))
                _format = format;

            _fileName = ListenerService.FormatFileName(fileName);
        }

        public void Start()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_fileName));
            _stream = new StreamWriter(_fileName, true);

            var master = MasterFileListener.GetMasterFileListener();
            lock (master.PadLock)
            {
                master.FileListeners[Name] = this;
            }
        }

        public void End()
        {
            var master = MasterFileListener.GetMasterFileListener();
            lock (master.PadLock)
            {
                master.FileListeners.Remove(Name);
            }

            try
            {
                _stream.Close();
                IsRunning = false;
            }
            catch (Exception ex)
            {
                throw; // lol baller
            }
        }

        public void WriteBuffer()
        {
            if (Buffer.Length != 0)
            {
                string toWrite;
                lock (PadLock)
                {
                    toWrite = Buffer.ToString();
                    Buffer.Clear();
                }

                _stream.Write(toWrite);
                _stream.Flush();
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
