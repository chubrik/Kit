using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils.Abstractions;
using Utils.Helpers;

namespace Utils.Services {
    public class BlobClient : IDataClient {

        private static ExceptionService ExceptionService => ExceptionService.Instance;
        private static LogService LogService => LogService.Instance;

        private static BlobClient instance;
        public static BlobClient Instance => instance ?? (instance = new BlobClient());
        private BlobClient() { }

        private static readonly AccessCondition accessCondition = new AccessCondition();
        private static readonly BlobRequestOptions options = new BlobRequestOptions();
        private static readonly OperationContext operationContext = new OperationContext();

        private CloudBlobContainer container;
        private string targetDirectory = string.Empty;

        public void Setup(
            string accountName = null,
            string accountKey = null,
            string containerName = null,
            string targetDirectory = null) {

            var account = CloudStorageAccount.Parse(
                $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey}");

            var client = account.CreateCloudBlobClient();
            container = client.GetContainerReference(containerName);

            if (targetDirectory != null)
                this.targetDirectory = targetDirectory;
        }

        #region Read

        public Task<string> ReadTextAsync(string path, CancellationToken cancellationToken) =>
            ReadBaseAsync(path, cancellationToken, (fullPath, blob) =>
                blob.DownloadTextAsync(Encoding.UTF8, accessCondition, options, operationContext, cancellationToken));

        public Task<List<string>> ReadLinesAsync(string path, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task<List<byte>> ReadBytesAsync(string path, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        private async Task<T> ReadBaseAsync<T>(string path, CancellationToken cancellationToken, Func<string, CloudBlockBlob, Task<T>> action) {
            try {
                var fullPath = PathHelper.Combine(targetDirectory, path);
                await LogService.LogAsync($"Start downloading blob \"{fullPath}\"", cancellationToken);
                var blob = container.GetBlockBlobReference(fullPath);
                var result = await action(fullPath, blob);
                await LogService.LogAsync($"End downloading blob \"{fullPath}\"", cancellationToken);
                return result;
            }
            catch (Exception exception) {
                await ExceptionService.RegisterAsync(exception, cancellationToken);
                throw;
            }
        }

        public async Task ReadAsync(string path, Stream target, CancellationToken cancellationToken) {
            try {
                var fullPath = PathHelper.Combine(targetDirectory, path);
                await LogService.LogAsync($"Start downloading blob \"{fullPath}\"", cancellationToken);
                var blob = container.GetBlockBlobReference(fullPath);
                await blob.DownloadToStreamAsync(target, accessCondition, options, operationContext, cancellationToken);
                await LogService.LogAsync($"End downloading blob \"{fullPath}\"", cancellationToken);
            }
            catch (Exception exception) {
                await ExceptionService.RegisterAsync(exception, cancellationToken);
                throw;
            }
        }

        public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        #endregion

        #region Write

        public Task WriteAsync(string path, string text, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task WriteAsync(string path, string[] lines, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task WriteAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task WriteAsync(string path, byte[] bytes, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public Task WriteAsync(string path, IEnumerable<byte> bytes, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public async Task WriteAsync(string path, Stream source, CancellationToken cancellationToken) {
            try {
                var fullPath = PathHelper.Combine(targetDirectory, path);
                await LogService.LogAsync($"Start uploading blob \"{fullPath}\"", cancellationToken);
                var blob = container.GetBlockBlobReference(fullPath);
                await blob.UploadFromStreamAsync(source, accessCondition, options, operationContext, cancellationToken);
                await LogService.LogAsync($"End uploading blob \"{fullPath}\"", cancellationToken);
            }
            catch (Exception exception) {
                await ExceptionService.RegisterAsync(exception, cancellationToken);
                throw;
            }
        }

        public Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        #endregion
    }
}
