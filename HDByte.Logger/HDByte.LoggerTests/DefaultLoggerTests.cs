﻿using HDByte.Logger;
using HDByte.Logger.Listeners;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace HDByte.LoggerTests
{
    public class DefaultLoggerTests
    {
        
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
        }

        [TearDown]
        public void TearDown()
        {
            var manager = LoggerManager.GetLoggerManager();
            manager.RemoveLogger("DefaultLogger");
            LoggerManager.Nullify();
        }

        [Test]
        public void CreatesDirectoryAndDefaultLogFile()
        {
            //Thread.Sleep(100);
            var manager = LoggerManager.GetLoggerManager();
            var defaultLog = manager.GetDefaultLogger();
            Assert.That(File.Exists(debugFileName));

            Assert.That(!File.Exists(traceFileName));

            manager = null;
        }

        [Test]
        public void CreatesDefaultTraceFile_EnableBeforeGetting()
        {
            var manager = LoggerManager.GetLoggerManager();
            manager.EnableTraceLogger();
            manager.EnableTraceLogger(); // make it doesn't get created twice

            var defaultLog = manager.GetDefaultLogger();

            Assert.That(File.Exists(debugFileName));
            Assert.That(File.Exists(traceFileName));
        }

        [Test]
        public void CreatesDefaultTraceFile_EnableUsingParameter()
        {
            var manager = LoggerManager.GetLoggerManager();

            var defaultLog = manager.GetDefaultLogger(true);

            Assert.That(File.Exists(debugFileName));
            Assert.That(File.Exists(traceFileName));

            defaultLog = manager.GetDefaultLogger(true); // make sure it doesn't get created twice
        }

        [Test]
        public void CreatesDefaultTraceFile_EnableAfterGettingDebugLogger()
        {
            var manager = LoggerManager.GetLoggerManager();

            var defaultLog = manager.GetDefaultLogger();

            Assert.That(File.Exists(debugFileName));
            Assert.That(!File.Exists(traceFileName));

            manager.EnableTraceLogger();
            Assert.That(File.Exists(debugFileName));
            Assert.That(File.Exists(traceFileName));

            manager.EnableTraceLogger(); // make sure it doesn't get created twice
        }

        [Test]
        public void CreatesDefaultTraceFile_EnableAfterGettingDebugLoggerUsingParameter()
        {
            var manager = LoggerManager.GetLoggerManager();

            var defaultLog = manager.GetDefaultLogger();

            Assert.That(File.Exists(debugFileName));
            Assert.That(!File.Exists(traceFileName));

            defaultLog = manager.GetDefaultLogger(true);
            Assert.That(File.Exists(debugFileName));
            Assert.That(File.Exists(traceFileName));
        }
    }
}
