using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kit
{
    public static class PathHelper
    {
        public static string Combine(params string[] paths)
        {
            var combined = Path.Combine(paths);

            if (combined.IsEmpty())
                return combined;

            var segments = Regex.Replace(combined, @"[\\/]+", "/").TrimEnd('/').Split('/');
            var output = new Stack<string>();

            foreach (var segment in segments)
            {
                if (segment == ".")
                    continue;

                if (segment == ".." && output.Count > 0)
                {
                    var previous = output.Peek();

                    if (previous.IsEmpty())
                        throw new InvalidOperationException($"Path \"{combined}\" is out of root");

                    if (previous != "..")
                    {
                        output.Pop();
                        continue;
                    }
                }

                output.Push(segment);
            }

            return output.Count == 1 && output.Peek().IsEmpty()
                ? "/"
                : output.Reverse().Join("/");
        }

        public static string Parent(string path) => Combine(path, "..");

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

        public static string? FileExtension(string path)
        {
            var fileName = FileName(path);
            var dotIndex = fileName.LastIndexOf('.');
            return dotIndex != -1 ? fileName.Substring(dotIndex + 1) : null;
        }
    }
}
