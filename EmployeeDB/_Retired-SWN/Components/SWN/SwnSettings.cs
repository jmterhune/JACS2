using System.Collections;

namespace tjc.Modules.EmployeeDB.Components.SWN
{
    /// <summary>
    /// Reads SWN credentials and the Live/Test mode flag out of a DNN module's
    /// settings hashtable. Used by both the SWN-button endpoints (SwnController)
    /// and the per-row sync that fires from the Phones tab (PhonesController).
    ///
    /// The setting keys match what the EmployeeDBModuleBase reads on the
    /// page side, so no additional configuration UI is needed.
    /// </summary>
    public static class SwnSettings
    {
        public class Credentials
        {
            public bool UseLive { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }

            /// <summary>
            /// True only when both username AND password came back non-blank.
            /// Callers use this to short-circuit SWN sync when the module
            /// simply hasn't been configured for SWN yet — no point firing
            /// a SOAP request that's guaranteed to 401.
            /// </summary>
            public bool IsConfigured
            {
                get
                {
                    return !string.IsNullOrWhiteSpace(Username)
                        && !string.IsNullOrWhiteSpace(Password);
                }
            }
        }

        public static Credentials Read(Hashtable settings)
        {
            // Local helper — read a string-valued setting with a fallback.
            string Get(string key, string fallback = "")
            {
                if (settings == null) return fallback;
                if (settings.Contains(key))
                {
                    var raw = settings[key];
                    var v = raw == null ? null : raw.ToString();
                    if (!string.IsNullOrWhiteSpace(v)) return v;
                }
                return fallback;
            }

            bool live = bool.TryParse(Get("Swn_UseLive", "false"), out var b) && b;
            return new Credentials
            {
                UseLive = live,
                Username = Get(live ? "Swn_LiveUsername" : "Swn_TestUsername"),
                Password = Get(live ? "Swn_LivePassword" : "Swn_TestPassword")
            };
        }
    }
}
