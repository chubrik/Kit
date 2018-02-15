using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utils.Abstractions;
using Utils.Helpers;

namespace Utils.Services {
    public class FileClient : IDataClient, ILogClient {

        private static ExceptionService ExceptionService => ExceptionService.Instance;
        private static LogService LogService => LogService.Instance;

        private static FileClient instance;
        public static FileClient Instance => instance ?? (instance = new FileClient());
        private FileClient() { }

        private string targetDirectory = "$work";
        private string logPath = "$work/$log.txt";

        public void Setup(string targetDirectory = null) {

            if (targetDirectory != null)
                this.targetDirectory = targetDirectory;
        }

        #region IDataClient

        #region Read

        public Task<string> ReadTextAsync(string path, CancellationToken cancellationToken) =>
            Task.FromResult(ReadText(path));

        public Task<List<string>> ReadLinesAsync(string path, CancellationToken cancellationToken) =>
            Task.FromResult(ReadLines(path));

        public Task<List<byte>> ReadBytesAsync(string path, CancellationToken cancellationToken) =>
            Task.FromResult(ReadBytes(path));

        public Task ReadAsync(string path, Stream target, CancellationToken cancellationToken) {
            ReadStream(path, target);
            return Task.CompletedTask;
        }

        public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken) =>
            Task.FromResult((Stream)OpenRead(path));

        #endregion

        #region Write

        public Task WriteAsync(string path, string text, CancellationToken cancellationToken) {
            Write(path, text);
            return Task.CompletedTask;
        }

        public Task WriteAsync(string path, string[] lines, CancellationToken cancellationToken) {
            Write(path, lines);
            return Task.CompletedTask;
        }

        public Task WriteAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken) {
            Write(path, lines);
            return Task.CompletedTask;
        }

        public Task WriteAsync(string path, byte[] bytes, CancellationToken cancellationToken) {
            Write(path, bytes);
            return Task.CompletedTask;
        }

        public Task WriteAsync(string path, IEnumerable<byte> bytes, CancellationToken cancellationToken) {
            Write(path, bytes);
            return Task.CompletedTask;
        }

        public Task WriteAsync(string path, Stream source, CancellationToken cancellationToken) {
            Write(path, source);
            return Task.CompletedTask;
        }

        public Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken) =>
            Task.FromResult((Stream)OpenWrite(path));

        #endregion
        
        #endregion

        #region ILogClient

        private bool wasIndent = false;

        public Task LogAsync(string message, CancellationToken cancellationToken, LogLevel level = LogLevel.Log) {
            File.AppendAllText(logPath, $"{message}\r\n");
            wasIndent = false;
            return Task.CompletedTask;
        }

        public Task LogInfoAsync(string message, CancellationToken cancellationToken) {
            File.AppendAllText(logPath, $"{GetIndent()}--- INFO ---\r\n{message}\r\n\r\n");
            return Task.CompletedTask;
        }

        public Task LogWarningAsync(string message, CancellationToken cancellationToken) {
            File.AppendAllText(logPath, $"{GetIndent()}--- WARNING ---\r\n{message}\r\n\r\n");
            return Task.CompletedTask;
        }

        public Task LogErrorAsync(string message, CancellationToken cancellationToken) {
            File.AppendAllText(logPath, $"{GetIndent()}--- ERROR ---\r\n{message}\r\n\r\n");
            return Task.CompletedTask;
        }

        private string GetIndent() {
            if (wasIndent)
                return string.Empty;
            else {
                wasIndent = true;
                return "\r\n";
            }
        }

        #endregion

        private static void CreateDir(string filePath) {
            var dirPath = PathHelper.GetParent(filePath);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }

        #region Obsolete

        [Obsolete]
        private string ReadText(string path) =>
            ReadBase(path, fullPath => File.ReadAllText(fullPath));

        [Obsolete]
        private List<string> ReadLines(string path) =>
            ReadBase(path, fullPath => File.ReadAllLines(fullPath).ToList());

        [Obsolete]
        private List<byte> ReadBytes(string path) =>
            ReadBase(path, fullPath => File.ReadAllBytes(fullPath).ToList());

        [Obsolete]
        private T ReadBase<T>(string path, Func<string, T> readFunc) {
            try {
                var fullPath = PathHelper.CombineLocal(targetDirectory, path);
                //LogService.WriteLine($"Read file \"{fullPath}\"");
                return readFunc(fullPath);
            }
            catch (Exception exception) {
                //ExceptionService.Register(exception);
                throw;
            }
        }

        [Obsolete]
        private void ReadStream(string path, Stream target) {
            using (var fs = OpenRead(path))
                fs.CopyTo(target);
        }

        [Obsolete]
        private FileStream OpenRead(string path) {
            try {
                var fullPath = PathHelper.CombineLocal(targetDirectory, path);
                //LogService.WriteLine($"Read file \"{fullPath}\"");
                return File.OpenRead(fullPath);
            }
            catch (Exception exception) {
                //ExceptionService.Register(exception);
                throw;
            }
        }

        [Obsolete]
        private void Write(string path, string text) =>
            WriteBase(path, fullPath => File.WriteAllText(fullPath, text));

        [Obsolete]
        private void Write(string path, string[] lines) =>
            WriteBase(path, fullPath => File.WriteAllLines(fullPath, lines));

        [Obsolete]
        private void Write(string path, IEnumerable<string> lines) =>
            WriteBase(path, fullPath => File.WriteAllLines(fullPath, lines.ToArray()));

        [Obsolete]
        private void Write(string path, byte[] bytes) =>
            WriteBase(path, fullPath => File.WriteAllBytes(fullPath, bytes));

        [Obsolete]
        private void Write(string path, IEnumerable<byte> bytes) =>
            WriteBase(path, fullPath => File.WriteAllBytes(fullPath, bytes.ToArray()));

        [Obsolete]
        private void Write(string path, Stream source) {
            using (var fs = File.OpenWrite(path))
                source.CopyTo(fs);
        }

        [Obsolete]
        private void WriteBase(string path, Action<string> writeAction) {
            try {
                var fullPath = PathHelper.CombineLocal(targetDirectory, path);
                //LogService.WriteLine($"Write file \"{fullPath}\"");
                CreateDir(fullPath);
                writeAction(fullPath);
            }
            catch (Exception exception) {
                //ExceptionService.Register(exception);
                throw;
            }
        }

        [Obsolete]
        private FileStream OpenWrite(string path) {
            try {
                var fullPath = PathHelper.CombineLocal(targetDirectory, path);
                //LogService.WriteLine($"Write file \"{fullPath}\"");
                CreateDir(fullPath);
                return File.OpenWrite(fullPath);
            }
            catch (Exception exception) {
                //ExceptionService.Register(exception);
                throw;
            }
        }

        #endregion
    }
}
