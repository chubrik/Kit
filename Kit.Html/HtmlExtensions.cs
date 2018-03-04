using HtmlAgilityPack;

namespace Kit.Html {
    public static class HtmlExtensions {

        public static string InnerHtml(this HtmlDocument document) =>
            document.DocumentNode.InnerHtml;

        public static string InnerText(this HtmlDocument document) =>
            document.DocumentNode.InnerText;

        public static HtmlNodeCollection SelectNodes(this HtmlDocument document, string xpath) =>
            document.DocumentNode.SelectNodes(xpath);

        public static HtmlNode SelectSingleNode(this HtmlDocument document, string xpath) =>
            document.DocumentNode.SelectSingleNode(xpath);
    }
}
