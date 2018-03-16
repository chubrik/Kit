using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kit.Http {
    internal class CachedResponse : IHttpResponse {

        public IHttpRequest Request => throw new NotImplementedException();

        public string HttpVersion => throw new NotImplementedException();

        public int StatusCode => throw new NotImplementedException();

        public string ReasonPhrase => throw new NotImplementedException();

        public string ConnectionString => throw new NotImplementedException();

        public string MimeType { get; }

        public bool IsText => MimeType.StartsWith("text/");

        public bool IsHtml => MimeType == "text/html";

        #region MyRegion

        private Func<List<string>> _getInfo;

        public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers =>
            throw new NotImplementedException();

        public string RawHeaders => throw new NotImplementedException();

        #endregion

        #region Text

        private Func<string> _getText;

        private string _text;

        public string GetText() => _text ?? (_text = _getText());

        #endregion

        #region Bytes

        private Func<byte[]> _getBytes;

        private byte[] _bytes;

        public byte[] GetBytes() => _bytes ?? (_bytes = _getBytes());

        #endregion

        public CachedResponse(
            string mimeType,
            Func<List<string>> getInfo,
            Func<string> getText,
            Func<byte[]> getBytes) {

            Debug.Assert(mimeType != null);
            MimeType = mimeType ?? throw new ArgumentNullException(nameof(mimeType));

            Debug.Assert(getInfo != null);
            _getInfo = getInfo ?? throw new ArgumentNullException(nameof(getInfo));

            Debug.Assert(getText != null);
            _getText = getText ?? throw new ArgumentNullException(nameof(getText));

            Debug.Assert(getBytes != null);
            _getBytes = getBytes ?? throw new ArgumentNullException(nameof(getBytes));
        }
    }
}
