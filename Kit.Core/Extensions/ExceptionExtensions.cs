using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kit
{
    public static class ExceptionExtensions
    {
        public static bool IsCanceled(this Exception exception)
        {
            if (exception is AggregateException)
                return ((AggregateException)exception).InnerExceptions.All(i => i.IsCanceled());

            return exception is TaskCanceledException || exception is OperationCanceledException;
        }

        public static Exception FirstInnerestException(this Exception exception)
        {
            if (exception is AggregateException aggregate && aggregate.InnerExceptions.Count > 0)
                return aggregate.InnerExceptions[0].FirstInnerestException();

            if (exception.InnerException != null)
                return exception.InnerException.FirstInnerestException();

            return exception;
        }
    }
}
