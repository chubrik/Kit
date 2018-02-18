using System;
using System.Diagnostics;
using System.Net.Http;

namespace Kit.Http {
    class HttpResponse {

        public HttpResponseMessage Model { get; }

        public string Body { get; }

        private string rawHeaders;

        public string RawHeaders =>
            rawHeaders ?? (rawHeaders = Model.RawHeaders());

        private string rawRequestHeaders;

        public string RawRequestHeaders =>
            rawRequestHeaders ?? (rawRequestHeaders = Model.RequestMessage.RawHeaders());

        private string formattedInfo;

        public string FormattedInfo =>
            formattedInfo ?? (formattedInfo =
                $"--- REQUEST HEADERS ---\r\n\r\n{RawRequestHeaders}\r\n\r\n\r\n" +
                $"--- RESPONSE HEADERS ---\r\n\r\n{RawHeaders}");

        public HttpResponse(HttpResponseMessage response, string body) {
            Debug.Assert(response != null);
            Model = response ?? throw new InvalidOperationException();
            Body = body;
        }
    }
}
