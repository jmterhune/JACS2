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
    // The Expert Types list is rendered and managed entirely client-side via the
    // Web API (Components/Api/TypesController.cs) + Scripts/ew-core.js + ew-admin.js.
    public partial class TypeList : ExpertWitnessModuleBase
    {
        private readonly INavigationManager _navigationManager;

        public TypeList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
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
                // jQuery for DataTables; antiforgery token so the Web API accepts our calls.
                JavaScript.RequestRegistration(CommonJs.jQuery);
                ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
