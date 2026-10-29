/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Abstractions;
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security.Permissions;
using Microsoft.Extensions.DependencyInjection;
using tjc.Modules.CourtCalendar.Components;

namespace tjc.Modules.CourtCalendar
{
    /// <summary>
    /// Shared base for the intranet Court Calendar authoring controls. Admins
    /// (module edit permission) manage judges, divisions and categories; an
    /// assigned judge or JA manages their own division's calendars. The calendar
    /// records live in the public (jud12) database, reached through the "Jud12"
    /// connection; the assigned user ids stored on judges are intranet accounts.
    /// </summary>
    public class CourtCalendarModuleBase : PortalModuleBase
    {
        protected readonly IHostSettings _hostSettings;
        private readonly INavigationManager _navigationManager;
        private readonly IJavaScriptLibraryHelper _jsLibraryHelper;

        public CourtCalendarModuleBase()
        {
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            _jsLibraryHelper = DependencyProvider.GetRequiredService<IJavaScriptLibraryHelper>();
            _jsLibraryHelper.RequestRegistration(CommonJs.DnnPlugins);
        }

        /// <summary>True when the current user is a module admin (manages all divisions/judges).</summary>
        public bool IsAdmin
        {
            get { return ModulePermissionController.CanEditModuleContent(ModuleConfiguration); }
        }

        /// <summary>True when the current user may manage the given division (admin, or its assigned judge/JA).</summary>
        public bool CanManageDivision(Division d)
        {
            if (d == null) return false;
            if (IsAdmin) return true;
            if (!Request.IsAuthenticated) return false;
            var judge = new JudgeController(_hostSettings).GetJudge(d.JudgeId);
            return judge != null && (judge.JudgeUserId == UserId || judge.JaUserId == UserId);
        }

        // ---- Query parameters ----
        public int DivisionIdParam { get { return ParamInt("did"); } }
        public int CalendarIdParam { get { return ParamInt("cid"); } }
        public int JudgeIdParam { get { return ParamInt("jid"); } }

        private int ParamInt(string key)
        {
            int v;
            return int.TryParse(Request.QueryString[key], out v) ? v : -1;
        }

        // ---- Navigation ----
        public string HomeUrl { get { return _navigationManager.NavigateURL(); } }
        public string ManageJudgesUrl { get { return _navigationManager.NavigateURL(TabId, "ManageJudges", "mid=" + ModuleId); } }
        public string EditJudgeUrl(int judgeId) { return _navigationManager.NavigateURL(TabId, "ManageJudges", "mid=" + ModuleId, "jid=" + judgeId); }
        public string ManageDivisionsUrl { get { return _navigationManager.NavigateURL(TabId, "ManageDivisions", "mid=" + ModuleId); } }
        public string EditDivisionUrl(int divisionId) { return _navigationManager.NavigateURL(TabId, "ManageDivisions", "mid=" + ModuleId, "did=" + divisionId); }
        public string ManageCategoriesUrl(int divisionId) { return _navigationManager.NavigateURL(TabId, "ManageCategories", "mid=" + ModuleId, "did=" + divisionId); }
        public string NewCalendarUrl(int divisionId) { return _navigationManager.NavigateURL(TabId, "EditCalendar", "mid=" + ModuleId, "did=" + divisionId); }
        public string EditCalendarUrl(int calendarId) { return _navigationManager.NavigateURL(TabId, "EditCalendar", "mid=" + ModuleId, "cid=" + calendarId); }
    }
}
