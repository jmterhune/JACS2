using System;
using System.Configuration;
using System.IO;
using System.Web;
using WinSCP;

namespace tjc.Modules.EmployeeDB.Components.Helpers
{
    /// <summary>
    /// Pushes the Crisis24 Person/HR Feed file to the Crisis24 SFTP site.
    ///
    /// Crisis24 accepts SFTP only (see "DELIVERY METHOD" in the Person
    /// Implementation Guide) with key-pair authentication against a public key
    /// they hold; our private key stays on this server.
    ///
    /// WinSCP is used rather than a raw SSH library because the sister feed
    /// modules on this farm already ship it (see tjc.Modules\Bar Member
    /// Import\Components\FtpSftpFileContext.cs) and WinSCPnet.dll +
    /// WinSCP.exe are already deployed to the site bin.
    ///
    /// ---------------------------- Configuration ----------------------------
    /// Required web.config appSetting:
    ///
    ///   Crisis24FTP — the SFTP destination. Credentials may be embedded:
    ///       sftp://user:password@host:22/inbound/
    ///     or supplied separately if you'd rather not put them in the URL:
    ///       sftp://host/inbound/
    ///
    /// Optional appSettings (each overrides the corresponding URL part):
    ///   Crisis24FtpUsername            — SFTP account name
    ///   Crisis24FtpPassword            — password, when not using a key
    ///   Crisis24SshPrivateKeyPath      — PuTTY .ppk private key for key-pair auth
    ///   Crisis24SshPrivateKeyPassphrase— passphrase protecting that key
    ///   Crisis24SshHostKeyFingerprint  — Crisis24's expected host key
    ///   Crisis24RemoteDirectory        — remote folder, if not in the URL
    ///   WinScpExecutablePath           — explicit path to WinSCP.exe
    /// -----------------------------------------------------------------------
    /// </summary>
    public static class Crisis24Uploader
    {
        /// <summary>Name of the appSetting holding the SFTP destination.</summary>
        public const string UrlSettingKey = "Crisis24FTP";

        public class Result
        {
            /// <summary>Full remote path the file was written to.</summary>
            public string RemotePath { get; set; }

            /// <summary>Host the file was delivered to (for the audit line).</summary>
            public string Host { get; set; }

            /// <summary>Bytes transferred.</summary>
            public int ByteCount { get; set; }
        }

        /// <summary>
        /// Writes <paramref name="content"/> to a temp file and uploads it as
        /// <paramref name="fileName"/>. Throws on any misconfiguration or
        /// transfer failure so the caller can surface the message; the temp
        /// file is always cleaned up.
        /// </summary>
        /// <summary>
        /// Turns the builder's CSV text into the exact bytes that go over the
        /// wire: UTF-8, no byte-order mark. Crisis24 accepts ASCII/UTF-8, and a
        /// BOM would be parsed as part of the first column heading.
        ///
        /// <see cref="Upload"/> and the Preview endpoint both go through here,
        /// so a previewed file is byte-for-byte what a real transmission sends.
        /// </summary>
        public static byte[] Encode(string content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            return new System.Text.UTF8Encoding(false).GetBytes(content);
        }

        public static Result Upload(string content, string fileName)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("A remote file name is required.", nameof(fileName));

            var target = ReadTarget();
            var bytes = Encode(content);

            // WinSCP transfers from disk, so the CSV is staged locally first.
            // A GUID-named temp file keeps two concurrent exports from
            // clobbering each other.
            var localPath = Path.Combine(Path.GetTempPath(),
                "crisis24-" + Guid.NewGuid().ToString("N") + ".csv");

            try
            {
                File.WriteAllBytes(localPath, bytes);

                var remotePath = CombineRemote(target.RemoteDirectory, fileName);

                using (var session = new Session())
                {
                    var exe = ResolveWinScpExecutable();
                    if (!string.IsNullOrEmpty(exe)) session.ExecutablePath = exe;

                    session.Open(target.SessionOptions);

                    var options = new TransferOptions { TransferMode = TransferMode.Binary };
                    var transfer = session.PutFiles(localPath, remotePath, false, options);
                    // Check() rethrows the underlying failure with WinSCP's
                    // own message, which is far more actionable than a bare
                    // "transfer failed".
                    transfer.Check();

                    return new Result
                    {
                        RemotePath = remotePath,
                        Host = target.SessionOptions.HostName,
                        ByteCount = bytes.Length
                    };
                }
            }
            finally
            {
                try { if (File.Exists(localPath)) File.Delete(localPath); }
                catch
                {
                    // A stranded temp file is not worth failing an otherwise
                    // successful upload over; the OS reclaims %TEMP% anyway.
                }
            }
        }

