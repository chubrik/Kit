using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kit
{
    public static class Kit
    {
        private static readonly CancellationTokenSource _сancellationTokenSource = new CancellationTokenSource();
        public static CancellationToken CancellationToken => _сancellationTokenSource.Token;
        private static readonly string _formattedStartTime = DateTimeOffset.Now.ToString("dd.MM.yyyy HH.mm.ss");
        private static bool _pressAnyKeyToExit = true;
        internal static string BaseDirectory { get; private set; } = "$work";
        internal static string WorkingDirectory { get; private set; } = string.Empty;
        private static string _diagnosticsDirectory = "$diagnostics";
        private static DateTimeOffset _cancellationRequestTime;
        private static bool _isCanceled;
        private static bool _isFailed;
        internal static bool IsTest { get; private set; }

        public static string DiagnisticsCurrentDirectory =>
            PathHelper.Combine(_diagnosticsDirectory, _formattedStartTime);

        #region Setup & Initialize

        public static void Setup(
            bool? pressAnyKeyToExit = null,
            string baseDirectory = null,
            string workingDirectory = null,
            string diagnosticsDirectory = null,
            bool? test = null)
        {
            if (pressAnyKeyToExit != null)
                _pressAnyKeyToExit = (bool)pressAnyKeyToExit;

            if (baseDirectory != null)
                BaseDirectory = baseDirectory;

            if (workingDirectory != null)
                WorkingDirectory = workingDirectory;

            if (diagnosticsDirectory != null)
                _diagnosticsDirectory = diagnosticsDirectory;

            if (test != null)
                IsTest = (bool)test;
        }

        private static void Initialize()
        {
            try
            {
                throw new Exception("Test exception");
            }
            catch (Exception exception)
            {
                ExceptionHandler.Register(exception, level: LogLevel.Log);
                ReportService.Report(exception.Message, exception.ToString(), logLevel: LogLevel.Log);
                // no throw for test exception
            }
        }

        #endregion

        #region Execute delegate

        public static void Execute(Action @delegate) =>
            Execute(cancellationToken =>
            {
                @delegate();
                return Task.CompletedTask;
            });

        public static void Execute(Func<Task> delegateAsync) =>
            Execute(cancellationToken => delegateAsync());

        public static void Execute(Func<CancellationToken, Task> delegateAsync)
        {
            try
            {
                var startTime = DateTimeOffset.Now;
                LogService.LogInfo("Kit started");
                Initialize();
                LogService.Log($"Kit ready at {TimeHelper.FormattedLatency(startTime)}");
                ExecuteBaseAsync(delegateAsync, "Main delegate").Wait();
                ThreadService.AwaitAll();

                if (_isFailed)
                    LogService.LogError($"Failed at {TimeHelper.FormattedLatency(startTime)}");
                else if (_isCanceled)
                    LogService.LogWarning($"Canceled at {TimeHelper.FormattedLatency(startTime)}");
                else
                    LogService.LogInfo($"Completed at {TimeHelper.FormattedLatency(startTime)}");
            }
            catch (Exception exception)
            {
                Debug.Fail(exception.ToString());

                if (IsTest)
                    throw;

                _isFailed = true;
                ConsoleClient.Disable();
                _сancellationTokenSource.Cancel();
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine((Console.CursorLeft > 0 ? "\n" : string.Empty) + "\n Kit internal error \n");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(exception.ToString().Trim());
                // no throw for internal error
            }

            if ((_isFailed || _pressAnyKeyToExit) && !IsTest)
            {
                Console.Write("\nPress any key to exit...");
                Console.ReadKey(true);
            }
        }

        internal static async Task ExecuteBaseAsync(Func<CancellationToken, Task> delegateAsync, string delegateName)
        {
            var startTime = DateTimeOffset.Now;

            try
            {
                LogService.Log($"{delegateName} started");
                await delegateAsync(CancellationToken);
                LogService.Log($"{delegateName} completed at {TimeHelper.FormattedLatency(startTime)}");
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsAllowed());

                if (IsTest)
                    throw;

                if (exception.IsCanceled())
                {
                    _isCanceled = true;
                    ExceptionHandler.Register(exception, level: LogLevel.Warning);
                    LogService.Log($"{delegateName} cancellation time is {TimeHelper.FormattedLatency(_cancellationRequestTime)}");
                    LogService.LogWarning($"{delegateName} canceled at {TimeHelper.FormattedLatency(startTime)}");
                }
                else
                {
                    _isFailed = true;

                    if (!LogService.Clients.Contains(ConsoleClient.Instance))
                        LogService.Clients.Add(ConsoleClient.Instance);

                    ExceptionHandler.Register(exception);
                    ReportService.ReportError(exception.Message, exception.ToString());
                    LogService.LogError($"{delegateName} failed at {TimeHelper.FormattedLatency(startTime)}");
                    Cancel();
                }

                // no throw for delegate error
            }
        }

        #endregion

        #region Utils

        public static void Cancel()
        {
            if (_cancellationRequestTime == default(DateTimeOffset))
                _cancellationRequestTime = DateTimeOffset.Now;

            LogService.Log("Kit cancel requested", level: _isFailed ? LogLevel.Log : LogLevel.Warning);

            if (!_сancellationTokenSource.IsCancellationRequested)
            {
                ConsoleClient.ReduceToLogOnly();
                _сancellationTokenSource.Cancel();
            }
        }

        #endregion
    }
}
