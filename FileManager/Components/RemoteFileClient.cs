/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Instrumentation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace tjc.Modules.FileManager.Components
{
    public class RemoteFileEntry
    {
        public string Path { get; set; }
        public long Size { get; set; }
        public DateTime Modified { get; set; }
    }

    public class RemoteListing
    {
        public List<string> Directories { get; set; } = new List<string>();
        public List<RemoteFileEntry> Files { get; set; } = new List<RemoteFileEntry>();
    }

    /// <summary>
    /// Calls the FileManager API on the public site. Every request is signed with a shared secret
    /// (HMAC-SHA256 over timestamp, nonce, action, share and path); the secret never leaves the server.
    /// Settings, in web.config appSettings:
    ///   FileManagerRemote.BaseUrl  site root of the public site, e.g. https://host/
    ///   FileManagerRemote.Secret   shared HMAC secret, identical to the public site's (32+ characters)
    ///   FileManagerRemote.AllowUntrustedCertificate  DEV ONLY. "true" accepts any certificate from the
    ///                              BaseUrl host (for a self-signed dev certificate). Never set it on test or live.
    /// The signing format must match RemoteRequestSigner in the public module (tjc.Modules\FileManager).
    /// </summary>
    public class RemoteFileClient
    {
        public const string BaseUrlSetting = "FileManagerRemote.BaseUrl";
        public const string SecretSetting = "FileManagerRemote.Secret";
        public const string AllowUntrustedCertificateSetting = "FileManagerRemote.AllowUntrustedCertificate";

        private const string TimestampHeader = "X-FM-Timestamp";
        private const string NonceHeader = "X-FM-Nonce";
        private const string SignatureHeader = "X-FM-Signature";
        private const int ListingCacheSeconds = 60;

        private static readonly HttpClient Http = CreateHttpClient();

        private readonly string _baseUrl;
        private readonly string _secret;

        public RemoteFileClient()
        {
            _baseUrl = (WebConfigurationManager.AppSettings[BaseUrlSetting] ?? string.Empty).Trim().TrimEnd('/');
            _secret = WebConfigurationManager.AppSettings[SecretSetting];
        }

        private static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            string baseUrl = (WebConfigurationManager.AppSettings[BaseUrlSetting] ?? string.Empty).Trim();
            bool allowUntrusted = string.Equals(WebConfigurationManager.AppSettings[AllowUntrustedCertificateSetting], "true", StringComparison.OrdinalIgnoreCase);
            if (allowUntrusted && Uri.TryCreate(baseUrl, UriKind.Absolute, out Uri baseUri))
            {
                // Dev only. Limited to the configured host so no other HTTPS call from this site is affected.
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                    errors == System.Net.Security.SslPolicyErrors.None ||
                    string.Equals(message.RequestUri.Host, baseUri.Host, StringComparison.OrdinalIgnoreCase);
                LoggerSource.Instance.GetLogger(typeof(RemoteFileClient)).Warn(
                    AllowUntrustedCertificateSetting + " is on: certificate errors from " + baseUri.Host + " are ignored. Dev only.");
            }

            return new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(10) };
        }

        public bool IsConfigured
        {
            get { return _baseUrl.Length > 0 && !string.IsNullOrEmpty(_secret); }
        }

        /// <summary>The share's file list, cached briefly so page views don't each call the public site.</summary>
        public RemoteListing GetListing(string share)
        {
            string cacheKey = "FileManagerRemote.Listing." + share;
            var cached = HttpRuntime.Cache[cacheKey] as RemoteListing;
            if (cached != null)
            {
                return cached;
            }

            using (HttpRequestMessage request = Build("list", share, null))
            using (HttpResponseMessage response = Http.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult())
            {
                response.EnsureSuccessStatusCode();
                string json = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                RemoteListing listing = JsonConvert.DeserializeObject<RemoteListing>(json) ?? new RemoteListing();
                HttpRuntime.Cache.Insert(cacheKey, listing, null, DateTime.UtcNow.AddSeconds(ListingCacheSeconds),
                    System.Web.Caching.Cache.NoSlidingExpiration);
                return listing;
            }
        }

        /// <summary>
        /// Starts a file download. Returns once the headers arrive; the caller reads the body
        /// from Content and must dispose the response.
        /// </summary>
        public HttpResponseMessage OpenFile(string share, string path)
        {
            using (HttpRequestMessage request = Build("file", share, path))
            {
                return Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        private HttpRequestMessage Build(string action, string share, string path)
        {
            if (!IsConfigured)
            {
                throw new InvalidOperationException("FileManager remote access is not configured (" + BaseUrlSetting + ", " + SecretSetting + ").");
            }

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string nonce = Guid.NewGuid().ToString("N");
            string signature = Sign(timestamp, nonce, action, share, path);

            string url = _baseUrl + "/API/FileManager/RemoteFiles/" + (action == "list" ? "List" : "File")
                + "?share=" + Uri.EscapeDataString(share);
            if (path != null)
            {
                url += "&path=" + Uri.EscapeDataString(path);
            }

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add(TimestampHeader, timestamp.ToString());
            request.Headers.Add(NonceHeader, nonce);
            request.Headers.Add(SignatureHeader, signature);
            return request;
        }

        private string Sign(long timestamp, string nonce, string action, string share, string path)
        {
            string toSign = string.Join("\n", timestamp.ToString(), nonce, action, share ?? string.Empty, path ?? string.Empty);
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret)))
            {
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(toSign)));
            }
        }
    }
}
