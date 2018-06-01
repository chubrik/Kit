using System;
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

        private DateTimeOffset _previousDate = DateTimeOffset.Now;

        public void PushToLog(string message, LogLevel level = LogLevel.Log)
        {
            if (level < _minLevel)
                return;

            var now = DateTimeOffset.Now;

            if (now.Day != _previousDate.Day)
                WriteLine((Position.Left > 0 ? "\n" : string.Empty) + $"\n{now.ToString("dd.MM.yyyy")}\n");

            _previousDate = now;
            ConsoleColor? color;

            switch (level)
            {
                case LogLevel.Log:
                    color = ConsoleColor.DarkGray;
                    break;

                case LogLevel.Info:
                    color = null;
                    break;

                case LogLevel.Success:
                    color = ConsoleColor.Green;
                    break;

                case LogLevel.Warning:
                    color = ConsoleColor.Yellow;
                    break;

                case LogLevel.Error:
                    color = ConsoleColor.Red;
                    break;

                default:
                    Debug.Fail(string.Empty);
                    throw new ArgumentOutOfRangeException(nameof(level));
            }

            var fullMessage = Position.Left > 0 ? "\n" : string.Empty;
            fullMessage += $"{now.ToString("HH:mm:ss")} - {message}\n";
            WriteBase(fullMessage, color, position: null, isLog: true);
        }

        #endregion

        #region Write

        public static ConsolePosition WriteLine() => WriteLine(string.Empty);

        public static ConsolePosition WriteLine(
            string text, ConsoleColor? color = null, ConsolePosition position = null) =>
            Write($"{text}\n", color, position);

        public static ConsolePosition Write(
            string text, ConsoleColor? color = null, ConsolePosition position = null) =>
            WriteBase(text, color, position, isLog: false);

        private static readonly object _lock = new object();

        public static ConsolePosition WriteBase(
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
