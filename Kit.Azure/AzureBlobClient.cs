using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kit.Azure {
    public class AzureBlobClient : IDataClient, IReportClient {

        private static AzureBlobClient _instance;
        public static AzureBlobClient Instance => _instance ?? (_instance = new AzureBlobClient());
        private AzureBlobClient() { }

        private static readonly AccessCondition _accessCondition = new AccessCondition();
        private static readonly BlobRequestOptions _options = new BlobRequestOptions();
        private static readonly OperationContext _operationContext = new OperationContext();
        private static CloudBlobContainer _container;
        private static string _workingDirectory = string.Empty;
        private static int _logCounter = 0;

        #region Setup

        public static void Setup(
            string accountName = null,
            string accountKey = null,
            string containerName = null,
            string workingDirectory = null) {

            var account = CloudStorageAccount.Parse(
                $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey}");

            var client = account.CreateCloudBlobClient();
            _container = client.GetContainerReference(containerName);

            if (workingDirectory != null)
                _workingDirectory = workingDirectory;

            ExceptionHandler.DataClients.Add(Instance);
            ReportService.Clients.Add(Instance);
        }

        #endregion

        #region IDataClient

        public void PushToWrite(string path, string text, string targetDirectory = null) =>
            WriteAsync(path, text, targetDirectory: targetDirectory).Wait(); //todo queue

        #endregion

        #region IReportClient

        private static int _reportCounter = 0;

        public void PushToReport(string subject, string body, IEnumerable<string> attachmentPaths, string targetDirectory) {
            Debug.Assert(attachmentPaths != null);

            if (attachmentPaths == null)
                throw new InvalidOperationException();

            _reportCounter++;
            var paddedCount = _reportCounter.ToString().PadLeft(3, '0');
            var fileName = PathHelper.SafeFileName($"{paddedCount} {subject}.txt");
            WriteAsync(fileName, $"{subject}\r\n\r\n{body}\r\n", targetDirectory: targetDirectory).Wait(); //todo queue
            var attachmentCounter = 0;

            foreach (var attachmentPath in attachmentPaths)
                using (var stream = FileClient.OpenRead(attachmentPath))
                    WriteAsync($"{paddedCount}-{++attachmentCounter} {PathHelper.FileName(attachmentPath)}",
                        stream, targetDirectory: targetDirectory).Wait(); //todo queue
        }

        #endregion

        #region Read

        #region Extensions

        public static string ReadText(string path, string targetDirectory = null) =>
            ReadTextAsync(path, targetDirectory).Result;

        public static List<string> ReadLines(string path, string targetDirectory = null) =>
            ReadLinesAsync(path, targetDirectory).Result;

        public static byte[] ReadBytes(string path, string targetDirectory = null) =>
            ReadBytesAsync(path, targetDirectory).Result;

        public static void Read(string path, Stream target, string targetDirectory = null) =>
            ReadAsync(path, target, targetDirectory).Wait();

        #endregion

        public static Task<string> ReadTextAsync(string path, string targetDirectory = null) =>
            ReadBaseAsync(path, (fullPath, blob) =>
                blob.DownloadTextAsync(Encoding.UTF8, _accessCondition, _options, _operationContext, Kit.CancellationToken),
                targetDirectory: targetDirectory);

        public static Task<List<string>> ReadLinesAsync(string path, string targetDirectory = null) =>
            throw new NotImplementedException();

        public static Task<byte[]> ReadBytesAsync(string path, string targetDirectory = null) =>
            throw new NotImplementedException();

        public static async Task ReadAsync(string path, Stream target, string targetDirectory = null) {
            var startTime = DateTimeOffset.Now;
            var fullPath = PathHelper.Combine(targetDirectory ?? _workingDirectory, path);
            var logLabel = $"Download blob #{++_logCounter}";
            LogService.Log($"{logLabel}: {fullPath}");

            try {
                var blob = _container.GetBlockBlobReference(fullPath);
                await blob.DownloadToStreamAsync(target, _accessCondition, _options, _operationContext, Kit.CancellationToken);
                LogService.Log($"{logLabel} completed at {TimeHelper.FormattedLatency(startTime)}");
            }
            catch (Exception exception) {
                if (!exception.IsCanceled()) Debug.Fail(exception.ToString());
                LogService.Log($"{logLabel} failed at {TimeHelper.FormattedLatency(startTime)}");
                ExceptionHandler.Register(exception);
                throw;
            }
        }

        public static Stream OpenRead(string path, string targetDirectory = null) => throw new NotImplementedException();

        private static async Task<T> ReadBaseAsync<T>(
            string path, Func<string, CloudBlockBlob, Task<T>> action, string targetDirectory) {

            var startTime = DateTimeOffset.Now;
            var fullPath = PathHelper.Combine(targetDirectory ?? _workingDirectory, path);
            var logLabel = $"Download blob #{++_logCounter}";
            LogService.Log($"{logLabel}: {fullPath}");

            try {
                var blob = _container.GetBlockBlobReference(fullPath);
                var result = await action(fullPath, blob);
                LogService.Log($"{logLabel} completed at {TimeHelper.FormattedLatency(startTime)}");
                return result;
            }
            catch (Exception exception) {
                if (!exception.IsCanceled()) Debug.Fail(exception.ToString());
                LogService.Log($"{logLabel} failed at {TimeHelper.FormattedLatency(startTime)}");
                ExceptionHandler.Register(exception);
                throw;
            }
        }

        #endregion

        #region Write

        #region Extensions

        public static void Write(string path, string text, string targetDirectory = null) =>
            WriteAsync(path, text, targetDirectory: targetDirectory).Wait();

        public static void Write(string path, IEnumerable<string> lines, string targetDirectory = null) =>
            WriteAsync(path, lines, targetDirectory: targetDirectory).Wait();

        public static void Write(string path, byte[] bytes, string targetDirectory = null) =>
            WriteAsync(path, bytes, targetDirectory: targetDirectory).Wait();

        public static void Write(string path, Stream source, string targetDirectory = null) =>
            WriteAsync(path, source, targetDirectory: targetDirectory).Wait();

        #endregion

        public static Task WriteAsync(string path, string text, string targetDirectory = null) {
            return WriteBaseAsync(path, blob =>
                blob.UploadTextAsync(text, Encoding.UTF8, _accessCondition, _options, _operationContext, Kit.CancellationToken),
                targetDirectory: targetDirectory);
        }

        public static Task WriteAsync(string path, IEnumerable<string> lines, string targetDirectory = null) =>
            throw new NotImplementedException();

        public static Task WriteAsync(string path, byte[] bytes, string targetDirectory = null) =>
            throw new NotImplementedException();

        public static async Task WriteAsync(string path, Stream source, string targetDirectory = null) {
            var startTime = DateTimeOffset.Now;
            var fullPath = PathHelper.Combine(targetDirectory ?? _workingDirectory, path);
            var logLabel = $"Upload blob #{++_logCounter}";
            LogService.Log($"{logLabel}: {fullPath}");

            try {
                var blob = _container.GetBlockBlobReference(fullPath);
                await blob.UploadFromStreamAsync(source, _accessCondition, _options, _operationContext, Kit.CancellationToken);
                LogService.Log($"{logLabel} completed at {TimeHelper.FormattedLatency(startTime)}");
            }
            catch (Exception exception) {
                if (!exception.IsCanceled()) Debug.Fail(exception.ToString());
                LogService.Log($"{logLabel} failed at {TimeHelper.FormattedLatency(startTime)}");
                ExceptionHandler.Register(exception);
                throw;
            }
        }

        public static Stream OpenWrite(string path, string targetDirectory = null) => throw new NotImplementedException();

        private static async Task WriteBaseAsync(
            string path, Func<CloudBlockBlob, Task> action, string targetDirectory) {

            var startTime = DateTimeOffset.Now;
            var fullPath = PathHelper.Combine(targetDirectory ?? _workingDirectory, path);
            var logLabel = $"Upload blob #{++_logCounter}";
            LogService.Log($"{logLabel}: {fullPath}");

            try {
                var blob = _container.GetBlockBlobReference(fullPath);
                await action(blob);
                LogService.Log($"{logLabel} completed at {TimeHelper.FormattedLatency(startTime)}");
            }
            catch (Exception exception) {
                if (!exception.IsCanceled()) Debug.Fail(exception.ToString());
                LogService.Log($"{logLabel} failed at {TimeHelper.FormattedLatency(startTime)}");
                ExceptionHandler.Register(exception);
                throw;
            }
        }

        #endregion
    }
}
