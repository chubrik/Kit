using System;
using System.Collections.Generic;
using System.IO;
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
            Task.Run(() => client.GetTextAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static string GetText(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => client.GetTextAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

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
            using (var response = await client.GetAsync(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds))
                return await response.GetTextAsync();
        }

        #endregion

        #region Get bytes

        public static byte[] GetBytes(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => client.GetBytesAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static byte[] GetBytes(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => client.GetBytesAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

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
            using (var response = await client.GetAsync(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds))
                return await response.GetBytesAsync();
        }

        #endregion

        #region Get stream

        public static Stream GetStream(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => client.GetStreamAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Stream GetStream(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => client.GetStreamAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<Stream> GetStreamAsync(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetStreamAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<Stream> GetStreamAsync(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetStreamAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<Stream> GetStreamAsync(
            this HttpClient client, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetStreamAsync(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static async Task<Stream> GetStreamAsync(
            this HttpClient client, Uri uri, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            using (var response = await client.GetAsync(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds))
                return await response.GetStreamAsync();
        }

        #endregion

        #region Get response

        public static IHttpResponse Get(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => client.GetAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static IHttpResponse Get(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => client.GetAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

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
            Task.Run(() => client.PostFormAsync(new Uri(url), form, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static IHttpResponse PostForm(
            this HttpClient client, Uri uri, IEnumerable<KeyValuePair<string, string>> form,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => client.PostFormAsync(uri, form, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

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
            Task.Run(() => client.PostMultipartAsync(new Uri(url), multipart, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static IHttpResponse PostMultipart(
            this HttpClient client, Uri uri, Dictionary<string, string> multipart,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => client.PostMultipartAsync(uri, multipart, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

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
            Task.Run(() => client.PostSerializedJsonAsync(new Uri(url), serializedJson, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static IHttpResponse PostSerializedJson(
            this HttpClient client, Uri uri, string serializedJson,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => client.PostSerializedJsonAsync(uri, serializedJson, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

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
