using System;

namespace Utils.Helpers {
    public class LogHelper {

        public static void Write(string value, ConsoleColor? color = null) {
            var time = DateTime.Now.ToString("HH:mm:ss");
            var origColor = Console.ForegroundColor;

            if (color != null)
                Console.ForegroundColor = (ConsoleColor)color;

            Console.Write(value.StartsWith("\r")
                ? $"\r{time} - {value.Substring(1)}"
                : $"{time} - {value}");

            Console.ForegroundColor = origColor;
        }

        public static void WriteLine(string value, ConsoleColor? color = null) {
            Write(value, color);
            Console.WriteLine();
        }
    }
}
