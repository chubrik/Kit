using HtmlAgilityPack;
using Kit.Http;
using System;
using System.Threading.Tasks;

namespace Kit.Html
{
    public static class HttpExtensions
    {
        public static HtmlDocument GetHtmlDoc(
            this HttpClient client, string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetHtmlDocAsync(client, url, cache: cache, cacheKey: cacheKey, repeat: repeat).Result;

        public static HtmlDocument GetHtmlDoc(
            this HttpClient client, Uri uri, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetHtmlDocAsync(client, uri, cache: cache, cacheKey: cacheKey, repeat: repeat).Result;

        public static Task<HtmlDocument> GetHtmlDocAsync(
            this HttpClient client, string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetHtmlDocAsync(client, new Uri(url), cache: cache, cacheKey: cacheKey, repeat: repeat);

        public static async Task<HtmlDocument> GetHtmlDocAsync(
            this HttpClient client, Uri uri, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetHtmlDoc(await client.GetAsync(uri, cache: cache, cacheKey: cacheKey, repeat: repeat));

        public static HtmlDocument GetHtmlDoc(this IHttpResponse httpResponse) =>
            httpResponse.GetText().ToHtmlDoc();
    }
}
