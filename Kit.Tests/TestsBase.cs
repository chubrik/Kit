using Kit.Azure;
using Kit.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Tests
{
    public class TestsBase
    {
        public static void TestExecute(string testName, Action @delegate) =>
            TestExecute(testName, cancellationToken =>
            {
                @delegate();
                return Task.CompletedTask;
            });

        public static void TestExecute(string testName, Func<Task> delegateAsync) =>
            TestExecute(testName, cancellationToken => delegateAsync());

        public static void TestExecute(string testName, Func<CancellationToken, Task> delegateAsync)
        {
            var baseDirectory = $"$tests/{testName}";

            if (Directory.Exists(baseDirectory))
                Directory.Delete(baseDirectory, recursive: true);

            Assert.IsFalse(Directory.Exists(baseDirectory));

            Kit.Setup(
                pressAnyKeyToExit: false,
                baseDirectory: baseDirectory);

            Kit.Execute(delegateAsync);
        }

        public static void Setup()
        {
            ConsoleClient.Setup(minLevel: LogLevel.Log);
            HttpClient.Setup(cache: CacheMode.Full);

            var azureStorageLogin = File.ReadAllLines("../../../../../azure-storage-login.txt");

            AzureBlobClient.Setup(
                accountName: azureStorageLogin[0],
                accountKey: azureStorageLogin[1],
                containerName: "test"
            );

            //Kit.Execute(() => new Tests().Run());
        }
    }
}
