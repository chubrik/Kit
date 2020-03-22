using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Kit
{
    public class FileClient : IDataClient, ILogClient, IReportClient
    {
        private static FileClient _instance;
        public static FileClient Instance => _instance ?? (_instance = new FileClient());
        protected FileClient() { }

        #region IDataClient

        public void PushToWrite(string path, string text, string targetDirectory = null) =>
            WriteBase(path, nativePath => File.WriteAllText(nativePath, text), targetDirectory);

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
            Write(fileName, $"{subject}\r\n\r\n{body}\r\n", targetDirectory);
            var attachmentCounter = 0;

            foreach (var attachmentPath in attachmentPaths)
                using (var stream = OpenRead(attachmentPath))
                    Write($"{paddedCount}-{++attachmentCounter} {PathHelper.FileName(attachmentPath)}",
                        stream, targetDirectory: targetDirectory);
        }

        #endregion

        #region ILogClient

        private const string LogTimeFormat = "dd.MM.yyyy HH:mm:ss.fff";
        private bool _isLogInitialized;
        private string _logNativeFilePath;

        private static readonly Dictionary<LogLevel, string> _logBadges =
            new Dictionary<LogLevel, string>
            {
                { LogLevel.Log,     " log   " },
                { LogLevel.Info,    "[INFO] " },
                { LogLevel.Success, "[SUCC] " },
                { LogLevel.Warning, "[WARN] " },
                { LogLevel.Error,   "[ERROR]" },
            };

        public void PushToLog(string message, LogLevel level = LogLevel.Log)
        {
            if (!_isLogInitialized)
                LogInitialize();

            var dateTime = DateTimeOffset.Now.ToString(LogTimeFormat);
            File.AppendAllText(_logNativeFilePath, $"{dateTime}  {_logBadges[level]} {message}\r\n");
        }

        private void LogInitialize()
        {
            Debug.Assert(!_isLogInitialized);

            if (_isLogInitialized)
                throw new InvalidOperationException();

            _isLogInitialized = true;
            _logNativeFilePath = NativePath(LogService.LogFileName, Kit.DiagnisticsCurrentDirectory);
            CreateDir(_logNativeFilePath);
        }

        #endregion

        #region Read

        public static string ReadText(string path, string targetDirectory = null) =>
            ReadBase(path, nativePath => File.ReadAllText(nativePath), targetDirectory);

        public static List<string> ReadLines(string path, string targetDirectory = null) =>
            ReadBase(path, nativePath => File.ReadAllLines(nativePath).ToList(), targetDirectory);

        public static byte[] ReadBytes(string path, string targetDirectory = null) =>
            ReadBase(path, nativePath => File.ReadAllBytes(nativePath), targetDirectory);

        public static void ReadTo(string path, Stream target, string targetDirectory = null)
        {
            using (var fs = OpenRead(path, targetDirectory))
                fs.CopyTo(target);
        }

        public static FileStream OpenRead(string path, string targetDirectory = null) =>
            ReadBase(path, nativePath => File.OpenRead(nativePath), targetDirectory);

        private static T ReadBase<T>(
            string path, Func<string, T> readFunc, string targetDirectory)
        {
            try
            {
                var nativePath = NativePath(path, targetDirectory);
                LogService.Log($"Read file \"{LogPath(nativePath)}\"");
                return readFunc(nativePath);
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsAllowed());
                throw;
            }
        }

        #endregion

        #region Write

        public static void Write(string path, string text, string targetDirectory = null) =>
            WriteBase(path, nativePath => File.WriteAllText(nativePath, text), targetDirectory);

        public static void Write(string path, string[] lines, string targetDirectory = null) =>
            WriteBase(path, nativePath => File.WriteAllLines(nativePath, lines), targetDirectory);

        public static void Write(string path, IEnumerable<string> lines, string targetDirectory = null) =>
            WriteBase(path, nativePath => File.WriteAllLines(nativePath, lines.ToArray()), targetDirectory);

        public static void Write(string path, byte[] bytes, string targetDirectory = null) =>
            WriteBase(path, nativePath => File.WriteAllBytes(nativePath, bytes), targetDirectory);

        public static void Write(string path, Stream source, string targetDirectory = null)
        {
            using (var fs = OpenWrite(path, targetDirectory))
                source.CopyTo(fs);
        }

        public static FileStream OpenWrite(string path, string targetDirectory = null)
        {
            try
            {
                var nativePath = NativePath(path, targetDirectory);
                CreateDir(nativePath);
                LogService.Log($"Write file \"{LogPath(nativePath)}\"");
                return File.OpenWrite(nativePath);
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsAllowed());
                throw;
            }
        }

        private static void WriteBase(string path, Action<string> writeAction, string targetDirectory)
        {
            try
            {
                var nativePath = NativePath(path, targetDirectory);
                CreateDir(nativePath);
                LogService.Log($"Write file \"{LogPath(nativePath)}\"");
                writeAction(nativePath);
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsAllowed());
                throw;
            }
        }

        #endregion

        #region Utils

        public static string NativePath(string path, string targetDirectory = null) =>
            PathHelper.Combine(Kit.BaseDirectory, targetDirectory ?? Kit.WorkingDirectory, path);

        public static bool Exists(string path, string targetDirectory = null) =>
            File.Exists(NativePath(path, targetDirectory));

        public static void AppendText(string path, string text, string targetDirectory = null)
        {
            var nativePath = NativePath(path, targetDirectory);
            CreateDir(nativePath);
            LogService.Log($"Append file \"{LogPath(nativePath)}\"");
            File.AppendAllText(nativePath, $"{text}\r\n");
        }

        public static void Delete(string path, string targetDirectory = null)
        {
            var nativePath = NativePath(path, targetDirectory);
            LogService.Log($"Delete file \"{LogPath(nativePath)}\"");
            File.Delete(nativePath);
        }

        public static List<string> FileNames(string path, string targetDirectory = null) =>
            Directory.GetFiles(NativePath(path, targetDirectory)).Select(PathHelper.FileName).ToList();

        private static void CreateDir(string nativeFilePath)
        {
            var nativeDir = PathHelper.Parent(nativeFilePath);

            if (!Directory.Exists(nativeDir))
            {
                Directory.CreateDirectory(nativeDir);
                LogService.Log($"Create directory \"{LogPath(nativeDir)}\"");
            }
        }

        public static string LogPath(string path)
        {
            path = path.Replace("/", @"\");

            return path.StartsWith(Kit.BaseDirectory.Replace("/", @"\"))
                ? "." + path.Substring(Kit.BaseDirectory.Length)
                : path;
        }

        #endregion
    }
}
