using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Kit.Http {
    internal class HttpResponse : IHttpResponse {

        internal HttpResponseMessage Original { get; }

        public IHttpRequest Request { get; }

        public string HttpVersion => Original.Version.ToString();

        public int StatusCode => (int)Original.StatusCode;

        public string ReasonPhrase => Original.ReasonPhrase;

        public string ConnectionString => $"HTTP/{HttpVersion} {StatusCode} {ReasonPhrase}";

        public string MimeType => Original.Content.Headers.ContentType.MediaType;

        public bool IsText => MimeType.StartsWith("text/");

        public bool IsHtml => MimeType == "text/html";

        #region Headers

        private Dictionary<string, IReadOnlyList<string>> headers;

        public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers {
            get {
                if (headers != null)
                    return headers;

                var result = new Dictionary<string, IReadOnlyList<string>>();

                foreach (var header in Original.Headers)
                    result.Add(header.Key, header.Value.ToList());

                foreach (var header in Original.Content.Headers)
                    result.Add(header.Key, header.Value.ToList());

                return headers = result.OrderBy(i => i.Key).ToDictionary(i => i.Key, i => i.Value);
            }
        }

        private string rawHeaders;

        public string RawHeaders {
            get {
                if (rawHeaders != null)
                    return rawHeaders;

                var lines = new List<string>();

                foreach (var header in Headers)
                    foreach (var item in header.Value)
                        lines.Add($"{header.Key}: {item}");

                return rawHeaders = lines.JoinLines();
            }
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

        public HttpResponse(HttpResponseMessage response, CookieCollection requestCookies) {
            Debug.Assert(response != null);
            Original = response ?? throw new ArgumentNullException(nameof(response));
            Request = new HttpRequest(response.RequestMessage, requestCookies);
        }
    }
}
