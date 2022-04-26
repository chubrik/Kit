﻿using Chubrik.Kit.Azure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chubrik.Kit.Tests
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

            using (var fileStream = FileClient.OpenRead("../../Kit.Tests.csproj"))
                AzureBlobClient.WriteFrom(testFileName, fileStream);

            using (var blobStream = AzureBlobClient.OpenRead(testFileName))
                FileClient.WriteFrom(testFileName, blobStream);
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
