using Kit.Abstractions;
using Kit.Clients;
using Kit.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Kit.Services {
    public class LogService {

        private LogService() { }

        public static readonly List<ILogClient> LogClients = new List<ILogClient> {
            ConsoleClient.Instance,
            FileClient.Instance
        };

        private static bool isEnable = true;
        private static bool isInitialized = false;
        private static string targetDirectory = Kit.DiagnosticsDirectory;

        public static void Setup(bool? isEnable = null, string targetDirectory = null) {

            if (isEnable != null)
                LogService.isEnable = (bool)isEnable;

            if (targetDirectory != null)
                LogService.targetDirectory = targetDirectory;
        }

        private static void Initialize() {
            Debug.Assert(!isInitialized);

            if (isInitialized)
                throw new InvalidOperationException();

            var fullTargetDir = PathHelper.CombineLocal(Kit.DiagnosticsDirectory);

            if (!Directory.Exists(fullTargetDir))
                Directory.CreateDirectory(fullTargetDir);

            isInitialized = true;
            Log("Initialize LogService");
        }

        public static void Log(string message, LogLevel level = LogLevel.Log) {

            if (!isEnable)
                return;

            if (!isInitialized)
                Initialize();

            foreach (var logClient in LogClients)
                logClient.PushToLog(message, level, targetDirectory);
        }

        public static void LogInfo(string message) => Log(message, LogLevel.Info);

        public static void LogWarning(string message) => Log(message, LogLevel.Warning);

        public static void LogError(string message) => Log(message, LogLevel.Error);
    }
}
