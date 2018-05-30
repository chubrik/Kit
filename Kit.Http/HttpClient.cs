using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Http
{
    public class HttpClient : IDisposable
    {
        private const string RegistryFileName = "$registry.txt";
        private const string InfoFileSuffix = ".txt";

        private static string _cacheDirectory = "http-cache";
        private static CacheMode _globalCacheMode = CacheMode.Disabled;
        private static bool _globalUseRepeat = true;
        private static int _globalTimeoutSeconds = 60;
        private static int _logCounter = 0;

        private readonly System.Net.Http.HttpClient _client;
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private readonly CacheMode _cacheMode;
        private readonly string _cacheKey;
        private readonly bool _useRepeat;
        private readonly int _timeoutSeconds;

        #region Setup & Consructor

        public static void Setup(
            string cacheDirectory = null,
            CacheMode? cache = null,
            bool? repeat = null,
            int? timeoutSeconds = null)
        {
            if (cacheDirectory != null)
                _cacheDirectory = cacheDirectory;

            if (cache != null)
                _globalCacheMode = (CacheMode)cache;

            if (repeat != null)
                _globalUseRepeat = (bool)repeat;

            if (timeoutSeconds != null)
                _globalTimeoutSeconds = (int)timeoutSeconds;
        }

        public HttpClient(CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            _cacheMode = cache ?? _globalCacheMode;
            _cacheKey = cacheKey ?? string.Empty;
            _useRepeat = repeat ?? _globalUseRepeat;
            _timeoutSeconds = timeoutSeconds ?? _globalTimeoutSeconds;

            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer = _cookieContainer
                //AllowAutoRedirect = false //todo redirect
            };

            _client = new System.Net.Http.HttpClient(handler)
            {
                Timeout = Timeout.InfiniteTimeSpan
            };

            SetHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            SetHeader("Accept-Encoding", "gzip, deflate");
            SetHeader("Accept-Language", "ru,en;q=0.9");
            SetHeader("Upgrade-Insecure-Requests", "1");
            SetHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36");
        }

        public void Dispose() => _client.Dispose();

        #endregion

        #region Headers

        public void SetHeader(string name, string value)
        {
            Debug.Assert(!name.IsNullOrEmpty() && value != null);

            if (name.IsNullOrEmpty() || value == null)
                throw new InvalidOperationException();

            _client.DefaultRequestHeaders.Remove(name);
            _client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
        }

        public void AddToHeader(string name, string value)
        {
            Debug.Assert(!name.IsNullOrEmpty() && value != null);

            if (name.IsNullOrEmpty() || value == null)
                throw new InvalidOperationException();

            _client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
        }

        public void RemoveHeader(string name)
        {
            Debug.Assert(!name.IsNullOrEmpty());

            if (name.IsNullOrEmpty())
                throw new InvalidOperationException();

            _client.DefaultRequestHeaders.Remove(name);
        }

        #endregion

        #region Get

        public async Task<IHttpResponse> GetAsync(
            Uri uri, CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            var response =
                await CacheAsync(
                    uri, "get",
                    () => GetOrRepeatAsync(uri, repeat ?? _useRepeat, timeoutSeconds ?? _timeoutSeconds),
                    cache ?? _cacheMode,
                    cacheKey ?? _cacheKey);

            if (response.IsHtml)
                SetHeader("Referer", uri.AbsoluteUri);

            return response;
        }

        private async Task<HttpResponse> GetOrRepeatAsync(Uri uri, bool repeat, int timeoutSeconds)
        {
            if (!repeat)
                return await GetBaseAsync(uri, timeoutSeconds);

            HttpResponse response = null;
            await HttpHelper.RepeatAsync(async () => response = await GetBaseAsync(uri, timeoutSeconds));
            return response;
        }

        private async Task<HttpResponse> GetBaseAsync(Uri uri, int timeoutSeconds)
        {
            var logLabel = $"Http get #{++_logCounter}";
            var requestCookies = _cookieContainer.GetCookies(uri);
            var repeat12030Count = 3;
            var repeatLabelPart = string.Empty;

            Retry:
            var startTime = DateTimeOffset.Now;
            LogService.Log($"{logLabel}{repeatLabelPart}: {uri.AbsoluteUri}");
            HttpResponseMessage response = null;

            try
            {
                response = await HttpHelper.TimeoutAsync(cancellationToken =>
                    _client.GetAsync(uri, cancellationToken), timeoutSeconds);

                LogService.Log($"{logLabel} completed at {TimeHelper.FormattedLatency(startTime)}");
            }
            catch (Exception exception)
            {
                if (exception.Has12030() && --repeat12030Count > 0)
                {
                    LogService.LogWarning($"{logLabel} terminated with native HTTP error. Will repeat...");
                    ExceptionHandler.Register(exception, level: LogLevel.Warning);
                    repeatLabelPart = " (repeat)";
                    goto Retry;
                }

                Debug.Fail(exception.ToString());
                throw;
            }

            var statusCode = response.StatusCode;

            //todo redirect
            if (statusCode == HttpStatusCode.Found)
                return await GetBaseAsync(FixRedirectUri(uri, response.Headers.Location), timeoutSeconds);

            if (!response.IsSuccessStatusCode)
            {
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

        public async Task<IHttpResponse> PostFormAsync(
            Uri uri, IEnumerable<KeyValuePair<string, string>> form,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            return await PostAsync(uri, new FormUrlEncodedContent(form), cache, cacheKey, repeat, timeoutSeconds);
        }

        public async Task<IHttpResponse> PostMultipartAsync(
            Uri uri, Dictionary<string, string> multipart,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            var context = new MultipartFormDataContent(
                "----WebKitFormBoundary" + DateTimeOffset.Now.Ticks.ToString("x"));

            foreach (var keyValue in multipart)
            {
                var value = new StringContent(keyValue.Value);
                value.Headers.Remove("Content-Type");
                value.Headers.Remove("Content-Length");
                context.Add(value, $"\"{keyValue.Key}\"");
            }

            return await PostAsync(uri, context, cache, cacheKey, repeat, timeoutSeconds);
        }

        public async Task<IHttpResponse> PostSerializedJsonAsync(
            Uri uri, string serializedJson,
            CacheMode? cache = null, string cacheKey = null, bool? repeat = null, int? timeoutSeconds = null)
        {
            var content = new StringContent(serializedJson, Encoding.UTF8, "application/json");
            return await PostAsync(uri, content, cache, cacheKey, repeat, timeoutSeconds);
        }

        private async Task<IHttpResponse> PostAsync(
            Uri uri, HttpContent content, CacheMode? cache, string cacheKey, bool? repeat, int? timeoutSeconds)
        {
            var response =
                await CacheAsync(uri, "post",
                    () => PostOrRepeatAsync(uri, content, repeat ?? _useRepeat, timeoutSeconds ?? _timeoutSeconds),
                    cache ?? _cacheMode,
                    cacheKey ?? _cacheKey);

            if (response.IsHtml)
                SetHeader("Referer", uri.AbsoluteUri);

            return response;
        }

        private async Task<HttpResponse> PostOrRepeatAsync(
            Uri uri, HttpContent content, bool repeat, int timeoutSeconds)
        {
            if (!repeat)
                return await PostBaseAsync(uri, content, timeoutSeconds);

            HttpResponse response = null;
            await HttpHelper.RepeatAsync(async () => response = await PostBaseAsync(uri, content, timeoutSeconds));
            return response;
        }

        private async Task<HttpResponse> PostBaseAsync(Uri uri, HttpContent content, int timeoutSeconds)
        {
            var startTime = DateTimeOffset.Now;
            var logLabel = $"Http post #{++_logCounter}";
            LogService.Log($"{logLabel}: {uri.AbsoluteUri}");

            var requestCookies = _cookieContainer.GetCookies(uri);
            HttpResponseMessage response;

            try
            {
                SetHeader("Cache-Control", "max-age=0");
                SetHeader("Origin", $"{uri.Scheme}://{uri.Host}");

                response = await HttpHelper.TimeoutAsync(cancellationToken =>
                    _client.PostAsync(uri, content, cancellationToken), timeoutSeconds);

                LogService.Log($"{logLabel} completed at {TimeHelper.FormattedLatency(startTime)}");
            }
            catch (Exception exception)
            {
                Debug.Fail(exception.ToString());
                RemoveHeader("Cache-Control");
                throw;
            }
            finally
            {
                RemoveHeader("Origin");
            }

            //todo redirect
            if (response.StatusCode == HttpStatusCode.Found)
                return await GetBaseAsync(FixRedirectUri(uri, response.Headers.Location), timeoutSeconds);

            RemoveHeader("Cache-Control");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Http post status {(int)response.StatusCode}: {uri.AbsoluteUri}");

            return new HttpResponse(response, requestCookies);
        }

        #endregion

        #region Cache

        private static bool _isCacheInitialized;
        private static Dictionary<string, CacheInfo> _registry;
        private static int _cacheCounter = 0;

        private async Task<IHttpResponse> CacheAsync(
            Uri uri, string actionName, Func<Task<HttpResponse>> httpAction, CacheMode cache, string cacheKey)
        {
            if (cache == CacheMode.Disabled)
                return await httpAction();

            if (!_isCacheInitialized)
                CacheInitialize();

            var key = "(" + $"{cacheKey};{actionName}".TrimStart(';') + ")";
            var cachedName = $"{key} {uri.AbsoluteUri}";
            string paddedCount;
            string bodyFileName;
            var targetDirectory = PathHelper.Combine(Kit.DiagnisticsCurrentDirectory, _cacheDirectory);

            if (cache == CacheMode.Full && _registry.ContainsKey(cachedName))
            {
                LogService.Log($"Http {actionName} cached: {uri.AbsoluteUri}");
                var fileInfo = _registry.GetValue(cachedName);
                bodyFileName = fileInfo.BodyFileName;
                paddedCount = bodyFileName.Substring(0, 4);

                if (FileClient.Exists(bodyFileName, targetDirectory)) //todo ... && infoFileName
                    return new CachedResponse(
                        mimeType: fileInfo.MimeType,
                        getInfo: () => FileClient.ReadLines(paddedCount + InfoFileSuffix, targetDirectory),
                        getText: () => FileClient.ReadText(bodyFileName, targetDirectory),
                        getBytes: () => FileClient.ReadBytes(bodyFileName, targetDirectory)
                    );
            }
            else
            {
                paddedCount = (++_cacheCounter).ToString().PadLeft(4, '0');
                bodyFileName = PathHelper.SafeFileName($"{paddedCount} {key} {uri.AbsoluteUri}"); // no .ext
            }

            var response = await httpAction();
            var infoFileName = paddedCount + InfoFileSuffix;
            FixCacheFileExtension(response, ref bodyFileName);
            FileClient.Write(infoFileName, response.FormattedInfo, targetDirectory);

            if (response.IsText)
                FileClient.Write(bodyFileName, response.GetText(), targetDirectory);
            else
                FileClient.Write(bodyFileName, response.GetBytes(), targetDirectory);

            lock (RegistryFileName)
                FileClient.AppendText(RegistryFileName, $"{cachedName} | {response.MimeType} | {bodyFileName}", targetDirectory);

            _registry[cachedName] = new CacheInfo { MimeType = response.MimeType, BodyFileName = bodyFileName };
            return response;
        }

        private static void CacheInitialize()
        {
            Debug.Assert(!_isCacheInitialized);

            if (_isCacheInitialized)
                throw new InvalidOperationException();

            _isCacheInitialized = true;
            _registry = new Dictionary<string, CacheInfo>();
            var targetDirectory = PathHelper.Combine(Kit.DiagnisticsCurrentDirectory, _cacheDirectory);

            if (FileClient.Exists(RegistryFileName, targetDirectory))
            {
                var lines = FileClient.ReadLines(RegistryFileName, targetDirectory);

                foreach (var line in lines)
                {
                    var splitted = line.Split('|');

                    _registry[splitted[0].Trim()] =
                        new CacheInfo
                        {
                            MimeType = splitted[1].Trim(),
                            BodyFileName = splitted[2].Trim()
                        };
                }
            }
        }

        private static void FixCacheFileExtension(HttpResponse response, ref string fileName)
        {
            if (response.IsHtml && !fileName.EndsWith(".html"))
                fileName += ".html";

            else if (response.IsText && !fileName.EndsWith(".txt"))
                fileName += ".txt";
        }

        #endregion

        #region Utils

        private static Uri FixRedirectUri(Uri original, Uri rawPart) =>
            new Uri(new Uri($"{original.Scheme}://{original.Host}"), rawPart);

        #endregion
    }
}
