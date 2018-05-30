using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kit.Http
{
    public static class HttpExtensions
    {
        #region Get

        public static string GetText(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetTextAsync(
                url, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static byte[] GetBytes(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetBytesAsync(
                url, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static IHttpResponse Get(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetAsync(
                url, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        //

        public static string GetText(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetTextAsync(
                uri, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static byte[] GetBytes(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetBytesAsync(
                uri, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static IHttpResponse Get(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetAsync(
                uri, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        //

        public static Task<string> GetTextAsync(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetTextAsync(
                new Uri(url), cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<byte[]> GetBytesAsync(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetBytesAsync(
                new Uri(url), cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> GetAsync(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetAsync(
                new Uri(url), cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        //

        public static async Task<string> GetTextAsync(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            var response = await client.GetAsync(
                uri, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

            return response.GetText();
        }

        public static async Task<byte[]> GetBytesAsync(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            var response = await client.GetAsync(
                uri, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

            return response.GetBytes();
        }

        #endregion

        #region Post

        public static IHttpResponse PostForm(
            this HttpClient client, string url, IEnumerable<KeyValuePair<string, string>> form,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostFormAsync(
                url, form, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static IHttpResponse PostMultipart(
            this HttpClient client, string url, Dictionary<string, string> multipart,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostMultipartAsync(
                url, multipart, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static IHttpResponse PostSerializedJson(
            this HttpClient client, string url, string serializedJson,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostSerializedJsonAsync(
                url, serializedJson, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        //

        public static IHttpResponse PostForm(
            this HttpClient client, Uri uri, IEnumerable<KeyValuePair<string, string>> form,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostFormAsync(
                uri, form, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static IHttpResponse PostMultipart(
            this HttpClient client, Uri uri, Dictionary<string, string> multipart,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostMultipartAsync(
                uri, multipart, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static IHttpResponse PostSerializedJson(
            this HttpClient client, Uri uri, string serializedJson,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostSerializedJsonAsync(
                uri, serializedJson, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        //

        public static Task<IHttpResponse> PostFormAsync(
            this HttpClient client, string url, IEnumerable<KeyValuePair<string, string>> form,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostFormAsync(
                new Uri(url), form, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostMultipartAsync(
            this HttpClient client, string url, Dictionary<string, string> multipart,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostMultipartAsync(
                new Uri(url), multipart, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostSerializedJsonAsync(
            this HttpClient client, string url, string serializedJson,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.PostSerializedJsonAsync(
                new Uri(url), serializedJson, cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion
    }
}
