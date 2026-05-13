using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace tjc.Modules.EmployeeDB
{
    public class EmployeeDBModuleBase : PortalModuleBase
    {
        protected readonly INavigationManager _navigationManager;

        public EmployeeDBModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            JavaScript.RequestRegistration(CommonJs.jQuery);
        }

        public string ReportUrl
        {
            get
            {
                if (Settings.Contains("Employee_ReportUrl"))
                    return Settings["Employee_ReportUrl"].ToString();
                return "";
            }
        }

        public int EmployeeId
        {
            get
            {
                var qs = Request.QueryString["EmployeeId"];
                if (qs != null && int.TryParse(qs, out int id))
                    return id;
                return -1;
            }
        }

        public string HrAdminRole
        {
            get
            {
                if (Settings.Contains("HrAdminRole"))
                    return Settings["HrAdminRole"].ToString();
                return "HR Admin";
            }
        }

        public bool IsHrAdmin
        {
            get
            {
                if (UserId <= 0) return false;
                return UserInfo.IsInRole(HrAdminRole) || UserInfo.IsSuperUser;
            }
        }

        /// <summary>True for portal Administrators role members and super users.
        /// Used to gate the Departments admin tab on the Employees list — the
        /// tjc_gl_group table is shared site-wide, so only DNN site admins can
        /// modify it from this module.</summary>
        public bool IsSiteAdmin
        {
            get
            {
                if (UserId <= 0) return false;
                if (UserInfo.IsSuperUser) return true;
                var adminRole = PortalSettings?.AdministratorRoleName;
                return !string.IsNullOrEmpty(adminRole) && UserInfo.IsInRole(adminRole);
            }
        }

        public string HomeUrl { get { return _navigationManager.NavigateURL(); } }

        #region SWN credentials

        public string SwnTestUsername
        {
            get { return Settings.Contains("Swn_TestUsername") ? Settings["Swn_TestUsername"].ToString() : ""; }
        }

        public string SwnTestPassword
        {
            get { return Settings.Contains("Swn_TestPassword") ? Settings["Swn_TestPassword"].ToString() : ""; }
        }

        public string SwnLiveUsername
        {
            get { return Settings.Contains("Swn_LiveUsername") ? Settings["Swn_LiveUsername"].ToString() : ""; }
        }

        public string SwnLivePassword
        {
            get { return Settings.Contains("Swn_LivePassword") ? Settings["Swn_LivePassword"].ToString() : ""; }
        }

        public bool SwnUseLive
        {
            get
            {
                if (!Settings.Contains("Swn_UseLive")) return false;
                bool result;
                return bool.TryParse(Settings["Swn_UseLive"].ToString(), out result) && result;
            }
        }

        public string SwnUsername { get { return SwnUseLive ? SwnLiveUsername : SwnTestUsername; } }
        public string SwnPassword { get { return SwnUseLive ? SwnLivePassword : SwnTestPassword; } }

        #endregion

        #region Helpdesk-notify email

        /// <summary>"From" address used when the Edit page sends a change-summary
        /// email after Save. Default mirrors the legacy aspx behaviour.</summary>
        public string NotifyFromEmail
        {
            get
            {
                if (Settings.Contains("Notify_FromEmail"))
                {
                    var v = Settings["Notify_FromEmail"].ToString();
                    if (!string.IsNullOrWhiteSpace(v)) return v;
                }
                return "hr@jud12.flcourts.org";
            }
        }

        /// <summary>"To" address (or comma-separated list) used when the Edit page
        /// sends a change-summary email after Save.</summary>
        public string NotifyToEmail
        {
            get
            {
                if (Settings.Contains("Notify_ToEmail"))
                {
                    var v = Settings["Notify_ToEmail"].ToString();
                    if (!string.IsNullOrWhiteSpace(v)) return v;
                }
                return "helpdesk@jud12.flcourts.org";
            }
        }

        /// <summary>Whether to send the change-summary email at all. Defaults to
        /// false so existing test sites don't accidentally start emailing on save.</summary>
        public bool NotifyOnSave
        {
            get
            {
                if (!Settings.Contains("Notify_OnSave")) return false;
                bool result;
                return bool.TryParse(Settings["Notify_OnSave"].ToString(), out result) && result;
            }
        }

        #endregion

        #region New Hire IT Worksheet

        /// <summary>"To" address for the helpdesk notification when a New Hire
        /// IT Worksheet is submitted. Defaults to the production helpdesk
        /// distribution list. Override per-environment via the
        /// <c>Nhit_HelpdeskEmail</c> module setting (so dev/test sites can
        /// route mail to a sandbox inbox).</summary>
        public string NhitHelpdeskEmail
        {
            get
            {
                if (Settings.Contains("Nhit_HelpdeskEmail"))
                {
                    var v = Settings["Nhit_HelpdeskEmail"].ToString();
                    if (!string.IsNullOrWhiteSpace(v)) return v;
                }
                return "helpdesk@jud12.flcourts.org";
            }
        }

        #endregion
    }
}
