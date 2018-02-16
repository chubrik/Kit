﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Azure {
    public class AzureBlobClient : IDataClient {

        private static AzureBlobClient instance;
        public static AzureBlobClient Instance => instance ?? (instance = new AzureBlobClient());
        private AzureBlobClient() { }

        private static readonly AccessCondition accessCondition = new AccessCondition();
        private static readonly BlobRequestOptions options = new BlobRequestOptions();
        private static readonly OperationContext operationContext = new OperationContext();

        private static CloudBlobContainer container;
        private static string targetDirectory = string.Empty;

        public static void Setup(
            string accountName = null,
            string accountKey = null,
            string containerName = null,
            string targetDirectory = null) {

            var account = CloudStorageAccount.Parse(
                $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey}");

            var client = account.CreateCloudBlobClient();
            container = client.GetContainerReference(containerName);

            if (targetDirectory != null)
                AzureBlobClient.targetDirectory = targetDirectory;
        }

        #region IDataClient

        public void PushToWrite(string path, string text, string targetDirectory = null) =>
            WriteAsync(path, text, CancellationToken.None, targetDirectory);

        #endregion

        #region Read

        public static Task<string> ReadTextAsync(string path, CancellationToken cancellationToken) =>
            ReadBaseAsync(path, cancellationToken, (fullPath, blob) =>
                blob.DownloadTextAsync(Encoding.UTF8, accessCondition, options, operationContext, cancellationToken));

        public static Task<List<string>> ReadLinesAsync(string path, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public static Task<List<byte>> ReadBytesAsync(string path, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public static async Task ReadAsync(string path, Stream target, CancellationToken cancellationToken) {
            try {
                var fullPath = PathHelper.Combine(targetDirectory, path);
                LogService.Log($"Start downloading blob \"{fullPath}\"");
                var blob = container.GetBlockBlobReference(fullPath);
                await blob.DownloadToStreamAsync(target, accessCondition, options, operationContext, cancellationToken);
                LogService.Log($"End downloading blob \"{fullPath}\"");
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
                throw;
            }
        }

        public static Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        private static async Task<T> ReadBaseAsync<T>(
            string path, CancellationToken cancellationToken, Func<string, CloudBlockBlob, Task<T>> action) {

            try {
                var fullPath = PathHelper.Combine(targetDirectory, path);
                LogService.Log($"Start downloading blob \"{fullPath}\"");
                var blob = container.GetBlockBlobReference(fullPath);
                var result = await action(fullPath, blob);
                LogService.Log($"End downloading blob \"{fullPath}\"");
                return result;
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
                throw;
            }
        }

        #endregion

        #region Write

        public static Task WriteAsync(
            string path, string text, CancellationToken cancellationToken, string targetDirectory = null) =>
            throw new NotImplementedException();

        public static Task WriteAsync(string path, string[] lines, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public static Task WriteAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public static Task WriteAsync(string path, byte[] bytes, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public static Task WriteAsync(string path, IEnumerable<byte> bytes, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
        
        public static async Task WriteAsync(string path, Stream source, CancellationToken cancellationToken) {
            try {
                var fullPath = PathHelper.Combine(targetDirectory, path);
                LogService.Log($"Start uploading blob \"{fullPath}\"");
                var blob = container.GetBlockBlobReference(fullPath);
                await blob.UploadFromStreamAsync(source, accessCondition, options, operationContext, cancellationToken);
                LogService.Log($"End uploading blob \"{fullPath}\"");
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
                throw;
            }
        }

        public static Stream OpenWrite(string path) =>
            throw new NotImplementedException();

        public static Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        #endregion
    }
}
