namespace HDByte.Logger.Listeners
{
    public interface IListener
    {
        string Name { get; set; }
        LoggingLevel MinimumImportance { get; set; }
        void LogAction(LogMessage message);
        void Start();
        void End();
    }
}
