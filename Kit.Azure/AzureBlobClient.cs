using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Azure
{
    public class AzureBlobClient : IDataClient, IReportClient
    {
        private static AzureBlobClient? _instance;
        public static AzureBlobClient Instance => _instance ??= new AzureBlobClient();
        private AzureBlobClient() { }

        private static readonly AccessCondition _accessCondition = new AccessCondition();
        private static readonly BlobRequestOptions _options = new BlobRequestOptions();
        private static readonly OperationContext _operationContext = new OperationContext();
        private static CloudBlobContainer _container;
        private static string _workingDirectory = string.Empty;
        private static int _logCounter = 0;

        #region Setup

        public static void Setup(
            string? accountName = null,
            string? accountKey = null,
            string? containerName = null,
            string? workingDirectory = null)
        {
            var account = CloudStorageAccount.Parse(
                $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey}");

            var client = account.CreateCloudBlobClient();
            _container = client.GetContainerReference(containerName);

            if (workingDirectory != null)
                _workingDirectory = workingDirectory;

            ExceptionHandler.Clients.Add(Instance);
            ReportService.Clients.Add(Instance);
        }

        #endregion

        #region IDataClient

        public void PushToWrite(string path, string text, string? targetDirectory = null) =>
            Task.Run(() => WriteAsync(path, text, targetDirectory: targetDirectory)).Wait(); //todo queue

        #endregion

        #region IReportClient

        private static int _reportCounter = 0;

        public void PushToReport(string subject, string body, IEnumerable<string> attachmentPaths, string targetDirectory)
        {
            Debug.Assert(attachmentPaths != null);

            if (attachmentPaths == null)
                throw new ArgumentNullException(nameof(attachmentPaths));

            _reportCounter++;
            var paddedCount = _reportCounter.ToString().PadLeft(3, '0');
            var fileName = PathHelper.SafeFileName($"{paddedCount} {subject}.txt");

            Task.Run(() => WriteAsync(
                fileName, $"{subject}\r\n\r\n{body}\r\n", targetDirectory: targetDirectory)).Wait(); //todo queue

            var attachmentCounter = 0;

            foreach (var attachmentPath in attachmentPaths)
                using (var stream = FileClient.OpenRead(attachmentPath))
                    Task.Run(() => WriteAsync(
                        $"{paddedCount}-{++attachmentCounter} {PathHelper.FileName(attachmentPath)}",
                        stream, targetDirectory: targetDirectory)).Wait(); //todo queue
        }

        #endregion

        #region Read

        #region Extensions

        public static string ReadText(string path, string? targetDirectory = null) =>
            Task.Run(() => ReadTextAsync(
                path, Kit.CancellationToken, targetDirectory: targetDirectory)).Result;

        public static Task<string> ReadTextAsync(string path, string? targetDirectory = null) =>
            ReadTextAsync(path, Kit.CancellationToken, targetDirectory: targetDirectory);

        public static List<string> ReadLines(string path, string? targetDirectory = null) =>
            Task.Run(() => ReadLinesAsync(
                path, Kit.CancellationToken, targetDirectory: targetDirectory)).Result;

        public static Task<List<string>> ReadLinesAsync(string path, string? targetDirectory = null) =>
            ReadLinesAsync(path, Kit.CancellationToken, targetDirectory: targetDirectory);

        public static async Task<List<string>> ReadLinesAsync(
            string path, CancellationToken cancellationToken, string? targetDirectory = null) =>
            (await ReadTextAsync(path, cancellationToken, targetDirectory: targetDirectory)).SplitLines();

        public static byte[] ReadBytes(string path, string? targetDirectory = null) =>
            Task.Run(() => ReadBytesAsync(
                path, Kit.CancellationToken, targetDirectory: targetDirectory)).Result;

        public static Task<byte[]> ReadBytesAsync(string path, string? targetDirectory = null) =>
            ReadBytesAsync(path, Kit.CancellationToken, targetDirectory: targetDirectory);

        public static void ReadTo(string path, Stream target, string? targetDirectory = null) =>
            Task.Run(() => ReadToAsync(
                path, target, Kit.CancellationToken, targetDirectory: targetDirectory)).Wait();

        public static Task ReadToAsync(string path, Stream target, string? targetDirectory = null) =>
            ReadToAsync(path, target, Kit.CancellationToken, targetDirectory: targetDirectory);

        public static Stream OpenRead(string path, string? targetDirectory = null) =>
            Task.Run(() => OpenReadAsync(
                path, Kit.CancellationToken, targetDirectory: targetDirectory)).Result;

        public static Task<Stream> OpenReadAsync(string path, string? targetDirectory = null) =>
            OpenReadAsync(path, Kit.CancellationToken, targetDirectory: targetDirectory);

        #endregion

        public static Task<string> ReadTextAsync(
            string path, CancellationToken cancellationToken, string? targetDirectory = null) =>
            ReadBaseAsync(path, targetDirectory, blob =>
                blob.DownloadTextAsync(
                    Encoding.UTF8, _accessCondition, _options, _operationContext, cancellationToken));

        public static Task<byte[]> ReadBytesAsync(
            string path, CancellationToken cancellationToken, string? targetDirectory = null) =>
            throw new NotImplementedException();

        public static async Task ReadToAsync(
            string path, Stream target, CancellationToken cancellationToken, string? targetDirectory = null)
        {
            var startTime = DateTimeOffset.Now;
            var nativePath = PathHelper.Combine(targetDirectory ?? _workingDirectory, path);
            var logLabel = $"Download blob #{++_logCounter}";
            LogService.Log($"{logLabel}: {nativePath}");

            try
            {
                var blob = _container.GetBlockBlobReference(nativePath);

                await blob.DownloadToStreamAsync(
                    target, _accessCondition, _options, _operationContext, cancellationToken);

                LogService.Log($"{logLabel} completed at {TimeHelper.FormattedLatency(startTime)}");
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsAllowed());
                throw;
            }
        }

        public static async Task<Stream> OpenReadAsync(
            string path, CancellationToken cancellationToken, string? targetDirectory = null)
        {
            try
            {
                var nativePath = PathHelper.Combine(targetDirectory ?? _workingDirectory, path);
                var blob = _container.GetBlockBlobReference(nativePath);
                return await blob.OpenReadAsync(_accessCondition, _options, _operationContext, cancellationToken);
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsAllowed());
                throw;
            }
        }

        private static async Task<T> ReadBaseAsync<T>(
            string path, string? targetDirectory, Func<CloudBlockBlob, Task<T>> action)
        {
            var startTime = DateTimeOffset.Now;
            var nativePath = PathHelper.Combine(targetDirectory ?? _workingDirectory, path);
            var logLabel = $"Download blob #{++_logCounter}";
            LogService.Log($"{logLabel}: {nativePath}");

            try
            {
                var blob = _container.GetBlockBlobReference(nativePath);
                var result = await action(blob);
                LogService.Log($"{logLabel} completed at {TimeHelper.FormattedLatency(startTime)}");
                return result;
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsAllowed());
                throw;
            }
        }

        #endregion

        #region Write

        #region Extensions

        public static void Write(string path, string text, string? targetDirectory = null) =>
            Task.Run(() => WriteAsync(
                path, text, Kit.CancellationToken, targetDirectory: targetDirectory)).Wait();

        public static Task WriteAsync(string path, string text, string? targetDirectory = null) =>
            WriteAsync(path, text, Kit.CancellationToken, targetDirectory: targetDirectory);

        public static void Write(
            string path, IEnumerable<string> lines, string? targetDirectory = null) =>
            Task.Run(() => WriteAsync(
                path, lines.JoinLines(), Kit.CancellationToken, targetDirectory: targetDirectory)).Wait();

        public static Task WriteAsync(
            string path, IEnumerable<string> lines, string? targetDirectory = null) =>
            WriteAsync(path, lines.JoinLines(), Kit.CancellationToken, targetDirectory: targetDirectory);

        public static Task WriteAsync(
            string path, IEnumerable<string> lines, CancellationToken cancellationToken, string? targetDirectory = null) =>
            WriteAsync(path, lines.JoinLines(), cancellationToken, targetDirectory: targetDirectory);

        public static void Write(string path, byte[] bytes, string? targetDirectory = null) =>
            Task.Run(() => WriteAsync(
                path, bytes, Kit.CancellationToken, targetDirectory: targetDirectory)).Wait();

        public static Task WriteAsync(string path, byte[] bytes, string? targetDirectory = null) =>
            WriteAsync(path, bytes, Kit.CancellationToken, targetDirectory: targetDirectory);

        public static void Write(string path, Stream source, string? targetDirectory = null) =>
            Task.Run(() => WriteAsync(
                path, source, Kit.CancellationToken, targetDirectory: targetDirectory)).Wait();

        public static Task WriteAsync(string path, Stream source, string? targetDirectory = null) =>
            WriteAsync(path, source, Kit.CancellationToken, targetDirectory: targetDirectory);

        public static Stream OpenWrite(string path, string? targetDirectory = null) =>
            Task.Run(() => OpenWriteAsync(
                path, Kit.CancellationToken, targetDirectory: targetDirectory)).Result;

        public static Task<Stream> OpenWriteAsync(string path, string? targetDirectory = null) =>
            OpenWriteAsync(path, Kit.CancellationToken, targetDirectory: targetDirectory);

        #endregion

        public static Task WriteAsync(
            string path, string text, CancellationToken cancellationToken, string? targetDirectory = null) =>
            WriteBaseAsync(path, targetDirectory, blob =>
                blob.UploadTextAsync(
                    text, Encoding.UTF8, _accessCondition, _options, _operationContext, cancellationToken));

        public static Task WriteAsync(
            string path, byte[] bytes, CancellationToken cancellationToken, string? targetDirectory = null) =>
            throw new NotImplementedException();

        public static Task WriteAsync(
            string path, Stream source, CancellationToken cancellationToken, string? targetDirectory = null) =>
            WriteBaseAsync(path, targetDirectory, blob =>
                blob.UploadFromStreamAsync(
                    source, _accessCondition, _options, _operationContext, cancellationToken));

        public static async Task<Stream> OpenWriteAsync(
            string path, CancellationToken cancellationToken, string? targetDirectory = null)
        {
            try
            {
                var nativePath = PathHelper.Combine(targetDirectory ?? _workingDirectory, path);
                var blob = GetBlobToWrite(nativePath);
                return await blob.OpenWriteAsync(_accessCondition, _options, _operationContext, cancellationToken);
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsAllowed());
                throw;
            }
        }

        private static async Task WriteBaseAsync(
            string path, string? targetDirectory, Func<CloudBlockBlob, Task> action)
        {
            var startTime = DateTimeOffset.Now;
            var nativePath = PathHelper.Combine(targetDirectory ?? _workingDirectory, path);
            var logLabel = $"Upload blob #{++_logCounter}";
            LogService.Log($"{logLabel}: {nativePath}");

            try
            {
                var blob = GetBlobToWrite(nativePath);
                await action(blob);
                LogService.Log($"{logLabel} completed at {TimeHelper.FormattedLatency(startTime)}");
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsAllowed());
                throw;
            }
        }

        #endregion

        #region Utils

        private static readonly Dictionary<string, string> MimeTypes = new Dictionary<string, string>
        {
            { "html", "text/html" },
            { "json", "application/json" },
            { "txt", "text/plain" },
            { "xml", "application/xml" },
        };

        private static CloudBlockBlob GetBlobToWrite(string nativePath)
        {
            var blob = _container.GetBlockBlobReference(nativePath);
            var extension = PathHelper.FileExtension(nativePath);

            if (extension != null && MimeTypes.ContainsKey(extension))
                blob.Properties.ContentType = MimeTypes[extension];

            return blob;
        }

        #endregion
    }
}
