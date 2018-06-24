using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Kit.Http
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

        string GetText();
        byte[] GetBytes();
        Stream GetStream();
        Task<string> GetTextAsync();
        Task<byte[]> GetBytesAsync();
        Task<Stream> GetStreamAsync();
    }
}
