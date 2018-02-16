using System;
using System.Diagnostics;

namespace Kit {
    public class ConsoleClient : ILogClient {

        private static ConsoleClient instance;
        public static ConsoleClient Instance => instance ?? (instance = new ConsoleClient());
        private ConsoleClient() { }

        private static LogLevel minLogLevel = LogLevel.Info;

        public static void Setup(LogLevel? minLogLevel = null) {

            if (minLogLevel != null)
                ConsoleClient.minLogLevel = (LogLevel)minLogLevel;
        }

        #region ILogClient

        public void PushToLog(string message, LogLevel level = LogLevel.Log, string targetDirectory = null) {

            if (level < minLogLevel)
                return;

            ConsoleColor? color;

            switch (level) {

                case LogLevel.Log:
                    color = ConsoleColor.DarkGray;
                    break;

                case LogLevel.Info:
                    color = null;
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
            var time = DateTime.Now.ToString("HH:mm:ss");
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
