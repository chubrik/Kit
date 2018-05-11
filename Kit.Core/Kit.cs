using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kit
{
    public class Kit
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

        internal static string DiagnisticsCurrentDirectory =>
            PathHelper.Combine(_diagnosticsDirectory, _formattedStartTime);

        #region Setup & Initialize

        public static void Setup(
            bool? pressAnyKeyToExit = null,
            string baseDirectory = null,
            string workingDirectory = null,
            string diagnosticsDirectory = null)
        {
            if (pressAnyKeyToExit != null)
                _pressAnyKeyToExit = (bool)pressAnyKeyToExit;

            if (baseDirectory != null)
                BaseDirectory = baseDirectory;

            if (workingDirectory != null)
                WorkingDirectory = workingDirectory;

            if (diagnosticsDirectory != null)
                _diagnosticsDirectory = diagnosticsDirectory;
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
            }
        }

        #endregion

        public static void Execute(Action @delegate) =>
            Execute(cancellationToken =>
            {
                @delegate();
                return Task.CompletedTask;
            });

        public static void Execute(Func<Task> delegateAsync) =>
            Execute(cancellationToken => delegateAsync());

        public static void Execute(Func<CancellationToken, Task> delegateAsync) =>
            ExecuteAsync(delegateAsync).Wait();

        private static async Task ExecuteAsync(Func<CancellationToken, Task> delegateAsync)
        {
            var startTime = DateTimeOffset.Now;

            try
            {
                LogService.LogInfo("Kit started");
                Initialize();
                LogService.Log($"Kit ready at {TimeHelper.FormattedLatency(startTime)}");
                await delegateAsync(CancellationToken);
            }
            catch (Exception exception)
            {
                if (!exception.IsCanceled())
                {
                    Debug.Fail(exception.ToString());
                    SetFailed();
                }
                else
                    SetCanceled();

                ExceptionHandler.Register(exception);

                if (!exception.IsCanceled())
                    Cancel();

                ReportService.Report(exception.Message, exception.ToString());
            }

            await ThreadService.AwaitAll();

            if (_isFailed)
                LogService.LogError($"Failed at {TimeHelper.FormattedLatency(startTime)}");

            else if (_isCanceled)
            {
                LogService.Log($"Cancellation time is {TimeHelper.FormattedLatency(_cancellationRequestTime)}");
                LogService.LogWarning($"Canceled at {TimeHelper.FormattedLatency(startTime)}");
            }
            else
                LogService.LogInfo($"Completed at {TimeHelper.FormattedLatency(startTime)}");

            if (_pressAnyKeyToExit || _isFailed)
            {
                Console.Write("\nPress any key to exit...");
                Console.ReadKey(true);
            }
        }

        public static void Cancel()
        {
            if (_cancellationRequestTime == default(DateTimeOffset))
                _cancellationRequestTime = DateTimeOffset.Now;

            LogService.Log("Kit cancel requested", level: _isFailed ? LogLevel.Log : LogLevel.Warning);

            if (!_сancellationTokenSource.IsCancellationRequested)
                _сancellationTokenSource.Cancel();
        }

        internal static void SetCanceled() => _isCanceled = true;

        internal static void SetFailed()
        {
            _isFailed = true;

            if (!LogService.Clients.Contains(ConsoleClient.Instance))
                LogService.Clients.Add(ConsoleClient.Instance);
        }
    }
}
