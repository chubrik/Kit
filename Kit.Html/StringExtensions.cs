using HtmlAgilityPack;

namespace Chubrik.Kit.Html
{
    public static class StringExtensions
    {
        public static HtmlDocument ToHtmlDoc(this string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }
    }
}
