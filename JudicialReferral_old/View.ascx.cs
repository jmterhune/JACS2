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

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.JudicialReferral.Components;

namespace tjc.Modules.JudicialReferral
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
    public partial class View : JudicialReferralModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    var ctl = new JudicialReferralController();
                    lnkAddReferral.NavigateUrl = EditUrl("referral");
                    PopulateJudgeList();
                    txtEndDate.Text = DateTime.Now.ToShortDateString();
                    txtStartDate.Text = DateTime.Now.AddDays(-30).ToShortDateString();
                    if (UserId > 0)
                    {
                        if (UserInfo.IsInRole(JaRole))
                            lnkAddReferral.Visible = true;
                        if ((UserInfo.IsInRole(JaRole) | UserInfo.IsInRole(JudgeRole)) & !UserInfo.IsSuperUser)
                        {
                            lblJudge.Visible = false;
                            drpJudge.Visible = false;
                        }
                        if (UserInfo.IsInRole(JudgeRole))
                        {
                            drpJudge.SelectedValue = UserId.ToString();
                            var list = ctl.GetReferralList(UserId);
                            rptReferral.DataSource = list;
                            rptReferral.DataBind();
                        }
                        else
                        {
                            var list = ctl.GetReferralList(0);
                            rptReferral.DataSource = list;
                            rptReferral.DataBind();
                            ltRecordMessage.Text = GetRecordMessage();
                        }
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        private string GetRecordMessage()
        {
            string message = "<li class='list-group-item list-group-item-dark'><strong>Filtered By:</strong></li>";
            if (drpJudge.SelectedIndex > 0)
                message += string.Format("<li class='list-group-item'>Judge: {0}</li>", drpJudge.SelectedItem.Text);
            if (drpStatus.SelectedIndex > 0)
                message += string.Format("<li class='list-group-item'>Status: {0}</li>", drpStatus.SelectedItem.Text);
            if (!string.IsNullOrEmpty(txtCaseNumber.Text))
                message += string.Format("<li class='list-group-item'>Case N0.:{0}</li>", txtCaseNumber.Text);
            if (!string.IsNullOrEmpty(txtMotionTitle.Text))
                message += string.Format("<li class='list-group-item'>Motion: {0}</li>", txtMotionTitle.Text);
            if (!string.IsNullOrEmpty(txtStartDate.Text))
                message += string.Format("<li class='list-group-item'>Start: {0}</li>", txtStartDate.Text);
            if (!string.IsNullOrEmpty(txtEndDate.Text))
                message += string.Format("<li class='list-group-item'>End: {0}</li>", txtEndDate.Text);
            return message;
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
        private void SendToJudge(Components.JudicialReferral objReferral)
        {
            string emailFrom = "noreply.intranet@jud12.flcourts.org";
            UserInfo user = UserController.GetUserById(PortalId, objReferral.JudgeID);
            string toEmail = user.Email;
            string subject = "New Judicial Referral Request";
            string body = string.Format("<p>Please review the <a href='{0}'>Judicial Referral Request</a> for case number {1}.</p>", EditUrl("rid", objReferral.ReferralID.ToString(), "review"), objReferral.CaseNumber);
            DotNetNuke.Services.Mail.Mail.SendEmail(emailFrom, toEmail, subject, body);
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
                if (UserInfo.IsInRole(CounselRole) | UserInfo.IsSuperUser)
                {
                    rptReferral.DataSource = list;
                    rptReferral.DataBind();
                    ltRecordMessage.Text = GetRecordMessage();
                    return;
                }
                if (UserInfo.IsInRole(JaRole))
                    rptReferral.DataSource = list.Where(x => x.JaID == UserId);
                if (UserInfo.IsInRole(JudgeRole))
                    rptReferral.DataSource = list.Where(x => x.JudgeID == UserId);

                rptReferral.DataBind();
                ltRecordMessage.Text = GetRecordMessage();
            }
        }
    }
}