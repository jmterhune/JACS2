using DotNetNuke.Entities.Modules;
using System;

namespace tjc.Modules.JudgeVacation
{
    public class JudgeVacationModuleBase : PortalModuleBase
    {
        public int CalenderID
        {
            get
            {
                string qs = Request.QueryString["calId"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public int CurrentYear
        {
            get
            {
                int year = DateTime.Now.Year;
                if (ViewState["CurrentYear"] != null)
                {
                    year = int.Parse(ViewState["CurrentYear"].ToString());
                }
                return year;
            }
            set
            {
                ViewState["CurrentYear"] = value;
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
        public string ReportingRole
        {
            get
            {
                if (Settings.Contains("ReportingRole"))
                    return Convert.ToString(Settings["ReportingRole"]);
                return "";
            }
        }
        public string EmailTo
        {
            get
            {
                if (Settings.Contains("EmailTo"))
                    return Convert.ToString(Settings["EmailTo"]);
                return "";
            }
        }
    }
}