using System.Threading;

namespace Chubrik.Kit
{
    public static class CancellationTokenExtensions
    {
        public static CancellationTokenSource CreateLinkedSource(this CancellationToken cancellationToken) =>
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    }
}
