using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kit {
    public class Kit {

        private static readonly CancellationTokenSource сancellationTokenSource = new CancellationTokenSource();
        public static CancellationToken CancellationToken => сancellationTokenSource.Token;
        private static readonly string formattedStartTime = DateTimeOffset.Now.ToString("dd.MM.yyyy HH.mm.ss");
        internal static string BaseDirectory = "$work";
        internal static string WorkingDirectory = string.Empty;
        private static string diagnosticsDirectory = "$diagnostics";

        internal static string DiagnisticsCurrentDirectory =>
            PathHelper.Combine(diagnosticsDirectory, formattedStartTime);

        #region Setup & Initialize

        public static void Setup(
            string baseDirectory = null,
            string workingDirectory = null,
            string diagnosticsDirectory = null) {

            if (baseDirectory != null)
                BaseDirectory = baseDirectory;

            if (workingDirectory != null)
                WorkingDirectory = workingDirectory;

            if (diagnosticsDirectory != null)
                Kit.diagnosticsDirectory = diagnosticsDirectory;
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
            Execute((CancellationToken cancellationToken) => {
                @delegate();
                return Task.CompletedTask;
            });

        public static void Execute(Func<Task> delegateAsync) =>
            Execute((CancellationToken cancellationToken) => {
                delegateAsync();
                return Task.CompletedTask;
            });

        public static void Execute(Func<CancellationToken, Task> delegateAsync) =>
            ExecuteAsync(delegateAsync).Wait();

        private static async Task ExecuteAsync(
            Func<CancellationToken, Task> delegateAsync) {

            var startTime = DateTimeOffset.Now;

            try {
                LogService.LogInfo("Kit is started");
                Initialize();
                LogService.Log($"Kit is ready at {TimeHelper.FormattedLatency(startTime)}");
                await delegateAsync(CancellationToken);
                LogService.LogInfo($"Completed at {TimeHelper.FormattedLatency(startTime)}");
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
                ReportService.Report(exception.Message, exception.ToString());
                LogService.LogError($"Failed at {TimeHelper.FormattedLatency(startTime)}");
            }

            Console.Write("\nPress any key to exit...");
            Console.ReadKey(true);
        }

        public static void Exit() => сancellationTokenSource.Cancel();
    }
}
