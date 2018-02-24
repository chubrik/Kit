using System;
using System.Diagnostics;

namespace Kit {
    public class ConsoleClient : ILogClient {

        private static ConsoleClient instance;
        public static ConsoleClient Instance => instance ?? (instance = new ConsoleClient());
        private ConsoleClient() { }

        private static LogLevel minLevel = LogLevel.Info;

        public static void Setup(LogLevel? minLevel = null) {

            if (minLevel != null)
                ConsoleClient.minLevel = (LogLevel)minLevel;
        }

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

            WriteLine(message, color);
        }

        #endregion

        public static void WriteLine(string message, ConsoleColor? color = null) =>
            Write($"{message}\r\n", color);

        public static void Write(string message, ConsoleColor? color = null) {
            var time = DateTimeOffset.Now.ToString("HH:mm:ss");
            var origColor = Console.ForegroundColor;

            if (color != null)
                Console.ForegroundColor = (ConsoleColor)color;

            if (message.StartsWith("\r")) {
                message = message.Substring(1);
                Console.Write($"\r{time} - {message}");
            }
            else
                Console.Write($"{time} - {message}");

            Console.ForegroundColor = origColor;
        }
    }
}
