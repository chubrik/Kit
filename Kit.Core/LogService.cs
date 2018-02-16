using System.Collections.Generic;

namespace Kit {
    public class LogService {

        private LogService() { }

        public static readonly List<ILogClient> LogClients = new List<ILogClient> {
            ConsoleClient.Instance,
            FileClient.Instance
        };

        private static bool isEnable = true;
        private static string targetDirectory = Kit.DiagnosticsDirectory;

        public static void Setup(bool? isEnable = null, string targetDirectory = null) {

            if (isEnable != null)
                LogService.isEnable = (bool)isEnable;

            if (targetDirectory != null)
                LogService.targetDirectory = targetDirectory;
        }
        
        public static void Log(string message, LogLevel level = LogLevel.Log) {

            if (!isEnable)
                return;

            foreach (var logClient in LogClients)
                logClient.PushToLog(message, level, targetDirectory);
        }

        public static void LogInfo(string message) => Log(message, LogLevel.Info);

        public static void LogWarning(string message) => Log(message, LogLevel.Warning);

        public static void LogError(string message) => Log(message, LogLevel.Error);
    }
}
