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

                while (result.Contains("/../"))
                    result = Regex.Replace(result, @"(?<=^|/)[^/]+/\.\./", "");

                return result;
            }
            catch (Exception exception) {
                ExceptionService.Register(exception);
                throw;
            }
        }
    }
}
