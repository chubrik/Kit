using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Chubrik.Kit.Http
{
    internal class HttpRequest : IHttpRequest
    {
        internal HttpRequestMessage Original { get; }
        public string HttpVersion => Original.Version.ToString();
        public string Method => Original.Method.ToString();
        public Uri RequestUri => Original.RequestUri;
        public string ConnectionString => $"{Method} {RequestUri.AbsoluteUri} HTTP/{HttpVersion}";

        #region Headers

        private IReadOnlyDictionary<string, IReadOnlyList<string>>? _headers;

        public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers
        {
            get
            {
                if (_headers != null)
                    return _headers;

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

                return _headers = result.OrderBy(i => i.Key).ToDictionary(i => i.Key, i => i.Value);
            }
        }

        private string? _rawHeaders;

        public string RawHeaders
        {
            get
            {
                if (_rawHeaders != null)
                    return _rawHeaders;

                var lines = new List<string>();

                foreach (var header in Headers)
                {
                    var separator = header.Key == "User-Agent" ? " " : header.Key == "Cookie" ? "; " : ", ";
                    lines.Add($"{header.Key}: {header.Value.Join(separator)}");
                }

                return _rawHeaders = lines.JoinLines();
            }
        }

        #endregion

        #region Content

        public bool HasContent => Original.Content != null;

        public string ReadText() => Task.Run(ReadTextAsync).Result;

        public byte[] ReadBytes() => Task.Run(ReadBytesAsync).Result;

        public Stream ReadStream() => Task.Run(ReadStreamAsync).Result;

        private string? _text;

        public async Task<string> ReadTextAsync() =>
            _text ??= await Original.Content.ReadAsStringAsync();

        private byte[]? _bytes;

        public async Task<byte[]> ReadBytesAsync() =>
            _bytes ??= await Original.Content.ReadAsByteArrayAsync();

        public Task<Stream> ReadStreamAsync() => Original.Content.ReadAsStreamAsync();

        #endregion

        #region Constructor & Dispose

        private readonly CookieCollection _cookies;

        public HttpRequest(HttpRequestMessage request, CookieCollection cookies)
        {
            Original = request ?? throw new ArgumentNullException(nameof(request));
            _cookies = cookies ?? throw new ArgumentNullException(nameof(cookies));
        }

        public void Dispose() => Original.Dispose();

        #endregion
    }
}
