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

        private Dictionary<string, IReadOnlyList<string>> _headers;

        public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers {
            get {
                if (_headers != null)
                    return _headers;

                var result = new Dictionary<string, IReadOnlyList<string>>();

                foreach (var header in Original.Headers)
                    result.Add(header.Key, header.Value.ToList());

                foreach (var header in Original.Content.Headers)
                    result.Add(header.Key, header.Value.ToList());

                return _headers = result.OrderBy(i => i.Key).ToDictionary(i => i.Key, i => i.Value);
            }
        }

        private string _rawHeaders;

        public string RawHeaders {
            get {
                if (_rawHeaders != null)
                    return _rawHeaders;

                var lines = new List<string>();

                foreach (var header in Headers)
                    foreach (var item in header.Value)
                        lines.Add($"{header.Key}: {item}");

                return _rawHeaders = lines.JoinLines();
            }
        }

        #endregion

        #region Content

        private string _text;
        public string GetText() => _text ?? (_text = Original.Content.ReadAsStringAsync().Result);

        private byte[] _bytes;
        public byte[] GetBytes() => _bytes ?? (_bytes = Original.Content.ReadAsByteArrayAsync().Result);

        #endregion

        #region Formatted

        private string _formattedInfo;

        public string FormattedInfo {
            get {
                if (_formattedInfo != null)
                    return _formattedInfo;

                var result =
                    $"--- RAW REQUEST ---\r\n\r\n{Request.ConnectionString}\r\n" +
                    $"{Request.RawHeaders}\r\n";

                if (Request.HasContent)
                    result += Request.GetText() + "\r\n\r\n";

                result +=
                    $"\r\n\r\n--- RESPONSE HEADERS ---\r\n\r\n{ConnectionString}\r\n" +
                    $"{RawHeaders}";

                return _formattedInfo = result;
            }
        }

        #endregion

        public HttpResponse(HttpResponseMessage response, CookieCollection requestCookies) {
            Debug.Assert(response != null);
            Original = response ?? throw new ArgumentNullException(nameof(response));
            Request = new HttpRequest(response.RequestMessage, requestCookies);
        }
    }
}