        // ------------------------------------------------------------------
        // Configuration
        // ------------------------------------------------------------------

        private class Target
        {
            public SessionOptions SessionOptions { get; set; }
            public string RemoteDirectory { get; set; }
        }

        /// <summary>
        /// Placeholder tokens that mean "the appSetting exists but nobody has
        /// filled in the real destination yet". The key is seeded with a
        /// template value so the setting is present and self-documenting before
        /// Crisis24 hands over the host and credentials, and we want that state
        /// to report cleanly instead of spending a connection timeout resolving
        /// a hostname that was never meant to exist.
        /// </summary>
        private static readonly string[] PlaceholderMarkers =
        {
            "USERNAME", "HOST.CRISIS24.COM", "EXAMPLE.COM", "CHANGEME", "TODO"
        };

        /// <summary>True when Crisis24FTP holds a real destination — present,
        /// and not still the seeded placeholder.</summary>
        public static bool IsConfigured
        {
            get
            {
                var raw = ConfigurationManager.AppSettings[UrlSettingKey];
                return !string.IsNullOrWhiteSpace(raw) && !IsPlaceholder(raw);
            }
        }

        private static bool IsPlaceholder(string url)
        {
            var upper = url.ToUpperInvariant();
            foreach (var marker in PlaceholderMarkers)
                if (upper.IndexOf(marker, StringComparison.Ordinal) >= 0) return true;
            return false;
        }

        private static Target ReadTarget()
        {
            var raw = ConfigurationManager.AppSettings[UrlSettingKey];
            if (string.IsNullOrWhiteSpace(raw))
            {
                throw new ConfigurationErrorsException(
                    "The \"" + UrlSettingKey + "\" appSetting is missing from web.config. " +
                    "Add the Crisis24 SFTP destination, e.g. " +
                    "<add key=\"" + UrlSettingKey + "\" value=\"sftp://user@host/inbound/\" />");
            }

            if (IsPlaceholder(raw))
            {
                throw new ConfigurationErrorsException(
                    "The Crisis24 SFTP destination hasn't been set up yet — the \"" + UrlSettingKey +
                    "\" appSetting in web.config still holds its placeholder value (" + raw.Trim() +
                    "). Replace it with the host and account Crisis24 provides, and add either a " +
                    "password or a \"Crisis24SshPrivateKeyPath\" key, then try again.");
            }

            raw = raw.Trim();
            // A bare "host/path" is a common way to write this; give Uri a
            // scheme so it parses instead of throwing.
            if (raw.IndexOf("://", StringComparison.Ordinal) < 0) raw = "sftp://" + raw;

            if (!Uri.TryCreate(raw, UriKind.Absolute, out var uri))
            {
                throw new ConfigurationErrorsException(
                    "The \"" + UrlSettingKey + "\" appSetting is not a valid URL: " + raw);
            }

            // The guide is explicit that Crisis24 accepts SFTP output only,
            // so reject a plain-FTP URL rather than silently sending employee
            // PII over an unencrypted channel.
            var scheme = uri.Scheme.ToLowerInvariant();
            if (scheme != "sftp" && scheme != "ssh")
            {
                throw new ConfigurationErrorsException(
                    "Crisis24 accepts SFTP only, but \"" + UrlSettingKey +
                    "\" specifies \"" + uri.Scheme + "\". Use an sftp:// URL.");
            }

            var userName = Setting("Crisis24FtpUsername");
            var password = Setting("Crisis24FtpPassword");
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                // UserInfo is "user" or "user:password"; either half may be
                // absent, and each appSetting above wins when present.
                var userInfo = uri.UserInfo ?? string.Empty;
                if (userInfo.Length > 0)
                {
                    var split = userInfo.Split(new[] { ':' }, 2);
                    if (string.IsNullOrEmpty(userName))
                        userName = Uri.UnescapeDataString(split[0]);
                    if (string.IsNullOrEmpty(password) && split.Length > 1)
                        password = Uri.UnescapeDataString(split[1]);
                }
            }

