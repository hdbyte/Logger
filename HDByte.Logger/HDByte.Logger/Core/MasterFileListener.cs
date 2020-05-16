using HDByte.Logger.Listeners;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace HDByte.Logger.Core
{
    public class MasterFileListener
    {
        private static readonly object _instancePadLock = new object();
        private static MasterFileListener _instance = null;

        private bool IsCompleted;
        private object _threadPadLock = new object();
        private Thread _thread;

        public object PadLock = new object();
        public Dictionary<string, FileListener> FileListeners = new Dictionary<string, FileListener>();

        public static MasterFileListener GetMasterFileListener()
        {
            if (_instance == null)
            {
                lock (_instancePadLock)
                {
                    if (_instance == null)
                    {
                        _instance = new MasterFileListener();
                        _instance.Start();
                    }
                }
            }

            return _instance;
        }

        public void Start()
        {
            if (_thread == null)
            {
                lock (_threadPadLock)
                {
                    if (_thread == null)
                    {
                        _thread = new Thread(() => LogThreadConsumer());
                        _thread.IsBackground = true;
                        _thread.Start();
                    }
                }
            }
        }

        private void LogThreadConsumer()
        {
            while (!IsCompleted)
            {
                lock (PadLock)
                {
                    foreach (KeyValuePair<string, FileListener> fileListener in FileListeners)
                    {
                        fileListener.Value.WriteBuffer();
                        Debug.WriteLine(fileListener.Value.Name);
                    }
                }
                Thread.Sleep(LoggerConfig.FileListenerBufferTime);                
            }
        }
    }
}
