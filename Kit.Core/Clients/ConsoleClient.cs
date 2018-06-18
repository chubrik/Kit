using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kit
{
    public class ConsoleClient : ILogClient
    {
        private static ConsoleClient _instance;
        public static ConsoleClient Instance => _instance ?? (_instance = new ConsoleClient());
        private ConsoleClient() { }

        public static ConsolePosition Position { get; private set; } = new ConsolePosition(0, 0);
        private static LogLevel _minLevel = LogLevel.Info;
        private static ConsoleMode _mode = ConsoleMode.Enabled;

        #region Setup

        public static void Setup(LogLevel? minLevel = null)
        {
            if (minLevel != null)
                _minLevel = (LogLevel)minLevel;
        }

        #endregion

        #region ILogClient

        private const string LogTimeFormat = "HH:mm:ss";
        private DateTimeOffset _previousDate = DateTimeOffset.Now;

        private static readonly Dictionary<LogLevel, ConsoleColor?> _logColors =
            new Dictionary<LogLevel, ConsoleColor?>
            {
                { LogLevel.Log, ConsoleColor.DarkGray },
                { LogLevel.Info, null },
                { LogLevel.Success, ConsoleColor.Green },
                { LogLevel.Warning, ConsoleColor.Yellow },
                { LogLevel.Error, ConsoleColor.Red },
            };

        public void PushToLog(string message, LogLevel level = LogLevel.Log)
        {
            if (level < _minLevel)
                return;

            var now = DateTimeOffset.Now;

            if (now.Day != _previousDate.Day)
                WriteBase($"\n{now.ToString("dd.MM.yyyy")}\n\n", color: null, position: null, isLog: true);

            _previousDate = now;
            var timePrefix = $"{now.ToString(LogTimeFormat)} - ";
            var maxWidth = Console.WindowWidth - timePrefix.Length - 1;

            if (message.Length > maxWidth)
            {
                var separator = '\n' + new String(' ', timePrefix.Length);
                message = SplitByWords(message, maxWidth).Join(separator);
            }

            WriteBase($"{timePrefix}{message}\n", color: _logColors[level], position: null, isLog: true);
        }

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

        #endregion

        #region Write

        public static ConsolePosition WriteLine() => WriteLine(string.Empty);

        public static ConsolePosition WriteLine(
            string text, ConsoleColor? color = null, ConsolePosition position = null) =>
            WriteBase($"{text}\n", color, position, isLog: false);

        public static ConsolePosition Write(
            string text, ConsoleColor? color = null, ConsolePosition position = null) =>
            WriteBase(text, color, position, isLog: false);

        private static readonly object _lock = new object();

        private static ConsolePosition WriteBase(
            string text, ConsoleColor? color, ConsolePosition position, bool isLog)
        {
            lock (_lock)
            {
                if (_mode == ConsoleMode.Disabled || (_mode == ConsoleMode.LogOnly && !isLog))
                    return Position;

                var origColor = Console.ForegroundColor;
                var origTop = Console.CursorTop;
                var origLeft = Console.CursorLeft;
                var isMoved = position != null && !position.Equals(Position);
                ConsolePosition endPosition;

                try
                {
                    if (color != null)
                        Console.ForegroundColor = (ConsoleColor)color;

                    if (isMoved)
                    {
                        Console.CursorVisible = false;
                        Console.SetCursorPosition(position.Left, position.Top);
                    }

                    if (isLog && origLeft > 0)
                        text = $"\n{text}";

                    Console.Write(text);
                }
                catch (ArgumentOutOfRangeException exception)
                {
                    Debug.Fail(exception.ToString());
                    ExceptionHandler.Register(exception, level: LogLevel.Warning);
                    // no throw for overflow warning
                }
                finally
                {
                    Console.ForegroundColor = origColor;
                    endPosition = new ConsolePosition(Console.CursorTop, Console.CursorLeft);

                    if (isMoved)
                    {
                        Console.SetCursorPosition(origLeft, origTop);
                        Console.CursorVisible = true;
                    }
                    else
                        Position = endPosition;
                }

                return endPosition;
            }
        }

        #endregion

        #region Utils

        public static void ReduceToLogOnly()
        {
            if (_mode == ConsoleMode.Enabled)
                _mode = ConsoleMode.LogOnly;
        }

        public static void Disable()
        {
            lock (_lock)
                _mode = ConsoleMode.Disabled;
        }

        #endregion
    }
}
