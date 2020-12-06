using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Kit
{
    public class FileClient : IDataClient, ILogClient, IReportClient
    {
        private static FileClient? _instance;
        public static FileClient Instance => _instance ??= new FileClient();
        protected FileClient() { }

        #region IDataClient

        public void PushToWrite(string path, string text) =>
            WriteBase(path, nativePath => File.WriteAllText(nativePath, text));

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
            WriteText(filePath, $"{subject}\r\n\r\n{body}\r\n");
            var attachmentCounter = 0;

            foreach (var attachmentPath in attachmentPaths)
                using (var readStream = OpenRead(attachmentPath))
                    WriteFrom($"{paddedCount}-{++attachmentCounter} {PathHelper.FileName(attachmentPath)}", readStream);
        }

        #endregion

        #region ILogClient

        private const string LogTimeFormat = "dd.MM.yyyy HH:mm:ss.fff";
        private bool _isLogInitialized;
        private string? _logNativeFilePath;

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
            File.AppendAllText(_logNativeFilePath!, $"{dateTime}  {_logBadges[level]} {message}\r\n");
        }

        private void LogInitialize()
        {
            if (_isLogInitialized)
                throw new InvalidOperationException();

            _isLogInitialized = true;
            _logNativeFilePath = NativePath(PathHelper.Combine(Kit.DiagnisticsCurrentDirectory, LogService.LogFileName));
            CreateDir(_logNativeFilePath);
        }

        #endregion

        #region Read

        public static string ReadText(string path) =>
            ReadBase(path, nativePath => File.ReadAllText(nativePath));

        public static List<string> ReadLines(string path) =>
            ReadBase(path, nativePath => File.ReadAllLines(nativePath).ToList());

        public static byte[] ReadBytes(string path) =>
            ReadBase(path, nativePath => File.ReadAllBytes(nativePath));

        public static dynamic ReadObject(string path) => ReadObject<object>(path);

        public static T ReadObject<T>(string path) where T : class, new() =>
            ReadBase(path, nativePath =>
            {
                using var readStream = File.OpenRead(nativePath);
                return JsonHelper.Deserialize<T>(readStream);
            });

        public static void ReadTo(string path, Stream target)
        {
            using var source = OpenRead(path);
            source.CopyTo(target);
        }

        public static FileStream OpenRead(string path) =>
            ReadBase(path, nativePath => File.OpenRead(nativePath));

        private static T ReadBase<T>(string path, Func<string, T> readFunc)
        {
            try
            {
                var nativePath = NativePath(path);
                LogService.Log($"Read file \"{PathForLog(nativePath)}\"");
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

        public static void WriteText(string path, string text) =>
            WriteBase(path, nativePath => File.WriteAllText(nativePath, text));

        public static void WriteLines(string path, IEnumerable<string> lines) =>
            WriteBase(path, nativePath => File.WriteAllLines(nativePath, lines.ToArray()));

        public static void WriteBytes(string path, byte[] bytes) =>
            WriteBase(path, nativePath => File.WriteAllBytes(nativePath, bytes));

        public static void WriteObject<T>(string path, T obj) =>
            WriteBase(path, nativePath =>
            {
                using var writeStream = File.Create(nativePath);
                JsonHelper.Serialize(obj, writeStream);
            });

        public static void WriteFrom(string path, Stream source)
        {
            using var target = OpenWrite(path);
            source.CopyTo(target);
        }

        public static FileStream OpenWrite(string path)
        {
            try
            {
                var nativePath = NativePath(path);
                CreateDir(nativePath);
                LogService.Log($"Write file \"{PathForLog(nativePath)}\"");
                return File.OpenWrite(nativePath);
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsAllowed());
                throw;
            }
        }

        private static void WriteBase(string path, Action<string> writeAction)
        {
            try
            {
                var nativePath = NativePath(path);
                CreateDir(nativePath);
                LogService.Log($"Write file \"{PathForLog(nativePath)}\"");
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

        public static string NativePath(string path) =>
            PathHelper.Combine(Kit.BaseDirectory, Kit.WorkingDirectory, path);

        public static bool Exists(string path) =>
            File.Exists(NativePath(path));

        public static void AppendText(string path, string text)
        {
            var nativePath = NativePath(path);
            CreateDir(nativePath);
            LogService.Log($"Append file \"{PathForLog(nativePath)}\"");
            File.AppendAllText(nativePath, $"{text}\r\n");
        }

        public static void Delete(string path)
        {
            var nativePath = NativePath(path);
            LogService.Log($"Delete file \"{PathForLog(nativePath)}\"");
            File.Delete(nativePath);
        }

        public static List<string> FileNames(string path) =>
            Directory.GetFiles(NativePath(path)).Select(PathHelper.FileName).ToList();

        private static void CreateDir(string nativeFilePath)
        {
            var nativeDir = PathHelper.Parent(nativeFilePath);

            if (!Directory.Exists(nativeDir))
            {
                Directory.CreateDirectory(nativeDir);
                LogService.Log($"Create directory \"{PathForLog(nativeDir)}\"");
            }
        }

        public static string PathForLog(string path)
        {
            path = path.Replace("/", @"\");

            return path.StartsWith(Kit.BaseDirectory.Replace("/", @"\"))
                ? "." + path.Substring(Kit.BaseDirectory.Length)
                : path;
        }

        #endregion
    }
}
