using HtmlAgilityPack;
using Kit.Http;
using System.Threading.Tasks;

namespace Kit.Html
{
    public static class HttpResponseExtensions
    {
        public static HtmlDocument ReadHtmlDoc(this IHttpResponse response) =>
            response.ReadText().ToHtmlDoc();

        public static async Task<HtmlDocument> ReadHtmlDocAsync(this IHttpResponse response) =>
            (await response.ReadTextAsync()).ToHtmlDoc();
    }
}
