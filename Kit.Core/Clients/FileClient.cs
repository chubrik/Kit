using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Kit {
    public class FileClient : IDataClient, IReportClient, ILogClient {

        private static FileClient instance;
        public static FileClient Instance => instance ?? (instance = new FileClient());
        private FileClient() { }

        private static string baseDirectory = "$work";
        private static string workingDirectory = string.Empty;

        public static void Setup(string baseDirectory = null, string workingDirectory = null) {

            if (baseDirectory != null)
                FileClient.baseDirectory = baseDirectory;

            if (workingDirectory != null)
                FileClient.workingDirectory = workingDirectory;
        }

        #region IDataClient

        public void PushToWrite(string path, string text, string targetDirectory = null) =>
            WriteBase(path, fullPath => File.WriteAllText(fullPath, text), targetDirectory);

        #endregion

        #region IReportClient

        private static int reportCounter = 0;

        public void PushToReport(string subject, string body, string targetDirectory) {
            var count = (++reportCounter).ToString().PadLeft(3, '0');
            var fileName = PathHelper.SafeFileName($"{count} {subject}.txt");
            Write(fileName, $"{subject}\r\n\r\n{body}\r\n", targetDirectory);
        }

        #endregion

        #region ILogClient

        private bool logIndent = false;

        public void PushToLog(string message, LogLevel level = LogLevel.Log, string targetDirectory = null) {
            var fullMessage = $"{DateTimeOffset.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")} - {message}";
            var filePath = GetFullPath(LogService.LogFileName, targetDirectory);
            CreateDir(filePath);

            if (level == LogLevel.Log) {
                File.AppendAllText(filePath, logIndent ? $"\r\n{fullMessage}\r\n" : $"{fullMessage}\r\n");
                logIndent = false;
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

            File.AppendAllText(filePath, $"\r\n--- {header} ---\r\n{fullMessage}\r\n");
            logIndent = true;
        }

        #endregion

        #region Read

        public static string ReadText(string path, string targetDirectory = null) =>
            ReadBase(path, fullPath => File.ReadAllText(fullPath), targetDirectory);

        public static List<string> ReadLines(string path, string targetDirectory = null) =>
            ReadBase(path, fullPath => File.ReadAllLines(fullPath).ToList(), targetDirectory);

        public static List<byte> ReadBytes(string path) =>
            ReadBase(path, fullPath => File.ReadAllBytes(fullPath).ToList());

        public static void Read(string path, Stream target) {
            using (var fs = OpenRead(path))
                fs.CopyTo(target);
        }

        public static FileStream OpenRead(string path) {
            try {
                var fullPath = GetFullPath(path);
                LogService.Log($"Read file \"{fullPath}\"");
                return File.OpenRead(fullPath);
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
                throw;
            }
        }

        private static T ReadBase<T>(
            string path, Func<string, T> readFunc, string targetDirectory = null) {

            try {
                var fullPath = GetFullPath(path, targetDirectory);
                LogService.Log($"Read file \"{fullPath}\"");
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

        public static void Write(string path, string[] lines) =>
            WriteBase(path, fullPath => File.WriteAllLines(fullPath, lines));

        public static void Write(string path, IEnumerable<string> lines) =>
            WriteBase(path, fullPath => File.WriteAllLines(fullPath, lines.ToArray()));

        public static void Write(string path, byte[] bytes) =>
            WriteBase(path, fullPath => File.WriteAllBytes(fullPath, bytes));

        public static void Write(string path, IEnumerable<byte> bytes) =>
            WriteBase(path, fullPath => File.WriteAllBytes(fullPath, bytes.ToArray()));

        public static void Write(string path, Stream source) {
            using (var fs = File.OpenWrite(path))
                source.CopyTo(fs);
        }

        public static FileStream OpenWrite(string path) {
            try {
                var fullPath = GetFullPath(path);
                LogService.Log($"Write file \"{fullPath}\"");
                CreateDir(fullPath);
                return File.OpenWrite(fullPath);
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
                throw;
            }
        }

        private static void WriteBase(string path, Action<string> writeAction, string targetDirectory = null) {
            try {
                var fullPath = GetFullPath(path, targetDirectory);
                LogService.Log($"Write file \"{fullPath}\"");
                CreateDir(fullPath);
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
            File.Exists(GetFullPath(path, targetDirectory));

        public static void AppendText(string path, string text, string targetDirectory = null) {
            var fullPath = GetFullPath(path, targetDirectory);
            CreateDir(fullPath);
            File.AppendAllText(fullPath, $"{text}\r\n");
        }

        private static void CreateDir(string filePath) {
            var dirPath = PathHelper.Parent(filePath);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }

        private static string GetFullPath(string path, string targetDirectory = null) =>
            PathHelper.Combine(baseDirectory, targetDirectory ?? workingDirectory, path);
    }
}
