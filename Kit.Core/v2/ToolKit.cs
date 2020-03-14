using System;
using System.Diagnostics;

namespace Kit
{
    public static class ToolKit
    {
        public static T Handle<T>(string message, Func<T> block)
        {
            try
            {
                LogService.BeginTrace(message);
                var result = block();
                LogService.EndTrace(message + " completed");
                return result;
            }
            catch (Exception exception)
            {
                Debug.Assert(exception.IsAllowed());
                var logLevel = exception.IsAllowed() ? LogLevel.Debug : LogLevel.Error;
                ExceptionHandler.Register(exception, logLevel);

                if (exception.IsCanceled())
                    LogService.EndTrace(message + " canceled");

                else if (exception.IsTimeoutOrCanceled())
                    LogService.EndTrace(message + " timed out");

                else
                    LogService.EndTrace(message + " failed");

                throw;
            }
        }
    }
}
