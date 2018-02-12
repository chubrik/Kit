using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        #region Download

        public Task<string> DownloadTextAsync(string path, CancellationToken cancellationToken) =>
            DownloadBaseAsync(path, cancellationToken, (fullPath, blob) =>
                blob.DownloadTextAsync(Encoding.UTF8, accessCondition, options, operationContext, cancellationToken));

        public IReadOnlyList<string> DownloadLines(string path, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public IReadOnlyList<string> DownloadBytes(string path, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        private async Task<T> DownloadBaseAsync<T>(string path, CancellationToken cancellationToken, Func<string, CloudBlockBlob, Task<T>> action) {
            try {
                var fullPath = PathHelper.Combine(workDirectory, path);
                LogHelper.WriteLine($"Download blob \"{fullPath}\"");
                var blob = container.GetBlockBlobReference(fullPath);
                return await action(fullPath, blob);
            }
            catch (Exception exception) {
                ExceptionService.Register(exception);
                throw;
            }
        }

        public async Task DownloadStreamAsync(string path, Stream target, CancellationToken cancellationToken) {
            try {
                var fullPath = PathHelper.Combine(workDirectory, path);
                LogHelper.WriteLine($"Download blob \"{fullPath}\"");
                var blob = container.GetBlockBlobReference(fullPath);
                await blob.DownloadToStreamAsync(target, accessCondition, options, operationContext, cancellationToken);
            }
            catch (Exception exception) {
                ExceptionService.Register(exception);
                throw;
            }
        }

        #endregion

        #region Upload

        public Task UploadAsync(string path, string text, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task UploadAsync(string path, string[] lines, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task UploadAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task UploadAsync(string path, byte[] bytes, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task UploadAsync(string path, IEnumerable<byte> bytes, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public async Task UploadAsync(string path, Stream source, CancellationToken cancellationToken) {
            try {
                var fullPath = PathHelper.Combine(workDirectory, path);
                LogHelper.WriteLine($"Upload blob \"{fullPath}\"");
                var blob = container.GetBlockBlobReference(fullPath);
                await blob.UploadFromStreamAsync(source, accessCondition, options, operationContext, cancellationToken);
            }
            catch (Exception exception) {
                ExceptionService.Register(exception);
                throw;
            }
        }

        #endregion
    }
}
