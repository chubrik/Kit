using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kit
{
    public class ThreadService
    {
        private ThreadService() { }

        private static int _threadCount = 0;

        public static void StartNew(Func<Task> @delegate)
        {
            _threadCount++;
            LogService.Log($"Thread #{_threadCount} starting");

            new Thread(new ThreadStart(async () =>
            {
                var startTime = DateTimeOffset.Now;

                try
                {
                    LogService.Log($"Thread #{_threadCount} started");
                    await @delegate();
                    LogService.Log($"Thread #{_threadCount} completed at {TimeHelper.FormattedLatency(startTime)}");
                }
                catch (Exception exception)
                {
                    if (!exception.IsCanceled())
                    {
                        Debug.Fail(exception.ToString());
                        Kit.SetFailed();
                    }
                    else
                        Kit.SetCanceled();

                    ExceptionHandler.Register(exception);

                    if (!exception.IsCanceled())
                        Kit.Cancel();

                    ReportService.Report(exception.Message, exception.ToString());

                    if (exception.IsCanceled())
                        LogService.LogWarning($"Thread #{_threadCount} canceled at {TimeHelper.FormattedLatency(startTime)}"); //todo cancellation time
                    else
                        LogService.LogError($"Thread #{_threadCount} failed at {TimeHelper.FormattedLatency(startTime)}");
                }
                finally
                {
                    _threadCount--;
                }
            })).Start();
        }

        public static async Task AwaitAll()
        {
            if (_threadCount == 0)
                return;

            LogService.Log($"Waiting for {_threadCount} threads");
            var startTime = DateTimeOffset.Now;

            //todo limit
            while (_threadCount > 0)
                await Task.Delay(50);

            LogService.Log($"Waiting for {_threadCount} threads completed at {TimeHelper.FormattedLatency(startTime)}");
        }
    }
}
