using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kit.Http {
    public class HttpClient {

        private static HttpClient instance;
        public static HttpClient Instance => instance ?? (instance = new HttpClient());
        private HttpClient() { }

        private static bool isInitialized = false;
        private static System.Net.Http.HttpClient client; //todo dispose
        private static string cacheDirectory = "$http-cache";
        private const string registryFileName = "$registry.txt";
        private static CacheMode cacheMode = CacheMode.Disabled;
        private static string cacheKey = string.Empty;
        private static bool useRepeat = true;
        private static int cacheCounter = 0;
        private static Dictionary<string, CacheInfo> registry = new Dictionary<string, CacheInfo>();

        #region Setup & Initialize

        public static void Setup(
            bool? repeat = null,
            CacheMode? cache = null,
            string cacheDirectory = null,
            string cacheKey = null) {

            if (repeat != null)
                useRepeat = (bool)repeat;

            if (cache != null)
                cacheMode = (CacheMode)cache;

            if (cacheDirectory != null)
                HttpClient.cacheDirectory = cacheDirectory;

            if (cacheKey != null)
                HttpClient.cacheKey = cacheKey;
        }

        private static void Initialize() {
            Debug.Assert(!isInitialized);

            if (isInitialized)
                throw new InvalidOperationException();

            var handler = new HttpClientHandler {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                //AllowAutoRedirect = false //todo redirect
            };

            client = new System.Net.Http.HttpClient(handler);
            SetHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            SetHeader("Accept-Encoding", "gzip, deflate");
            SetHeader("Accept-Language", "ru,en;q=0.9");
            SetHeader("Upgrade-Insecure-Requests", "1");
            SetHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36");

            if (FileClient.Exists(registryFileName, cacheDirectory)) {
                var lines = FileClient.ReadLines(registryFileName, cacheDirectory);

                foreach (var line in lines) {
                    var splitted = line.Split('|');

                    registry[splitted[0].Trim()] =
                        new CacheInfo {
                            MimeType = splitted[1].Trim(),
                            BodyFileName = splitted[2].Trim()
                        };
                }
            }

            isInitialized = true;
        }

        #endregion

        public static void SetHeader(string name, string value) {
            client.DefaultRequestHeaders.Remove(name);

            if (value != null)
                client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
        }

        #region Get

        #region Extensions

        public static string GetText(string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetTextAsync(url, cache: cache, cacheKey: cacheKey, repeat: repeat).Result;

        public static byte[] GetBytes(string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetBytesAsync(new Uri(url), cache: cache, cacheKey: cacheKey, repeat: repeat).Result;

        public static IHttpResponse Get(string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetAsync(new Uri(url), cache: cache, cacheKey: cacheKey, repeat: repeat).Result;

        //

        public static Task<string> GetTextAsync(string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetTextAsync(new Uri(url), cache: cache, cacheKey: cacheKey, repeat: repeat);

        public static async Task<string> GetTextAsync(Uri uri, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) {
            var response = await GetAsync(uri, cache: cache, cacheKey: cacheKey, repeat: repeat);
            return response.GetText();
        }

        public static Task<byte[]> GetBytesAsync(string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetBytesAsync(new Uri(url), cache: cache, cacheKey: cacheKey, repeat: repeat);

        //

        public static async Task<byte[]> GetBytesAsync(Uri uri, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) {
            var response = await GetAsync(uri, cache: cache, cacheKey: cacheKey, repeat: repeat);
            return response.GetBytes();
        }

        public static Task<IHttpResponse> GetAsync(string url, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) =>
            GetAsync(new Uri(url), cache: cache, cacheKey: cacheKey, repeat: repeat);

        #endregion

        public static async Task<IHttpResponse> GetAsync(Uri uri, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) {

            var response =
                await GetAsync(uri,
                    cache: cache ?? cacheMode,
                    cacheKey: cacheKey ?? HttpClient.cacheKey,
                    repeat: repeat ?? useRepeat);

            if (response.IsHtml)
                SetHeader("Referer", uri.AbsoluteUri);

            return response;
        }

        private static async Task<IHttpResponse> GetAsync(Uri uri, CacheMode cache, string cacheKey, bool repeat) {

            if (!isInitialized)
                Initialize();

            if (cache == CacheMode.Disabled)
                return await GetAsync(uri, repeat);

            var key = "(" + $"{cacheKey};get".TrimStart(';') + ")";
            var cachedName = $"{key} {uri.AbsoluteUri}";
            string paddedCount;
            string bodyFileName;

            if (cache == CacheMode.Full && registry.ContainsKey(cachedName)) {
                LogService.Log($"Http get cached: {uri.AbsoluteUri}");
                var fileInfo = registry.GetValue(cachedName);
                bodyFileName = fileInfo.BodyFileName;
                paddedCount = bodyFileName.Substring(0, 4);

                if (FileClient.Exists(bodyFileName, cacheDirectory)) //todo ... && infoFileName
                    return new CachedResponse(
                        mimeType: fileInfo.MimeType,
                        getInfo: () => FileClient.ReadLines($"{paddedCount} {key} info.txt", cacheDirectory),
                        getText: () => FileClient.ReadText(bodyFileName, cacheDirectory),
                        getBytes: () => FileClient.ReadBytes(bodyFileName, cacheDirectory)
                    );
            }
            else {
                paddedCount = (++cacheCounter).ToString().PadLeft(4, '0');
                bodyFileName = $"{paddedCount} {key} {PathHelper.SafeFileName(uri.AbsoluteUri)}"; // no .ext
            }

            var response = await GetAsync(uri, repeat);
            var infoFileName = $"{paddedCount} {key} info.txt";
            FixCacheFileExtension(response, ref bodyFileName);
            FileClient.Write(infoFileName, response.FormattedInfo, cacheDirectory);

            if (response.IsText)
                FileClient.Write(bodyFileName, response.GetText(), cacheDirectory);
            else
                FileClient.Write(bodyFileName, response.GetBytes(), cacheDirectory);

            FileClient.AppendText(registryFileName, $"{cachedName} | {response.MimeType} | {bodyFileName}", cacheDirectory);
            registry[cachedName] = new CacheInfo { MimeType = response.MimeType, BodyFileName = bodyFileName };
            return response;
        }

        private static async Task<HttpResponse> GetAsync(Uri uri, bool repeat) {

            if (!repeat)
                return await GetBaseAsync(uri);

            HttpResponse response = null;

            await RepeatHelper.Repeat(async () => {
                response = await GetBaseAsync(uri);
            });

            return response;
        }

        private static async Task<HttpResponse> GetBaseAsync(Uri uri) {
            HttpResponseMessage response;

            try {
                var startTime = DateTimeOffset.Now;
                LogService.Log($"Http get started: {uri.AbsoluteUri}");
                response = await client.GetAsync(uri, Kit.CancellationToken);
                LogService.Log($"Http get completed at {TimeHelper.FormattedLatency(startTime)}");
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                var newException = new Exception($"Http stream exception: {uri.AbsoluteUri}", exception);
                ExceptionHandler.Register(newException, level: LogLevel.Warning);
                throw newException;
            }

            var statusCode = response.StatusCode;

            //todo redirect
            if (statusCode == HttpStatusCode.Found)
                return await GetBaseAsync(FixRedirectUri(uri, response.Headers.Location));

            if (!response.IsSuccessStatusCode) {
                Debug.Assert(statusCode == HttpStatusCode.NotFound);
                var message = $"Http get status {(int)statusCode}: {uri.AbsoluteUri}";

                if (statusCode != HttpStatusCode.NotFound)
                    throw new Exception(message);

                LogService.LogWarning(message);
            }

            return new HttpResponse(response);
        }

        #endregion

        #region Post

        #region Extensions

        public static IHttpResponse PostForm(string url, IEnumerable<KeyValuePair<string, string>> form) =>
            PostFormAsync(url, form).Result;

        public static IHttpResponse PostJson(string url, object json) =>
            PostJsonAsync(url, json).Result;

        public static IHttpResponse PostMultipart(string url, Dictionary<string, string> multipart) =>
            PostMultipartAsync(url, multipart).Result;

        //

        public static Task<IHttpResponse> PostFormAsync(string url, IEnumerable<KeyValuePair<string, string>> form) =>
            PostFormAsync(new Uri(url), form);

        public static Task<IHttpResponse> PostJsonAsync(string url, object json) =>
            PostJsonAsync(new Uri(url), json);

        public static Task<IHttpResponse> PostMultipartAsync(string url, Dictionary<string, string> multipart) =>
            PostMultipartAsync(new Uri(url), multipart);

        #endregion

        public static async Task<IHttpResponse> PostFormAsync(Uri uri, IEnumerable<KeyValuePair<string, string>> form) =>
            await PostBaseAsync(uri, new FormUrlEncodedContent(form));

        public static async Task<IHttpResponse> PostJsonAsync(Uri uri, object json) {
            var serialized = JsonConvert.SerializeObject(json);
            return await PostBaseAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));
        }

        public static async Task<IHttpResponse> PostMultipartAsync(Uri uri, Dictionary<string, string> multipart) {

            var context = new MultipartFormDataContent(
                "----WebKitFormBoundary" + DateTimeOffset.Now.Ticks.ToString("x"));

            foreach (var keyValue in multipart) {
                var value = new StringContent(keyValue.Value);
                value.Headers.Remove("Content-Type");
                value.Headers.Remove("Content-Length");
                context.Add(value, $"\"{keyValue.Key}\"");
            }

            return await PostBaseAsync(uri, context);
        }

        private static async Task<HttpResponse> PostBaseAsync(Uri uri, HttpContent content) {
            HttpResponseMessage response;

            try {
                var startTime = DateTimeOffset.Now;
                LogService.Log($"Http post started: {uri.AbsoluteUri}");
                SetHeader("Cache-Control", "max-age=0");
                SetHeader("Origin", $"{uri.Scheme}://{uri.Host}");
                response = await client.PostAsync(uri, content, Kit.CancellationToken);
                LogService.Log($"Http post completed at {TimeHelper.FormattedLatency(startTime)}");
            }
            catch (Exception exception) {
                Debug.Fail(exception.ToString());
                ExceptionHandler.Register(exception);
                SetHeader("Cache-Control", null);
                throw;
            }
            finally {
                SetHeader("Origin", null);
            }

            //todo redirect
            if (response.StatusCode == HttpStatusCode.Found)
                return await GetBaseAsync(FixRedirectUri(uri, response.Headers.Location));

            SetHeader("Cache-Control", null);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Http post status {(int)response.StatusCode}: {uri.AbsoluteUri}");

            return new HttpResponse(response);
        }

        #endregion

        private static Uri FixRedirectUri(Uri original, Uri rawPart) =>
            new Uri(new Uri($"{original.Scheme}://{original.Host}"), rawPart);

        private static void FixCacheFileExtension(HttpResponse response, ref string fileName) {

            if (response.IsHtml && !fileName.EndsWith(".html"))
                fileName += ".html";

            else if (response.IsText && !fileName.EndsWith(".txt"))
                fileName += ".txt";
        }
    }
}
