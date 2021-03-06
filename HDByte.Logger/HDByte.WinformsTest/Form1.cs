﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HDByte.Logger;
using HDByte.Logger.Listeners;

namespace HDByte.WinformsTest
{
    public partial class Form1 : Form
    {
        LoggerService Log;
        LoggerService DefaultLogger;


        public Form1()
        {
            InitializeComponent();

            var manager = LoggerManager.GetLoggerManager();
            DefaultLogger = manager.GetDefaultLogger(true);

            Log = manager.CreateLogger("TestRun")
                .AttachListener(LoggingLevel.Trace, new ConsoleListener());

            DefaultLogger.Debug("debug only");
            DefaultLogger.Trace("trace only");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Log.Debug("Button 1 Clicked!");
            DefaultLogger.Error("Button 1 Clicked!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DefaultLogger.Fatal("Button 2 Clicked! Fatal Error");

            var xx = LoggerManager.GetLoggerManager().EnableTraceLogger().GetDefaultLogger();

            xx.Info("information only.......");

            Thread.Sleep(500);

            var manager = LoggerManager.GetLoggerManager();
            manager.CreateLogger("TimestampTest")
                .AttachListener(LoggingLevel.Trace, new FileListener(@"C:\Logs\$$[processname]$$\$$[launchtimestamp=yyyy-MM-dd HH_mm_ss]$$\timestamptest.txt", "$$[timestamp]$$    $$[level]$$   $$[message]$$"));


            var timestampTest = manager.GetLogger("TimestampTest");

            timestampTest.Info("YO, does this have the sametimestamp?");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoggerConfig.SetCustomVariable("testLol", "NintendoSwitch");

            var manager = LoggerManager.GetLoggerManager();

            var guidx = new FileListener(@"C:\Logs\$$[processname]$$\$$[timestamp=yyyy-MM-dd HH_mm_ss]$$\$$[custom=testLol]$$\custom.txt", "$$[message]$$");
            manager.CreateLogger("CustomTest")
                .AttachListener(LoggingLevel.Trace, guidx);
            var Logger = manager.GetLogger("CustomTest");

            Logger.Info("This is a custom variable test");

            DefaultLogger.Info("button3 clicked");
            Thread.Sleep(500);  // Give the logging service enough time to process the message before logger is actually removed
            manager.RemoveLogger("CustomTest");
        }
    }
}
