using Kit.Azure;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Tests {
    public class Test {

        public async Task RunAsync(CancellationToken ct) {
            LogService.LogInfo("Hello World!");

            using (var stream = FileClient.OpenRead("../Test.cs"))
                await AzureBlobClient.WriteAsync("file.ext", stream, ct);

            using (var stream = FileClient.OpenWrite("Test2.cs"))
                await AzureBlobClient.ReadAsync("file.ext", stream, ct);
        }
    }
}
