using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kit {
    public class Kit {

        private static readonly string formattedStartTime = DateTimeOffset.Now.ToString("dd.MM.yyyy HH.mm.ss");
        internal static string BaseDirectory = "$work";
        internal static string WorkingDirectory = string.Empty;
        private static string diagnosticsDirectory = "$diagnostics";

        internal static string DiagnisticsCurrentDirectory =>
            PathHelper.Combine(diagnosticsDirectory, formattedStartTime);

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

        public static void Execute(Action @delegate) {

            Task delegateAsync(CancellationToken CancellationToken) {
                @delegate();
                return Task.CompletedTask;
            }

            ExecuteAsync(delegateAsync, CancellationToken.None).Wait();
        }

        public static void Execute(Func<CancellationToken, Task> delegateAsync) =>
            ExecuteAsync(delegateAsync, CancellationToken.None).Wait();

        private static async Task ExecuteAsync(
            Func<CancellationToken, Task> delegateAsync, CancellationToken cancellationToken) {

            try {
                LogService.Log("Start");
                Initialize();
                LogService.Log("Ready");
                await delegateAsync(cancellationToken);
                LogService.LogInfo("Complete");
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
                ReportService.ReportError(exception.Message, exception.ToString());
                LogService.LogError("Fail");
            }

            Console.Write("\nPress any key to exit...");
            Console.ReadKey(true);
        }

        private static void Initialize() {
            try {
                throw new Exception("Test exception");
            }
            catch (Exception exception) {
                ExceptionHandler.Register(exception, LogLevel.Log);
                ReportService.Report(exception.Message, exception.ToString());
            }
        }
    }
}
