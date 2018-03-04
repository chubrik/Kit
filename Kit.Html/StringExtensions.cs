using HtmlAgilityPack;

namespace Kit.Html {
    public static class StringExtensions {

        public static HtmlDocument ToHtml(this string html) {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }
    }
}
