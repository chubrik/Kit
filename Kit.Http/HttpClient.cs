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
        private static System.Net.Http.HttpClient client;
        private static string cacheDirectory = "$http-cache";
        private static string cacheKey = string.Empty;
        private static int cacheCounter = 0;
        private const string registryFileName = "$registry.txt";
        private static Dictionary<string, string> registry = new Dictionary<string, string>();

        public static void Setup(string cacheDirectory = null, string cacheKey = null) {

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
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = false
            };

            client = new System.Net.Http.HttpClient(handler);
            SetHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            SetHeader("Accept-Encoding", "gzip, deflate, br");
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
            string url, CancellationToken cancellationToken, string cacheKey = null) =>
            GetAsync(new Uri(url), cancellationToken, cacheKey);

        public static async Task<string> GetAsync(
            Uri uri, CancellationToken cancellationToken, string cacheKey = null) {

            if (!isInitialized)
                Initialize();

            var url = uri.AbsoluteUri;
            LogService.Log($"Http get: {url}");
            var cachedName = GetCachedName(url, $"{cacheKey ?? HttpClient.cacheKey};get");
            string cachedFileName;

            if (registry.ContainsKey(cachedName)) {
                cachedFileName = registry.GetValue(cachedName);

                if (FileClient.Exists(cachedFileName, cacheDirectory))
                    return FileClient.ReadText(cachedFileName, cacheDirectory);
            }
            else {
                var count = (++cacheCounter).ToString().PadLeft(4, '0');
                cachedFileName = PathHelper.SafeFileName($"{count} {cachedName}.txt");
            }

            string body = null;

            await RepeatHelper.Repeat(async () => {
                var response = await client.GetAsync(uri, cancellationToken);
                var statusCode = response.StatusCode;

                if (statusCode == HttpStatusCode.Found) {
                    body = await GetAsync(FixUri(uri, response.Headers.Location), cancellationToken);
                    return;
                }

                if (response.IsSuccessStatusCode) {
                    body = await response.Content.ReadAsStringAsync();
                    return;
                }

                var message = $"Status {statusCode}: {url}";
                Debug.Assert(statusCode == HttpStatusCode.NotFound);

                if (statusCode == HttpStatusCode.NotFound)
                    LogService.LogWarning(message);
                else
                    throw new InvalidOperationException(message);

            }, cancellationToken);

            if (body != null) {
                FileClient.Write(cachedFileName, body, cacheDirectory);

                if (!registry.ContainsKey(cachedName))
                    FileClient.AppendText(registryFileName, $"{cachedName} | {cachedFileName}", cacheDirectory);

                SetHeader("Referer", url);
            }

            return body;
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

        private static string GetCachedName(string url, string cacheKey) =>
            $"({cacheKey.TrimStart(';')}) {url}";

        private static Uri FixUri(Uri original, Uri rawPart) =>
            new Uri(new Uri($"{original.Scheme}://{original.Host}"), rawPart);
    }
}
