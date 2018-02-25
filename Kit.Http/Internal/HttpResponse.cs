using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

namespace Kit.Http {
    internal class HttpResponse : IHttpResponse {

        internal HttpResponseMessage Original { get; }

        private HttpRequest request;
        public IHttpRequest Request => request ?? (request = new HttpRequest(Original.RequestMessage));

        public string HttpVersion => Original.Version.ToString();

        public int StatusCode => (int)Original.StatusCode;

        public string ReasonPhrase => Original.ReasonPhrase;

        public string ConnectionString => $"HTTP/{HttpVersion} {StatusCode} {ReasonPhrase}";

        public string MimeType => Original.Content.Headers.ContentType.MediaType;

        public bool IsText => MimeType.StartsWith("text/");

        public bool IsHtml => MimeType == "text/html";

        #region Headers

        //todo

        private IReadOnlyDictionary<string, IReadOnlyList<string>> headers;

        public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers =>
            headers ?? (headers = GetHeaders());

        private IReadOnlyDictionary<string, IReadOnlyList<string>> GetHeaders() =>
            Original.Headers.ToDictionary(i => i.Key, i => (IReadOnlyList<string>)i.Value.ToList());

        private string rawHeaders;

        public string RawHeaders => rawHeaders ?? (rawHeaders = GetRawHeaders());

        private string GetRawHeaders() {
            var lines = Original.ToString().SplitLines();
            var result = lines.Skip(2).Take(lines.Count - 3).Select(i => i.TrimStart());
            return result.JoinLines();
        }

        #endregion

        #region Text

        private string text;

        public string GetText() => text ?? (text = Original.Content.ReadAsStringAsync().Result);

        #endregion

        #region Bytes

        private byte[] bytes;

        public byte[] GetBytes() => bytes ?? (bytes = Original.Content.ReadAsByteArrayAsync().Result);

        #endregion

        //todo post data
        public string FormattedInfo =>
            $"--- REQUEST ---\r\n\r\n{Request.ConnectionString}\r\n" +
            $"{Request.RawHeaders}\r\n\r\n\r\n" +
            $"--- RESPONSE ---\r\n\r\n{ConnectionString}\r\n" +
            $"{RawHeaders}";

        public HttpResponse(HttpResponseMessage response) {
            Debug.Assert(response != null);
            Original = response ?? throw new ArgumentNullException(nameof(response));
        }
    }
}
