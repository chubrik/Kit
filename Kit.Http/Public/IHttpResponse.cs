using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Chubrik.Kit.Http
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

        string ReadText();
        byte[] ReadBytes();
        Stream ReadStream();
        Task<string> ReadTextAsync();
        Task<byte[]> ReadBytesAsync();
        Task<Stream> ReadStreamAsync();
    }
}
