using HDByte.Logger;
using NUnit.Framework;
using System;

namespace HDByte.LoggerTests
{
    public class LoggerManagerTests
    {
        LoggerManager manager;

        [SetUp]
        public void Setup()
        {
            manager = LoggerManager.GetLoggerManager();
        }

        [Test]
        public void GetLoggerManager_AlwaysReturnsSameInstance()
        {
            var secondInstance = new LoggerManager();
            var thirdInstance = LoggerManager.GetLoggerManager();

            Assert.That(manager, Is.SameAs(thirdInstance));
            Assert.That(manager, Is.Not.SameAs(secondInstance));
        }

        [Test]
        public void GetDefaultLogger_AlwaysReturnsSameInstance()
        {
            var firstInstance = manager.GetDefaultLogger();
            var secondInstance = manager.GetDefaultLogger();
            var thirdInstance = new LoggerService("DefaultLogger");

            Assert.That(firstInstance, Is.SameAs(secondInstance));
            Assert.That(firstInstance, Is.Not.SameAs(thirdInstance));
        }

        [Test]
        public void IsLoggerActive_Works()
        {
            manager.CreateLogger("test");

            Assert.That(manager.IsLoggerActive("test"), Is.True);
            Assert.That(manager.IsLoggerActive("test2"), Is.False);
        }

        [Test]
        public void CreateRemoveAndGetLogger()
        {
            manager.CreateLogger("test2");
            Assert.That(manager.IsLoggerActive("test2"), Is.True);

            var test2 = manager.GetLogger("test2");
            Assert.That(test2.Name == "test2");

            Assert.Throws<Logger.Exceptions.LoggerAlreadyExistsException>(delegate ()
            {
                manager.CreateLogger("test2");
            });

            manager.RemoveLogger("test2");
            Assert.That(manager.IsLoggerActive("test2"), Is.False);

            Assert.Throws<Logger.Exceptions.LoggerNotFoundException>(delegate ()
            {
                var logger = manager.GetLogger("test2");
            });
        }

    }
}