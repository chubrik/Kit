﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Http
{
    public static class HttpServiceExtensions
    {
        #region Get text

        public static string GetText(
            this HttpService http, string url,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.GetTextAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static string GetText(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.GetTextAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<string> GetTextAsync(
            this HttpService http, string url,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetTextAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<string> GetTextAsync(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetTextAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<string> GetTextAsync(
            this HttpService http, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetTextAsync(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static async Task<string> GetTextAsync(
            this HttpService http, Uri uri, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            using var response = await http.GetAsync(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);
            return await response.ReadTextAsync();
        }

        #endregion

        #region Get bytes

        public static byte[] GetBytes(
            this HttpService http, string url,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.GetBytesAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static byte[] GetBytes(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.GetBytesAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<byte[]> GetBytesAsync(
            this HttpService http, string url,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetBytesAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<byte[]> GetBytesAsync(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetBytesAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<byte[]> GetBytesAsync(
            this HttpService http, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetBytesAsync(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static async Task<byte[]> GetBytesAsync(
            this HttpService http, Uri uri, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            using var response = await http.GetAsync(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);
            return await response.ReadBytesAsync();
        }

        #endregion

        #region Get JSON dynamic

        public static dynamic? GetJson(
            this HttpService http, string url,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.GetJsonAsync<object>(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static dynamic? GetJson(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.GetJsonAsync<object>(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<dynamic?> GetJsonAsync(
            this HttpService http, string url,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetJsonAsync<object>(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<dynamic?> GetJsonAsync(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetJsonAsync<object>(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<dynamic?> GetJsonAsync(
            this HttpService http, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetJsonAsync<object>(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<dynamic?> GetJsonAsync(
            this HttpService http, Uri uri, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetJsonAsync<object>(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion

        #region Get JSON generic

        public static T? GetJson<T>(
            this HttpService http, string url,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            Task.Run(() => http.GetJsonAsync<T>(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static T? GetJson<T>(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            Task.Run(() => http.GetJsonAsync<T>(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<T?> GetJsonAsync<T>(
            this HttpService http, string url,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            http.GetJsonAsync<T>(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<T?> GetJsonAsync<T>(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            http.GetJsonAsync<T>(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<T?> GetJsonAsync<T>(
            this HttpService http, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class =>
            http.GetJsonAsync<T>(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static async Task<T?> GetJsonAsync<T>(
            this HttpService http, Uri uri, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
            where T : class
        {
            using var stream = await http.GetStreamAsync(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);
            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            return new JsonSerializer().Deserialize<T>(jsonTextReader);
        }

        #endregion

        #region Get stream

        public static Stream GetStream(
            this HttpService http, string url,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.GetStreamAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Stream GetStream(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.GetStreamAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<Stream> GetStreamAsync(
            this HttpService http, string url,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetStreamAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<Stream> GetStreamAsync(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetStreamAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<Stream> GetStreamAsync(
            this HttpService http, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetStreamAsync(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static async Task<Stream> GetStreamAsync(
            this HttpService http, Uri uri, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            using var response = await http.GetAsync(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);
            return await response.ReadStreamAsync();
        }

        #endregion

        #region Get response

        public static IHttpResponse Get(
            this HttpService http, string url,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.GetAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static IHttpResponse Get(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.GetAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<IHttpResponse> GetAsync(
            this HttpService http, string url,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> GetAsync(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> GetAsync(
            this HttpService http, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetAsync(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion

        #region Post form

        public static IHttpResponse PostForm(
            this HttpService http, string url, IEnumerable<KeyValuePair<string, string>> form,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.PostFormAsync(new Uri(url), form, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static IHttpResponse PostForm(
            this HttpService http, Uri uri, IEnumerable<KeyValuePair<string, string>> form,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.PostFormAsync(uri, form, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<IHttpResponse> PostFormAsync(
            this HttpService http, string url, IEnumerable<KeyValuePair<string, string>> form,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostFormAsync(new Uri(url), form, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostFormAsync(
            this HttpService http, Uri uri, IEnumerable<KeyValuePair<string, string>> form,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostFormAsync(uri, form, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostFormAsync(
            this HttpService http, string url, IEnumerable<KeyValuePair<string, string>> form, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostFormAsync(new Uri(url), form, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion

        #region Post multipart

        public static IHttpResponse PostMultipart(
            this HttpService http, string url, Dictionary<string, string> multipart,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.PostMultipartAsync(new Uri(url), multipart, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static IHttpResponse PostMultipart(
            this HttpService http, Uri uri, Dictionary<string, string> multipart,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.PostMultipartAsync(uri, multipart, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<IHttpResponse> PostMultipartAsync(
            this HttpService http, string url, Dictionary<string, string> multipart,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostMultipartAsync(new Uri(url), multipart, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostMultipartAsync(
            this HttpService http, Uri uri, Dictionary<string, string> multipart,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostMultipartAsync(uri, multipart, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostMultipartAsync(
            this HttpService http, string url, Dictionary<string, string> multipart, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostMultipartAsync(new Uri(url), multipart, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion

        #region Post serialized JSON

        public static IHttpResponse PostSerializedJson(
            this HttpService http, string url, string serializedJson,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.PostSerializedJsonAsync(new Uri(url), serializedJson, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static IHttpResponse PostSerializedJson(
            this HttpService http, Uri uri, string serializedJson,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.PostSerializedJsonAsync(uri, serializedJson, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<IHttpResponse> PostSerializedJsonAsync(
            this HttpService http, string url, string serializedJson,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostSerializedJsonAsync(new Uri(url), serializedJson, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostSerializedJsonAsync(
            this HttpService http, Uri uri, string serializedJson,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostSerializedJsonAsync(uri, serializedJson, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostSerializedJsonAsync(
            this HttpService http, string url, string serializedJson, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostSerializedJsonAsync(new Uri(url), serializedJson, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion

        #region Post bytes

        public static IHttpResponse PostBytes(
            this HttpService http, string url, byte[] bytes,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.PostBytesAsync(new Uri(url), bytes, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static IHttpResponse PostBytes(
            this HttpService http, Uri uri, byte[] bytes,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.PostBytesAsync(uri, bytes, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<IHttpResponse> PostBytesAsync(
            this HttpService http, string url, byte[] bytes,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostBytesAsync(new Uri(url), bytes, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostBytesAsync(
            this HttpService http, Uri uri, byte[] bytes,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostBytesAsync(uri, bytes, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostBytesAsync(
            this HttpService http, string url, byte[] bytes, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostBytesAsync(new Uri(url), bytes, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion

        #region Post JSON

        public static IHttpResponse PostJson(
            this HttpService http, string url, object json,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.PostJsonAsync(new Uri(url), json, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static IHttpResponse PostJson(
            this HttpService http, Uri uri, object json,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.PostJsonAsync(uri, json, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<IHttpResponse> PostJsonAsync(
            this HttpService http, string url, object json,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostJsonAsync(new Uri(url), json, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostJsonAsync(
            this HttpService http, Uri uri, object json,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostJsonAsync(uri, json, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostJsonAsync(
            this HttpService http, string url, object json, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostJsonAsync(new Uri(url), json, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<IHttpResponse> PostJsonAsync(
            this HttpService http, Uri uri, object json, CancellationToken cancellationToken,
            CacheMode? cache = null, string? cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.PostSerializedJsonAsync(uri, JsonConvert.SerializeObject(json), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        #endregion

        #region Cookies

        public static void SetCookie(this HttpService http, string url, string name, string value) =>
            http.SetCookie(new Uri(url), name, value);

        public static void RemoveCookie(this HttpService http, string url, string name) =>
            http.RemoveCookie(new Uri(url), name);

        #endregion
    }
}
