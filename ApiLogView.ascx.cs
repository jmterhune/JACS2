using DotNetNuke.Abstractions;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace tjc.Modules.jacs
{
    /// <summary>
    /// Admin page for searching the api_log table. Requires an admin user —
    /// non-admins are redirected to the module's main view.
    /// </summary>
    public partial class ApiLogView : JACSModuleBase
    {
        private readonly INavigationManager _navigationManager;

        public ApiLogView()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                navbar.ModuleContext = this;
                navbar.ActiveLink = "lnkApiLog";
                if (UserId <= 0 || !UserInfo.IsAdmin)
                {
                    Response.Redirect(_navigationManager.NavigateURL(), true);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
