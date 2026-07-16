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
    // The Evaluation Types (templates) list and its requirement builder are rendered
    // client-side via the Web API (Components/Api/TemplatesController.cs) +
    // Scripts/ew-core.js + ew-admin.js. Expert types come from the Types endpoint.
    public partial class TemplateList : ExpertWitnessModuleBase
    {
        private readonly INavigationManager _navigationManager;

        public TemplateList()
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
