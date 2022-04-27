using System;
using System.Collections.Generic;
using Console = Chubrik.XConsole.XConsole;

namespace Chubrik.Kit
{
    public class ConsoleClient : ILogClient
    {
        private static ConsoleClient? _instance;
        public static ConsoleClient Instance => _instance ??= new ConsoleClient();
        private ConsoleClient() { }

        private static bool _isEnabled = true;
        private static LogLevel _minLevel = LogLevel.Log;

        #region Setup

        public static void Setup(LogLevel? minLevel = null)
        {
            if (minLevel != null)
                _minLevel = (LogLevel)minLevel;
        }

        #endregion

        #region IsAvailable

        // https://stackoverflow.com/questions/6408588/how-to-tell-if-there-is-a-console

        private bool? _isAvailable;

        public bool IsAvailable
        {
            get
            {
                if (_isAvailable == null)
                {
                    _isAvailable = true;
                    try { var windowHeight = Console.WindowHeight; }
                    catch { _isAvailable = false; }
                }

                return _isAvailable.Value;
            }
        }

        #endregion

        #region ILogClient

        private const string LogTimeFormat = "HH:mm:ss";
        private DateTimeOffset _previousDate = DateTimeOffset.Now;

        private static readonly Dictionary<LogLevel, string> _logColors =
            new()
            {
                { LogLevel.Log, "d`" },
                { LogLevel.Info, string.Empty },
                { LogLevel.Success, "G`" },
                { LogLevel.Warning, "Y`" },
                { LogLevel.Error, "R`" },
            };

        public void PushToLog(string message, LogLevel level = LogLevel.Log)
        {
            if (!IsAvailable || level < _minLevel)
                return;

            var now = DateTimeOffset.Now;

            if (now.Day != _previousDate.Day)
                WriteBase($"\n{now:dd.MM.yyyy}\n", color: string.Empty);

            _previousDate = now;
            var timePrefix = now.ToString(LogTimeFormat) + " ";
            var maxWidth = Console.WindowWidth - timePrefix.Length - 1;

            if (message.Length > maxWidth)
            {
                var separator = '\n' + new string(' ', timePrefix.Length);
                message = SplitByWords(message, maxWidth).Join(separator);
            }

            WriteBase(timePrefix + message, color: _logColors[level]);
        }

        private static void WriteBase(string text, string color)
        {
            if (_isEnabled)
                Console.Sync(() =>
                {
                    if (Console.CursorLeft > 0)
                        text = $"\n{text}";

                    Console.WriteLine(color + text);
                });
        }

        #endregion

        #region Utils

        private static List<string> SplitByWords(string text, int maxWidth)
        {
            var lines = new List<string>();

            while (text.Length > maxWidth)
            {
                var index = text.LastIndexOf(' ', maxWidth);

                if (index == -1)
                    index = maxWidth;

                lines.Add(text.Substring(0, index).TrimEnd());
                text = text.Substring(index + 1);
            }

            lines.Add(text);
            return lines;
        }

        public static void Disable() => _isEnabled = false;

        #endregion
    }
}
