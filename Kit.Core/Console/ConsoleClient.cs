using System;
using System.Diagnostics;

namespace Kit {
    public class ConsoleClient : ILogClient {

        private static ConsoleClient instance;
        public static ConsoleClient Instance => instance ?? (instance = new ConsoleClient());

        private ConsoleClient() { }

        public static ConsolePosition Position { get; private set; } = new ConsolePosition(0, 0);
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

        public static ConsolePosition WriteLine() => WriteLine(string.Empty);

        public static ConsolePosition WriteLine(string text, ConsoleColor? color = null, ConsolePosition position = null) {
            var startPosition = position ?? Position;
            WriteBase($"{text}\r\n", color, startPosition);
            return Position = new ConsolePosition(startPosition.Top + 1, 0);
        }

        public static ConsolePosition Write(string text, ConsoleColor? color = null, ConsolePosition position = null) {
            var startPosition = position ?? Position;
            WriteBase(text, color, startPosition);
            return Position = new ConsolePosition(startPosition.Top, startPosition.Left + text.Length);
        }

        private static readonly object _lock = new object();

        private static void WriteBase(string text, ConsoleColor? color, ConsolePosition position) {
            lock (_lock) {
                var originalColor = Console.ForegroundColor;
                var originalTop = Console.CursorTop;
                var originalLeft = Console.CursorLeft;

                try {
                    if (color != null)
                        Console.ForegroundColor = (ConsoleColor)color;

                    Console.SetCursorPosition(position.Left, position.Top);
                    Console.Write(text);
                }
                catch (ArgumentOutOfRangeException exception) {
                    Debug.Fail(exception.ToString());
                    ExceptionHandler.Register(exception);
                }
                finally {
                    Console.SetCursorPosition(originalLeft, originalTop);
                    Console.ForegroundColor = originalColor;
                }
            }
        }
    }
}
