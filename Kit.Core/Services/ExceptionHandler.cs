using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Kit
{
    public class ExceptionHandler
    {
        private ExceptionHandler() { }

        private static int _counter = 0;

        public static readonly List<IDataClient> DataClients = new List<IDataClient> {
            FileClient.Instance
        };

        public static void Register(Exception exception, LogLevel level = LogLevel.Error)
        {
            var startTime = DateTimeOffset.Now;
            var message = Regex.Replace(exception.Message, @"\r?\n", " ") + GetPlaceSuffix(exception);
            var count = ++_counter;
            var logLabel = $"Exception #{count}";
            LogService.Log($"{logLabel}: {message}", level);
            var fullText = $"{logLabel}\r\n{message}\r\n{ExtendedDump(exception)}";
            var fileName = PathHelper.SafeFileName($"{count.ToString().PadLeft(3, '0')} {message}.txt");

            foreach (var client in DataClients)
                client.PushToWrite(fileName, fullText, Kit.DiagnisticsCurrentDirectory);

            LogService.Log($"{logLabel} registered at {TimeHelper.FormattedLatency(startTime)}");
        }

        #region Utils

        private static string GetPlaceSuffix(Exception exception)
        {
            var match = Regex.Match(exception.ToString(), @"(\w+\.cs):line (\d+)");

            return match.Success
                ? $" ({match.Groups[1].Value}:{match.Groups[2].Value})"
                : string.Empty;
        }

        private static string ExtendedDump(Exception exception)
        {
            var result = string.Empty;
            var innerest = exception.FirstInnerestException();

            if (innerest != exception)
                result += $"\r\n\r\n\r\nFIRST INNEREST EXCEPTION:\r\n\r\n{innerest}\r\n";

            var dump = exception.ToString().Replace(" --->", "\r\n--->");
            result += $"\r\n\r\n\r\nFULL DUMP:\r\n\r\n{dump}\r\n";
            return result;
        }

        #endregion
    }
}
