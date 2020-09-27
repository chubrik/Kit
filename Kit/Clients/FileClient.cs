using Newtonsoft.Json;
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
            Debug.Assert(attachmentPaths != null);

            if (attachmentPaths == null)
                throw new ArgumentNullException(nameof(attachmentPaths));

            _reportCounter++;
            var paddedCount = _reportCounter.ToString().PadLeft(3, '0');
            var filePath = PathHelper.Combine(targetDirectory, PathHelper.SafeFileName($"{paddedCount} {subject}.txt"));
            Write(filePath, $"{subject}\r\n\r\n{body}\r\n");
            var attachmentCounter = 0;

            foreach (var attachmentPath in attachmentPaths)
                using (var stream = OpenRead(attachmentPath))
                    Write($"{paddedCount}-{++attachmentCounter} {PathHelper.FileName(attachmentPath)}", stream);
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
            Debug.Assert(!_isLogInitialized);

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

        public static dynamic ReadJson(string path) => ReadJson<object>(path);

        public static T ReadJson<T>(string path) where T : class
        {
            Debug.Assert(path != null);

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var nativePath = NativePath(path);

            return LogService.Log($"Read json file \"{LogPath(nativePath)}\"", () =>
            {
                using var fileStream = File.OpenRead(nativePath);
                using var streamReader = new StreamReader(fileStream);
                using var jsonTextReader = new JsonTextReader(streamReader);
                var json = new JsonSerializer().Deserialize<T>(jsonTextReader);

                if (json == null)
                    throw new InvalidOperationException($"Wrong json content \"{LogPath(nativePath)}\"");

                return json;
            });
        }

        public static void ReadTo(string path, Stream target)
        {
            using var fs = OpenRead(path);
            fs.CopyTo(target);
        }

        public static FileStream OpenRead(string path) =>
            ReadBase(path, nativePath => File.OpenRead(nativePath));

        private static T ReadBase<T>(string path, Func<string, T> readFunc)
        {
            try
            {
                var nativePath = NativePath(path);
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

        public static void Write(string path, string text) =>
            WriteBase(path, nativePath => File.WriteAllText(nativePath, text));

        public static void Write(string path, string[] lines) =>
            WriteBase(path, nativePath => File.WriteAllLines(nativePath, lines));

        public static void Write(string path, IEnumerable<string> lines) =>
            WriteBase(path, nativePath => File.WriteAllLines(nativePath, lines.ToArray()));

        public static void Write(string path, byte[] bytes) =>
            WriteBase(path, nativePath => File.WriteAllBytes(nativePath, bytes));

        public static void Write<T>(string path, T json) where T : class
        {
            Debug.Assert(path != null);

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            Debug.Assert(json != null);

            if (json == null)
                throw new ArgumentNullException(nameof(json));

            var nativePath = NativePath(path);

            LogService.Log($"Write json file \"{LogPath(nativePath)}\"", () =>
            {
                var dirPath = PathHelper.Parent(nativePath);

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                    LogService.Log($"Create directory \"{LogPath(dirPath)}\"");
                }

                if (Exists(path))
                {
                    LogService.Log("Delete previous file");
                    File.Delete(nativePath);
                }

                CreateDir(nativePath);

                using var fileStream = File.OpenWrite(nativePath);
                using var streamWriter = new StreamWriter(fileStream);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);
                new JsonSerializer().Serialize(jsonTextWriter, json);
                jsonTextWriter.Close();
            });
        }

        public static void Write(string path, Stream source)
        {
            using var fs = OpenWrite(path);
            source.CopyTo(fs);
        }

        public static FileStream OpenWrite(string path)
        {
            try
            {
                var nativePath = NativePath(path);
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

        private static void WriteBase(string path, Action<string> writeAction)
        {
            try
            {
                var nativePath = NativePath(path);
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

        public static string NativePath(string path) =>
            PathHelper.Combine(Kit.BaseDirectory, Kit.WorkingDirectory, path);

        public static bool Exists(string path) =>
            File.Exists(NativePath(path));

        public static void AppendText(string path, string text)
        {
            var nativePath = NativePath(path);
            CreateDir(nativePath);
            LogService.Log($"Append file \"{LogPath(nativePath)}\"");
            File.AppendAllText(nativePath, $"{text}\r\n");
        }

        public static void Delete(string path)
        {
            var nativePath = NativePath(path);
            LogService.Log($"Delete file \"{LogPath(nativePath)}\"");
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
                LogService.Log($"Create directory \"{LogPath(nativeDir)}\"");
            }
        }

        private static string LogPath(string path)
        {
            path = path.Replace("/", @"\");

            return path.StartsWith(Kit.BaseDirectory.Replace("/", @"\"))
                ? "." + path.Substring(Kit.BaseDirectory.Length)
                : path;
        }

        #endregion
    }
}
