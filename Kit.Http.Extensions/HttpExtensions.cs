using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Kit.Http {
    public static class HttpExtensions {

        public static IHttpResponse PostJson(this HttpClient client, string url, object json) =>
            PostJsonAsync(client, url, json).Result;

        public static IHttpResponse PostJson(this HttpClient client, Uri uri, object json) =>
            PostJsonAsync(client, uri, json).Result;

        public static Task<IHttpResponse> PostJsonAsync(this HttpClient client, string url, object json) =>
            PostJsonAsync(client, new Uri(url), json);

        public static async Task<IHttpResponse> PostJsonAsync(this HttpClient client, Uri uri, object json) =>
            await client.PostSerializedJsonAsync(uri, JsonConvert.SerializeObject(json));
    }
}
