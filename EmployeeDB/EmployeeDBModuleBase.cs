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

        public string HomeUrl { get { return _navigationManager.NavigateURL(); } }
    }
}
