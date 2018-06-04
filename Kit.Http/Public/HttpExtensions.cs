using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Http
{
    public static class HttpExtensions
    {
        #region Get

        #region Get text

        public static string GetText(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetTextAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static string GetText(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetTextAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static Task<string> GetTextAsync(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetTextAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<string> GetTextAsync(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetTextAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<string> GetTextAsync(
            this HttpClient client, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetTextAsync(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static async Task<string> GetTextAsync(
            this HttpClient client, Uri uri, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            var response = await client.GetAsync(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

            return response.GetText();
        }

        #endregion

        #region Get bytes

        public static byte[] GetBytes(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetBytesAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static byte[] GetBytes(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetBytesAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static Task<byte[]> GetBytesAsync(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetBytesAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<byte[]> GetBytesAsync(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetBytesAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<byte[]> GetBytesAsync(
            this HttpClient client, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetBytesAsync(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static async Task<byte[]> GetBytesAsync(
            this HttpClient client, Uri uri, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            var response = await client.GetAsync(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

            return response.GetBytes();
        }

        #endregion

        #region Get response

        public static IHttpResponse Get(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static IHttpResponse Get(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static Task<IHttpResponse> GetAsync(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> GetAsync(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> GetAsync(
            this HttpClient client, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetAsync(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion

        #endregion

        #region Post

        #region Post form

        public static IHttpResponse PostForm(
            this HttpClient client, string url, IEnumerable<KeyValuePair<string, string>> form,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostFormAsync(new Uri(url), form, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static IHttpResponse PostForm(
            this HttpClient client, Uri uri, IEnumerable<KeyValuePair<string, string>> form,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostFormAsync(uri, form, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static Task<IHttpResponse> PostFormAsync(
            this HttpClient client, string url, IEnumerable<KeyValuePair<string, string>> form,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostFormAsync(new Uri(url), form, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostFormAsync(
            this HttpClient client, Uri uri, IEnumerable<KeyValuePair<string, string>> form,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostFormAsync(uri, form, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostFormAsync(
            this HttpClient client, string url, IEnumerable<KeyValuePair<string, string>> form, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostFormAsync(new Uri(url), form, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion

        #region Post multipart

        public static IHttpResponse PostMultipart(
            this HttpClient client, string url, Dictionary<string, string> multipart,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostMultipartAsync(new Uri(url), multipart, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static IHttpResponse PostMultipart(
            this HttpClient client, Uri uri, Dictionary<string, string> multipart,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostMultipartAsync(uri, multipart, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static Task<IHttpResponse> PostMultipartAsync(
            this HttpClient client, string url, Dictionary<string, string> multipart,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostMultipartAsync(new Uri(url), multipart, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostMultipartAsync(
            this HttpClient client, Uri uri, Dictionary<string, string> multipart,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostMultipartAsync(uri, multipart, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostMultipartAsync(
            this HttpClient client, string url, Dictionary<string, string> multipart, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostMultipartAsync(new Uri(url), multipart, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion

        #region Post serialized JSON

        public static IHttpResponse PostSerializedJson(
            this HttpClient client, string url, string serializedJson,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostSerializedJsonAsync(new Uri(url), serializedJson, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static IHttpResponse PostSerializedJson(
            this HttpClient client, Uri uri, string serializedJson,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostSerializedJsonAsync(uri, serializedJson, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static Task<IHttpResponse> PostSerializedJsonAsync(
            this HttpClient client, string url, string serializedJson,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostSerializedJsonAsync(new Uri(url), serializedJson, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostSerializedJsonAsync(
            this HttpClient client, Uri uri, string serializedJson,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostSerializedJsonAsync(uri, serializedJson, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostSerializedJsonAsync(
            this HttpClient client, string url, string serializedJson, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostSerializedJsonAsync(new Uri(url), serializedJson, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion

        #endregion
    }
}
