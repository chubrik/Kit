using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Utils.Helpers;

namespace Utils.Services {
    public class ExceptionService {

        private static LogService LogService => LogService.Instance;

        private static ExceptionService instance;
        public static ExceptionService Instance => instance ?? (instance = new ExceptionService());
        private ExceptionService() { }

        private bool isEnable = true;
        private bool isInitialized = false;
        private string targetDirectory = "$exceptions";
        private int counter = 1;

        public void Setup(bool? isEnable = null, string targetDirectory = null) {

            if (isEnable != null)
                this.isEnable = (bool)isEnable;

            if (targetDirectory != null)
                this.targetDirectory = targetDirectory;
        }

        private void Initialize() {
            LogService.WriteLine($"Initialize ExceptionService");
            Debug.Assert(!isInitialized);

            if (isInitialized)
                throw new InvalidOperationException();

            var fullTargetDir = PathHelper.CombineLocal(targetDirectory);

            if (Directory.Exists(fullTargetDir))
                counter += Directory.GetFiles(fullTargetDir).Length;
            else
                Directory.CreateDirectory(fullTargetDir);

            isInitialized = true;
        }

        public void Register(Exception exception, bool isCritical = true) {

            if (!isEnable)
                return;

            if (!isInitialized)
                Initialize();

            if (exception.Data.Contains("registered"))
                return;

            var message = exception.Message;
            var match = Regex.Match(exception.ToString(), @"(\w+\.cs):line (\d+)");

            if (match.Success)
                message += $" ({match.Groups[1].Value}:{match.Groups[2].Value})";

            LogService.WriteLine($"EXCEPTION #{counter}: {message}");
            ConsoleHelper.WriteLine(message, isCritical ? ConsoleColor.Red : ConsoleColor.Yellow);
            var text = $"Exception #{counter}\n{message}\n\n";

            var thisException = exception;

            while (true) {
                text += $"\n{thisException.ToString().Replace(" --->", "\n   --->")}\n";
                thisException = thisException.InnerException;

                if (thisException == null)
                    break;

                text += "\n\nINNER EXCEPTION:\n";
            }

            text = text.Replace("\n", "\r\n").Replace("\r\r", "\r");
            var fileName = $"{counter.ToString().PadLeft(3, '0')} {message}.txt";
            fileName = Regex.Replace(fileName, @"[^A-Za-z0-9.,()'# -]", "_");
            var filePath = PathHelper.CombineLocal(targetDirectory, fileName);
            File.WriteAllText(filePath, text);

            exception.Data["registered"] = true;
        }
    }
}
