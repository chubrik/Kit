using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Kit {
    public class ExceptionHandler {

        private ExceptionHandler() { }

        public static readonly List<IDataClient> DataClients = new List<IDataClient> {
            FileClient.Instance
        };
        
        private static bool isInitialized = false;
        private static string targetDirectory = Kit.DiagnosticsDirectory;
        private static int counter = 1;

        public static void Setup(string targetDirectory = null) {
            
            if (targetDirectory != null)
                ExceptionHandler.targetDirectory = targetDirectory;
        }
        
        public static void Register(Exception exception, LogLevel level = LogLevel.Error) {
            
            if (exception.Data.Contains("registered"))
                return;

            var message = exception.Message;
            var match = Regex.Match(exception.ToString(), @"(\w+\.cs):line (\d+)");

            if (match.Success)
                message += $" ({match.Groups[1].Value}:{match.Groups[2].Value})";

            LogService.Log(message, level);
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
            fileName = fileName.Replace('\"', '\'');
            fileName = Regex.Replace(fileName, @"[^a-zа-яё0-9.,()'# -]", "_", RegexOptions.IgnoreCase);

            foreach (var client in DataClients)
                client.PushToWrite(fileName, text, targetDirectory);

            counter++;
            exception.Data["registered"] = true;
        }
    }
}
