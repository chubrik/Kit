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

        public void PushToWrite(string path, string text) =>
            Task.Run(() => WriteTextAsync(path, text)).Wait(); //todo queue

        #endregion

        #region IReportClient

        private static int _reportCounter = 0;

        public void PushToReport(string subject, string body, IEnumerable<string> attachmentPaths, string targetDirectory)
        {
            if (attachmentPaths == null)
                throw new ArgumentNullException(nameof(attachmentPaths));

            _reportCounter++;
            var paddedCount = _reportCounter.ToString().PadLeft(3, '0');
            var filePath = PathHelper.Combine(targetDirectory, PathHelper.SafeFileName($"{paddedCount} {subject}.txt"));
            Task.Run(() => WriteTextAsync(filePath, $"{subject}\r\n\r\n{body}\r\n")).Wait(); //todo queue

            var attachmentCounter = 0;

            foreach (var attachmentPath in attachmentPaths)
                using (var readStream = FileClient.OpenRead(attachmentPath))
                    Task.Run(() =>
                        WriteFromAsync($"{paddedCount}-{++attachmentCounter} {PathHelper.FileName(attachmentPath)}", readStream)
                    ).Wait(); //todo queue
        }

        #endregion

        #region Read

        #region Extensions

        public static string ReadText(string path) =>
            Task.Run(() => ReadTextAsync(path, Kit.CancellationToken)).Result;

        public static Task<string> ReadTextAsync(string path) =>
            ReadTextAsync(path, Kit.CancellationToken);

        public static List<string> ReadLines(string path) =>
            Task.Run(() => ReadLinesAsync(path, Kit.CancellationToken)).Result;

        public static Task<List<string>> ReadLinesAsync(string path) =>
            ReadLinesAsync(path, Kit.CancellationToken);

        public static async Task<List<string>> ReadLinesAsync(string path, CancellationToken cancellationToken) =>
            (await ReadTextAsync(path, cancellationToken)).SplitLines();
#if DEBUG
        public static byte[] ReadBytes(string path) =>
            Task.Run(() => ReadBytesAsync(path, Kit.CancellationToken)).Result;

        public static Task<byte[]> ReadBytesAsync(string path) =>
            ReadBytesAsync(path, Kit.CancellationToken);
#endif
        public static void ReadTo(string path, Stream target) =>
            Task.Run(() => ReadToAsync(path, target, Kit.CancellationToken)).Wait();

        public static Task ReadToAsync(string path, Stream target) =>
            ReadToAsync(path, target, Kit.CancellationToken);

        public static Stream OpenRead(string path) =>
            Task.Run(() => OpenReadAsync(path, Kit.CancellationToken)).Result;

        public static Task<Stream> OpenReadAsync(string path) =>
            OpenReadAsync(path, Kit.CancellationToken);

        #endregion

        public static Task<string> ReadTextAsync(string path, CancellationToken cancellationToken) =>
            ReadBaseAsync(path, blob => blob.DownloadTextAsync(
                Encoding.UTF8, _accessCondition, _options, _operationContext, cancellationToken));
#if DEBUG
        public static Task<byte[]> ReadBytesAsync(string path, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
#endif
        public static async Task ReadToAsync(string path, Stream target, CancellationToken cancellationToken)
        {
            var startTime = DateTimeOffset.Now;
            var nativePath = PathHelper.Combine(_workingDirectory, path);
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

        public static async Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken)
        {
            try
            {
                var nativePath = PathHelper.Combine(_workingDirectory, path);
                var blob = _container.GetBlockBlobReference(nativePath);
                return await blob.OpenReadAsync(_accessCondition, _options, _operationContext, cancellationToken);
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsAllowed());
                throw;
            }
        }

        private static async Task<T> ReadBaseAsync<T>(string path, Func<CloudBlockBlob, Task<T>> action)
        {
            var startTime = DateTimeOffset.Now;
            var nativePath = PathHelper.Combine(_workingDirectory, path);
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

        public static void WriteText(string path, string text) =>
            Task.Run(() => WriteTextAsync(path, text, Kit.CancellationToken)).Wait();

        public static Task WriteTextAsync(string path, string text) =>
            WriteTextAsync(path, text, Kit.CancellationToken);

        public static void WriteLines(string path, IEnumerable<string> lines) =>
            Task.Run(() => WriteTextAsync(path, lines.JoinLines(), Kit.CancellationToken)).Wait();

        public static Task WriteLinesAsync(string path, IEnumerable<string> lines) =>
            WriteTextAsync(path, lines.JoinLines(), Kit.CancellationToken);

        public static Task WriteLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken) =>
            WriteTextAsync(path, lines.JoinLines(), cancellationToken);
#if DEBUG
        public static void WriteBytes(string path, byte[] bytes) =>
            Task.Run(() => WriteBytesAsync(path, bytes, Kit.CancellationToken)).Wait();

        public static Task WriteBytesAsync(string path, byte[] bytes) =>
            WriteBytesAsync(path, bytes, Kit.CancellationToken);
#endif
        public static void WriteFrom(string path, Stream source) =>
            Task.Run(() => WriteFromAsync(path, source, Kit.CancellationToken)).Wait();

        public static Task WriteFromAsync(string path, Stream source) =>
            WriteFromAsync(path, source, Kit.CancellationToken);

        public static Stream OpenWrite(string path) =>
            Task.Run(() => OpenWriteAsync(path, Kit.CancellationToken)).Result;

        public static Task<Stream> OpenWriteAsync(string path) =>
            OpenWriteAsync(path, Kit.CancellationToken);

        #endregion

        public static Task WriteTextAsync(string path, string text, CancellationToken cancellationToken) =>
            WriteBaseAsync(path, blob => blob.UploadTextAsync(
                text, Encoding.UTF8, _accessCondition, _options, _operationContext, cancellationToken));
#if DEBUG
        public static Task WriteBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
#endif
        public static Task WriteFromAsync(string path, Stream source, CancellationToken cancellationToken) =>
            WriteBaseAsync(path, blob => blob.UploadFromStreamAsync(
                source, _accessCondition, _options, _operationContext, cancellationToken));

        public static async Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken)
        {
            try
            {
                var nativePath = PathHelper.Combine(_workingDirectory, path);
                var blob = GetBlobToWrite(nativePath);
                return await blob.OpenWriteAsync(_accessCondition, _options, _operationContext, cancellationToken);
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsAllowed());
                throw;
            }
        }

        private static async Task WriteBaseAsync(string path, Func<CloudBlockBlob, Task> action)
        {
            var startTime = DateTimeOffset.Now;
            var nativePath = PathHelper.Combine(_workingDirectory, path);
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
