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

        public static Exception SingleMostInnerExceptionOrNull(this Exception exception) =>
            exception is AggregateException ? null
            : exception.InnerException == null ? exception
            : SingleMostInnerExceptionOrNull(exception.InnerException);
    }
}
