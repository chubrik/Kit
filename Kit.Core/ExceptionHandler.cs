using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Kit {
    public class ExceptionHandler {

        private ExceptionHandler() { }

        private static int counter = 0;

        public static readonly List<IDataClient> DataClients = new List<IDataClient> {
            FileClient.Instance
        };

        public static void Register(Exception exception, LogLevel level = LogLevel.Error) {

            if (exception.Data.Contains("registered"))
                return;

            var startTime = DateTimeOffset.Now;
            var message = exception.Message.Replace("\r\n", " ");
            var match = Regex.Match(exception.ToString(), @"(\w+\.cs):line (\d+)");

            if (match.Success)
                message += $" ({match.Groups[1].Value}:{match.Groups[2].Value})";

            counter++;
            var paddedCount = counter.ToString().PadLeft(3, '0');
            LogService.Log($"Exception {paddedCount}: {message}", level);
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
            var fileName = PathHelper.SafeFileName($"{paddedCount} {message}.txt");

            LogService.Log($"Exception register started");

            foreach (var client in DataClients)
                client.PushToWrite(fileName, text, Kit.DiagnisticsCurrentDirectory);
            
            LogService.Log($"Exception register completed at {TimeHelper.FormattedLatency(startTime)}");
            exception.Data["registered"] = true;
        }
    }
}
