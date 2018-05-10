using System;
using System.Collections.Generic;

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

            var message = ExceptionHelper.OneLineMessageWithPlace(exception);
            var count = ++_counter;
            var logLabel = $"Exception #{count}";
            LogService.Log($"{logLabel}: {message}", level);
            var text = $"{logLabel}\r\n{message}\r\n{ExceptionHelper.ExtendedDump(exception)}";
            var fileName = PathHelper.SafeFileName($"{count.ToString().PadLeft(3, '0')} {message}.txt");

            foreach (var client in DataClients)
                client.PushToWrite(fileName, text, Kit.DiagnisticsCurrentDirectory);

            LogService.Log($"{logLabel} registered at {TimeHelper.FormattedLatency(startTime)}");
        }
    }
}
