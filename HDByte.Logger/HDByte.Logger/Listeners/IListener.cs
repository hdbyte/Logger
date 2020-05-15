using System;

namespace HDByte.Logger.Listeners
{
    public interface IListener
    {
        Guid ID { get; set; }
        string Name { get; set; }
        LoggingLevel MinimumImportance { get; set; }
        void LogAction(LogMessage message);
        void Start();
        void End();

        bool IsRunning { get; }
    }
}
