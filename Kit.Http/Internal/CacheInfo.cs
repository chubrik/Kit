namespace Kit.Http
{
    internal class CacheInfo
    {
        public string MimeType { get; }

        public string BodyFileName { get; }

        public CacheInfo(string mimeType, string bodyFileName)
        {
            MimeType = mimeType;
            BodyFileName = bodyFileName;
        }
    }
}
