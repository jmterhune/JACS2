using DotNetNuke.Abstractions;
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Web.UI.WebControls;
using tjc.Modules.HearingLog.Components;
namespace tjc.Modules.HearingLog
{
    public partial class View : HearingsLogModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private readonly IHostSettings _hostSettings;
        public View()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _jsLibraryHelper.RequestRegistration(CommonJs.DnnPlugins);
                if (UserId > 0)
                {
                    if (UserInfo.IsAdmin)
                        lnkAdmin.Visible = true;
                }
                var tc = new CourtCounselController(_hostSettings);
                lnkAdmin.NavigateUrl = EditUrl("Admin");
                txtStartDate.Text = DateTime.Now.AddDays(-120).ToShortDateString();
                lnkCourtCounsel.NavigateUrl = PageUrl;
                txtEndDate.Text = DateTime.Now.ToShortDateString();
                if (UserInfo.IsInRole(ChiefJudgeRole))
                {
                    var users = RoleController.Instance.GetUsersByRole(PortalId, JudgeRole);
                    foreach (var user in users)
                    {
                        if(user.UserID!= UserId)
                        drpJudges.Items.Add(new ListItem(user.DisplayName, user.UserID.ToString()));
                    }
                    drpJudges.Visible = true;
                }
                hfSessionTimeout.Value = Session.Timeout.ToString();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}