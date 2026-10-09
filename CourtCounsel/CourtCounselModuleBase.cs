/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/

using DotNetNuke.Abstractions;
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Web.UI;

namespace tjc.Modules.CourtCounsel
{
    public class CourtCounselModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;
        protected readonly IHostSettings _hostSettings;
        private readonly IJavaScriptLibraryHelper _jsLibraryHelper;

        public CourtCounselModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
            _jsLibraryHelper = DependencyProvider.GetRequiredService<IJavaScriptLibraryHelper>();
            _jsLibraryHelper.RequestRegistration(CommonJs.DnnPlugins);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            RegisterSessionMonitor();
        }

        private void RegisterSessionMonitor()
        {
            if (Request.IsAuthenticated == false) return;

            Page.ClientScript.RegisterClientScriptInclude(GetType(), "SessionMonitorScript", ResolveUrl("~/DesktopModules/tjc.modules/CourtCounsel/Scripts/session-monitor.js"));

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

        public string AdminRole
        {
            get
            {
                if (Settings.Contains("AdminRole"))
                    return Settings["AdminRole"].ToString();
                return "";
            }
        }

        public string TemplateText
        {
            get
            {
                if (Settings.Contains("template"))
                    return Settings["template"].ToString();
                return "";
            }
        }

        public int LogId
        {
            get
            {
                var qs = Request.QueryString["lid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }

        public string CaseNumber
        {
            get
            {
                var qs = Request.QueryString["cn"];
                return qs ?? string.Empty;
            }
        }

        public string PartyName
        {
            get
            {
                var qs = Request.QueryString["pn"];
                return qs ?? string.Empty;
            }
        }

        public string AttorneyName
        {
            get
            {
                var qs = Request.QueryString["att"];
                return qs ?? string.Empty;
            }
        }

        public string StatusFilter
        {
            get
            {
                var qs = Request.QueryString["sf"];
                return qs ?? string.Empty;
            }
        }

        public bool IsFutureAction
        {
            get
            {
                var qs = Request.QueryString["fa"];
                return qs != null;
            }
        }

        public bool IsAdmin
        {
            get
            {
                if (UserId > 0)
                    return UserInfo.IsInRole(AdminRole);
                return false;
            }
        }

        public string SearchUrl { get { return _navigationManager.NavigateURL(); } }
        public string DataEntryUrl { get { return EditUrl("EditHistory"); } }
        public string ReportsUrl { get { return EditUrl("Reports"); } }
        public string DataSheetUrl { get { return EditUrl("DataSheet"); } }
        public string AdminUrl { get { return EditUrl("Admin"); } }
        public string UpdateCaseNameUrl { get { return EditUrl("UpdateCaseName"); } }
    }
}
