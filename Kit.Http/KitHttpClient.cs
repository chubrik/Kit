﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kit.Http
{
    public class KitHttpClient : HttpClient
    {
        public HttpClientHandler Handler { get; }

        // Global
        private static bool _globalAddDefaultHeaders = true;
        private static CacheMode _globalCacheMode = CacheMode.Disabled;
        private static string _globalCacheDirectory = "http-cache";
        private static string _globalCacheTag = string.Empty;
        private static bool _globalUseRepeat = false;
        private static TimeSpan _globalRequestDelay = TimeSpan.Zero;
        private static TimeSpan _globalRequestTimeout = TimeSpan.FromSeconds(60);

        // Instance
        private CacheMode _cacheMode;
        private string _cacheDirectory;
        private string _cacheTag;
        private bool _useRepeat;
        private TimeSpan _requestDelay;
        private TimeSpan _requestTimeout;

        #region Construction

        public KitHttpClient(
            bool disposeHandler = true,
            bool? addDefaultHeaders = null,
            CacheMode? cacheMode = null,
            string cacheDirectory = null,
            string cacheTag = null,
            bool? useRepeat = null,
            TimeSpan? requestDelay = null,
            TimeSpan? requestTimeout = null)
            : this(
                  handler: CreateDefaultHandler(),
                  disposeHandler: disposeHandler,
                  addDefaultHeaders: addDefaultHeaders,
                  cacheMode: cacheMode,
                  cacheDirectory: cacheDirectory,
                  cacheTag: cacheTag,
                  useRepeat: useRepeat,
                  requestDelay: requestDelay,
                  requestTimeout: requestTimeout)
        { }

        public KitHttpClient(
            HttpClientHandler handler,
            bool disposeHandler = true,
            bool? addDefaultHeaders = null,
            CacheMode? cacheMode = null,
            string cacheDirectory = null,
            string cacheTag = null,
            bool? useRepeat = null,
            TimeSpan? requestDelay = null,
            TimeSpan? requestTimeout = null)
            : base(
                  handler: handler,
                  disposeHandler: disposeHandler)
        {
            Debug.Assert(handler != null);
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));

            _cacheMode = cacheMode ?? _globalCacheMode;
            _cacheDirectory = cacheDirectory ?? _globalCacheDirectory;
            _cacheTag = cacheTag ?? _globalCacheTag;
            _useRepeat = useRepeat ?? _globalUseRepeat;
            _requestDelay = requestDelay ?? _globalRequestDelay;
            _requestTimeout = requestTimeout ?? _globalRequestTimeout;

            if (addDefaultHeaders ?? _globalAddDefaultHeaders)
                foreach (var header in _defaultHeaders)
                    SetHeader(header.Key, header.Value);
        }

        private static HttpClientHandler CreateDefaultHandler() => new HttpClientHandler
        {
            AllowAutoRedirect = true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            CookieContainer = new CookieContainer(),
            UseCookies = true
        };

        private static readonly Dictionary<string, string> _defaultHeaders = new Dictionary<string, string>
        {
            { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8" },
            { "Accept-Encoding", "gzip, deflate" },
            { "Accept-Language", "en;q=0.9" },
            { "Upgrade-Insecure-Requests", "1" },
            { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.92 Safari/537.36" }
        };

        #endregion

        #region Setup

        public static void GlobalSetup(
            CacheMode? cacheMode = null,
            string cacheDirectory = null,
            string cacheTag = null,
            bool? useRepeat = null,
            TimeSpan? requestDelay = null,
            TimeSpan? requestTimeout = null)
        {
            if (cacheMode != null)
                _globalCacheMode = cacheMode.Value;

            if (cacheDirectory != null)
                _globalCacheDirectory = cacheDirectory;

            if (cacheTag != null)
                _globalCacheTag = cacheTag;

            if (useRepeat != null)
                _globalUseRepeat = useRepeat.Value;

            if (requestDelay != null)
                _globalRequestDelay = requestDelay.Value;

            if (requestTimeout != null)
                _globalRequestTimeout = requestTimeout.Value;
        }

        public void Setup(
            CacheMode? cacheMode = null,
            string cacheDirectory = null,
            string cacheTag = null,
            bool? useRepeat = null,
            TimeSpan? requestDelay = null,
            TimeSpan? requestTimeout = null)
        {
            if (cacheMode != null)
                _cacheMode = cacheMode.Value;

            if (cacheDirectory != null)
                _cacheDirectory = cacheDirectory;

            if (cacheTag != null)
                _cacheTag = cacheTag;

            if (useRepeat != null)
                _useRepeat = useRepeat.Value;

            if (requestDelay != null)
                _requestDelay = requestDelay.Value;

            if (requestTimeout != null)
                _requestTimeout = requestTimeout.Value;
        }

        #endregion

        #region Headers

        public void SetHeader(string name, string value)
        {
            Debug.Assert(!name.IsNullOrEmpty());

            if (name.IsNullOrEmpty())
                throw new ArgumentNullOrEmptyException(nameof(name));

            Debug.Assert(value != null);

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            DefaultRequestHeaders.Remove(name);
            DefaultRequestHeaders.TryAddWithoutValidation(name, value);
        }

        public void AddToHeader(string name, string value)
        {
            Debug.Assert(!name.IsNullOrEmpty());

            if (name.IsNullOrEmpty())
                throw new ArgumentNullOrEmptyException(nameof(name));

            Debug.Assert(value != null);

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            DefaultRequestHeaders.TryAddWithoutValidation(name, value);
        }

        public void RemoveHeader(string name)
        {
            Debug.Assert(!name.IsNullOrEmpty());

            if (name.IsNullOrEmpty())
                throw new ArgumentNullOrEmptyException(nameof(name));

            DefaultRequestHeaders.Remove(name);
        }

        public void SetCookie(string url, string name, string value) => SetCookie(new Uri(url), name, value);

        public void SetCookie(Uri uri, string name, string value)
        {
            Debug.Assert(uri != null);

            if (uri == null)
                throw new ArgumentNullException(nameof(name));

            Debug.Assert(!name.IsNullOrEmpty());

            if (name.IsNullOrEmpty())
                throw new ArgumentNullOrEmptyException(nameof(name));

            Debug.Assert(value != null);

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Debug.Assert(Handler.CookieContainer != null);
            var cookieContainer = Handler.CookieContainer ?? throw new InvalidOperationException();
            cookieContainer.Add(GetBaseUri(uri), new Cookie(name, value));
        }

        public void RemoveCookie(string url, string name) => RemoveCookie(new Uri(url), name);

        public void RemoveCookie(Uri uri, string name)
        {
            Debug.Assert(uri != null);

            if (uri == null)
                throw new ArgumentNullException(nameof(name));

            Debug.Assert(!name.IsNullOrEmpty());

            if (name.IsNullOrEmpty())
                throw new ArgumentNullOrEmptyException(nameof(name));

            var cookie = Handler.CookieContainer?.GetCookies(GetBaseUri(uri))[name];

            if (cookie != null)
                cookie.Expired = true;
        }

        private static Uri GetBaseUri(Uri uri) => new Uri($"{uri.Scheme}://{uri.Host}:{uri.Port}/");

        #endregion

        #region Get content overloads

        public Task<string> GetStringAsync(string requestUri, CancellationToken cancellationToken) =>
            GetStringAsync(new Uri(requestUri), cancellationToken);

        public async Task<string> GetStringAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            using (var response = await GetAsync(requestUri, cancellationToken))
                return await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
        }

        public Task<byte[]> GetByteArrayAsync(string requestUri, CancellationToken cancellationToken) =>
            GetByteArrayAsync(new Uri(requestUri), cancellationToken);

        public async Task<byte[]> GetByteArrayAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            using (var response = await GetAsync(requestUri, cancellationToken))
                return await response.EnsureSuccessStatusCode().Content.ReadAsByteArrayAsync();
        }

        public Task<Stream> GetStreamAsync(string requestUri, CancellationToken cancellationToken) =>
            GetStreamAsync(new Uri(requestUri), cancellationToken);

        public async Task<Stream> GetStreamAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            using (var response = await GetAsync(requestUri, cancellationToken))
                return await response.EnsureSuccessStatusCode().Content.ReadAsStreamAsync();
        }

        #endregion
    }
}
