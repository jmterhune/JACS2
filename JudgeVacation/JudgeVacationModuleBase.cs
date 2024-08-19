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
    }
}