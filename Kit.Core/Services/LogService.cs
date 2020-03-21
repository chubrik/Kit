using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kit
{
    public static class LogService
    {
        internal const string LogFileName = "$log.txt";

        private static readonly Stack<Stopwatch> _stopwatches = new Stack<Stopwatch>();

        public static readonly List<ILogClient> Clients = new List<ILogClient> {
            TraceClient.Instance,
            ConsoleClient.Instance
        };

        #region Extensions

        public static void Info(string message) => Log(message, LogLevel.Info);
        public static void Success(string message) => Log(message, LogLevel.Success);
        public static void Warning(string message) => Log(message, LogLevel.Warning);
        public static void Error(string message) => Log(message, LogLevel.Error);

        public static void BeginInfo(string message) => Begin(message, LogLevel.Info);

        public static void EndInfo(string message) => End(message, LogLevel.Info);
        public static void EndSuccess(string message) => End(message, LogLevel.Success);
        public static void EndWarning(string message) => End(message, LogLevel.Warning);
        public static void EndError(string message) => End(message, LogLevel.Error);

        public static void Log(string message, Action block) => Handle(message, block, LogLevel.Log, LogLevel.Log);
        public static void Info(string message, Action block) => Handle(message, block, LogLevel.Info, LogLevel.Info);
        public static void InfoSuccess(string message, Action block) => Handle(message, block, LogLevel.Info, LogLevel.Success);

        public static T Log<T>(string message, Func<T> block) => Handle(message, block, LogLevel.Log, LogLevel.Log);
        public static T Info<T>(string message, Func<T> block) => Handle(message, block, LogLevel.Info, LogLevel.Info);
        public static T InfoSuccess<T>(string message, Func<T> block) => Handle(message, block, LogLevel.Info, LogLevel.Success);

        #endregion

        public static void Log(string message, LogLevel level = LogLevel.Log)
        {
            var indentedMessage = string.Concat(Enumerable.Repeat("- ", _stopwatches.Count)) + message;

            foreach (var client in Clients)
                client.PushToLog(indentedMessage, level);
        }

        public static void Begin(string message, LogLevel level = LogLevel.Log)
        {
            Log($"{message} - started...", level);

            lock (_stopwatches)
                _stopwatches.Push(Stopwatch.StartNew());
        }

        public static void End(string message, LogLevel level = LogLevel.Log)
        {
            Stopwatch sw;

            lock (_stopwatches)
                sw = _stopwatches.Pop();

            sw.Stop();
            Log($"{message} - completed at {TimeHelper.FormattedLatency(sw.Elapsed)}", level);
        }

        #region Handle

        private static void Handle(string message, Action block, LogLevel startLevel, LogLevel successLevel)
        {
            Begin(message, startLevel);

            try
            {
                block();
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsCanceled());
                throw ProceedException(message, exception);
            }

            End(message, successLevel);
        }

        private static T Handle<T>(string message, Func<T> block, LogLevel startLevel, LogLevel successLevel)
        {
            Begin(message, startLevel);
            T result;

            try
            {
                result = block();
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsCanceled());
                throw ProceedException(message, exception);
            }

            End(message, successLevel);
            return result;
        }

        private static Exception ProceedException(string message, Exception exception)
        {
            Stopwatch sw;

            lock (_stopwatches)
                sw = _stopwatches.Pop();

            sw.Stop();
            ExceptionHandler.Register(exception);

            if (exception.IsCanceled())
                Warning($"{message} - canceled at {TimeHelper.FormattedLatency(sw.Elapsed)}");
            else if (exception.IsTimeoutOrCanceled())
                Error($"{message} - timed out at {TimeHelper.FormattedLatency(sw.Elapsed)}");
            else
                Error($"{message} - failed at {TimeHelper.FormattedLatency(sw.Elapsed)}");

            return exception;
        }

        #endregion
    }
}
