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
            string logoffUrl;
            try
            {
                logoffUrl = _navigationManager.NavigateURL(TabId, "Logoff");
            }
            catch
            {
                logoffUrl = null;
            }
            if (string.IsNullOrEmpty(logoffUrl))
            {
                // Fallback: append /ctl/Logoff to the current page path. DNN's URL
                // provider handles this at any depth.
                string current = Request.Url.AbsolutePath.TrimEnd('/');
                logoffUrl = current + "/ctl/Logoff";
            }

            string init =
                "(function(){function go(){if(window.SessionMonitor){SessionMonitor.init({" +
                "timeoutMinutes:" + timeoutMinutes.ToString("0") + "," +
                "warningMinutes:5," +
                "logoffUrl:'" + logoffUrl.Replace("\\", "\\\\").Replace("'", "\\'") + "'," +
                "keepAliveUrl:'/'" +
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
    }
}
