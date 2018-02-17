using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kit {
    public class PathHelper {

        public static string Combine(params string[] paths) {
            var result = Path.Combine(paths).Replace(@"\", "/");

            while (result.Contains("../"))
                result = Regex.Replace(result, @"(?<=^|/)[^/]+/\.\./", "");

            return result;
        }
        
        public static string GetParent(string path) =>
            path.Contains('/') ? path.Substring(0, path.LastIndexOf('/')) : string.Empty;
    }
}
