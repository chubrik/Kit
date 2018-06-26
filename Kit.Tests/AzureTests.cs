using Kit.Azure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kit.Tests
{
    [TestClass]
    public class AzureTests : TestsBase
    {
        [TestMethod]
        public void WriteAndRead()
        {
            TestInitialize($"{GetType().Name}.{nameof(WriteAndRead)}");
            Setup();
            var testFileName = "test.xml";

            using (var readStream = FileClient.OpenRead("../../Kit.Tests.csproj"))
                AzureBlobClient.Write(testFileName, readStream);

            using (var writeStream = FileClient.OpenWrite(testFileName))
                AzureBlobClient.ReadTo(testFileName, writeStream);
        }

        private static void Setup()
        {
            var credentials = FileClient.ReadLines("../../../../azure-credentials.txt");

            AzureBlobClient.Setup(
                accountName: credentials[0],
                accountKey: credentials[1],
                containerName: "test"
            );
        }
    }
}
