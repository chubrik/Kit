using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Kit.Http {
    public static class HttpRequestMessageExtensions {

        public static string RawHeaders(this HttpRequestMessage request) {

            var headers = new List<string> {
                $"{request.Method} {request.RequestUri} HTTP/{request.Version}"
            };

            headers.AddRange(request.Headers.Select(i => $"{i.Key}: {i.Value.Join(", ")}"));
            //todo Connection, Host

            return headers.JoinLines();
        }
    }
}
