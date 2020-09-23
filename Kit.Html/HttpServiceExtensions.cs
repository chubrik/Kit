using HtmlAgilityPack;
using Kit.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Html
{
    public static class HttpServiceExtensions
    {
        public static HtmlDocument GetHtmlDoc(
            this HttpService http, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.GetHtmlDocAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static HtmlDocument GetHtmlDoc(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            Task.Run(() => http.GetHtmlDocAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds)).Result;

        public static Task<HtmlDocument> GetHtmlDocAsync(
            this HttpService http, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetHtmlDocAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<HtmlDocument> GetHtmlDocAsync(
            this HttpService http, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetHtmlDocAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<HtmlDocument> GetHtmlDocAsync(
            this HttpService http, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            http.GetHtmlDocAsync(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static async Task<HtmlDocument> GetHtmlDocAsync(
            this HttpService http, Uri uri, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            using var response = await http.GetAsync(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);
            return await response.ReadHtmlDocAsync();
        }
    }
}
