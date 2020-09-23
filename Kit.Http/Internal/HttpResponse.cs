using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kit.Http
{
    internal class HttpResponse : IHttpResponse
    {
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

        public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers
        {
            get
            {
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

        public string RawHeaders
        {
            get
            {
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

        public string ReadText() => Task.Run(ReadTextAsync).Result;

        public byte[] ReadBytes() => Task.Run(ReadBytesAsync).Result;

        public Stream ReadStream() => Task.Run(ReadStreamAsync).Result;

        private string _text;

        public async Task<string> ReadTextAsync() =>
            _text ??= await Original.Content.ReadAsStringAsync();

        private byte[] _bytes;

        public async Task<byte[]> ReadBytesAsync() =>
            _bytes ??= await Original.Content.ReadAsByteArrayAsync();

        public Task<Stream> ReadStreamAsync() => Original.Content.ReadAsStreamAsync();

        #endregion

        #region Formatted

        private string _formattedInfo;

        public string FormattedInfo
        {
            get
            {
                if (_formattedInfo != null)
                    return _formattedInfo;

                var result =
                    $"--- RAW REQUEST ---\r\n\r\n{Request.ConnectionString}\r\n" +
                    $"{Request.RawHeaders}\r\n";

                if (Request.HasContent)
                    result += Request.ReadText() + "\r\n\r\n";

                result +=
                    $"\r\n\r\n--- RESPONSE HEADERS ---\r\n\r\n{ConnectionString}\r\n" +
                    $"{RawHeaders}";

                return _formattedInfo = result;
            }
        }

        #endregion

        #region Constructor & Dispose

        public HttpResponse(HttpResponseMessage response, CookieCollection requestCookies)
        {
            Debug.Assert(response != null);
            Original = response ?? throw new ArgumentNullException(nameof(response));
            Request = new HttpRequest(response.RequestMessage, requestCookies);
        }

        public void Dispose() => Original.Dispose();

        #endregion
    }
}
