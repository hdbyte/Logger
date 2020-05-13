using HDByte.Logger;
using HDByte.Logger.Listeners;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;

namespace HDByte.LoggerTests
{
    public class DefaultLoggerTests
    {
        LoggerManager manager;
        static string debugFileName;
        static string traceFileName;

        [SetUp]
        public void Setup()
        {
            var currentProc = Process.GetCurrentProcess();
            var splitProcessNameArray = currentProc.ProcessName.Split('.');
            var processName = splitProcessNameArray[splitProcessNameArray.Length - 1];

            debugFileName = $@"C:\Logs\{processName}\{LoggerConfig.LaunchDateTime.ToString("yyyy-MM-dd HH_mm_ss")}\debug.txt";
            traceFileName = $@"C:\Logs\{processName}\{LoggerConfig.LaunchDateTime.ToString("yyyy-MM-dd HH_mm_ss")}\trace.txt";

            if (Directory.Exists(Path.GetDirectoryName($@"C:\Logs\{processName}\")))
                Directory.Delete(Path.GetDirectoryName($@"C:\Logs\{processName}\"), true);

            manager = LoggerManager.GetLoggerManager();
            var defaultLog = manager.EnableTraceLogger().GetDefaultLogger();
        }

        [Test]
        public void CreatesDirectoryAndFile()
        {
            Assert.That(File.Exists(debugFileName));
            Assert.That(File.Exists(traceFileName));
        }
    }
}
