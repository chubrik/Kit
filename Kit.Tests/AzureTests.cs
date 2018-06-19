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

            using (var stream = FileClient.OpenRead("../../Kit.Tests.csproj"))
                AzureBlobClient.Write(testFileName, stream);

            using (var stream = FileClient.OpenWrite(testFileName))
                AzureBlobClient.Read(testFileName, stream);
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
