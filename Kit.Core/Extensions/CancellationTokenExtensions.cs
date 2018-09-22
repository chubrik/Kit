using System.Threading;

namespace Kit
{
    public static class CancellationTokenExtensions
    {
        public static CancellationTokenSource CreateLinkedSource(this CancellationToken cancellationToken) =>
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    }
}
