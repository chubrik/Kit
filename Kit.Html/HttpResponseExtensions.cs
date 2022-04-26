using Chubrik.Kit.Http;
using HtmlAgilityPack;
using System.Threading.Tasks;

namespace Chubrik.Kit.Html
{
    public static class HttpResponseExtensions
    {
        public static HtmlDocument ReadHtmlDoc(this IHttpResponse response) =>
            response.ReadText().ToHtmlDoc();

        public static async Task<HtmlDocument> ReadHtmlDocAsync(this IHttpResponse response) =>
            (await response.ReadTextAsync()).ToHtmlDoc();
    }
}
