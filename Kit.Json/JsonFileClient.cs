using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace Kit
{
    public class JsonFileClient : FileClient
    {
        private JsonFileClient() { }

        public static T Read<T>(string path, string targetDirectory = null) where T : class
        {
            Debug.Assert(path != null);

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var nativePath = NativePath(path, targetDirectory);

            return LogService.Log($"Read json file \"{LogPath(nativePath)}\"", () =>
            {
                using (var fileStream = File.OpenRead(nativePath))
                using (var streamReader = new StreamReader(fileStream))
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    var obj = new JsonSerializer().Deserialize<T>(jsonTextReader);

                    if (obj.Equals(null))
                        throw new InvalidOperationException($"Wrong json content \"{LogPath(nativePath)}\"");

                    return obj;
                }
            });
        }

        public static void Write<T>(string path, T obj, string targetDirectory = null) where T : class
        {
            Debug.Assert(path != null);

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            Debug.Assert(obj != null);

            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var nativePath = NativePath(path, targetDirectory);

            LogService.Log($"Write json file \"{LogPath(nativePath)}\"", () =>
            {
                var dirPath = PathHelper.Parent(nativePath);

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                    LogService.Log($"Create directory \"{LogPath(dirPath)}\"");
                }

                if (Exists(path, targetDirectory))
                {
                    LogService.Log("Delete previous file");
                    File.Delete(nativePath);
                }

                CreateDir(nativePath);

                using (var fileStream = File.OpenWrite(nativePath))
                using (var streamWriter = new StreamWriter(fileStream))
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    new JsonSerializer().Serialize(jsonTextWriter, obj);
                    jsonTextWriter.Close();
                }
            });
        }

        private static void CreateDir(string nativeFilePath)
        {
            var nativeDir = PathHelper.Parent(nativeFilePath);

            if (!Directory.Exists(nativeDir))
            {
                Directory.CreateDirectory(nativeDir);
                LogService.Log($"Create directory \"{LogPath(nativeDir)}\"");
            }
        }
    }
}
