/*
' Copyright (c) 2022  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.CourtCounsel.Components;
using tjc.Modules.JudicialReferral.Components;
namespace tjc.Modules.CourtCounsel
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from JudicialReferralModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class JudicialReferral : CourtCounselModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public JudicialReferral()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    PopulateJudgeList();
                    lnkSearch.NavigateUrl = _navigationManager.NavigateURL();
                    var ctl = new JudicialReferralController();
                    drpStatus.SelectedValue = "2";

                    if (UserId > 0)
                    {
                        var list = ctl.GetReferralList();
                        rptReferral.DataSource = list;
                        rptReferral.DataBind();
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        private void PopulateJudgeList()
        {
            DotNetNuke.Security.Roles.RoleController rCtl = new DotNetNuke.Security.Roles.RoleController();
            var judgeList = DotNetNuke.Security.Roles.RoleController.Instance.GetUsersByRole(PortalId, JudgeRole);

            drpJudge.AppendDataBoundItems = true;
            drpJudge.DataTextField = "DisplayName";
            drpJudge.DataValueField = "UserId";
            drpJudge.DataSource = judgeList.OrderBy(jud => jud.DisplayName);
            drpJudge.DataBind();
        }
        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            int judgeId = -1;
            string casenumber = "";
            string motionTitle = "";
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime enddate = DateTime.Now;
            int status = -1;
            if (!string.IsNullOrEmpty(txtStartDate.Text))
                DateTime.TryParse(txtStartDate.Text, out startDate);
            if (!string.IsNullOrEmpty(txtEndDate.Text))
                DateTime.TryParse(txtEndDate.Text, out enddate);
            if (drpJudge.SelectedValue != "")
                judgeId = Int32.Parse(drpJudge.SelectedValue);
            if (txtCaseNumber.Text != "")
                casenumber = txtCaseNumber.Text;
            if (txtMotionTitle.Text != "")
                motionTitle = txtMotionTitle.Text;
            if (drpStatus.SelectedValue != "")
                status = Int32.Parse(drpStatus.SelectedValue);
            if (UserId > 0)
            {
                var ctl = new JudicialReferralController();
                var list = ctl.GetReferralList(startDate, enddate, casenumber, judgeId, motionTitle, status);
                rptReferral.DataSource = list;
                rptReferral.DataBind();
            }
        }
    }
}