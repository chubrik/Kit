using System.Collections.Generic;

namespace Kit {
    public class LogService {

        private LogService() { }

        internal const string LogFileName = "$log.txt";

        public static readonly List<ILogClient> Clients = new List<ILogClient> {
            ConsoleClient.Instance,
            FileClient.Instance
        };

        public static void Log(string message, LogLevel level = LogLevel.Log) {

            foreach (var client in Clients)
                client.PushToLog(message, level, Kit.DiagnisticsCurrentDirectory);
        }

        public static void LogInfo(string message) => Log(message, LogLevel.Info);

        public static void LogSuccess(string message) => Log(message, LogLevel.Success);

        public static void LogWarning(string message) => Log(message, LogLevel.Warning);

        public static void LogError(string message) => Log(message, LogLevel.Error);
    }
}
