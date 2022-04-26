using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Chubrik.Kit.Http
{
    public interface IHttpRequest : IDisposable
    {
        string HttpVersion { get; }
        string Method { get; }
        Uri RequestUri { get; }
        string ConnectionString { get; }

        IReadOnlyDictionary<string, IReadOnlyList<string>> Headers { get; }
        string RawHeaders { get; }

        bool HasContent { get; }

        string ReadText();
        byte[] ReadBytes();
        Stream ReadStream();
        Task<string> ReadTextAsync();
        Task<byte[]> ReadBytesAsync();
        Task<Stream> ReadStreamAsync();
    }
}
