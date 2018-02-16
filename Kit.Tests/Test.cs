using Kit.Clients;
using Kit.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Tests {
    public class Test {

        public async Task RunAsync(CancellationToken ct) {
            LogService.LogInfo("Hello World!");

            using (var stream = FileClient.OpenRead("../Test.cs"))
                await BlobClient.WriteAsync("file.ext", stream, ct);

            using (var stream = FileClient.OpenWrite("Test2.cs"))
                await BlobClient.ReadAsync("file.ext", stream, ct);
        }
    }
}
