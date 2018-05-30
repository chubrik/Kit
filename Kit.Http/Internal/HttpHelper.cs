using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Http
{
    internal class HttpHelper
    {
        public static async Task RepeatAsync(Func<Task> action, int repeatCount = 10, int pauseSeconds = 10)
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

                if (exception.IsTimeoutOrCanceled())
                    LogService.LogWarning($"Repeat {count} of {repeatCount} failed by timeout");
                else
                {
                    LogService.LogWarning($"Repeat {count} of {repeatCount} failed. Waiting {pauseSeconds} seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(pauseSeconds), Kit.CancellationToken);
                }

                goto Retry;
            }
        }

        public static async Task<T> TimeoutAsync<T>(Func<CancellationToken, Task<T>> action, int timeoutSeconds)
        {
            var cts = Kit.NewLinkedCancellationTokenSource();
            var timeIsOut = false;

            var timerTask = Task.Factory.StartNew(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(timeoutSeconds), cts.Token);
                    timeIsOut = true;
                    cts.Cancel();
                }
                catch (TaskCanceledException) {
                    // no throw for canceled timer
                }
            });

            try
            {
                return await action(cts.Token);
            }
            catch (TaskCanceledException exception)
            {
                if (timeIsOut)
                    throw new TimeoutException($"The operation has timed out ({timeoutSeconds} seconds).", exception);

                throw;
            }
            finally
            {
                if (!cts.IsCancellationRequested)
                    cts.Cancel();

                await timerTask;
            }
        }
    }
}
