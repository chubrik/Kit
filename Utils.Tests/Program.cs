using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Utils.Services;

namespace Utils.Tests {
    public class Program {

        public static ExceptionService ExceptionService => ExceptionService.Instance;
        public static BlobService BlobService => BlobService.Instance;

        public static void Main(string[] args) => MainAsync(args, CancellationToken.None).Wait();

        private static async Task MainAsync(string[] args, CancellationToken cancellationToken) {
            try {
                Initialize(args);
                await new Test().Run(cancellationToken);
            }
            catch (Exception exception) {
                ExceptionService.Register(exception);
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
