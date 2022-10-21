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
using DotNetNuke.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Web.UI.WebControls;

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
    public partial class Review : JudicialReferralModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public Review()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    cmdCancel.NavigateUrl = _navigationManager.NavigateURL();
                    PopulateJudgeList();
                    PopulateForm();

                }

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void PopulateJudgeList()
        {
            var judgeList = DotNetNuke.Security.Roles.RoleController.Instance.GetUsersByRole(PortalId, JudgeRole);
            drpJudge.AppendDataBoundItems = true;
            drpJudge.DataTextField = "DisplayName";
            drpJudge.DataValueField = "UserId";
            drpJudge.DataSource = judgeList.OrderBy(jud => jud.DisplayName);
            drpJudge.DataBind();
        }

        protected void PopulateForm()
        {
            var ctl = new JudicialReferralController();
            if (IsJa)
                hdIsJa.Value = "1";
            if (ReferralID > 0)
            {
                Components.JudicialReferral objReferral = ctl.GetReferral(ReferralID);
                if (objReferral != null)
                {
                    rptFiles.DataSource = objReferral.Attachments;
                    rptFiles.DataBind();
                    if (rptFiles.Items.Count < 1)
                    {
                        rptFiles.Visible = false;
                        ltAttachments.Text = "No Attachment";
                    }
                    txtCaseNumber.Text = objReferral.CaseNumber;
                    txtCaseParties.Text = objReferral.CaseParties;
                    txtMotionDate.Text = objReferral.MotionDate.Value.ToShortDateString();
                    txtMotionTitle.Text = objReferral.MotionTitle;
                    drpCaseType.SelectedValue = objReferral.CaseType;
                    drpJudge.SelectedValue = objReferral.JudgeID.ToString();
                    chkMotionCorrect.Checked = objReferral.MotionCorrect;
                    chkMotionDirected.Checked = objReferral.MotionDirected;
                    chkMotionOther.Checked = objReferral.MotionOther;
                    chkMotionVacate.Checked = objReferral.MotionVacate;
                    string[] motionList = null;
                    string[] judgeMotions = null;
                    if (!string.IsNullOrEmpty(objReferral.DirectedMotions))
                    {
                        motionList = objReferral.DirectedMotions.Split('|');
                        foreach (var s in motionList)
                        {
                            var value = clsMotionList.Items.FindByValue(s);
                            if (value != null)
                                value.Selected = true;
                        }
                    }
                    if (!string.IsNullOrEmpty(objReferral.JudgeMotions))
                        judgeMotions = objReferral.JudgeMotions.Split('|');
                    cmdSave.Visible = false;
                    pnlJudge.Enabled = false;
                    pnlJA.Enabled = false;

                    if (objReferral.Status == Components.JudicialReferral.Statuses.NewReferral | objReferral.Status == Components.JudicialReferral.Statuses.RetainedByJudge | objReferral.Status == Components.JudicialReferral.Statuses.ReferredToCounsel)
                    {
                        if (IsJudge)
                        {
                            pnlJudge.Enabled = true;
                            cmdSave.Visible = true;
                        }
                        else if (IsJa & objReferral.Status == Components.JudicialReferral.Statuses.NewReferral)
                        {
                            pnlJA.Enabled = true;
                            cmdSave.Visible = true;
                            cmdComplete.Visible = false;
                        }
                    }
                    if (objReferral.CounselAssistance)
                    {
                        chkYes.Checked = true;
                        foreach (string s in judgeMotions)
                        {
                            var checkItem = clsResponse.Items.FindByText(s);
                            if (checkItem != null)
                                checkItem.Selected = true;
                            else
                            {
                                clsResponse.Items[4].Selected = true;
                                txtOther.Text += s;
                            }
                        }
                        if (IsCounsel)
                        {
                            cmdSave.Text = "Copy to Counsel Log";
                            cmdSave.Visible = true;
                            if (CounselRecordExists(objReferral))
                                cmdSave.OnClientClick = @"return confirm('Are you Sure?\n\rA record with the same case number and judge already exists.');";
                        }
                    }
                    else if (objReferral.Status == Components.JudicialReferral.Statuses.RetainedByJudge)
                        chkNo.Checked = true;
                    if (objReferral.Status == Components.JudicialReferral.Statuses.Complete)
                    {
                        pnlJA.Enabled = false;
                        pnlJudge.Enabled = false;

                        if (!IsJudge)
                        {
                            cmdComplete.Visible = false;
                            cmdSave.Visible = false;
                        }
                        cmdComplete.Text = "Revert Completed Status";
                    }
                    else
                    {
                        cmdComplete.Text = "Order Completed?";
                        cmdComplete.Visible = false;
                        if (IsJudge)
                        {
                            pnlJudge.Enabled = true;
                            cmdComplete.Visible = true;
                            cmdSave.Visible = true;
                        }
                    }
                }
                else
                {
                    pnlJudge.Enabled = false;
                    pnlJA.Enabled = false;
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Unable to find requested record", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                }
            }
            else
            {
                pnlJA.Enabled = true;
                pnlJudge.Visible = false;
            }
        }

        private void SendToCounsel(Components.JudicialReferral objReferral)
        {
            UserInfo objJudge = UserController.GetUserById(PortalId, objReferral.JudgeID);

            string emailFrom = objJudge.Email;
            string toEmail = CourtCounselEmail;
            string subject = "Judicial Referral Request";
            string body = string.Format("<p>Please review the <a href='{0}'>Judicial Referral Request</a> for case number {1}.</p>", EditUrl("rid", objReferral.ReferralID.ToString(), "review"), objReferral.CaseNumber);
            DotNetNuke.Services.Mail.Mail.SendEmail(emailFrom, toEmail, subject, body);
        }

        private bool CounselRecordExists(Components.JudicialReferral objReferral)
        {
            var ctl = new JudicialReferralController();
            return false; //ctl.CounselRecordExists(objReferral);
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new JudicialReferralController();
            Components.JudicialReferral objReferral = ctl.GetReferral(ReferralID);
            if (objReferral != null)
            {
                if (objReferral.Status == Components.JudicialReferral.Statuses.NewReferral & IsJa)
                {
                    objReferral.CaseParties = txtCaseParties.Text;
                    objReferral.CaseNumber = txtCaseNumber.Text;
                    objReferral.MotionDate = DateTime.Parse(txtMotionDate.Text);
                    objReferral.MotionTitle = txtMotionTitle.Text;
                    objReferral.CaseType = drpCaseType.SelectedValue;
                    objReferral.JudgeID = Int32.Parse(drpJudge.SelectedValue);
                    objReferral.MotionVacate = chkMotionVacate.Checked;
                    objReferral.MotionCorrect = chkMotionCorrect.Checked;
                    objReferral.MotionDirected = chkMotionDirected.Checked;
                    objReferral.MotionOther = chkMotionOther.Checked;
                    string directedMotions = "";
                    foreach (ListItem item in clsMotionList.Items)
                    {
                        if ((item.Selected))
                            directedMotions += item.Value + "|";
                    }

                    objReferral.DirectedMotions = directedMotions.Trim('|');
                    ctl.UpdateReferral(objReferral);
                    Response.Redirect(_navigationManager.NavigateURL(), true);
                    return;
                }
                if ((objReferral.Status != Components.JudicialReferral.Statuses.Complete) & IsJudge)
                {
                    objReferral.CounselAssistance = chkYes.Checked;
                    objReferral.JudgeResponseDate = DateTime.Now;
                    string judgeResponse = "";
                    bool otherSelected = false;
                    foreach (ListItem item in clsResponse.Items)
                    {
                        if ((item.Selected) & item.Value.ToLower() != "other")
                            judgeResponse += item.Value + "|";
                        if (item.Value == "other")
                            otherSelected = true;
                    }
                    if (txtOther.Text.Length > 0 & otherSelected)
                        judgeResponse += txtOther.Text;
                    objReferral.JudgeMotions = judgeResponse.Trim('|');
                    if (chkYes.Checked)
                        objReferral.Status = Components.JudicialReferral.Statuses.ReferredToCounsel;
                    else
                        objReferral.Status = Components.JudicialReferral.Statuses.RetainedByJudge;
                    ctl.UpdateReferral(objReferral);
                    SendToCounsel(objReferral);
                    Response.Redirect(_navigationManager.NavigateURL(), true);
                    return;
                }
                if (objReferral.Status == Components.JudicialReferral.Statuses.ReferredToCounsel & IsCounsel)
                {
                    objReferral.CounselReceivedDate = DateTime.Now;
                    ctl.UpdateReferral(objReferral);
                    // ctl.CopyToCounselLog(objReferral);
                    Response.Redirect(_navigationManager.NavigateURL(), true);
                }
            }
        }

        protected void cmdComplete_Click(object sender, EventArgs e)
        {
            var ctl = new JudicialReferralController();

            Components.JudicialReferral objReferral = ctl.GetReferral(ReferralID);
            if (objReferral.Status == Components.JudicialReferral.Statuses.Complete)
            {
                if (objReferral.CounselAssistance)
                    objReferral.Status = Components.JudicialReferral.Statuses.ReferredToCounsel;
                else
                    objReferral.Status = Components.JudicialReferral.Statuses.RetainedByJudge;
            }
            else
                objReferral.Status = Components.JudicialReferral.Statuses.Complete;
            ctl.UpdateReferral(objReferral);
            Response.Redirect(_navigationManager.NavigateURL(), true);
        }
    }
}