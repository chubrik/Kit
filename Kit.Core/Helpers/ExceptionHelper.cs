using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Kit
{
    public static class ExceptionHelper
    {
        public static string OneLineMessageWithPlace(Exception exception)
        {
            var message = exception.Message.Replace("\r\n", " ");
            var match = Regex.Match(exception.ToString(), @"(\w+\.cs):line (\d+)");

            if (match.Success)
                message += $" ({match.Groups[1].Value}:{match.Groups[2].Value})";

            return message;
        }

        public static string FullDump(Exception exception)
        {
            var dump = exception.ToString().Replace(" --->", "\n   --->") + "\n";

            if (exception is AggregateException)
            {
                Debug.Fail(string.Empty);
                throw new NotImplementedException();
            }

            if (exception.InnerException != null)
                dump += "\n\nINNER EXCEPTION:\n\n" + FullDump(exception.InnerException);

            return dump;
        }
    }
}
