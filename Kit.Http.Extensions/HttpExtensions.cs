using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Kit.Http
{
    public static class HttpExtensions
    {
        public static IHttpResponse PostJson(
            this HttpClient client, string url, object json,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostJsonAsync(
                url, json, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static IHttpResponse PostJson(
            this HttpClient client, Uri uri, object json,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostJsonAsync(
                uri, json, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static Task<IHttpResponse> PostJsonAsync(
            this HttpClient client, string url, object json,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostJsonAsync(
                new Uri(url), json, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostJsonAsync(
            this HttpClient client, Uri uri, object json,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostSerializedJsonAsync(
                uri, JsonConvert.SerializeObject(json),
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);
    }
}
