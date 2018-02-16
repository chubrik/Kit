using Kit.Clients;
using System.IO;

namespace Kit.Tests {
    public class Program {

        public static void Main(string[] args) {

            Kit.Setup(
                baseDirectory: args.Length > 0 ? args[0] : null
            );

            var azureStorageLogin = File.ReadAllLines("../../azure-storage-login.txt");

            BlobClient.Setup(
                accountName: azureStorageLogin[0],
                accountKey: azureStorageLogin[1],
                containerName: "test"
            );

            Kit.Run(ct => new Test().RunAsync(ct));
        }
    }
}
