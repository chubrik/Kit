using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kit.Http {
    public class HttpClient : IDisposable {
        
        private readonly System.Net.Http.HttpClient client;
        private CookieContainer cookieContainer = new CookieContainer();
        private string cacheDirectory = "$http-cache";
        private const string registryFileName = "$registry.txt";
        private const string infoFileSuffix = "$.txt";
        private CacheMode cacheMode = CacheMode.Disabled;
        private string cacheKey = string.Empty;
        private bool useRepeat = true;
        private static Dictionary<string, CacheInfo> registry = new Dictionary<string, CacheInfo>();
        private static int cacheCounter = 0;

        #region Consructor

        public HttpClient(
            bool? repeat = null,
            CacheMode? cache = null,
            string cacheDirectory = null,
            string cacheKey = null) {

            if (repeat != null)
                useRepeat = (bool)repeat;

            if (cache != null)
                cacheMode = (CacheMode)cache;

            if (cacheDirectory != null)
                this.cacheDirectory = cacheDirectory;

            if (cacheKey != null)
                this.cacheKey = cacheKey;

            var handler = new HttpClientHandler {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer = cookieContainer
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
        }

        public void Dispose() => client.Dispose();

        #endregion

        #region Headers

        public void SetHeader(string name, string value) {
            Debug.Assert(name != null && value != null);

            if (name == null || value == null)
                throw new InvalidOperationException();

            client.DefaultRequestHeaders.Remove(name);

            if (value != null)
                client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
        }

        public void AddToHeader(string name, string value) {
            Debug.Assert(name != null && value != null);

            if (name == null || value == null)
                throw new InvalidOperationException();

            client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
        }

        public void RemoveHeader(string name) {
            Debug.Assert(name != null);

            if (name == null)
                throw new InvalidOperationException();

            client.DefaultRequestHeaders.Remove(name);
        }

        #endregion

        #region Get
        
        public async Task<IHttpResponse> GetAsync(Uri uri, CacheMode? cache = null, string cacheKey = null, bool? repeat = null) {

            var response =
                await CacheAsync(uri, "get",
                    () => GetAsync(uri, repeat: repeat ?? useRepeat),
                    cache: cache ?? cacheMode,
                    cacheKey: cacheKey ?? this.cacheKey);

            if (response.IsHtml)
                SetHeader("Referer", uri.AbsoluteUri);

            return response;
        }

        private async Task<HttpResponse> GetAsync(Uri uri, bool repeat) {

            if (!repeat)
                return await GetBaseAsync(uri);

            HttpResponse response = null;

            await RepeatHelper.Repeat(async () => {
                response = await GetBaseAsync(uri);
            });

            return response;
        }

        private async Task<HttpResponse> GetBaseAsync(Uri uri) {
            HttpResponseMessage response;
            var requestCookies = cookieContainer.GetCookies(uri);

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

            return new HttpResponse(response, requestCookies);
        }

        #endregion

        #region Post
        
        public async Task<IHttpResponse> PostFormAsync(Uri uri, IEnumerable<KeyValuePair<string, string>> form) =>
            await PostAsync(uri, new FormUrlEncodedContent(form));

        public async Task<IHttpResponse> PostJsonAsync(Uri uri, object json) {
            var serialized = JsonConvert.SerializeObject(json);
            return await PostAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));
        }

        public async Task<IHttpResponse> PostMultipartAsync(Uri uri, Dictionary<string, string> multipart) {

            var context = new MultipartFormDataContent(
                "----WebKitFormBoundary" + DateTimeOffset.Now.Ticks.ToString("x"));

            foreach (var keyValue in multipart) {
                var value = new StringContent(keyValue.Value);
                value.Headers.Remove("Content-Type");
                value.Headers.Remove("Content-Length");
                context.Add(value, $"\"{keyValue.Key}\"");
            }

            return await PostAsync(uri, context);
        }

        private async Task<IHttpResponse> PostAsync(
            Uri uri, HttpContent content, CacheMode? cache = null, string cacheKey = null) {

            var response =
                await CacheAsync(uri, "post",
                    () => PostBaseAsync(uri, content),
                    cache: cache ?? cacheMode,
                    cacheKey: cacheKey ?? this.cacheKey);

            if (response.IsHtml)
                SetHeader("Referer", uri.AbsoluteUri);

            return response;
        }

        private async Task<HttpResponse> PostBaseAsync(Uri uri, HttpContent content) {
            HttpResponseMessage response;
            var requestCookies = cookieContainer.GetCookies(uri);

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
                RemoveHeader("Cache-Control");
                throw;
            }
            finally {
                RemoveHeader("Origin");
            }

            //todo redirect
            if (response.StatusCode == HttpStatusCode.Found)
                return await GetBaseAsync(FixRedirectUri(uri, response.Headers.Location));

            RemoveHeader("Cache-Control");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Http post status {(int)response.StatusCode}: {uri.AbsoluteUri}");

            return new HttpResponse(response, requestCookies);
        }

        #endregion

        #region Cache

        private async Task<IHttpResponse> CacheAsync(
            Uri uri, string actionName, Func<Task<HttpResponse>> httpAction, CacheMode cache, string cacheKey) {
            
            if (cache == CacheMode.Disabled)
                return await httpAction();

            var key = "(" + $"{cacheKey};{actionName}".TrimStart(';') + ")";
            var cachedName = $"{key} {uri.AbsoluteUri}";
            string paddedCount;
            string bodyFileName;

            if (cache == CacheMode.Full && registry.ContainsKey(cachedName)) {
                LogService.Log($"Http {actionName} cached: {uri.AbsoluteUri}");
                var fileInfo = registry.GetValue(cachedName);
                bodyFileName = fileInfo.BodyFileName;
                paddedCount = bodyFileName.Substring(0, 4);

                if (FileClient.Exists(bodyFileName, cacheDirectory)) //todo ... && infoFileName
                    return new CachedResponse(
                        mimeType: fileInfo.MimeType,
                        getInfo: () => FileClient.ReadLines($"{paddedCount} {infoFileSuffix}", cacheDirectory),
                        getText: () => FileClient.ReadText(bodyFileName, cacheDirectory),
                        getBytes: () => FileClient.ReadBytes(bodyFileName, cacheDirectory)
                    );
            }
            else {
                paddedCount = (++cacheCounter).ToString().PadLeft(4, '0');
                bodyFileName = $"{paddedCount} {key} {PathHelper.SafeFileName(uri.AbsoluteUri)}"; // no .ext
            }

            var response = await httpAction();
            var infoFileName = $"{paddedCount} {infoFileSuffix}";
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

        private static void FixCacheFileExtension(HttpResponse response, ref string fileName) {

            if (response.IsHtml && !fileName.EndsWith(".html"))
                fileName += ".html";

            else if (response.IsText && !fileName.EndsWith(".txt"))
                fileName += ".txt";
        }

        #endregion

        private static Uri FixRedirectUri(Uri original, Uri rawPart) =>
            new Uri(new Uri($"{original.Scheme}://{original.Host}"), rawPart);
    }
}
