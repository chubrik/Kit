using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Http {
    public class HttpClient {

        private static HttpClient instance;
        public static HttpClient Instance => instance ?? (instance = new HttpClient());
        private HttpClient() { }

        private static bool isInitialized = false;
        private static System.Net.Http.HttpClient client; //todo dispose
        private static bool useRepeat = true;
        private static bool useCache = true;
        private static string cacheDirectory = "$http-cache";
        private static string cacheKey = string.Empty;
        private static int cacheCounter = 0;
        private const string registryFileName = "$registry.txt";
        private static Dictionary<string, string> registry = new Dictionary<string, string>();

        public static void Setup(
            bool? useRepeat = null,
            bool? useCache = null,
            string cacheDirectory = null,
            string cacheKey = null) {

            if (useRepeat != null)
                HttpClient.useRepeat = (bool)useRepeat;

            if (useCache != null)
                HttpClient.useCache = (bool)useCache;

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
                    registry.Add(splitted[0].Trim(), splitted[1].Trim());
                }
            }

            isInitialized = true;
        }

        public static void SetHeader(string name, string value) {
            client.DefaultRequestHeaders.Remove(name);

            if (value != null)
                client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
        }

        public static Task<string> GetAsync(
            string url,
            CancellationToken cancellationToken,
            bool? useRepeat = null,
            bool? useCache = null,
            string cacheKey = null) =>
            GetAsync(new Uri(url), cancellationToken, useRepeat: useRepeat, useCache: useCache, cacheKey: cacheKey);

        public static async Task<string> GetAsync(
            Uri uri,
            CancellationToken cancellationToken,
            bool? useRepeat = null,
            bool? useCache = null,
            string cacheKey = null) {

            if (!isInitialized)
                Initialize();

            HttpResponse response;

            if (!(useCache ?? HttpClient.useCache)) {
                response = await GetOrRepeatAsync(uri, useRepeat ?? HttpClient.useRepeat, cancellationToken);
                return response.Body;
            }

            var key = "(" + $"{cacheKey ?? HttpClient.cacheKey};get".TrimStart(';') + ")";
            var cachedName = $"{key} {uri.AbsoluteUri}";
            string paddedCount;
            string bodyFileName;

            if (registry.ContainsKey(cachedName)) {
                bodyFileName = registry.GetValue(cachedName);

                if (FileClient.Exists(bodyFileName, cacheDirectory)) {
                    LogService.Log($"Http get (cached): {uri.AbsoluteUri}");
                    return FileClient.ReadText(bodyFileName, cacheDirectory);
                }
                else
                    paddedCount = bodyFileName.Substring(0, 4);
            }
            else {
                paddedCount = (++cacheCounter).ToString().PadLeft(4, '0');
                bodyFileName = $"{paddedCount} {key} {PathHelper.SafeFileName(uri.AbsoluteUri)}.txt";
            }

            response = await GetOrRepeatAsync(uri, useRepeat ?? HttpClient.useRepeat, cancellationToken);

            if (response.Body == null)
                return null;

            var infoFileName = $"{paddedCount} {key} info.txt";
            FileClient.Write(bodyFileName, response.Body, cacheDirectory);
            FileClient.Write(infoFileName, response.FormattedInfo, cacheDirectory);

            if (!registry.ContainsKey(cachedName))
                FileClient.AppendText(registryFileName, $"{cachedName} | {bodyFileName}", cacheDirectory);

            SetHeader("Referer", uri.AbsoluteUri);
            return response.Body;
        }

        private static async Task<HttpResponse> GetOrRepeatAsync(
            Uri uri, bool useRepeat, CancellationToken cancellationToken) {

            if (!useRepeat)
                return await GetBaseAsync(uri, cancellationToken);

            HttpResponse response = null;

            await RepeatHelper.Repeat(async () => {
                response = await GetBaseAsync(uri, cancellationToken);
            }, cancellationToken);

            return response;
        }

        private static async Task<HttpResponse> GetBaseAsync(Uri uri, CancellationToken cancellationToken) {
            LogService.Log($"Http get: {uri.AbsoluteUri}");
            var response = await client.GetAsync(uri, cancellationToken);
            var statusCode = response.StatusCode;

            //todo redirect
            if (statusCode == HttpStatusCode.Found)
                return await GetBaseAsync(FixUri(uri, response.Headers.Location), cancellationToken);

            if (response.IsSuccessStatusCode)
                return new HttpResponse(response, await response.Content.ReadAsStringAsync());

            var message = $"Status {statusCode}: {uri.AbsoluteUri}";
            Debug.Assert(statusCode == HttpStatusCode.NotFound);

            if (statusCode != HttpStatusCode.NotFound)
                throw new InvalidOperationException(message);

            LogService.LogWarning(message);
            return new HttpResponse(response, body: null);
        }

        public Task<string> PostFormAsync(
            string url, IEnumerable<KeyValuePair<string, string>> data, CancellationToken cancellationToken) =>
            PostFormAsync(new Uri(url), data, cancellationToken);

        public Task<string> PostFormAsync(
            Uri uri, IEnumerable<KeyValuePair<string, string>> data, CancellationToken cancellationToken) =>
            PostBaseAsync(uri, new FormUrlEncodedContent(data), cancellationToken);

        public Task<string> PostJsonAsync(string url, object json, CancellationToken cancellationToken) =>
            PostJsonAsync(new Uri(url), json, cancellationToken);

        public Task<string> PostJsonAsync(Uri uri, object json, CancellationToken cancellationToken) {
            var serialized = JsonConvert.SerializeObject(json);

            return PostBaseAsync(
                uri, new StringContent(serialized, Encoding.UTF8, "application/json"), cancellationToken);
        }

        public Task<string> PostMultipartAsync(
            string url, Dictionary<string, string> data, CancellationToken cancellationToken) =>
            PostMultipartAsync(new Uri(url), data, cancellationToken);

        public Task<string> PostMultipartAsync(
            Uri uri, Dictionary<string, string> data, CancellationToken cancellationToken) {

            var context = new MultipartFormDataContent("----WebKitFormBoundaryfiPAGQ2wTpqYFEHb"); //todo

            foreach (var keyValue in data) {
                var value = new StringContent(keyValue.Value);
                value.Headers.Remove("Content-Type");
                value.Headers.Remove("Content-Length");
                context.Add(value, $"\"{keyValue.Key}\"");
            }

            return PostBaseAsync(uri, context, cancellationToken);
        }

        private async Task<string> PostBaseAsync(
            Uri uri, HttpContent content, CancellationToken cancellationToken) {

            HttpResponseMessage response;

            try {
                SetHeader("Cache-Control", "max-age=0");
                SetHeader("Origin", $"{uri.Scheme}://{uri.Host}");
                response = await client.PostAsync(uri, content, cancellationToken);
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
                return await GetAsync(FixUri(uri, response.Headers.Location), cancellationToken);

            SetHeader("Cache-Control", null);

            string body = null;

            if (response.IsSuccessStatusCode)
                body = await response.Content.ReadAsStringAsync();

            Debug.Assert(body != null);

            if (body == null)
                throw new InvalidOperationException();

            return body;
        }

        private static Uri FixUri(Uri original, Uri rawPart) =>
            new Uri(new Uri($"{original.Scheme}://{original.Host}"), rawPart);
    }
}
