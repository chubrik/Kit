using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Abstractions {
    public interface IDataClient {

        Task<string> ReadTextAsync(string path, CancellationToken cancellationToken);

        Task<List<string>> ReadLinesAsync(string path, CancellationToken cancellationToken);

        Task<List<byte>> ReadBytesAsync(string path, CancellationToken cancellationToken);

        Task ReadAsync(string path, Stream target, CancellationToken cancellationToken);

        Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken);

        Task WriteAsync(string path, string text, CancellationToken cancellationToken);

        Task WriteAsync(string path, string[] lines, CancellationToken cancellationToken);

        Task WriteAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken);

        Task WriteAsync(string path, byte[] bytes, CancellationToken cancellationToken);

        Task WriteAsync(string path, IEnumerable<byte> bytes, CancellationToken cancellationToken);

        Task WriteAsync(string path, Stream source, CancellationToken cancellationToken);

        Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken);
    }
}
