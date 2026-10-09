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
    public partial class CourtCounsel : HearingsLogModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private readonly IHostSettings _hostSettings;
        public CourtCounsel()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _jsLibraryHelper.RequestRegistration(CommonJs.DnnPlugins);
                var tc = new CourtCounselController(_hostSettings);
                txtStartDate.Text = DateTime.Now.AddDays(-120).ToShortDateString();
                txtEndDate.Text = DateTime.Now.ToShortDateString();
                lnkHearingLog.NavigateUrl = PageUrl;
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