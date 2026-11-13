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

namespace tjc.Modules.PressCart
{
    /// <summary>
    /// Shared base for the intranet presentation cart administration controls: the
    /// submitted requests, and the division, courtroom and courthouse lists the public
    /// form picks from. All of it lives in the public (jud12) database, reached through
    /// the "Jud12" connection. These screens used to sit in the public PressCart module
    /// behind an "Admin" link; they are here now.
    /// </summary>
    public class PressCartModuleBase : PortalModuleBase
    {
        protected readonly IHostSettings _hostSettings;
        private readonly INavigationManager _navigationManager;

        public PressCartModuleBase()
        {
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            DependencyProvider.GetRequiredService<IJavaScriptLibraryHelper>().RequestRegistration(CommonJs.DnnPlugins);
        }

        /// <summary>
        /// True when the current user may change a request's status, delete one, or edit
        /// the division / courtroom / courthouse lists. Viewing is governed by the
        /// module's own view permission.
        /// </summary>
        public bool CanManage
        {
            get { return ModulePermissionController.CanEditModuleContent(ModuleConfiguration); }
        }

        /// <summary>
        /// Html-encodes a value bound in the markup. Nearly every text column on
        /// tjc_pressCart is nullable and plenty of them really are null — CartType
        /// alone is null on roughly 3,400 of the 4,900 rows — so Eval returns null
        /// and calling ToString() on it in the markup throws.
        /// </summary>
        protected string Enc(object value)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(value));
        }

        /// <summary>The request named by the "pid" query string parameter, or -1.</summary>
        public int PressCartIdParam
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["pid"], out id) ? id : -1;
            }
        }

        public string ListUrl
        {
            get { return _navigationManager.NavigateURL(TabId, "", "mid=" + ModuleId); }
        }

        public string RequestUrl(int pressCartId)
        {
            return _navigationManager.NavigateURL(TabId, "ViewRequest", "mid=" + ModuleId, "pid=" + pressCartId);
        }

        public string DivisionUrl
        {
            get { return _navigationManager.NavigateURL(TabId, "Division", "mid=" + ModuleId); }
        }

        public string CourtroomUrl
        {
            get { return _navigationManager.NavigateURL(TabId, "Courtroom", "mid=" + ModuleId); }
        }

        public string CourthouseUrl
        {
            get { return _navigationManager.NavigateURL(TabId, "Courthouse", "mid=" + ModuleId); }
        }
    }
}
