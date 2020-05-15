using HDByte.Logger.Listeners;
using System;
using System.Collections.Generic;
using System.Text;

namespace HDByte.Logger
{
    public static class LoggerConfig
    {
        public static string DefaultLoggerPath = @"C:\Logs\$$[processname]$$\$$[launchtimestamp=yyyy-MM-dd HH_mm_ss]$$\";
        public static DateTime LaunchDateTime = DateTime.Now;

        private static Dictionary<string, string> CustomVariables = new Dictionary<string, string>();

        public static int FileListenerBufferTime = 50;
        public static string GetCustomVariable(string name)
        {
            if (!CustomVariables.ContainsKey(name))
                throw new Exception($"No Custom Variable Name of '{name}' exists in LoggerConfig.");

            return CustomVariables[name];
        }

        public static void SetCustomVariable(string name, string value)
        {
            CustomVariables[name] = value;
        }

        private static bool _defaultTraceLogger;
        public static bool DefaultTraceLogger
        {
            get => _defaultTraceLogger;
            set
            {
                _defaultTraceLogger = value;
                if (value)
                {
                    var manager = LoggerManager.GetLoggerManager();

                    if (manager.IsLoggerActive("DefaultLogger"))
                    {
                        var loggerService = manager.GetDefaultLogger();
                        loggerService.AttachListener(LoggingLevel.Trace, new FileListener(DefaultLoggerPath + "trace.txt"));
                    }
                    
                }
            }
        }
    }
}
