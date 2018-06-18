using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kit
{
    public class TraceClient : ILogClient
    {
        private static TraceClient _instance;
        public static TraceClient Instance => _instance ?? (_instance = new TraceClient());
        private TraceClient() { }

        #region ILogClient

        private const string LogCategory = "   " + nameof(Kit);
        private const string LogTimeFormat = "dd.MM.yyyy HH:mm:ss.fff";

        private static readonly Dictionary<LogLevel, string> _logSigns =
            new Dictionary<LogLevel, string>
            {
                { LogLevel.Log, " " },
                { LogLevel.Info, "i" },
                { LogLevel.Success, "s" },
                { LogLevel.Warning, "w" },
                { LogLevel.Error, "e" },
            };

        private static readonly Dictionary<LogLevel, string> _logBadges =
            new Dictionary<LogLevel, string>
            {
                { LogLevel.Log, string.Empty },
                { LogLevel.Info, "INFO: " },
                { LogLevel.Success, "SUCCESS: " },
                { LogLevel.Warning, "WARNING: " },
                { LogLevel.Error, "ERROR: " },
            };

        public void PushToLog(string message, LogLevel level = LogLevel.Log)
        {
            var textLine =
                $" {_logSigns[level]} {DateTimeOffset.Now.ToString(LogTimeFormat)} - " +
                _logBadges[level] + message;

            Trace.WriteLine(textLine, LogCategory);
        }

        #endregion
    }
}
