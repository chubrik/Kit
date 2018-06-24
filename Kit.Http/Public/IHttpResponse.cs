using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Kit.Http
{
    public interface IHttpResponse : IDisposable
    {
        IHttpRequest Request { get; }

        string HttpVersion { get; }
        int StatusCode { get; }
        string ReasonPhrase { get; }
        string ConnectionString { get; }

        string MimeType { get; }
        bool IsText { get; }
        bool IsHtml { get; }

        IReadOnlyDictionary<string, IReadOnlyList<string>> Headers { get; }
        string RawHeaders { get; }

        string GetText();
        byte[] GetBytes();
        Stream GetStream();
        Task<string> GetTextAsync();
        Task<byte[]> GetBytesAsync();
        Task<Stream> GetStreamAsync();
    }
}
