using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Utils.Helpers {
    public class PathHelper {

        public static string Combine(params string[] paths) {
            var result = Path.Combine(paths).Replace(@"\", "/");

            while (result.Contains("../"))
                result = Regex.Replace(result, @"(?<=^|/)[^/]+/\.\./", "");

            return result;
        }

        public static string CombineLocal(params string[] paths) {
            var allPaths = new string[paths.Length + 1];
            allPaths[0] = Utils.BaseDirectory;

            for (var i = 0; i < paths.Length; i++)
                allPaths[i + 1] = paths[i];

            return Combine(allPaths);
        }

        public static string GetParent(string path) =>
            path.Contains('/') ? path.Substring(0, path.LastIndexOf('/')) : string.Empty;
    }
}
