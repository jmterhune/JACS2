/*
' Copyright (c) 2024  Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Abstractions;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace tjc.Modules.ExpertWitness
{
    // The Requests list (list / read-only detail / delete) is rendered client-side
    // via the Web API (Components/Api/RequestsController.cs) + Scripts/ew-core.js + ew-admin.js.
    public partial class RequestList : ExpertWitnessModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private readonly IJavaScriptLibraryHelper _jsLibraryHelper;

        public RequestList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            _jsLibraryHelper = DependencyProvider.GetRequiredService<IJavaScriptLibraryHelper>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsAdmin)
                {
                    Response.Redirect(_navigationManager.NavigateURL());
                    return;
                }
                _jsLibraryHelper.RequestRegistration(CommonJs.jQuery);
                ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
