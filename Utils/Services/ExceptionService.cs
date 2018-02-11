using System;
using System.IO;
using System.Text.RegularExpressions;
using Utils.Helpers;

namespace Utils.Services {
    public class ExceptionService {

        private static ExceptionService _instance;
        public static ExceptionService Instance => _instance ?? (_instance = new ExceptionService());
        private ExceptionService() { }

        private const string dirName = "exceptions";

        public void Register(Exception exception, bool isCritical = true) {
            var number = 1;

            if (Directory.Exists(dirName))
                number += Directory.GetFiles(dirName).Length;
            else
                Directory.CreateDirectory(dirName);

            var message = $"#{number} {exception.Message}";
            var match = Regex.Match(exception.ToString(), @"(\w+\.cs):line (\d+)");

            if (match.Success)
                message += $" ({match.Groups[1].Value}:{match.Groups[2].Value})";

            LogHelper.WriteLine(message, isCritical ? ConsoleColor.Red : ConsoleColor.DarkYellow);
            var text = $"Exception #{number}\nMessage: {exception.Message}\n\n";

            while (true) {
                text += $"\n{exception.ToString().Replace(" --->", "\n   --->")}\n";
                exception = exception.InnerException;

                if (exception == null)
                    break;

                text += "\n\nINNER EXCEPTION:\n";
            }

            text = text.Replace("\n", "\r\n").Replace("\r\r", "\r");
            File.WriteAllText($"{dirName}/Exception #{number.ToString().PadLeft(3, '0')}.txt", text);
        }
    }
}
