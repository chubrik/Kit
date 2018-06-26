using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Http
{
    public static class HttpExtensions
    {
        #region Get JSON dynamic

        public static dynamic GetJson(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetJsonAsync<object>(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static dynamic GetJson(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetJsonAsync<object>(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static Task<dynamic> GetJsonAsync(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetJsonAsync<object>(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<dynamic> GetJsonAsync(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetJsonAsync<object>(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<dynamic> GetJsonAsync(
            this HttpClient client, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetJsonAsync<object>(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<dynamic> GetJsonAsync(
            this HttpClient client, Uri uri, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetJsonAsync<object>(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion

        #region Get JSON generic

        public static T GetJson<T>(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            client.GetJsonAsync<T>(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static T GetJson<T>(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            client.GetJsonAsync<T>(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static Task<T> GetJsonAsync<T>(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            client.GetJsonAsync<T>(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<T> GetJsonAsync<T>(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            client.GetJsonAsync<T>(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<T> GetJsonAsync<T>(
            this HttpClient client, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            client.GetJsonAsync<T>(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static async Task<T> GetJsonAsync<T>(
            this HttpClient client, Uri uri, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class
        {
            using (var stream = await client.GetStreamAsync(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds))
            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
                return new JsonSerializer().Deserialize<T>(jsonTextReader);
        }

        #endregion

        #region Post JSON

        public static IHttpResponse PostJson(
            this HttpClient client, string url, object json,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostJsonAsync(new Uri(url), json, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static IHttpResponse PostJson(
            this HttpClient client, Uri uri, object json,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostJsonAsync(uri, json, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static Task<IHttpResponse> PostJsonAsync(
            this HttpClient client, string url, object json,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostJsonAsync(new Uri(url), json, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostJsonAsync(
            this HttpClient client, Uri uri, object json,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostJsonAsync(uri, json, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostJsonAsync(
            this HttpClient client, string url, object json, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostJsonAsync(new Uri(url), json, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostJsonAsync(
            this HttpClient client, Uri uri, object json, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostSerializedJsonAsync(uri, JsonConvert.SerializeObject(json), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion
    }
}
