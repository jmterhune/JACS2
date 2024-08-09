using DotNetNuke.Entities.Modules;
using System;

namespace tjc.Modules.HearingLog
{
    public class HearingsLogModuleBase : PortalModuleBase
    {
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