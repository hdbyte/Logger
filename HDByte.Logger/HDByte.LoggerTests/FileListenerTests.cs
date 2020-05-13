using HDByte.Logger;
using HDByte.Logger.Listeners;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace HDByte.LoggerTests
{
    public class FileListenerTests
    {
        LoggerManager manager;
        string LogFileDirectory = @"C:\Logs\LoggerManagerTests\";

        [SetUp]
        public void Setup()
        {
            if (Directory.Exists(Path.GetDirectoryName(LogFileDirectory)))
                Directory.Delete(Path.GetDirectoryName(LogFileDirectory), true);

            manager = LoggerManager.GetLoggerManager();
        }

        [Test]
        public void CreatesFile()
        {
            string logFileName = LogFileDirectory + "createTest.log";
            var Log = manager.CreateLogger("createTest")
                .AttachListener(LoggingLevel.Trace, new FileListener(logFileName, "$$[message]$$"));
            Log.Trace("createTestMessage");

            Thread.Sleep(300);
            manager.RemoveLogger("createTest");
            Thread.Sleep(300);

            Assert.That(File.Exists(logFileName));

            string[] lines = System.IO.File.ReadAllLines(logFileName);
            Assert.That(lines[0], Is.EqualTo("createTestMessage"));
        }

        [Test]
        public void FileNameParseTest()
        {
            LoggerConfig.SetCustomVariable("testFileName", "Kitty.txt");

            string logFileName = LogFileDirectory + @"$$[processname]$$\$$[launchtimestamp=yyyy-MM-dd HH_mm_ss]$$\$$[timestamp=yyyy-MM-dd]$$\$$[custom=testFileName]$$";
            var Log = manager.CreateLogger("fileParseTest")
                .AttachListener(LoggingLevel.Trace, new FileListener(logFileName, "$$[message]$$"));

            Thread.Sleep(300);
            manager.RemoveLogger("fileParseTest");
            Thread.Sleep(300);


            var currentProc = Process.GetCurrentProcess();
            var splitProcessNameArray = currentProc.ProcessName.Split('.');
            var processName = splitProcessNameArray[splitProcessNameArray.Length - 1];

            var launchTimeStamp = LoggerConfig.LaunchDateTime.ToString("yyyy-MM-dd HH_mm_ss");

            var timeStamp = DateTime.Now.ToString("yyyy-MM-dd");

            var custom = "Kitty.txt";

            var parsedFileName = $@"{LogFileDirectory}{processName}\{launchTimeStamp}\{timeStamp}\{custom}";
            Assert.That(File.Exists(parsedFileName));
        }

        [Test]
        public void MessageParseTest()
        {
            LoggerConfig.SetCustomVariable("favoritePet", "Cat");

            string logFileName = LogFileDirectory + "parseTest.log";
            var Log = manager.CreateLogger("parseTest")
                .AttachListener(LoggingLevel.Trace, new FileListener(logFileName, "$$[timestamp]$$|$$[shorttimestamp]$$|$$[level]$$|$$[message]$$"));
            Log.Trace("message23");

            Thread.Sleep(100);
            manager.RemoveLogger("parseTest");
            Thread.Sleep(100);

            Assert.That(File.Exists(logFileName));

            string[] lines = System.IO.File.ReadAllLines(logFileName);

            var data = lines[0].Split('|');
            var longTS = Convert.ToDateTime(data[0]).ToString("yyyy-MM-dd HH");
            var shortTS = Convert.ToDateTime(data[1]).ToString("HH");
            var level = data[2];
            var message = data[3];

            Assert.That(longTS, Is.EqualTo(DateTime.Now.ToString("yyyy-MM-dd HH")));
            Assert.That(shortTS, Is.EqualTo(DateTime.Now.ToString("HH")));
            Assert.That(level, Is.EqualTo("TRACE"));
            Assert.That(message, Is.EqualTo("message23"));
        }

        [Test]
        public void LevelTest()
        {
            string logFileName = LogFileDirectory + "levelTest.log";
            var Log = manager.CreateLogger("levelTest")
                .AttachListener(LoggingLevel.Debug, new FileListener(logFileName, "$$[message]$$"));
            Log.Debug("levelTestMessage");
            Log.Trace("levelTestMessage2");

            Thread.Sleep(300);
            manager.RemoveLogger("levelTest");
            Thread.Sleep(300);

            Assert.That(File.Exists(logFileName));

            string[] lines = System.IO.File.ReadAllLines(logFileName);
            Assert.That(lines[0], Is.EqualTo("levelTestMessage"));
            Assert.That(lines.Length, Is.EqualTo(1));
        }
    }
}