            if (string.IsNullOrEmpty(userName))
            {
                throw new ConfigurationErrorsException(
                    "No SFTP user name found. Put it in the \"" + UrlSettingKey +
                    "\" URL (sftp://user@host/...) or in a \"Crisis24FtpUsername\" appSetting.");
            }

            var options = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = uri.Host,
                UserName = userName
            };
            // Uri fills in -1 when the URL carries no explicit port; leaving
            // PortNumber at 0 lets WinSCP use the SFTP default of 22.
            if (!uri.IsDefaultPort && uri.Port > 0) options.PortNumber = uri.Port;
            if (!string.IsNullOrEmpty(password)) options.Password = password;

            var keyPath = Setting("Crisis24SshPrivateKeyPath");
            if (!string.IsNullOrEmpty(keyPath))
            {
                if (!File.Exists(keyPath))
                {
                    throw new ConfigurationErrorsException(
                        "The private key at \"Crisis24SshPrivateKeyPath\" was not found: " + keyPath);
                }
                options.SshPrivateKeyPath = keyPath;
                var passphrase = Setting("Crisis24SshPrivateKeyPassphrase");
                if (!string.IsNullOrEmpty(passphrase)) options.PrivateKeyPassphrase = passphrase;
            }
            else if (string.IsNullOrEmpty(password))
            {
                throw new ConfigurationErrorsException(
                    "No SFTP credentials found. Supply a password (in the \"" + UrlSettingKey +
                    "\" URL or a \"Crisis24FtpPassword\" appSetting) or a key file via " +
                    "\"Crisis24SshPrivateKeyPath\".");
            }

            var hostKey = Setting("Crisis24SshHostKeyFingerprint");
            if (!string.IsNullOrEmpty(hostKey))
            {
                options.SshHostKeyFingerprint = hostKey;
            }
            else
            {
                // Without a pinned fingerprint WinSCP refuses to connect at
                // all. Accepting any key keeps the first transmission working
                // before Crisis24 has given us their fingerprint; set
                // Crisis24SshHostKeyFingerprint as soon as you have it, since
                // this leaves the session open to a man-in-the-middle.
                options.SshHostKeyPolicy = SshHostKeyPolicy.GiveUpSecurityAndAcceptAny;
            }

            var remoteDir = Setting("Crisis24RemoteDirectory");
            if (string.IsNullOrEmpty(remoteDir)) remoteDir = uri.AbsolutePath;

            return new Target { SessionOptions = options, RemoteDirectory = remoteDir };
        }

        private static string Setting(string key)
        {
            var v = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(v) ? string.Empty : v.Trim();
        }

        /// <summary>Joins the remote directory and file name into one path.</summary>
        private static string CombineRemote(string directory, string fileName)
        {
            var dir = (directory ?? string.Empty).Trim();
            if (dir.Length == 0 || dir == "/") return "/" + fileName;
            if (!dir.StartsWith("/", StringComparison.Ordinal)) dir = "/" + dir;
            if (!dir.EndsWith("/", StringComparison.Ordinal)) dir += "/";
            return dir + fileName;
        }

        /// <summary>
        /// Locates WinSCP.exe, which WinSCPnet.dll shells out to.
        ///
        /// It normally looks beside its own assembly, but ASP.NET shadow-copies
        /// bin assemblies into the Temporary ASP.NET Files tree, where the exe
        /// isn't — so point at the real bin folder explicitly. Returns an empty
        /// string if we can't find it, letting WinSCP's own probing (and its
        /// clearer error message) take over.
        /// </summary>
        private static string ResolveWinScpExecutable()
        {
            var configured = Setting("WinScpExecutablePath");
            if (configured.Length > 0) return File.Exists(configured) ? configured : string.Empty;

            var candidates = new[]
            {
                SafeBinDirectory(),
                AppDomain.CurrentDomain.BaseDirectory == null
                    ? null
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"),
                AppDomain.CurrentDomain.BaseDirectory
            };

            foreach (var dir in candidates)
            {
                if (string.IsNullOrEmpty(dir)) continue;
                var path = Path.Combine(dir, "WinSCP.exe");
                if (File.Exists(path)) return path;
            }
            return string.Empty;
        }

        private static string SafeBinDirectory()
        {
            try
            {
                // Throws outside a hosted ASP.NET request (e.g. a unit test).
                return HttpRuntime.BinDirectory;
            }
            catch
            {
                return null;
            }
        }
    }
}
