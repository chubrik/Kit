using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Utils.Helpers;
using Utils.Models;

namespace Utils.Services {
    public class BlobService {

        private static ExceptionService ExceptionService => ExceptionService.Instance;

        private static readonly AccessCondition accessCondition = new AccessCondition();
        private static readonly BlobRequestOptions options = new BlobRequestOptions();
        private static readonly OperationContext operationContext = new OperationContext();

        private CloudBlobContainer container;
        private string workDirectory;

        public BlobService(BlobOptions options) {

            var account = CloudStorageAccount.Parse(
                $"DefaultEndpointsProtocol=https;AccountName={options.AccountName};AccountKey={options.AccountKey}");

            var client = account.CreateCloudBlobClient();
            container = client.GetContainerReference(options.ContainerName);
            workDirectory = options.WorkDirectory ?? string.Empty;
        }

        public async Task<bool> DownloadAsync(Stream target, string path, CancellationToken cancellationToken) {
            try {
                var fullPath = Path.Combine(workDirectory, path);
                LogHelper.WriteLine($"Download blob \"{fullPath}\"");
                var blob = container.GetBlockBlobReference(fullPath);
                await blob.DownloadToStreamAsync(target, accessCondition, options, operationContext, cancellationToken);
                return true;
            }
            catch (Exception exception) {
                ExceptionService.Register(exception);
                return false;
            }
        }

        public async Task<bool> UploadAsync(Stream source, string path, CancellationToken cancellationToken) {
            try {
                var fullPath = Path.Combine(workDirectory, path);
                LogHelper.WriteLine($"Upload blob \"{fullPath}\"");
                var blob = container.GetBlockBlobReference(fullPath);
                await blob.UploadFromStreamAsync(source, accessCondition, options, operationContext, cancellationToken);
                return true;
            }
            catch (Exception exception) {
                ExceptionService.Register(exception);
                return false;
            }
        }
    }
}
