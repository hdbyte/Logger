using System;
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
        LoggerService Log2;


        public Form1()
        {
            InitializeComponent();

            var manager = LoggerManager.GetLoggerManager().EnableTraceLogger();
            Log = manager.CreateLogger("TestRun")
                .AttachListener(LoggingLevel.Trace, new ConsoleListener());

            Log2 = manager.GetDefaultLogger();

            Log.Trace("Form1 complete!");
            Log2.Trace("trace only");
            Log2.Debug("debug only");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Log.Debug("Button 1 Clicked!");
            Log2.Error("Button 1 Clicked!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Log2.Fatal("Button 2 Clicked! Fatal Error");

            var xx = LoggerManager.GetLoggerManager().GetDefaultLogger();

            xx.Information("information only.......");

            Thread.Sleep(500);

            var manager = LoggerManager.GetLoggerManager();
            manager.CreateLogger("TimestampTest")
                .AttachListener(LoggingLevel.Trace, new FileListener(@"C:\Logs\$$[processname]$$\$$[timestamp=yyyy-MM-dd HH_mm_ss]$$\timestamptest.txt", "$$[timestamp]$$    $$[level]$$   $$[message]$$"));


            var timestampTest = manager.GetLogger("TimestampTest");

            timestampTest.Information("YO, does this have the sametimestamp?");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoggerConfig.SetCustomVariable("testLol", "NintendoSwitch");

            var manager = LoggerManager.GetLoggerManager();
            manager.CreateLogger("CustomTest")
                .AttachListener(LoggingLevel.Trace, new FileListener(@"C:\Logs\$$[processname]$$\$$[timestamp=yyyy-MM-dd HH_mm_ss]$$\$$[custom=testLol]$$\custom.txt", "$$[message]$$"));
            var Logger = manager.GetLogger("CustomTest");

            Logger.Information("This is a custom variable test");
        }
    }
}
