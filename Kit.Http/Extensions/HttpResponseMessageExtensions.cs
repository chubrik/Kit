using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Kit.Http {
    public static class HttpResponseMessageExtensions {

        public static string RawHeaders(this HttpResponseMessage response) {

            var headers = new List<string> {
                $"HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}"
            };

            var lines = response.ToString().SplitLines();
            headers.AddRange(lines.Skip(2).Take(lines.Count - 3).Select(i => i.TrimStart()));
            return headers.JoinLines();
        }
    }
}
