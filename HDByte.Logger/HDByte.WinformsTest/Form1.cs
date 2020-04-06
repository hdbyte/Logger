using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

            var manager = LoggerManager.GetLoggerManager();
            Log = manager.CreateLogger("TestRun")
                .AttachListener(LoggingLevel.Trace, new ConsoleListener());
            Log2 = manager.CreateLogger("TestRun2")
                .AttachListener(LoggingLevel.Debug, new ConsoleListener())
                .AttachListener(LoggingLevel.Debug, new FileListener(@"C:\Logs\$$[processname]$$\$$[timestamp=yyyy-MM-dd HH_mm_ss]$$.txt"));

            Log.Trace("Form1 complete!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Log.Debug("Button 1 Clicked!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Log2.Fatal("Button 2 Clicked! Fatal Error");
        }
    }
}
