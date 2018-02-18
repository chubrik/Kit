using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kit {
    public class RepeatHelper {

        public static async Task<bool> Repeat(
            Func<Task> action, CancellationToken cancellationToken, int repeatCount = 5, int waitSeconds = 5) {

            var count = 0;

            while (++count <= repeatCount)
                try {
                    await action();
                    return true;
                }
                catch (Exception exception) {

                    if (repeatCount > 1)
                        LogService.LogInfo($"Repeat {count} of {repeatCount}:");

                    if (count < repeatCount) {
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
