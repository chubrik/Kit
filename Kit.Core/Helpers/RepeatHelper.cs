using System;
using System.Threading.Tasks;

namespace Kit
{
    public class RepeatHelper
    {
        public static async Task Repeat(Func<Task> action, int repeatCount = 10, int waitSeconds = 10)
        {
            var count = 0;
            Retry:

            try
            {
                if (++count > 1)
                    LogService.LogInfo($"Repeat {count} of {repeatCount}:");

                await action();
                return;
            }
            catch (Exception exception)
            {
                if (exception.IsCanceled() || count == repeatCount)
                    throw;

                ExceptionHandler.Register(exception);
                LogService.LogWarning($"Repeat {count} of {repeatCount} failed. Waiting {waitSeconds} seconds...");
                await Task.Delay(TimeSpan.FromSeconds(waitSeconds), Kit.CancellationToken);
                goto Retry;
            }
        }
    }
}
