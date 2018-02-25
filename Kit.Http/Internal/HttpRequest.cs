using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

namespace Kit.Http {
    internal class HttpRequest : IHttpRequest {

        internal HttpRequestMessage Original { get; }

        public string HttpVersion => Original.Version.ToString();
        public string Method => Original.Method.ToString();
        public Uri RequestUri => Original.RequestUri;
        public string ConnectionString => $"{Method} {RequestUri.AbsoluteUri} HTTP/{HttpVersion}";

        #region Headers

        //todo

        private IReadOnlyDictionary<string, IReadOnlyList<string>> headers;

        public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers =>
            headers ?? (headers = GetHeaders());

        private IReadOnlyDictionary<string, IReadOnlyList<string>> GetHeaders() =>
            Original.Headers.ToDictionary(i => i.Key, i => (IReadOnlyList<string>)i.Value.ToList());

        private string rawHeaders;

        public string RawHeaders => rawHeaders ?? (rawHeaders = GetRawHeaders());

        private string GetRawHeaders() =>
            Original.Headers.Select(i => $"{i.Key}: {i.Value.Join(", ")}").JoinLines();

        #endregion

        public HttpRequest(HttpRequestMessage request) {
            Debug.Assert(request != null);
            Original = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
