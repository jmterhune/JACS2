/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using Microsoft.Extensions.DependencyInjection;

namespace tjc.Modules.CDSPAdmin
{
    /// <summary>
    /// Shared base for the CDSP admin module controls. Exposes the DNN
    /// navigation manager and ensures jQuery is registered (DataTables needs it).
    /// </summary>
    public class CDSPModuleBase : PortalModuleBase
    {
        protected readonly INavigationManager _navigationManager;

        public CDSPModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            JavaScript.RequestRegistration(CommonJs.jQuery);
        }

        /// <summary>URL of the module's default view (the submission list).</summary>
        public string ListUrl
        {
            get { return _navigationManager.NavigateURL(); }
        }
    }
}
