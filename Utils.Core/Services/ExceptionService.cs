using System;
using System.IO;
using System.Text.RegularExpressions;
using Utils.Helpers;

namespace Utils.Services {
    public class ExceptionService {

        private static ExceptionService instance;
        public static ExceptionService Instance => instance ?? (instance = new ExceptionService());
        private ExceptionService() { }

        private string targetDirectory = "$exceptions";

        public void Setup(string targetDirectory = null) {

            if (targetDirectory != null)
                this.targetDirectory = targetDirectory;
        }

        public void Register(Exception exception, bool isCritical = true) {

            if (exception.Data.Contains("registered"))
                return;

            var number = 1;
            var fullTargetDirectory = PathHelper.CombineLocal(targetDirectory);

            if (Directory.Exists(fullTargetDirectory))
                number += Directory.GetFiles(fullTargetDirectory).Length;
            else
                Directory.CreateDirectory(fullTargetDirectory);

            var message = exception.Message;
            var match = Regex.Match(exception.ToString(), @"(\w+\.cs):line (\d+)");

            if (match.Success)
                message += $" ({match.Groups[1].Value}:{match.Groups[2].Value})";

            LogHelper.WriteLine(message, isCritical ? ConsoleColor.Red : ConsoleColor.Yellow);
            var text = $"Exception #{number}\n{message}\n\n";

            var thisException = exception;

            while (true) {
                text += $"\n{thisException.ToString().Replace(" --->", "\n   --->")}\n";
                thisException = thisException.InnerException;

                if (thisException == null)
                    break;

                text += "\n\nINNER EXCEPTION:\n";
            }

            text = text.Replace("\n", "\r\n").Replace("\r\r", "\r");
            var fileName = $"{number.ToString().PadLeft(3, '0')} {message}.txt";
            fileName = Regex.Replace(fileName, @"[^A-Za-z0-9.,()'# -]", "_");
            var filePath = PathHelper.CombineLocal(targetDirectory, fileName);
            File.WriteAllText(filePath, text);

            exception.Data["registered"] = true;
        }
    }
}
