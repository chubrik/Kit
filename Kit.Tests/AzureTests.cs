using Kit.Azure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Kit.Tests
{
    [TestClass]
    public class AzureTests : TestsBase
    {
        private static readonly string[] Credentials = File.ReadAllLines("../../../../../azure-credentials.txt");

        [TestMethod]
        public void WriteAndRead()
        {
            var testName = $"{GetType().Name}.{nameof(WriteAndRead)}";

            TestExecute(testName, () =>
            {
                Setup();
                var testFileName = "test.json";

                using (var stream = FileClient.OpenRead("../../Kit.Tests.runtimeconfig.json"))
                    AzureBlobClient.Write(testFileName, stream, targetDirectory: nameof(WriteAndRead));

                using (var stream = FileClient.OpenWrite(testFileName))
                    AzureBlobClient.Read(testFileName, stream, targetDirectory: nameof(WriteAndRead));
            });

            //ReportService.ReportSuccess("My report", "My report body", "../Kit.Core.dll");
            //throw new Exception("My exception");
        }

        private static void Setup() =>
            AzureBlobClient.Setup(
                accountName: Credentials[0],
                accountKey: Credentials[1],
                containerName: "test"
            );
    }
}
