using HtmlAgilityPack;
using Kit.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Html
{
    public static class HttpExtensions
    {
        public static HtmlDocument GetHtmlDoc(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetHtmlDocAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static HtmlDocument GetHtmlDoc(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetHtmlDocAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds).Result;

        public static Task<HtmlDocument> GetHtmlDocAsync(
            this HttpClient client, string url,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetHtmlDocAsync(new Uri(url), Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<HtmlDocument> GetHtmlDocAsync(
            this HttpClient client, Uri uri,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetHtmlDocAsync(uri, Kit.CancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static Task<HtmlDocument> GetHtmlDocAsync(
            this HttpClient client, string url, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null) =>
            client.GetHtmlDocAsync(new Uri(url), cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds);

        public static async Task<HtmlDocument> GetHtmlDocAsync(
            this HttpClient client, Uri uri, CancellationToken cancellationToken,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            using (var response = await client.GetAsync(uri, cancellationToken,
                cache: cache, cacheKey: cacheKey, repeat: repeat, timeoutSeconds: timeoutSeconds))
                return await response.GetHtmlDocAsync();
        }

        public static HtmlDocument GetHtmlDoc(this IHttpResponse httpResponse) =>
            httpResponse.GetText().ToHtmlDoc();

        public static async Task<HtmlDocument> GetHtmlDocAsync(this IHttpResponse httpResponse) =>
            (await httpResponse.GetTextAsync()).ToHtmlDoc();
    }
}
