/*
' Copyright (c) 2022  Joe Terhune
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

namespace tjc.Modules.CourtCounsel
{
    public class CourtCounselModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public CourtCounselModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
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
        public string JudgeRole
        {
            get
            {
                if (Settings.Contains("JudgeRole"))
                    return Settings["JudgeRole"].ToString();
                return "Judge";
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
        public int AssignmentId
        {
            get
            {
                var qs = Request.QueryString["aid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }

        }
        public DateTime CurrentDate
        {
            get
            {
                if (ViewState["CurrentDate"] != null)
                {
                    return DateTime.Parse(ViewState["CurrentDate"].ToString());
                }
                return DateTime.Now;
            }
            set
            {
                ViewState["CurrentDate"] = value;
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
        public bool Pending
        {
            get
            {
                var qs = Request.QueryString["pending"];
                if (qs != null)
                    return true;
                return false;
            }

        }
        public RecordStatus LogRecordStatus
        {
            get
            {
                var qs = Request.QueryString["ls"];

                if (qs != null)
                {
                    Int32.TryParse(qs, out int status);
                    return (RecordStatus)status;
                }

                return RecordStatus.other;
            }

        }
        public RecordStatus AssignmentRecordStatus
        {
            get
            {
                var qs = Request.QueryString["as"];

                if (qs != null)
                {
                    Int32.TryParse(qs, out int status);
                    return (RecordStatus)status;
                }

                return RecordStatus.other;
            }

        }
        public bool IsAdmin
        {
            get
            {
                if (UserId > 0)
                {
                    return UserInfo.IsInRole(AdminRole);
                }
                else { return false; }
            }
        }

        public enum RecordStatus
        { created = 0, updated = 1, deleted = 2, future = 3, other = 4, fileUpload = 5 }
        public string CaseListUrl { get { return _navigationManager.NavigateURL(); } }
        public string MemberListUrl { get { return EditUrl("member"); } }
        public string PhasesListUrl { get { return EditUrl("phase"); } }
        public string TimeSpanListUrl { get { return EditUrl("timespan"); } }
        public string ActionListUrl { get { return EditUrl("action"); } }
        public string CaseTypeListUrl { get { return EditUrl("casetype"); } }

        public int DefaultReminderPeriod
        {
            get
            {
                if (Settings.Contains("DefaultReminderPeriod"))
                {
                    Int32.TryParse(Settings["DefaultReminderPeriod"].ToString(), out int days);
                    return days;
                }
                return 10;
            }
        }
        public string SharePointSiteURL
        {
            get
            {
                if (Settings.Contains("SharePointSiteURL"))
                    return Settings["SharePointSiteURL"].ToString();

                return "";
            }

        }
        public string DocumentDriveId
        {
            get
            {
                if (Settings.Contains("DocumentDriveId"))
                    return Settings["DocumentDriveId"].ToString();

                return "";
            }
        }

        public string OrdersDriveId
        {
            get
            {
                if (Settings.Contains("OrderDriveId"))
                    return Settings["OrderDriveId"].ToString();

                return "";
            }
        }
        public string OrderPath
        {
            get
            {
                if (Settings.Contains("OrderPath"))
                    return Settings["OrderPath"].ToString();

                return "";
            }
        }
        public string DocumentLibraryURL
        {
            get
            {
                if (Settings.Contains("DocumentLibraryURL"))
                    return Settings["DocumentLibraryURL"].ToString();

                return "";
            }
        }
        public string DocumentLibraryName
        {
            get
            {
                if (Settings.Contains("DocumentLibraryName"))
                    return Settings["DocumentLibraryName"].ToString();

                return "";
            }
        }

    }

}