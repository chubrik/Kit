using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kit.Http {
    public static class HttpExtensions {

        #region Get

        public static string GetText(
            this HttpClient client, string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetTextAsync(client, url, cache: cache, cacheKey: cacheKey, repeat: repeat).Result;

        public static byte[] GetBytes(
            this HttpClient client, string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetBytesAsync(client, url, cache: cache, cacheKey: cacheKey, repeat: repeat).Result;

        public static IHttpResponse Get(
            this HttpClient client, string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetAsync(client, url, cache: cache, cacheKey: cacheKey, repeat: repeat).Result;

        //

        public static string GetText(
            this HttpClient client, Uri uri, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetTextAsync(client, uri, cache: cache, cacheKey: cacheKey, repeat: repeat).Result;

        public static byte[] GetBytes(
            this HttpClient client, Uri uri, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetBytesAsync(client, uri, cache: cache, cacheKey: cacheKey, repeat: repeat).Result;

        public static IHttpResponse Get(
            this HttpClient client, Uri uri, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            client.GetAsync(uri, cache: cache, cacheKey: cacheKey, repeat: repeat).Result;

        //

        public static Task<string> GetTextAsync(
            this HttpClient client, string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetTextAsync(client, new Uri(url), cache: cache, cacheKey: cacheKey, repeat: repeat);

        public static Task<byte[]> GetBytesAsync(
            this HttpClient client, string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetBytesAsync(client, new Uri(url), cache: cache, cacheKey: cacheKey, repeat: repeat);

        public static Task<IHttpResponse> GetAsync(
            this HttpClient client, string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            client.GetAsync(new Uri(url), cache: cache, cacheKey: cacheKey, repeat: repeat);

        //

        public static async Task<string> GetTextAsync(
            this HttpClient client, Uri uri, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) {
            var response = await client.GetAsync(uri, cache: cache, cacheKey: cacheKey, repeat: repeat);
            return response.GetText();
        }
        
        public static async Task<byte[]> GetBytesAsync(
            this HttpClient client, Uri uri, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) {
            var response = await client.GetAsync(uri, cache: cache, cacheKey: cacheKey, repeat: repeat);
            return response.GetBytes();
        }
        
        #endregion

        #region Post

        public static IHttpResponse PostForm(
            this HttpClient client, string url, IEnumerable<KeyValuePair<string, string>> form) =>
            PostFormAsync(client, url, form).Result;

        public static IHttpResponse PostJson(this HttpClient client, string url, object json) =>
            PostJsonAsync(client, url, json).Result;

        public static IHttpResponse PostMultipart(
            this HttpClient client, string url, Dictionary<string, string> multipart) =>
            PostMultipartAsync(client, url, multipart).Result;

        //

        public static IHttpResponse PostForm(
            this HttpClient client, Uri uri, IEnumerable<KeyValuePair<string, string>> form) =>
            client.PostFormAsync(uri, form).Result;

        public static IHttpResponse PostJson(this HttpClient client, Uri uri, object json) =>
            client.PostJsonAsync(uri, json).Result;

        public static IHttpResponse PostMultipart(
            this HttpClient client, Uri uri, Dictionary<string, string> multipart) =>
            client.PostMultipartAsync(uri, multipart).Result;

        //

        public static Task<IHttpResponse> PostFormAsync(
            this HttpClient client, string url, IEnumerable<KeyValuePair<string, string>> form) =>
            client.PostFormAsync(new Uri(url), form);

        public static Task<IHttpResponse> PostJsonAsync(this HttpClient client, string url, object json) =>
            client.PostJsonAsync(new Uri(url), json);

        public static Task<IHttpResponse> PostMultipartAsync(
            this HttpClient client, string url, Dictionary<string, string> multipart) =>
            client.PostMultipartAsync(new Uri(url), multipart);

        #endregion
    }
}
