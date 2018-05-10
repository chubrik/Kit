using System;
using System.Text.RegularExpressions;

namespace Kit
{
    internal static class ExceptionHelper
    {
        public static string OneLineMessageWithPlace(Exception exception)
        {
            var message = exception.Message.Replace("\r\n", " ");
            var match = Regex.Match(exception.ToString(), @"(\w+\.cs):line (\d+)");

            if (match.Success)
                message += $" ({match.Groups[1].Value}:{match.Groups[2].Value})";

            return message;
        }

        public static string ExtendedDump(Exception exception)
        {
            var result = string.Empty;
            var innerest = exception.FirstInnerestException();

            if (innerest != exception)
                result += $"\r\n\r\n\r\nFIRST INNEREST EXCEPTION:\r\n\r\n{innerest}\r\n";

            result += $"\r\n\r\n\r\nFULL DUMP:\r\n\r\n{exception}\r\n";
            return result;
        }
    }
}
