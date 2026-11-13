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

namespace tjc.Modules.InterpreterRequest
{
    /// <summary>
    /// Shared base for the intranet interpreter request administration controls.
    /// The requests themselves are submitted on the public site and live in the
    /// public (jud12) database, reached through the "Jud12" connection.
    /// </summary>
    public class InterpreterRequestModuleBase : PortalModuleBase
    {
        protected readonly IHostSettings _hostSettings;
        private readonly INavigationManager _navigationManager;

        public InterpreterRequestModuleBase()
        {
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            DependencyProvider.GetRequiredService<IJavaScriptLibraryHelper>().RequestRegistration(CommonJs.DnnPlugins);
        }

        /// <summary>
        /// True when the current user may change a request's status or delete it.
        /// Viewing is governed by the module's own view permission.
        /// </summary>
        public bool CanManage
        {
            get { return ModulePermissionController.CanEditModuleContent(ModuleConfiguration); }
        }

        /// <summary>
        /// Html-encodes a value bound in the markup. Every text column on the request
        /// table is nullable, so Eval returns null for a blank one and calling
        /// ToString() on it in the markup would throw.
        /// </summary>
        protected string Enc(object value)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(value));
        }

        /// <summary>The request named by the "rid" query string parameter, or -1.</summary>
        public int RequestIdParam
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["rid"], out id) ? id : -1;
            }
        }

        public string ListUrl
        {
            get { return _navigationManager.NavigateURL(TabId, "", "mid=" + ModuleId); }
        }

        public string RequestUrl(int interpreterRequestId)
        {
            return _navigationManager.NavigateURL(TabId, "ViewRequest", "mid=" + ModuleId, "rid=" + interpreterRequestId);
        }
    }
}
