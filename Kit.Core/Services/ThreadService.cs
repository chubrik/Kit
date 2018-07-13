using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kit
{
    public class ThreadService
    {
        private ThreadService() { }

        private static readonly List<Thread> _threads = new List<Thread>();

        public static void StartNew(Action @delegate) =>
            StartNew(cancellationToken =>
            {
                @delegate();
                return Task.CompletedTask;
            });

        public static void StartNew(Func<Task> delegateAsync) =>
            StartNew(cancellationToken => delegateAsync());

        public static void StartNew(Func<CancellationToken, Task> delegateAsync)
        {
            var threadName = $"Thread #{_threads.Count + 1}";
            LogService.Log($"{threadName} starting");

            var thread = new Thread(new ThreadStart(() =>
                Task.Run(() => Kit.ExecuteBaseAsync(delegateAsync, threadName)).Wait()));

            _threads.Add(thread);
            thread.Start();
        }

        public static void AwaitAll()
        {
            var count = _threads.Count;

            if (count == 0)
                return;

            var startTime = DateTimeOffset.Now;
            var logText = "Waiting for " + count + (count > 1 ? " threads" : " thread");
            LogService.Log(logText);
            _threads.ForEach(i => i.Join()); //todo timeout
            LogService.Log($"{logText} completed at {TimeHelper.FormattedLatency(startTime)}");
        }
    }
}
