using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kit
{
    public class LogService
    {
        private LogService() { }

        internal const string LogFileName = "$log.txt";

        private static readonly Stack<Stopwatch> _stopwatches = new Stack<Stopwatch>();

        public static readonly List<ILogClient> Clients = new List<ILogClient> {
            TraceClient.Instance,
            ConsoleClient.Instance
        };

        #region Extensions

        public static void LogInfo(string message) => Log(message, LogLevel.Info);
        public static void LogSuccess(string message) => Log(message, LogLevel.Success);
        public static void LogWarning(string message) => Log(message, LogLevel.Warning);
        public static void LogError(string message) => Log(message, LogLevel.Error);

        public static void BeginInfo(string message) => Begin(message, LogLevel.Info);
        public static void BeginSuccess(string message) => Begin(message, LogLevel.Success);
        public static void BeginWarning(string message) => Begin(message, LogLevel.Warning);
        public static void BeginError(string message) => Begin(message, LogLevel.Error);

        public static void EndInfo(string message) => End(message, LogLevel.Info);
        public static void EndSuccess(string message) => End(message, LogLevel.Success);
        public static void EndWarning(string message) => End(message, LogLevel.Warning);
        public static void EndError(string message) => End(message, LogLevel.Error);

        #endregion

        public static void Log(string message, LogLevel level = LogLevel.Log)
        {
            var indentedMessage = string.Concat(Enumerable.Repeat("- ", _stopwatches.Count)) + message;

            foreach (var client in Clients)
                client.PushToLog(indentedMessage, level);
        }

        public static void Begin(string message, LogLevel level = LogLevel.Log)
        {
            Log(message, level);

            lock (_stopwatches)
                _stopwatches.Push(Stopwatch.StartNew());
        }

        public static void End(string message, LogLevel level = LogLevel.Log)
        {
            Stopwatch sw;

            lock (_stopwatches)
                sw = _stopwatches.Pop();

            sw.Stop();
            Log($"{message} at {TimeHelper.FormattedLatency(sw.Elapsed)}", level);
        }
    }
}
