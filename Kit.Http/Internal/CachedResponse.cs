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

        private Func<List<string>> getInfo;

        public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers =>
            throw new NotImplementedException();

        public string RawHeaders => throw new NotImplementedException();

        #endregion

        #region Text

        private Func<string> getText;

        private string text;

        public string GetText() => text ?? (text = getText());

        #endregion

        #region Bytes

        private Func<byte[]> getBytes;

        private byte[] bytes;

        public byte[] GetBytes() => bytes ?? (bytes = getBytes());

        #endregion

        public CachedResponse(
            string mimeType,
            Func<List<string>> getInfo,
            Func<string> getText,
            Func<byte[]> getBytes) {

            Debug.Assert(mimeType != null);
            MimeType = mimeType ?? throw new ArgumentNullException(nameof(mimeType));

            Debug.Assert(getInfo != null);
            this.getInfo = getInfo ?? throw new ArgumentNullException(nameof(getInfo));

            Debug.Assert(getText != null);
            this.getText = getText ?? throw new ArgumentNullException(nameof(getText));

            Debug.Assert(getBytes != null);
            this.getBytes = getBytes ?? throw new ArgumentNullException(nameof(getBytes));
        }
    }
}
