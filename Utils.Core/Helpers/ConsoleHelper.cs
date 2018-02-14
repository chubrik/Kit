using System;
using Utils.Services;

namespace Utils.Helpers {
    public class ConsoleHelper {

        private static LogService LogService => LogService.Instance;

        public static void WriteLine(string message, ConsoleColor? color = null) {
            Write(message, color);
            Console.WriteLine();
        }

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
            LogService.WriteLine($"Write to console: {message}");
        }
    }
}
