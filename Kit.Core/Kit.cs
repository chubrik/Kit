using Kit.Abstractions;
using Kit.Services;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kit {
    public class Kit {
        
        public static string BaseDirectory { get; private set; } = string.Empty;
        public static string DiagnosticsDirectory { get; private set; } = "$diagnostics";

        public static void Setup(string baseDirectory = null) {

            if (baseDirectory != null)
                BaseDirectory = baseDirectory;
        }

        public static void Run(Action @delegate) {

            Task delegateAsync(CancellationToken CancellationToken) {
                @delegate();
                return Task.CompletedTask;
            }

            RunAsync(delegateAsync, CancellationToken.None).Wait();
        }

        public static void Run(Func<CancellationToken, Task> delegateAsync) =>
            RunAsync(delegateAsync, CancellationToken.None).Wait();

        private static async Task RunAsync(Func<CancellationToken, Task> delegateAsync, CancellationToken cancellationToken) {
            try {
                Initialize();
                await delegateAsync(cancellationToken);
                LogService.LogInfo("Done.");
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionService.Register(exception);
            }

            Console.Write("\nPress any key to exit...");
            Console.ReadKey(true);
        }

        private static void Initialize() {
            try {
                throw new Exception("Test exception");
            }
            catch (Exception exception) {
                ExceptionService.Register(exception, LogLevel.Log);
            }
        }
    }
}
