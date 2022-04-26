using System;
using System.Linq;

namespace Chubrik.Kit
{
    public static class ExceptionExtensions
    {
        public static bool IsCanceled(this Exception exception)
        {
            if (exception is AggregateException aggregate)
                return aggregate.InnerExceptions.All(IsCanceled);

            return exception is OperationCanceledException;
        }

        public static bool IsTimeoutOrCanceled(this Exception exception)
        {
            if (exception is AggregateException aggregate)
                return aggregate.InnerExceptions.All(IsTimeoutOrCanceled);

            return exception is TimeoutException || exception.IsCanceled();
        }

        public static bool IsAllowed(this Exception exception) =>
            exception.IsTimeoutOrCanceled() || Kit.IsTest;

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
