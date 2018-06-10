using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Http
{
    internal class HttpHelper
    {
        private const int RepeatCount = 10;
        private const int RepeatPauseSeconds = 10;

        public static async Task RepeatAsync(Func<Task> action, CancellationToken cancellationToken)
        {
            var count = 0;
            Retry:

            try
            {
                if (++count > 1)
                    LogService.LogInfo($"Repeat {count} of {RepeatCount}:");

                await action();
                return;
            }
            catch (Exception exception)
            {
                if (exception.IsCanceled() || count == RepeatCount)
                    throw;

                ExceptionHandler.Register(exception, LogLevel.Warning);

                if (exception.IsTimeoutOrCanceled())
                    LogService.LogWarning($"Repeat {count} of {RepeatCount} failed by timeout");
                else
                {
                    LogService.LogWarning($"Repeat {count} of {RepeatCount} failed. Waiting {RepeatPauseSeconds} seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(RepeatPauseSeconds), cancellationToken);
                }

                goto Retry;
            }
        }

        public static async Task<T> TimeoutAsync<T>(
            int timeoutSeconds, CancellationToken cancellationToken, Func<CancellationToken, Task<T>> action)
        {
            var cts = cancellationToken.GetNestedSource();
            var timeIsOut = false;

            var timerTask = Task.Factory.StartNew(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(timeoutSeconds), cts.Token);
                    timeIsOut = true;
                    cts.Cancel();
                }
                catch (TaskCanceledException)
                {
                    // no throw for canceled timer
                }
            });

            try
            {
                return await action(cts.Token);
            }
            catch (OperationCanceledException exception)
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
