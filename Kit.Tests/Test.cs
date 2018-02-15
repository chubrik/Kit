using Kit.Clients;
using Kit.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Tests {
    public class Test {

        private BlobClient BlobClient => BlobClient.Instance;
        private ExceptionService ExceptionService => ExceptionService.Instance;
        private FileClient FileClient => FileClient.Instance;

        public async Task RunAsync(CancellationToken ct) {
            Console.WriteLine("Hello World!");

            using (var stream = await FileClient.OpenReadAsync("../Test.cs", ct))
                await BlobClient.WriteAsync("file.ext", stream, ct);

            using (var stream = await FileClient.OpenWriteAsync("Test2.cs", ct))
                await BlobClient.ReadAsync("file.ext", stream, ct);
        }
    }
}
