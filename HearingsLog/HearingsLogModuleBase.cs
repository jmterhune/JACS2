using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace tjc.Modules.HearingLog
{
    public class HearingsLogModuleBase : PortalModuleBase
    {
        protected readonly IJavaScriptLibraryHelper _jsLibraryHelper;

        public HearingsLogModuleBase()
        {
            _jsLibraryHelper = DependencyProvider.GetRequiredService<IJavaScriptLibraryHelper>();
        }

        public string ChiefJudgeRole
        {
            get
            {
                if (Settings.Contains("ChiefJudgeRole"))
                    return Convert.ToString(Settings["ChiefJudgeRole"]);
                return "";
            }
        }
        public string JudgeRole
        {
            get
            {
                if (Settings.Contains("JudgeRole"))
                    return Convert.ToString(Settings["JudgeRole"]);
                return "";
            }
        }
        public string JaRole
        {
            get
            {
                if (Settings.Contains("JaRole"))
                    return Convert.ToString(Settings["JaRole"]);
                return "";
            }
        }
        public string PageUrl
        {
            get
            {
                if (Settings.Contains("PageUrl"))
                    return Convert.ToString(Settings["PageUrl"]);
                return "";
            }
        }

        public string HasChiefJudgeRole
        {
            get
            {
                string returnValue = "false";
                if (!string.IsNullOrEmpty(ChiefJudgeRole))
                {
                    if (UserId > 0 && UserInfo.IsInRole(ChiefJudgeRole))
                        returnValue ="true";
                }
                return returnValue;
            }
        }
    }
}