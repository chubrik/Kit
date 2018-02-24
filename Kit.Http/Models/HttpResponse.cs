using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kit.Http {
    public class HttpResponse {

        internal HttpResponseMessage Original { get; }

        public string MimeType { get; }

        public bool IsText => MimeType.StartsWith("text/");

        public bool IsHtml => MimeType == "text/html";

        #region Headers

        private Func<Task<Dictionary<string, List<string>>>> getHeadersAsync;

        private Dictionary<string, List<string>> headers;

        public Dictionary<string, List<string>> GetHeaders() => GetHeadersAsync().Result;

        public async Task<Dictionary<string, List<string>>> GetHeadersAsync() {

            if (headers != null)
                return headers;

            var result = await getHeadersAsync();
            Debug.Assert(result != null);

            if (result == null)
                throw new InvalidOperationException();

            Debug.Assert(headers == null);

            if (headers != null)
                throw new InvalidOperationException();

            return headers = result;
        }

        #endregion

        #region Text

        private Func<Task<string>> getTextAsync;

        private string text;

        public string GetText() => GetTextAsync().Result;

        public async Task<string> GetTextAsync() {

            if (text != null)
                return text;

            var result = await getTextAsync();
            Debug.Assert(result != null);

            if (result == null)
                throw new InvalidOperationException();

            Debug.Assert(text == null);

            if (text != null)
                throw new InvalidOperationException();

            return text = result;
        }

        #endregion

        #region Bytes

        private Func<Task<byte[]>> getBytesAsync;

        private byte[] bytes;

        public async Task<byte[]> GetBytesAsync() {

            if (bytes != null)
                return bytes;

            var result = await getBytesAsync();
            Debug.Assert(result != null);

            if (result == null)
                throw new InvalidOperationException();

            Debug.Assert(bytes == null);

            if (bytes != null)
                throw new InvalidOperationException();

            return bytes = result;
        }

        #endregion

        #region Raw

        //todo

        private string rawHeaders;

        public string RawHeaders =>
            rawHeaders ?? (rawHeaders = Original.RawHeaders());

        private string rawRequestHeaders;

        public string RawRequestHeaders =>
            rawRequestHeaders ?? (rawRequestHeaders = Original.RequestMessage.RawHeaders());

        private string formattedInfo;

        public string FormattedInfo =>
            formattedInfo ?? (formattedInfo =
                $"--- REQUEST HEADERS ---\r\n\r\n{RawRequestHeaders}\r\n\r\n\r\n" +
                $"--- RESPONSE HEADERS ---\r\n\r\n{RawHeaders}");

        #endregion

        public HttpResponse(HttpResponseMessage response) {
            Debug.Assert(response != null);
            Original = response ?? throw new InvalidOperationException();
            MimeType = response.Content.Headers.ContentType.MediaType;

            getHeadersAsync = () => {
                var dict = response.Headers.ToDictionary(i => i.Key, i => i.Value.ToList());
                return Task.FromResult(dict);
            };

            getTextAsync = () => response.Content.ReadAsStringAsync();
            getBytesAsync = () => response.Content.ReadAsByteArrayAsync();
        }

        public HttpResponse(
            string mimeType,
            Func<Task<Dictionary<string, List<string>>>> getHeadersAsync,
            Func<Task<string>> getTextAsync,
            Func<Task<byte[]>> getBytesAsync) {

            MimeType = mimeType;

            Debug.Assert(getHeadersAsync != null);
            this.getHeadersAsync = getHeadersAsync ?? throw new InvalidOperationException();

            Debug.Assert(getTextAsync != null);
            this.getTextAsync = getTextAsync ?? throw new InvalidOperationException();

            Debug.Assert(getBytesAsync != null);
            this.getBytesAsync = getBytesAsync ?? throw new InvalidOperationException();
        }
    }
}
