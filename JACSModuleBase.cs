/*
' Copyright (c) 2025  Joe Terhune
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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace tjc.Modules.jacs
{
    public class JACSModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public JACSModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
        }
        public int AttorneyId
        {
            get
            {
                var qs = Request.QueryString["aid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public int CourtId
        {
            get
            {
                var qs = Request.QueryString["cid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public int TemplateId
        {
            get
            {
                var qs = Request.QueryString["tid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public int EventId
        {
            get
            {
                var qs = Request.QueryString["eid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public int TimeSlotId
        {
            get
            {
                var qs = Request.QueryString["sid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }


        #region "Urls"

        public string MainViewUrl { get { return _navigationManager.NavigateURL(); } }
        public string UserListUrl { get { return EditUrl("user"); } }
        public string RoleListUrl { get { return EditUrl("role"); } }
        public string PermissionListUrl { get { return EditUrl("permission"); } }
        public string CategoryListUrl { get { return EditUrl("category"); } }
        public string AttorneyListUrl { get { return EditUrl("attorney"); } }
        public string CountyListUrl { get { return EditUrl("county"); } }
        public string CourtListUrl { get { return EditUrl("court"); } }
        public string CourtEditUrl { get { return EditUrl("court-edit"); } }
        public string CourtCalendarUrl { get { return EditUrl("court-calendar"); } }
        public string CourtTypeListUrl { get { return EditUrl("court-type"); } }
        public string CourtPermissionListUrl { get { return EditUrl("court-permission"); } }
        public string DocketPrintUrl { get { return EditUrl("docket-print"); } }
        public string EventListUrl { get { return EditUrl("event"); } }
        public string EventEditUrl { get { return EditUrl("event-edit"); } }
        public string EventCalendarUrl { get { return EditUrl("event-calendar"); } }
        public string EventStatusListUrl { get { return EditUrl("event-status"); } }
        public string EventTypeListUrl { get { return EditUrl("event-type"); } }
        public string HolidayListUrl { get { return EditUrl("holiday"); } }
        public string JudgeListUrl { get { return EditUrl("judge"); } }
        public string MotionListUrl { get { return EditUrl("motion"); } }
        public string TemplateListUrl { get { return EditUrl("template"); } }
        public string TemplateConfigUrl { get { return EditUrl("template-config"); } }
        public string TimeSlotListUrl { get { return EditUrl("time-slot"); } }
        public string UserDefinedFieldUrl { get { return EditUrl("user-fields"); } }
        public string EventRevisionUrl { get { return EditUrl("revise"); } }
        public string TruncateCalendarUrl { get { return EditUrl("truncate-calendar"); } }
        public string ExtendCalendarUrl { get { return EditUrl("extend-calendar"); } }
        #endregion

        #region Module Settings
        public string QuickReferenceUrl { get { return Settings.Contains("QuickRefUrl") ? Settings["QuickRefUrl"].ToString() : ""; } }

        public bool IsAdmin
        {
            get
            {
                if (UserInfo.IsInRole(AdminRole) || UserInfo.IsAdmin)
                    return true;
                return false;
            }
        }
        public string AdminRole
        {
            get
            {
                if (Settings.Contains("AdminRole"))
                    return Settings["AdminRole"].ToString();

                return "JACS Admin";
            }
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
        public bool IsJudge
        {
            get
            {
                if (UserInfo.IsInRole(JudgeRole))
                    return true;
                return false;
            }
        }
        public string JaRole
        {
            get
            {
                if (Settings.Contains("JaRole"))
                    return Settings["JaRole"].ToString();

                return "Judicial Assistant";
            }
        }
        public string JacsUserRole
        {
            get
            {
                if (Settings.Contains("JacsUserRole"))
                    return Settings["JacsUserRole"].ToString();

                return "JACS User";
            }
        }
        #endregion #endregion
    }
}