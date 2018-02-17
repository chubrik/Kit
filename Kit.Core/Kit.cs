using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kit {
    public class Kit {

        internal static string DiagnosticsDirectory = "$diagnostics";
        internal const string LogFileName = "$log.txt";

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
                LogService.LogInfo("Start");
                TextException();
                await delegateAsync(cancellationToken);
                LogService.LogInfo("Done");
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
            }

            Console.Write("\nPress any key to exit...");
            Console.ReadKey(true);
        }

        private static void TextException() {
            try {
                throw new Exception("Test exception");
            }
            catch (Exception exception) {
                ExceptionHandler.Register(exception, LogLevel.Log);
            }
        }
    }
}
