using HtmlAgilityPack;
using Kit.Http;
using System;
using System.Threading.Tasks;

namespace Kit.Html {
    public static class HttpExtensions {

        public static HtmlDocument GetHtml(
            this HttpClient client, string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetHtmlAsync(client, url, cache: cache, cacheKey: cacheKey, repeat: repeat).Result;

        public static HtmlDocument GetHtml(
            this HttpClient client, Uri uri, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetHtmlAsync(client, uri, cache: cache, cacheKey: cacheKey, repeat: repeat).Result;

        public static Task<HtmlDocument> GetHtmlAsync(
            this HttpClient client, string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetHtmlAsync(client, new Uri(url), cache: cache, cacheKey: cacheKey, repeat: repeat);

        public static async Task<HtmlDocument> GetHtmlAsync(
            this HttpClient client, Uri uri, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetHtml(await client.GetAsync(uri, cache: cache, cacheKey: cacheKey, repeat: repeat));

        public static HtmlDocument GetHtml(this IHttpResponse httpResponse) =>
            httpResponse.GetText().ToHtml();
    }
}
