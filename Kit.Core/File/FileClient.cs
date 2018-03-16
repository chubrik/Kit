using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Kit {
    public class FileClient : IDataClient, IReportClient, ILogClient {

        private static FileClient _instance;
        public static FileClient Instance => _instance ?? (_instance = new FileClient());
        private FileClient() { }

        #region IDataClient

        public void PushToWrite(string path, string text, string targetDirectory = null) =>
            WriteBase(path, fullPath => File.WriteAllText(fullPath, text), targetDirectory);

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
            Write(fileName, $"{subject}\r\n\r\n{body}\r\n", targetDirectory);
            var attachmentCounter = 0;

            foreach (var attachmentPath in attachmentPaths)
                using (var stream = OpenRead(attachmentPath))
                    Write($"{paddedCount}-{++attachmentCounter} {PathHelper.FileName(attachmentPath)}",
                        stream, targetDirectory: targetDirectory);
        }

        #endregion

        #region ILogClient

        private bool _isLogInitialized;
        private string _logFullPath;
        private bool _logIndent = false;

        public void PushToLog(string message, LogLevel level = LogLevel.Log) {

            if (!_isLogInitialized)
                LogInitialize();

            lock (this) {

                if (level == LogLevel.Log) {
                    File.AppendAllText(_logFullPath, _logIndent ? $"\r\n{MessageLine(message)}" : MessageLine(message));
                    _logIndent = false;
                    return;
                }

                string header;

                switch (level) {

                    case LogLevel.Info:
                        header = "INFO";
                        break;

                    case LogLevel.Success:
                        header = "SUCCESS";
                        break;

                    case LogLevel.Warning:
                        header = "WARNING";
                        break;

                    case LogLevel.Error:
                        header = "ERROR";
                        break;

                    default:
                        Debug.Fail(string.Empty);
                        throw new ArgumentOutOfRangeException(nameof(level));
                }

                File.AppendAllText(_logFullPath, $"\r\n--- {header} ---\r\n{MessageLine(message)}");
                _logIndent = true;
            }
        }

        private void LogInitialize() {
            Debug.Assert(!_isLogInitialized);

            if (_isLogInitialized)
                throw new InvalidOperationException();

            _isLogInitialized = true;
            _logFullPath = FullPath(LogService._logFileName, Kit.DiagnisticsCurrentDirectory);
            CreateDir(_logFullPath);
        }

        private static string MessageLine(string message) =>
            $"{DateTimeOffset.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")} - {message}\r\n";

        #endregion

        #region Read

        public static string ReadText(string path, string targetDirectory = null) =>
            ReadBase(path, fullPath => File.ReadAllText(fullPath), targetDirectory);

        public static List<string> ReadLines(string path, string targetDirectory = null) =>
            ReadBase(path, fullPath => File.ReadAllLines(fullPath).ToList(), targetDirectory);

        public static byte[] ReadBytes(string path, string targetDirectory = null) =>
            ReadBase(path, fullPath => File.ReadAllBytes(fullPath), targetDirectory);

        public static void Read(string path, Stream target) {
            using (var fs = OpenRead(path))
                fs.CopyTo(target);
        }

        public static FileStream OpenRead(string path) {
            try {
                var fullPath = FullPath(path);
                LogService.Log($"Read file: {fullPath}");
                return File.OpenRead(fullPath);
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
                throw;
            }
        }

        private static T ReadBase<T>(
            string path, Func<string, T> readFunc, string targetDirectory) {

            try {
                var fullPath = FullPath(path, targetDirectory);
                LogService.Log($"Read file: {fullPath}");
                return readFunc(fullPath);
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
                throw;
            }
        }

        #endregion

        #region Write

        public static void Write(string path, string text, string targetDirectory = null) =>
            WriteBase(path, fullPath => File.WriteAllText(fullPath, text), targetDirectory);

        public static void Write(string path, string[] lines, string targetDirectory = null) =>
            WriteBase(path, fullPath => File.WriteAllLines(fullPath, lines), targetDirectory);

        public static void Write(string path, IEnumerable<string> lines, string targetDirectory = null) =>
            WriteBase(path, fullPath => File.WriteAllLines(fullPath, lines.ToArray()), targetDirectory);

        public static void Write(string path, byte[] bytes, string targetDirectory = null) =>
            WriteBase(path, fullPath => File.WriteAllBytes(fullPath, bytes), targetDirectory);

        public static void Write(string path, Stream source, string targetDirectory = null) {
            using (var fs = OpenWrite(path, targetDirectory))
                source.CopyTo(fs);
        }

        public static FileStream OpenWrite(string path, string targetDirectory = null) {
            try {
                var fullPath = FullPath(path, targetDirectory);
                CreateDir(fullPath);
                LogService.Log($"Write file: {fullPath}");
                return File.OpenWrite(fullPath);
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
                throw;
            }
        }

        private static void WriteBase(string path, Action<string> writeAction, string targetDirectory) {
            try {
                var fullPath = FullPath(path, targetDirectory);
                CreateDir(fullPath);
                LogService.Log($"Write file: {fullPath}");
                writeAction(fullPath);
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
                throw;
            }
        }

        #endregion

        public static bool Exists(string path, string targetDirectory = null) =>
            File.Exists(FullPath(path, targetDirectory));

        public static void AppendText(string path, string text, string targetDirectory = null) {
            var fullPath = FullPath(path, targetDirectory);
            CreateDir(fullPath);
            LogService.Log($"Append file: {fullPath}");
            File.AppendAllText(fullPath, $"{text}\r\n");
        }

        public static void Delete(string path, string targetDirectory = null) {
            var fullPath = FullPath(path, targetDirectory);
            LogService.Log($"Delete file: {fullPath}");
            File.Delete(fullPath);
        }

        public static string FullPath(string path, string targetDirectory = null) =>
            PathHelper.Combine(Kit._baseDirectory, targetDirectory ?? Kit._workingDirectory, path);

        public static List<string> FileNames(string path = "") =>
            Directory.GetFiles(FullPath(path)).Select(PathHelper.FileName).ToList();

        private static void CreateDir(string fullPath) {
            var dirPath = PathHelper.Parent(fullPath);

            if (!Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
                LogService.Log($"Create directory: {dirPath}");
            }
        }
    }
}
