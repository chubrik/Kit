using System;
using System.Diagnostics;
using System.IO;
using Utils.Helpers;

namespace Utils.Services {
    public class LogService {

        private static LogService instance;
        public static LogService Instance => instance ?? (instance = new LogService());
        private LogService() { }

        private bool isEnable = true;
        private bool isInitialized = false;
        private const string fileName = "$log.txt";
        private string filePath = string.Empty;

        public void Setup(bool? isEnable = null) {

            if (isEnable != null)
                this.isEnable = (bool)isEnable;
        }

        private void Initialize() {
            Debug.Assert(!isInitialized);

            if (isInitialized)
                throw new InvalidOperationException();

            var targetDir = PathHelper.CombineLocal(FileService.Instance.TargetDirectory);

            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            filePath = PathHelper.Combine(targetDir, fileName);
            isInitialized = true;
            WriteLine($"Initialize LogService");
        }

        public void WriteLine(string message) {

            if (!isEnable)
                return;

            if (!isInitialized)
                Initialize();

            File.AppendAllText(filePath, $"{message}\r\n");
        }
    }
}
