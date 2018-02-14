using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utils.Helpers;

namespace Utils.Services {
    public class FileService {

        private static ExceptionService ExceptionService => ExceptionService.Instance;
        private static LogService LogService => LogService.Instance;

        private static FileService instance;
        public static FileService Instance => instance ?? (instance = new FileService());
        private FileService() { }

        internal string TargetDirectory = "$work";

        public void Setup(string targetDirectory = null) {

            if (targetDirectory != null)
                TargetDirectory = targetDirectory;
        }

        #region Read

        public string ReadText(string path) =>
            ReadBase(path, fullPath => File.ReadAllText(fullPath));

        public IReadOnlyList<string> ReadLines(string path) =>
            ReadBase(path, fullPath => File.ReadAllLines(fullPath));

        public IReadOnlyList<byte> ReadBytes(string path) =>
            ReadBase(path, fullPath => File.ReadAllBytes(fullPath));

        private T ReadBase<T>(string path, Func<string, T> readFunc) {
            try {
                var fullPath = PathHelper.CombineLocal(TargetDirectory, path);
                LogService.WriteLine($"Read file \"{fullPath}\"");
                return readFunc(fullPath);
            }
            catch (Exception exception) {
                ExceptionService.Register(exception);
                throw;
            }
        }

        public FileStream OpenRead(string path) {
            try {
                var fullPath = PathHelper.CombineLocal(TargetDirectory, path);
                LogService.WriteLine($"Read file \"{fullPath}\"");
                return File.OpenRead(fullPath);
            }
            catch (Exception exception) {
                ExceptionService.Register(exception);
                throw;
            }
        }

        #endregion

        #region Write

        public void Write(string path, string text) =>
            WriteBase(path, fullPath => File.WriteAllText(fullPath, text));

        public void Write(string path, string[] lines) =>
            WriteBase(path, fullPath => File.WriteAllLines(fullPath, lines));

        public void Write(string path, IEnumerable<string> lines) =>
            WriteBase(path, fullPath => File.WriteAllLines(fullPath, lines.ToArray()));

        public void Write(string path, byte[] bytes) =>
            WriteBase(path, fullPath => File.WriteAllBytes(fullPath, bytes));

        public void Write(string path, IEnumerable<byte> bytes) =>
            WriteBase(path, fullPath => File.WriteAllBytes(fullPath, bytes.ToArray()));

        private void WriteBase(string path, Action<string> writeAction) {
            try {
                var fullPath = PathHelper.CombineLocal(TargetDirectory, path);
                LogService.WriteLine($"Write file \"{fullPath}\"");
                CreateTree(fullPath);
                writeAction(fullPath);
            }
            catch (Exception exception) {
                ExceptionService.Register(exception);
                throw;
            }
        }

        public FileStream OpenWrite(string path) {
            try {
                var fullPath = PathHelper.CombineLocal(TargetDirectory, path);
                LogService.WriteLine($"Write file \"{fullPath}\"");
                CreateTree(fullPath);
                return File.OpenWrite(fullPath);
            }
            catch (Exception exception) {
                ExceptionService.Register(exception);
                throw;
            }
        }

        #endregion

        private void CreateTree(string path) {
            var dirPath = path.Contains('/') ? path.Substring(0, path.LastIndexOf('/')) : string.Empty;
            Directory.CreateDirectory(dirPath);
        }
    }
}
