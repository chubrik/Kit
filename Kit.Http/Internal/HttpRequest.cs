using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Kit.Http {
    internal class HttpRequest : IHttpRequest {

        internal HttpRequestMessage Original { get; }
        public string HttpVersion => Original.Version.ToString();
        public string Method => Original.Method.ToString();
        public Uri RequestUri => Original.RequestUri;
        public string ConnectionString => $"{Method} {RequestUri.AbsoluteUri} HTTP/{HttpVersion}";

        #region Headers

        private IReadOnlyDictionary<string, IReadOnlyList<string>> headers;

        public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers {
            get {
                if (headers != null)
                    return headers;

                var result = new Dictionary<string, IReadOnlyList<string>> {
                    { "Connection", new List<string> { "Keep-Alive" } },
                    { "Host", new List<string> { RequestUri.Host } }
                };

                foreach (var header in Original.Headers)
                    result.Add(header.Key, header.Value.ToList());

                if (Original.Content != null)
                    foreach (var header in Original.Content.Headers)
                        result.Add(header.Key, header.Value.ToList());

                var cookies = new List<string>();

                foreach (var cookie in _cookies)
                    cookies.Add(cookie.ToString());

                if (cookies.Count > 0)
                    result.Add("Cookie", cookies);

                return headers = result.OrderBy(i => i.Key).ToDictionary(i => i.Key, i => i.Value);
            }
        }

        private string rawHeaders;

        public string RawHeaders {
            get {
                if (rawHeaders != null)
                    return rawHeaders;

                var lines = new List<string>();

                foreach (var header in Headers) {
                    var separator = header.Key == "User-Agent" ? " " : header.Key == "Cookie" ? "; " : ", ";
                    lines.Add($"{header.Key}: {header.Value.Join(separator)}");
                }

                return rawHeaders = lines.JoinLines();
            }
        }

        #endregion

        #region Content

        public bool HasContent => Original.Content != null;

        private string text;
        public string GetText() => text ?? (text = Original.Content.ReadAsStringAsync().Result);

        private byte[] bytes;
        public byte[] GetBytes() => bytes ?? (bytes = Original.Content.ReadAsByteArrayAsync().Result);

        #endregion

        private CookieCollection _cookies;

        public HttpRequest(HttpRequestMessage request, CookieCollection cookies) {

            Debug.Assert(request != null);
            Original = request ?? throw new ArgumentNullException(nameof(request));

            Debug.Assert(cookies != null);
            _cookies = cookies ?? throw new ArgumentNullException(nameof(cookies));
        }
    }
}
