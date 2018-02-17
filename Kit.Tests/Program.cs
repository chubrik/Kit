using Kit.Azure;
using System.IO;

namespace Kit.Tests {
    public class Program {

        public static void Main(string[] args) {
            ConsoleClient.Setup(minLevel: LogLevel.Log);

            var azureStorageLogin = File.ReadAllLines("../../azure-storage-login.txt");

            AzureBlobClient.Setup(
                accountName: azureStorageLogin[0],
                accountKey: azureStorageLogin[1],
                containerName: "test"
            );

            Kit.Execute(ct => new Test().RunAsync(ct));
        }
    }
}
