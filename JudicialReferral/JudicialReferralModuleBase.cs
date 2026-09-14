using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Web.UI;

namespace tjc.Modules.JudicialReferral
{
    public class JudicialReferralModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;

        public JudicialReferralModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            RegisterSessionMonitor();
        }

        private void RegisterSessionMonitor()
        {
            if (Request.IsAuthenticated == false) return;

            Page.ClientScript.RegisterClientScriptInclude(GetType(), "SessionMonitorScript", ResolveUrl("~/DesktopModules/tjc.modules/JudicialReferral/Scripts/session-monitor.js"));

            double timeoutMinutes = System.Web.Security.FormsAuthentication.Timeout.TotalMinutes;
            // Log off through the Home tab (/Home/ctl/Logoff), not the current
            // one. Building this from the current tab produced a bare
            // /ctl/Logoff when the module sat at the site root, which isn't a
            // valid logoff URL; anchoring to Home also makes Home the page
            // DNN returns to after clearing the auth cookie.
            string logoffUrl;
            try
            {
                logoffUrl = _navigationManager.NavigateURL(PortalSettings.HomeTabId, "Logoff");
            }
            catch
            {
                logoffUrl = null;
            }
            if (string.IsNullOrEmpty(logoffUrl))
            {
                logoffUrl = "/Home/ctl/Logoff";
            }

            // How much life the auth ticket actually has left. The client
            // can't infer this from the timeout alone: ASP.NET reissues a
            // sliding-expiration cookie only once a request arrives past the
            // halfway point of the window, so a page load in the first half
            // does NOT extend the session. A client-side "now + timeout"
            // clock would then warn after the session had already died.
            // Sent as remaining seconds rather than an absolute time so
            // client/server clock skew can't distort it.
            int secondsRemaining = 0;
            var formsIdentity = Context.User.Identity as System.Web.Security.FormsIdentity;
            if (formsIdentity != null && formsIdentity.Ticket != null)
            {
                TimeSpan remaining = formsIdentity.Ticket.Expiration - DateTime.Now;
                if (remaining > TimeSpan.Zero) secondsRemaining = (int)remaining.TotalSeconds;
            }

            string init =
                "(function(){function go(){if(window.SessionMonitor){SessionMonitor.init({" +
                "timeoutMinutes:" + timeoutMinutes.ToString("0") + "," +
                "secondsRemaining:" + secondsRemaining + "," +
                "warningMinutes:20," +
                "logoffUrl:'" + logoffUrl.Replace("\\", "\\\\").Replace("'", "\\'") + "'" +
                "});}else{setTimeout(go,200);}}go();})();";

            ScriptManager.RegisterStartupScript(this, GetType(), "SessionMonitorInit", init, true);
        }

        public string JudgeRole
        {
            get
            {
                if (Settings.Contains("JudgeRole"))
                    return Settings["JudgeRole"].ToString();
                return "Judge";
            }
        }

        public string JaRole
        {
            get
            {
                if (Settings.Contains("JaRole"))
                    return Settings["JaRole"].ToString();
                return "Ja";
            }
        }

        public string CounselRole
        {
            get
            {
                if (Settings.Contains("CounselRole"))
                    return Settings["CounselRole"].ToString();
                return "Court Counsel";
            }
        }

        public string CounselAdminRole
        {
            get
            {
                if (Settings.Contains("CounselAdminRole"))
                    return Settings["CounselAdminRole"].ToString();
                return "Court Counsel Admin";
            }
        }

        public string TargetFolder
        {
            get
            {
                if (Settings.Contains("FolderName"))
                    return Settings["FolderName"].ToString();
                return "Judicial-Referral-Attachments";
            }
        }

        public string CourtCounselEmail
        {
            get
            {
                if (Settings.Contains("CourtCounselEmail"))
                    return Settings["CourtCounselEmail"].ToString();
                return "jterhune@jud12.flcourts.org";
            }
        }

        public int ReferralID
        {
            get
            {
                var qs = Request.QueryString["rid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }

        public bool IsJudge { get { return UserId > 0 && UserInfo.IsInRole(JudgeRole); } }
        public bool IsJa { get { return UserId > 0 && UserInfo.IsInRole(JaRole); } }
        public bool IsCounsel { get { return UserId > 0 && UserInfo.IsInRole(CounselRole); } }
        public bool IsCounselAdmin { get { return UserId > 0 && UserInfo.IsInRole(CounselAdminRole); } }

        public string HomeUrl { get { return _navigationManager.NavigateURL(); } }

        /// <summary>
        /// Trim and cap a string to the given DB column length so PetaPoco
        /// inserts/updates never overflow the underlying nvarchar column.
        /// </summary>
        protected static string Trunc(string value, int max)
        {
            value = (value ?? string.Empty).Trim();
            return value.Length > max ? value.Substring(0, max) : value;
        }
    }
}
