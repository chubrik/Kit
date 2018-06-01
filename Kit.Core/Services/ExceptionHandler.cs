﻿using System;
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

            if (exception.IsCanceled())
                level = LogLevel.Log;

            var message = OneLineMessageWithPlace(exception);
            var count = ++_counter;
            var logLabel = $"Exception #{count}";
            LogService.Log($"{logLabel}: {message}", level);
            var text = $"{logLabel}\r\n{message}\r\n{ExtendedDump(exception)}";
            var fileName = PathHelper.SafeFileName($"{count.ToString().PadLeft(3, '0')} {message}.txt");

            foreach (var client in DataClients)
                client.PushToWrite(fileName, text, Kit.DiagnisticsCurrentDirectory);

            LogService.Log($"{logLabel} registered at {TimeHelper.FormattedLatency(startTime)}");
        }

        #region Utils

        private static string OneLineMessageWithPlace(Exception exception)
        {
            var message = exception.Message.Replace("\r\n", " ");
            var match = Regex.Match(exception.ToString(), @"(\w+\.cs):line (\d+)");

            if (match.Success)
                message += $" ({match.Groups[1].Value}:{match.Groups[2].Value})";

            return message;
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
