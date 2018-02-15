using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Utils.Services;

namespace Utils.Tests {
    public class Program {

        public static ExceptionService ExceptionService => ExceptionService.Instance;
        public static BlobClient BlobService => BlobClient.Instance;

        public static void Main(string[] args) => MainAsync(args, CancellationToken.None).Wait();

        private static async Task MainAsync(string[] args, CancellationToken ct) {
            try {
                Initialize(args);
                await new Test().RunAsync(ct);
            }
            catch (Exception exception) {
                await ExceptionService.RegisterAsync(exception, ct);
                Debugger.Break();
            }
        }

        public static void Initialize(string[] args) {

            Utils.Setup(
                baseDirectory: args.Length > 0 ? args[0] : null
            );

            var lines = File.ReadAllLines("../../azure-storage-login.txt");

            BlobService.Setup(
                accountName: lines[0],
                accountKey: lines[1],
                containerName: "test"
            );
        }
    }
}
