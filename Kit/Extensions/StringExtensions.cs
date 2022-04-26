using System.Collections.Generic;
using System.Linq;

namespace Chubrik.Kit
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string value) => value.Length == 0;

        public static bool IsNullOrEmpty(this string? value) => string.IsNullOrEmpty(value);

        public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);

        public static List<string> SplitLines(this string value)
        {
            var result = value.Split('\n').Select(i => i.TrimEnd('\r')).ToList();

            if (result.Last().IsEmpty())
                result.RemoveAt(result.Count - 1);

            return result;
        }

        public static string Join(this IEnumerable<char> values) =>
            string.Join(string.Empty, values);

        public static string Join(this IEnumerable<string> values) =>
            string.Join(string.Empty, values);

        public static string Join(this IEnumerable<string> values, string separator) =>
            string.Join(separator, values);

        public static string JoinLines(this IEnumerable<string> values) =>
            string.Join("\r\n", values) + "\r\n";
    }
}
