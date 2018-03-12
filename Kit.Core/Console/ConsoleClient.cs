using System;
using System.Diagnostics;

namespace Kit {
    public class ConsoleClient : ILogClient {

        private static ConsoleClient instance;
        public static ConsoleClient Instance => instance ?? (instance = new ConsoleClient());
        private ConsoleClient() { }

        private static ConsolePosition _position = new ConsolePosition(0, 0);
        public static ConsolePosition Position => _position;

        private static LogLevel minLevel = LogLevel.Info;

        #region Setup

        public static void Setup(LogLevel? minLevel = null) {

            if (minLevel != null)
                ConsoleClient.minLevel = (LogLevel)minLevel;
        }

        #endregion

        #region ILogClient

        public void PushToLog(string message, LogLevel level = LogLevel.Log) {

            if (level < minLevel)
                return;

            ConsoleColor? color;

            switch (level) {

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

            var fullMessage = $"{DateTimeOffset.Now.ToString("HH:mm:ss")} - {message}";
            WriteLine(message, color);
        }

        #endregion

        #region Write

        public static ConsolePosition WriteLine() => WriteLine(string.Empty);

        public static ConsolePosition WriteLine(
            string text, ConsoleColor? color = null, ConsolePosition position = null) =>
            Write($"{text}\r\n", color, position);

        public static ConsolePosition Write(
            string text, ConsoleColor? color = null, ConsolePosition position = null) {

            lock (Position) {
                var origColor = Console.ForegroundColor;
                var origTop = Console.CursorTop;
                var origLeft = Console.CursorLeft;
                var isMoved = position != null && !position.Equals(Position);
                ConsolePosition endPosition;

                try {
                    if (color != null)
                        Console.ForegroundColor = (ConsoleColor)color;

                    if (isMoved) {
                        Console.CursorVisible = false;
                        Console.SetCursorPosition(position.Left, position.Top);
                    }

                    Console.Write(text);
                }
                catch (ArgumentOutOfRangeException exception) {
                    Debug.Fail(exception.ToString());
                    ExceptionHandler.Register(exception);
                }
                finally {
                    Console.ForegroundColor = origColor;
                    endPosition = new ConsolePosition(Console.CursorTop, Console.CursorLeft);

                    if (isMoved) {
                        Console.SetCursorPosition(origLeft, origTop);
                        Console.CursorVisible = true;
                    }
                    else
                        _position = endPosition;
                }

                return endPosition;
            }
        }

        #endregion
    }
}
