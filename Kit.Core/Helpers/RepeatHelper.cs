using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Kit {
    public class RepeatHelper {

        public static async Task<bool> Repeat(Func<Task> action, int repeatCount = 10, int waitSeconds = 10) {
            var count = 0;

            while (++count <= repeatCount)
                try {
                    await action();
                    return true;
                }
                catch (Exception exception) {

                    if (exception.IsCanceled()) {
                        ExceptionHandler.Register(exception);
                        throw;
                    }

                    if (repeatCount > 1)
                        LogService.LogInfo($"Repeat {count} of {repeatCount}:");

                    if (count < repeatCount) {
                        ExceptionHandler.Register(exception, LogLevel.Warning);
                        await Task.Delay(TimeSpan.FromSeconds(waitSeconds), Kit.CancellationToken);
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
