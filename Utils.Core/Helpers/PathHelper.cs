using System;
using System.IO;
using System.Text.RegularExpressions;
using Utils.Services;

namespace Utils.Helpers {
    public class PathHelper {

        private static ExceptionService ExceptionService => ExceptionService.Instance;

        public static string Combine(params string[] paths) {
            try {
                var result = Path.Combine(paths).Replace(@"\", "/");

                while (result.Contains("../"))
                    result = Regex.Replace(result, @"(?<=^|/)[^/]+/\.\./", "");

                return result;
            }
            catch (Exception exception) {
                ExceptionService.Register(exception);
                throw;
            }
        }

        public static string CombineLocal(params string[] paths) {
            var allPaths = new string[paths.Length + 1];
            allPaths[0] = Utils.BaseDirectory;

            for (var i = 0; i < paths.Length; i++)
                allPaths[i + 1] = paths[i];

            return Combine(allPaths);
        }
    }
}
