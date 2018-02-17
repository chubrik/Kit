using Kit.Azure;
using Kit.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Tests {
    public class Test {

        public async Task RunAsync(CancellationToken ct) {
            LogService.LogInfo("Hello World!");

            var html = await HttpClient.GetAsync("https://www.google.com/", ct);

            using (var stream = FileClient.OpenRead("../Test.cs"))
                await AzureBlobClient.WriteAsync("file.ext", stream, ct);

            using (var stream = FileClient.OpenWrite("Test2.cs"))
                await AzureBlobClient.ReadAsync("file.ext", stream, ct);

            ReportService.ReportSuccess("My report", "My report body");
            //throw new Exception("My exception");
        }
    }
}
