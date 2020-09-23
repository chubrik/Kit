using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Http
{
    public static class HttpServiceExtensions
    {
        #region Get JSON dynamic

        public static dynamic GetJson(
            this HttpService http, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.GetJsonAsync<object>(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static dynamic GetJson(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.GetJsonAsync<object>(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<dynamic> GetJsonAsync(
            this HttpService http, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetJsonAsync<object>(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<dynamic> GetJsonAsync(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetJsonAsync<object>(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<dynamic> GetJsonAsync(
            this HttpService http, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetJsonAsync<object>(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<dynamic> GetJsonAsync(
            this HttpService http, Uri uri, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetJsonAsync<object>(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion

        #region Get JSON generic

        public static T GetJson<T>(
            this HttpService http, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            Task.Run(() => http.GetJsonAsync<T>(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static T GetJson<T>(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            Task.Run(() => http.GetJsonAsync<T>(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<T> GetJsonAsync<T>(
            this HttpService http, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            http.GetJsonAsync<T>(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<T> GetJsonAsync<T>(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            http.GetJsonAsync<T>(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<T> GetJsonAsync<T>(
            this HttpService http, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            http.GetJsonAsync<T>(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static async Task<T> GetJsonAsync<T>(
            this HttpService http, Uri uri, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class
        {
            using (var stream = await http.GetStreamAsync(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds))
            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
                return new JsonSerializer().Deserialize<T>(jsonTextReader);
        }

        #endregion

        #region Post JSON

        public static IHttpResponse PostJson(
            this HttpService http, string url, object json,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.PostJsonAsync(new Uri(url), json, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static IHttpResponse PostJson(
            this HttpService http, Uri uri, object json,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.PostJsonAsync(uri, json, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<IHttpResponse> PostJsonAsync(
            this HttpService http, string url, object json,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostJsonAsync(new Uri(url), json, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostJsonAsync(
            this HttpService http, Uri uri, object json,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostJsonAsync(uri, json, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostJsonAsync(
            this HttpService http, string url, object json, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostJsonAsync(new Uri(url), json, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostJsonAsync(
            this HttpService http, Uri uri, object json, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostSerializedJsonAsync(uri, JsonConvert.SerializeObject(json), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion
    }
}
