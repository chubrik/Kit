using System;
using System.Threading;
using System.Threading.Tasks;
using Utils.Services;

namespace Utils.Tests {
    public class Test {

        private BlobClient BlobService => BlobClient.Instance;
        private ExceptionService ExceptionService => ExceptionService.Instance;
        private FileClient FileService => FileClient.Instance;

        public async Task RunAsync(CancellationToken ct) {
            Console.WriteLine("Hello World!");

            using (var stream = await FileService.OpenReadAsync("../Test.cs", ct))
                await BlobService.WriteAsync("file.ext", stream, ct);

            using (var stream = await FileService.OpenWriteAsync("Test2.cs", ct))
                await BlobService.ReadAsync("file.ext", stream, ct);
        }
    }
}
