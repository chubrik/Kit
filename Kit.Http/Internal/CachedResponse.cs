using System;
using System.Collections.Generic;
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

        private readonly Func<List<string>> _readInfo;

        public IReadOnlyDictionary<string, IReadOnlyList<string>> Headers =>
            throw new NotImplementedException();

        public string RawHeaders => throw new NotImplementedException();

        #endregion

        #region Content

        private string? _text;
        public string ReadText() => _text ??= _readText();

        private byte[]? _bytes;
        public byte[] ReadBytes() => _bytes ??= _readBytes();

        public Stream ReadStream() => _readStream();

        public Task<string> ReadTextAsync() => Task.FromResult(ReadText());

        public Task<byte[]> ReadBytesAsync() => Task.FromResult(ReadBytes());

        public Task<Stream> ReadStreamAsync() => Task.FromResult(ReadStream());

        #endregion

        #region Constructor & Dispose

        private readonly Func<string> _readText;
        private readonly Func<byte[]> _readBytes;
        private readonly Func<Stream> _readStream;

        public CachedResponse(
            string mimeType,
            Func<List<string>> readInfo,
            Func<string> readText,
            Func<byte[]> readBytes,
            Func<Stream> readStream)
        {
            MimeType = mimeType ?? throw new ArgumentNullException(nameof(mimeType));
            _readInfo = readInfo ?? throw new ArgumentNullException(nameof(readInfo));
            _readText = readText ?? throw new ArgumentNullException(nameof(readText));
            _readBytes = readBytes ?? throw new ArgumentNullException(nameof(readBytes));
            _readStream = readStream ?? throw new ArgumentNullException(nameof(readStream));
        }

        public void Dispose() { }

        #endregion
    }
}
