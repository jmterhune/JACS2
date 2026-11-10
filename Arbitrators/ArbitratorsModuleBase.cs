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
using System;

namespace tjc.Modules.Arbitrators
{
    /// <summary>
    /// Shared base for the Arbitrators review module controls. This is the
    /// internal review sister to the public-facing Arbitrators module
    /// (tjc.Modules\Arbitrators, on jud12.flcourts.org) - reviewers land here
    /// from the "review this application" link in the notification email that
    /// module sends, which carries the record id as the "aid" query parameter.
    /// </summary>
    public class ArbitratorsModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;
        protected readonly IHostSettings _hostSettings;
        private readonly IJavaScriptLibraryHelper _jsLibraryHelper;

        public ArbitratorsModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
            _jsLibraryHelper = DependencyProvider.GetRequiredService<IJavaScriptLibraryHelper>();
            _jsLibraryHelper.RequestRegistration(CommonJs.DnnPlugins);
        }

        /// <summary>The ArbitratorApplicationID from the "aid" query string parameter, or -1 if absent.</summary>
        public int ArbitratorApplicationId
        {
            get
            {
                var qs = Request.QueryString["aid"];
                int id;
                if (!string.IsNullOrEmpty(qs) && int.TryParse(qs, out id))
                    return id;
                return -1;
            }
        }

        /// <summary>Address used as the From address on approval/denial notification emails.</summary>
        public string NotificationFromEmail
        {
            get
            {
                if (Settings.Contains("NotificationFromEmail"))
                    return Settings["NotificationFromEmail"].ToString();
                return "arbitrators-noreply@jud12.flcourts.org";
            }
        }

        /// <summary>Optional address CC'd on every applicant notification email (e.g. a committee mailbox).</summary>
        public string NotificationCcEmail
        {
            get
            {
                if (Settings.Contains("NotificationCcEmail"))
                    return Settings["NotificationCcEmail"].ToString();
                return string.Empty;
            }
        }

        /// <summary>
        /// URL of the public-facing Arbitrators app on jud12.flcourts.org, from module settings.
        /// Returns empty unless it is an absolute http(s) URL, so a bad value can't render a script link.
        /// </summary>
        public string ExternalAppUrl
        {
            get
            {
                if (!Settings.Contains("ExternalAppUrl"))
                    return string.Empty;
                string url = Settings["ExternalAppUrl"].ToString().Trim();
                Uri parsed;
                if (Uri.TryCreate(url, UriKind.Absolute, out parsed) &&
                    (parsed.Scheme == Uri.UriSchemeHttp || parsed.Scheme == Uri.UriSchemeHttps))
                    return parsed.AbsoluteUri;
                return string.Empty;
            }
        }

        /// <summary>The dashboard (the module's default view).</summary>
        public string DashboardUrl { get { return _navigationManager.NavigateURL(); } }

        /// <summary>The full application list (the "list" control).</summary>
        public string ListUrl { get { return _navigationManager.NavigateURL(TabId, "list", "mid=" + ModuleId); } }

        /// <summary>
        /// True when the user may add and edit records. Access is controlled by the page permissions (the same
        /// way Approve/Deny works), so any signed-in user who can open the page qualifies; it is not tied to
        /// DNN's Edit mode or to module-level edit permission, which hid the Add and Edit buttons.
        /// </summary>
        public bool CanEditRecords { get { return UserId > 0; } }

        public string EditArbitratorUrl(int arbitratorApplicationId)
        {
            // A plain URL (not EditUrl, which can return a javascript popup call that breaks the list link's href).
            return _navigationManager.NavigateURL(TabId, "editarbitrator", "mid=" + ModuleId, "aid=" + arbitratorApplicationId);
        }

        /// <summary>The Edit Arbitrator control in "add" mode (no existing record; "new=1" instead of an aid).</summary>
        public string AddArbitratorUrl
        {
            get { return _navigationManager.NavigateURL(TabId, "editarbitrator", "mid=" + ModuleId, "new=1"); }
        }

        /// <summary>True when the Edit Arbitrator control was opened to add a record rather than edit one.</summary>
        public bool IsNewArbitrator { get { return Request.QueryString["new"] == "1"; } }

        public string DetailUrl(int arbitratorApplicationId)
        {
            return EditUrl("aid", arbitratorApplicationId.ToString(), "detail");
        }
    }
}
