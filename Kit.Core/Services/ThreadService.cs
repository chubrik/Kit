using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kit
{
    public class ThreadService
    {
        private ThreadService() { }

        private static int _count = 0;
        private static List<Exception> _exceptions = new List<Exception>();

        public static void StartNew(Func<Task> @delegate)
        {
            //todo log
            _count++;

            new Thread(new ThreadStart(async () =>
            {
                try
                {
                    await @delegate();
                }
                catch (Exception exception)
                {
                    Debug.Fail(exception.ToString());
                    ExceptionHandler.Register(exception);
                    _exceptions.Add(exception);
                }
                finally
                {
                    _count--;
                }
            })).Start();
        }

        public static async Task AwaitAll()
        {
            //todo log

            while (_count > 0)
                await Task.Delay(50);

            if (_exceptions.Count == 1)
                throw _exceptions[0];

            if (_exceptions.Count > 1)
                throw new AggregateException(_exceptions);
        }
    }
}
