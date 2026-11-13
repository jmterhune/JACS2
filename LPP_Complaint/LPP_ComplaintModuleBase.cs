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
using System.Web;

namespace tjc.Modules.LPP_Complaint
{
    /// <summary>
    /// Shared base for the intranet LPP complaint administration controls. The
    /// complaints themselves are submitted on the public site and live in the public
    /// (jud12) database, reached through the "Jud12" connection.
    /// </summary>
    public class LPP_ComplaintModuleBase : PortalModuleBase
    {
        protected readonly IHostSettings _hostSettings;
        private readonly INavigationManager _navigationManager;

        public LPP_ComplaintModuleBase()
        {
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            DependencyProvider.GetRequiredService<IJavaScriptLibraryHelper>().RequestRegistration(CommonJs.DnnPlugins);
        }

        /// <summary>
        /// True when the current user may change a complaint's status or delete it.
        /// Viewing is governed by the module's own view permission.
        /// </summary>
        public bool CanManage
        {
            get { return ModulePermissionController.CanEditModuleContent(ModuleConfiguration); }
        }

        /// <summary>
        /// Html-encodes a value bound in the markup. Every text column on the complaint
        /// table is nullable, so Eval returns null for a blank one and calling
        /// ToString() on it in the markup would throw.
        /// </summary>
        protected string Enc(object value)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(value));
        }

        /// <summary>The complaint named by the "cid" query string parameter, or -1.</summary>
        public int ComplaintIdParam
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["cid"], out id) ? id : -1;
            }
        }

        public string ListUrl
        {
            get { return _navigationManager.NavigateURL(TabId, "", "mid=" + ModuleId); }
        }

        public string ComplaintUrl(int lppComplaintId)
        {
            return _navigationManager.NavigateURL(TabId, "ViewComplaint", "mid=" + ModuleId, "cid=" + lppComplaintId);
        }
    }
}
