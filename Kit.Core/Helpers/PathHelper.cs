using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kit
{
    public class PathHelper
    {
        public static string Combine(params string[] paths)
        {
            var result = Path.Combine(paths).Replace(@"\", "/");

            while (result.Contains("../"))
            {
                if (result.StartsWith("../"))
                    return result;

                result = Regex.Replace(result, @"(?<=^|/)[^/]+/\.\./", string.Empty);
            }

            return result;
        }

        public static string Parent(string path) =>
            path.Contains('/') ? path.Substring(0, path.LastIndexOf('/')) : string.Empty;

        public static string FileName(string path)
        {
            var fileName = Path.GetFileName(path);
            var queryIndex = fileName.IndexOf('?');

            if (queryIndex != -1)
                fileName = fileName.Substring(0, queryIndex);

            return fileName;
        }

        public static string SafeFileName(string fileName)
        {
            var safeName = Regex.Replace(fileName, @"\r?\n", " ").Replace('\"', '\'');
            safeName = Regex.Replace(safeName, @"[\\/:*?<>|]", "_");
            return safeName.Length > 250 ? safeName.Substring(0, 250) : safeName;
        }
    }
}
