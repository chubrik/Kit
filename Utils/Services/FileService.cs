using System;
using System.IO;
using Utils.Helpers;

namespace Utils.Services {
    public class FileService {

        private static ExceptionService ExceptionService => ExceptionService.Instance;

        private string workDirectory;

        public FileService(string workDirectory = "work") => this.workDirectory = workDirectory;

        public byte[] ReadBytes(string path) {
            try {
                var fullPath = Path.GetFullPath($"{workDirectory}/{path}");
                LogHelper.WriteLine($"Read file \"{fullPath}\"");
                return File.ReadAllBytes(fullPath);
            }
            catch (Exception exception) {
                ExceptionService.Register(exception);
                return null;
            }
        }

        public bool WriteBytes(string path, byte[] bytes) {
            try {
                var fullPath = Path.GetFullPath($"{workDirectory}/{path}");
                LogHelper.WriteLine($"Write file \"{fullPath}\"");
                File.WriteAllBytes(fullPath, bytes);
                return true;
            }
            catch (Exception exception) {
                ExceptionService.Register(exception);
                return false;
            }
        }
    }
}
