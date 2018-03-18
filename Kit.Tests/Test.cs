using Kit.Azure;
using Kit.Http;
using System;

namespace Kit.Tests {
    public class Test {

        public void Run() {
            LogService.LogInfo("Hello World!");

            using (var http = new HttpClient()) {
                var html = http.GetText("https://www.google.com/");
            }

            using (var stream = FileClient.OpenRead("../Kit.Tests.runtimeconfig.json"))
                AzureBlobClient.Write("file.ext", stream);

            using (var stream = FileClient.OpenWrite("test.json"))
                AzureBlobClient.Read("file.ext", stream);

            ReportService.ReportSuccess("My report", "My report body", "../Kit.Core.dll");
            throw new Exception("My exception");
        }
    }
}
