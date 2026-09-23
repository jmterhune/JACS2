/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Abstractions;
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
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

        public string ListUrl { get { return _navigationManager.NavigateURL(); } }
        public string DetailUrl(int arbitratorApplicationId)
        {
            return EditUrl("aid", arbitratorApplicationId.ToString(), "detail");
        }
    }
}
