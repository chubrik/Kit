using System.Collections.Generic;

namespace Kit.Http {
    public interface IHttpResponse {

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
    }
}
