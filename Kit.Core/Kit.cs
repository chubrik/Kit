using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kit {
    public class Kit {

        internal static string DiagnosticsDirectory = "$diagnostics";

        public static void Setup(string diagnosticsDirectory = null) {

            if (diagnosticsDirectory != null)
                DiagnosticsDirectory = diagnosticsDirectory;
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
                LogService.LogSuccess("Complete");
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
