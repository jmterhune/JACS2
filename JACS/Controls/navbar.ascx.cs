using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.jacs;

namespace tjc.Modules.jacs.Controls
{
    public partial class navbar : UserControlBase
    {
        public string MainViewUrl { get; set; }
        public string RoleListUrl { get; set; }
        public string PermissionListUrl { get; set; }
        public string CategoryListUrl { get; set; }
        public string UserListUrl { get; set; }
        public string AttorneyListUrl { get; set; }
        public string CountyListUrl { get; set; }
        public string CourtListUrl { get; set; }
        public string CourtTypeListUrl { get; set; }
        public string CourtPermissionListUrl { get; set; }
        public string DocketPrintUrl { get; set; }
        public string EventListUrl { get; set; }
        public string EventStatusListUrl { get; set; }
        public string EventTypeListUrl { get; set; }
        public string HolidayListUrl { get; set; }
        public string JudgeListUrl { get; set; }
        public string MotionListUrl { get; set; }
        public string TemplateListUrl { get; set; }
        public string TimeSlotListUrl { get; set; }
        public string QuickReferenceUrl { get; set; }
        public string ActiveLink { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}