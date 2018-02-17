using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kit {
    public class RepeatHelper {

        public static async Task<bool> Repeat(
            Func<Task> action, CancellationToken cancellationToken, int waitSeconds = 5, int tryCount = 5) {

            var step = 0;

            while (++step <= tryCount)
                try {
                    await action();
                    return true;
                }
                catch (Exception exception) {
                    LogService.LogInfo($"Repeat {step} of {tryCount}:");

                    if (step < tryCount) {
                        ExceptionHandler.Register(exception, LogLevel.Warning);
                        await Task.Delay(TimeSpan.FromSeconds(waitSeconds), cancellationToken);
                    }
                    else {
                        Debug.Fail(exception.ToString());
                        ExceptionHandler.Register(exception);
                        throw;
                    }
                }

            return false;
        }
    }
}
