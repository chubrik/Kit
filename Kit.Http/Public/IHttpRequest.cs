using System;
using System.Collections.Generic;

namespace Kit.Http
{
    public interface IHttpRequest
    {
        string HttpVersion { get; }
        string Method { get; }
        Uri RequestUri { get; }
        string ConnectionString { get; }

        IReadOnlyDictionary<string, IReadOnlyList<string>> Headers { get; }
        string RawHeaders { get; }

        bool HasContent { get; }
        string GetText();
        byte[] GetBytes();
    }
}
