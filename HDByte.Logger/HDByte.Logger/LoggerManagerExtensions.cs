using System;
using System.Collections.Generic;
using System.Text;

namespace HDByte.Logger
{
    public static class LoggerManagerExtensions
    {
        public static LoggerManager EnableTraceLogger(this LoggerManager manager)
        {
            LoggerManager.DefaultTraceLoggerEnabled = true;
            return manager;
        }
    }
}
