using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kit {
    public class Kit {

        private static readonly CancellationTokenSource _сancellationTokenSource = new CancellationTokenSource();
        public static CancellationToken CancellationToken => _сancellationTokenSource.Token;
        private static readonly string _formattedStartTime = DateTimeOffset.Now.ToString("dd.MM.yyyy HH.mm.ss");
        private static bool _pressAnyKeyToExit = true;
        internal static string _baseDirectory = "$work";
        internal static string _workingDirectory = string.Empty;
        private static string _diagnosticsDirectory = "$diagnostics";

        internal static string DiagnisticsCurrentDirectory =>
            PathHelper.Combine(_diagnosticsDirectory, _formattedStartTime);

        #region Setup & Initialize

        public static void Setup(
            bool? pressAnyKeyToExit = null,
            string baseDirectory = null,
            string workingDirectory = null,
            string diagnosticsDirectory = null) {

            if (pressAnyKeyToExit != null)
                _pressAnyKeyToExit = (bool)pressAnyKeyToExit;

            if (baseDirectory != null)
                _baseDirectory = baseDirectory;

            if (workingDirectory != null)
                _workingDirectory = workingDirectory;

            if (diagnosticsDirectory != null)
                _diagnosticsDirectory = diagnosticsDirectory;
        }

        private static void Initialize() {
            try {
                throw new Exception("Test exception");
            }
            catch (Exception exception) {
                ExceptionHandler.Register(exception, level: LogLevel.Log);
                ReportService.Report(exception.Message, exception.ToString(), logLevel: LogLevel.Log);
            }
        }

        #endregion

        public static void Execute(Action @delegate) =>
            Execute(cancellationToken => {
                @delegate();
                return Task.CompletedTask;
            });

        public static void Execute(Func<Task> delegateAsync) =>
            Execute(cancellationToken => delegateAsync());

        public static void Execute(Func<CancellationToken, Task> delegateAsync) =>
            ExecuteAsync(delegateAsync).Wait();

        private static async Task ExecuteAsync(Func<CancellationToken, Task> delegateAsync) {
            var startTime = DateTimeOffset.Now;

            try {
                LogService.LogInfo("Kit started");
                Initialize();
                LogService.Log($"Kit ready at {TimeHelper.FormattedLatency(startTime)}");
                await delegateAsync(CancellationToken);
                LogService.LogInfo($"Completed at {TimeHelper.FormattedLatency(startTime)}");
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
                ReportService.Report(exception.Message, exception.ToString());
                LogService.LogError($"Failed at {TimeHelper.FormattedLatency(startTime)}");
            }

            if (_pressAnyKeyToExit) {
                Console.Write("\nPress any key to exit...");
                Console.ReadKey(true);
            }
        }

        public static void Exit() {
            LogService.LogWarning("Exit requested");
            _сancellationTokenSource.Cancel();
        }
    }
}
