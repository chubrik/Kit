using System.Threading;
using System.Threading.Tasks;

namespace Utils.Abstractions {
    public interface ILogClient {

        Task LogAsync(string message, CancellationToken cancellationToken, LogLevel level = LogLevel.Log);

        Task LogInfoAsync(string message, CancellationToken cancellationToken);

        Task LogWarningAsync(string message, CancellationToken cancellationToken);

        Task LogErrorAsync(string message, CancellationToken cancellationToken);
    }
}
