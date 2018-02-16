namespace Kit.Abstractions {
    public interface ILogClient {

        void PushToLog(string message, LogLevel level = LogLevel.Log, string targetDirectory = null);
    }
}
