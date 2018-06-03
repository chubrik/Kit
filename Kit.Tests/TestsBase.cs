using Kit.Azure;
using Kit.Http;
using Kit.Mail;
using System;
using System.IO;

namespace Kit.Tests
{
    public class TestsBase
    {
        public static void TestExecute(string baseDirectory, Action @delegate)
        {
            Kit.Setup(
                pressAnyKeyToExit: false,
                baseDirectory: baseDirectory);

            Kit.Execute(@delegate);
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

            var mailCredentials = File.ReadAllLines("../../../../../mail-credentials.txt");

            MailClient.Setup(
                host: mailCredentials[0],
                port: int.Parse(mailCredentials[1]),
                userName: mailCredentials[2],
                password: mailCredentials[3],
                from: mailCredentials[4],
                to: mailCredentials[5]
            );

            //Kit.Execute(() => new Tests().Run());
        }
    }
}
