using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using Microsoft.Extensions.DependencyInjection;
using System;

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

        public string HomeUrl { get { return _navigationManager.NavigateURL(); } }
    }
}
