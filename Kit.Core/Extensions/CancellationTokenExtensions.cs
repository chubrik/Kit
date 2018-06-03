using System.Threading;

namespace Kit
{
    public static class CancellationTokenExtensions
    {
        public static CancellationTokenSource GetNestedSource(this CancellationToken cancellationToken) =>
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    }
}
