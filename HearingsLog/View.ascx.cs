using DotNetNuke.Abstractions;
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
        public View() => _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                if (UserId > 0)
                {
                    if (UserInfo.IsAdmin)
                        lnkAdmin.Visible = true;
                }
                var tc = new HearingController();
                lnkAdmin.NavigateUrl = EditUrl("Admin");
                txtStartDate.Text = DateTime.Now.AddDays(-60).ToShortDateString();
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
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}