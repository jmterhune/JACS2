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
    // The Experts list (with Location/Type/Evaluation associations) is rendered and
    // managed client-side via the Web API (Components/Api/ExpertsController.cs) +
    // Scripts/ew-core.js + ew-admin.js. Lookups come from Locations/Types/Templates.
    public partial class ExpertList : ExpertWitnessModuleBase
    {
        private readonly INavigationManager _navigationManager;

        public ExpertList()
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
