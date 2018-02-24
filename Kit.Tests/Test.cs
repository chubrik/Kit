using Kit.Azure;
using Kit.Http;
using System;

namespace Kit.Tests {
    public class Test {

        public void Run() {
            LogService.LogInfo("Hello World!");

            var html = HttpClient.GetText("https://www.google.com/");

            using (var stream = FileClient.OpenRead("../Test.cs"))
                AzureBlobClient.Write("file.ext", stream);

            using (var stream = FileClient.OpenWrite("Test2.cs"))
                AzureBlobClient.Read("file.ext", stream);

            ReportService.ReportSuccess("My report", "My report body", "../Program.cs");
            throw new Exception("My exception");
        }
    }
}
