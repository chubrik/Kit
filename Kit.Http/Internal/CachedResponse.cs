using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Kit.Http
{
    internal class CachedResponse : IHttpResponse
    {
        public IHttpRequest Request => throw new NotImplementedException();
        public string HttpVersion => throw new NotImplementedException();
        public int StatusCode => throw new NotImplementedException();
        public string ReasonPhrase => throw new NotImplementedException();
        public string ConnectionString => throw new NotImplementedException();
        public string MimeType { get; }
        public bool IsText => MimeType.StartsWith("text/");
        public bool IsHtml => MimeType == "text/html";

        #region Headers

        private readonly Func<List<string>> _getInfo;

        public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers =>
            throw new NotImplementedException();

        public string RawHeaders => throw new NotImplementedException();

        #endregion

        #region Content

        private string _text;
        public string GetText() => _text ?? (_text = _getText());

        private byte[] _bytes;
        public byte[] GetBytes() => _bytes ?? (_bytes = _getBytes());

        public Stream GetStream() => _getStream();

        public Task<string> GetTextAsync() => Task.FromResult(GetText());

        public Task<byte[]> GetBytesAsync() => Task.FromResult(GetBytes());

        public Task<Stream> GetStreamAsync() => Task.FromResult(GetStream());

        #endregion

        #region Constructor & Dispose

        private readonly Func<string> _getText;
        private readonly Func<byte[]> _getBytes;
        private readonly Func<Stream> _getStream;

        public CachedResponse(
            string mimeType,
            Func<List<string>> getInfo,
            Func<string> getText,
            Func<byte[]> getBytes,
            Func<Stream> getStream)
        {
            Debug.Assert(mimeType != null);
            MimeType = mimeType ?? throw new ArgumentNullException(nameof(mimeType));

            Debug.Assert(getInfo != null);
            _getInfo = getInfo ?? throw new ArgumentNullException(nameof(getInfo));

            Debug.Assert(getText != null);
            _getText = getText ?? throw new ArgumentNullException(nameof(getText));

            Debug.Assert(getBytes != null);
            _getBytes = getBytes ?? throw new ArgumentNullException(nameof(getBytes));

            Debug.Assert(getStream != null);
            _getStream = getStream ?? throw new ArgumentNullException(nameof(getStream));
        }

        public void Dispose() { }

        #endregion
    }
}
