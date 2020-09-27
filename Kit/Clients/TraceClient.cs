using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kit
{
    public class TraceClient : ILogClient
    {
        private static TraceClient? _instance;
        public static TraceClient Instance => _instance ??= new TraceClient();
        private TraceClient() { }

        #region ILogClient

        private const string LogCategory = nameof(Kit);
        private const string LogTimeFormat = "dd.MM.yyyy HH:mm:ss.fff";

        private static readonly Dictionary<LogLevel, string> _logBadges =
            new Dictionary<LogLevel, string>
            {
                { LogLevel.Log,     " log   " },
                { LogLevel.Info,    "[INFO] " },
                { LogLevel.Success, "[SUCC] " },
                { LogLevel.Warning, "[WARN] " },
                { LogLevel.Error,   "[ERROR]" },
            };

        public void PushToLog(string message, LogLevel level = LogLevel.Log)
        {
            var dateTime = DateTimeOffset.Now.ToString(LogTimeFormat);
            Trace.WriteLine($" {dateTime}  {_logBadges[level]} {message}", LogCategory);
        }

        #endregion
    }
}
