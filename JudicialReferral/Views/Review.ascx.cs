/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.JudicialReferral.Components.Controllers;
using tjc.Modules.JudicialReferral.Components.Models;

namespace tjc.Modules.JudicialReferral.Views
{
    public partial class Review : JudicialReferralModuleBase
    {
        private readonly JudgeReferralController ctl = new JudgeReferralController();
        private readonly AttachmentController attCtl = new AttachmentController();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    cmdCancel.NavigateUrl = HomeUrl;
                    PopulateJudgeList();
                    PopulateForm();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PopulateJudgeList()
        {
            var rCtl = new RoleController();
            var judgeList = rCtl.GetUsersByRole(PortalId, JudgeRole);
            var judges = new List<UserInfo>();
            foreach (UserInfo j in judgeList) judges.Add(j);
            drpJudge.AppendDataBoundItems = true;
            drpJudge.DataTextField = "DisplayName";
            drpJudge.DataValueField = "UserId";
            drpJudge.DataSource = judges.OrderBy(j => j.DisplayName).ToList();
            drpJudge.DataBind();
        }

        private void PopulateForm()
        {
            if (IsJudge) hdIsJudge.Value = "1";

            if (ReferralID <= 0)
            {
                pnlJA.Enabled = true;
                pnlJudge.Visible = false;
                return;
            }

            var objReferral = ctl.GetReferral(ReferralID);
            if (objReferral == null)
            {
                pnlJudge.Enabled = false;
                pnlJA.Enabled = false;
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Unable to find requested record",
                    DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                return;
            }

            var files = attCtl.GetAttachmentsByReferral(objReferral.ReferralId).ToList();
            rptFiles.DataSource = files;
            rptFiles.DataBind();
            if (files.Count < 1)
            {
                rptFiles.Visible = false;
                ltAttachments.Text = "No Attachment";
            }

            PopulateCaseNumberFields(objReferral.CaseNumber);
            txtCaseParties.Text = objReferral.CaseParties;
            if (objReferral.MotionDate.HasValue)
                txtMotionDate.Text = objReferral.MotionDate.Value.ToString("yyyy-MM-dd");
            txtMotionTitle.Text = objReferral.MotionTitle;

            var judgeItem = drpJudge.Items.FindByValue(objReferral.JudgeId.ToString());
            if (judgeItem != null) drpJudge.SelectedValue = objReferral.JudgeId.ToString();

            if (!string.IsNullOrEmpty(objReferral.DirectedMotionsCriminal))
            {
                var motionList = objReferral.DirectedMotionsCriminal.Split('|');
                foreach (var s in motionList)
                {
                    var value = clsMotionList.Items.FindByValue(s);
                    if (value != null) value.Selected = true;
                }
            }

            rblDivisions.SelectedValue = objReferral.SelectedDivision.ToString();
            if (rblDivisions.SelectedIndex < 0) rblDivisions.SelectedValue = "0";

            switch (objReferral.SelectedDivision)
            {
                case 0:
                    chkStatusOrder.Checked = objReferral.StatusOrderCriminal;
                    if (objReferral.StatusOrderCriminalFiled.HasValue)
                        txtStatusOrderFiled.Text = objReferral.StatusOrderCriminalFiled.Value.ToString("yyyy-MM-dd");
                    chkMotionVacate.Checked = objReferral.MotionVacateCriminal;
                    chkMotionCorrect.Checked = objReferral.MotionCorrectCriminal;
                    if (objReferral.MotionCorrectCriminalFiled.HasValue)
                        txtMotionCorrectFiled.Text = objReferral.MotionCorrectCriminalFiled.Value.ToString("yyyy-MM-dd");
                    chkMotionDirected.Checked = objReferral.MotionDirectedCriminal;
                    chkOtherPostconviction.Checked = objReferral.OtherMotionCriminal;
                    txtPostconvictionCriminal.Text = objReferral.OtherMotionCriminalText;
                    chkPretrialCriminal.Checked = objReferral.PretrialMotionCriminal;
                    txtPretrialCriminal.Text = objReferral.PretrialMotionCriminalText;
                    chkResearchCriminal.Checked = objReferral.ResearchCriminal;
                    txtResearchCriminal.Text = objReferral.ResearchCriminalText;
                    break;
                case 1:
                    chkDismissCivil.Checked = objReferral.MotionDismissCivil;
                    chkSummaryJudgementCivil.Checked = objReferral.MotionSummaryJudgementCivil;
                    chkCompelDiscoveryCivil.Checked = objReferral.MotionDiscoveryCivil;
                    chkAttorneyFeesCivil.Checked = objReferral.MotionAttorneyFeeCivil;
                    chkPretrialCivil.Checked = objReferral.OtherMotionCivil;
                    txtPretrialCivil.Text = objReferral.OtherMotionCivilText;
                    chkResearchCivil.Checked = objReferral.ResearchMotionCivil;
                    txtResearchCivil.Text = objReferral.ResearchMotionCivilText;
                    break;
                case 2:
                    chkModifyTimeshareFamily.Checked = objReferral.PetitionTimeShareFamily;
                    chkModifySupportFamily.Checked = objReferral.PetitionChildSupportFamily;
                    chkCompelDiscoveryFamily.Checked = objReferral.MotionDiscoveryFamily;
                    chkAttorneyFeesFamily.Checked = objReferral.MotionAttorneyFeeFamily;
                    chkPretrialFamily.Checked = objReferral.OtherMotionFamily;
                    txtPretrialFamily.Text = objReferral.OtherMotionFamilyText;
                    chkResearchFamily.Checked = objReferral.ResearchMotionFamily;
                    txtResearchFamily.Text = objReferral.ResearchMotionFamilyText;
                    break;
                case 3:
                    txtAppeals.Text = objReferral.TypeOfAppeal;
                    break;
            }

            if (objReferral.RequestedCompletionDate.HasValue)
                txtRequestedCompletionDate.Text = objReferral.RequestedCompletionDate.Value.ToString("yyyy-MM-dd");

            cmdSave.Visible = false;
            pnlJudge.Enabled = false;

            if (objReferral.Status != (int)Statuses.Completed)
            {
                if (IsJudge)
                {
                    pnlJudge.Enabled = true;
                    cmdSave.Visible = true;
                }
                else if (IsJa && objReferral.Status == (int)Statuses.NewReferral)
                {
                    cmdSave.Visible = true;
                    cmdComplete.Visible = false;
                }
            }

            if (objReferral.CounselAssistance)
            {
                chkYes.Checked = true;
                if (!string.IsNullOrEmpty(objReferral.JudgeMotions))
                {
                    var judgeMotions = objReferral.JudgeMotions.Split('|');
                    foreach (var s in judgeMotions)
                    {
                        var item = clsResponse.Items.FindByText(s);
                        if (item != null)
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            if (clsResponse.Items.Count > 4)
                                clsResponse.Items[4].Selected = true;
                            txtOther.Text += s;
                        }
                    }
                }
            }
            else if (objReferral.Status == (int)Statuses.RetainedByJudge)
            {
                chkNo.Checked = true;
            }

            if (objReferral.Status == (int)Statuses.Completed)
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

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var objReferral = ctl.GetReferral(ReferralID);
            if (objReferral == null) return;

            objReferral.CaseParties = txtCaseParties.Text;
            objReferral.CaseNumber = GetCaseNumber();

            DateTime parsed;
            if (DateTime.TryParse(txtMotionDate.Text, out parsed))
                objReferral.MotionDate = parsed;

            objReferral.MotionTitle = txtMotionTitle.Text;
            if (!string.IsNullOrEmpty(drpJudge.SelectedValue))
                objReferral.JudgeId = int.Parse(drpJudge.SelectedValue);

            int selectedDivision = 0;
            int.TryParse(rblDivisions.SelectedValue, out selectedDivision);
            objReferral.SelectedDivision = selectedDivision;

            switch (objReferral.SelectedDivision)
            {
                case 0:
                    objReferral.StatusOrderCriminal = chkStatusOrder.Checked;
                    if (DateTime.TryParse(txtStatusOrderFiled.Text, out parsed))
                        objReferral.StatusOrderCriminalFiled = parsed;
                    objReferral.MotionVacateCriminal = chkMotionVacate.Checked;
                    objReferral.MotionCorrectCriminal = chkMotionCorrect.Checked;
                    if (DateTime.TryParse(txtMotionCorrectFiled.Text, out parsed))
                        objReferral.MotionCorrectCriminalFiled = parsed;
                    objReferral.MotionDirectedCriminal = chkMotionDirected.Checked;
                    var directed = new List<string>();
                    foreach (ListItem item in clsMotionList.Items)
                        if (item.Selected) directed.Add(item.Value);
                    objReferral.DirectedMotionsCriminal = string.Join("|", directed);
                    objReferral.OtherMotionCriminal = chkOtherPostconviction.Checked;
                    objReferral.OtherMotionCriminalText = txtPostconvictionCriminal.Text;
                    objReferral.PretrialMotionCriminal = chkPretrialCriminal.Checked;
                    objReferral.PretrialMotionCriminalText = txtPretrialCriminal.Text;
                    objReferral.ResearchCriminal = chkResearchCriminal.Checked;
                    objReferral.ResearchCriminalText = txtResearchCriminal.Text;
                    break;
                case 1:
                    objReferral.MotionDismissCivil = chkDismissCivil.Checked;
                    objReferral.MotionSummaryJudgementCivil = chkSummaryJudgementCivil.Checked;
                    objReferral.MotionDiscoveryCivil = chkCompelDiscoveryCivil.Checked;
                    objReferral.MotionAttorneyFeeCivil = chkAttorneyFeesCivil.Checked;
                    objReferral.OtherMotionCivil = chkPretrialCivil.Checked;
                    objReferral.OtherMotionCivilText = txtPretrialCivil.Text;
                    objReferral.ResearchMotionCivil = chkResearchCivil.Checked;
                    objReferral.ResearchMotionCivilText = txtResearchCivil.Text;
                    break;
                case 2:
                    objReferral.PetitionTimeShareFamily = chkModifyTimeshareFamily.Checked;
                    objReferral.PetitionChildSupportFamily = chkModifySupportFamily.Checked;
                    objReferral.MotionDiscoveryFamily = chkCompelDiscoveryFamily.Checked;
                    objReferral.MotionAttorneyFeeFamily = chkAttorneyFeesFamily.Checked;
                    objReferral.OtherMotionFamily = chkPretrialFamily.Checked;
                    objReferral.OtherMotionFamilyText = txtPretrialFamily.Text;
                    objReferral.ResearchMotionFamily = chkResearchFamily.Checked;
                    objReferral.ResearchMotionFamilyText = txtResearchFamily.Text;
                    break;
                case 3:
                    objReferral.TypeOfAppeal = txtAppeals.Text;
                    break;
            }

            if (objReferral.Status <= (int)Statuses.ReferredToCounsel && IsJa)
            {
                ctl.UpdateReferral(objReferral);
                Response.Redirect(HomeUrl);
                return;
            }

            if (objReferral.Status != (int)Statuses.Completed && IsJudge)
            {
                objReferral.CounselAssistance = chkYes.Checked;
                objReferral.JudgeResponseDate = DateTime.Now;
                if (DateTime.TryParse(txtRequestedCompletionDate.Text, out parsed))
                    objReferral.RequestedCompletionDate = parsed;

                var responses = new List<string>();
                bool otherSelected = false;
                foreach (ListItem item in clsResponse.Items)
                {
                    if (item.Selected && !item.Value.ToLower().Equals("other"))
                        responses.Add(item.Value);
                    if (item.Value == "other") otherSelected = item.Selected;
                }
                if (otherSelected && !string.IsNullOrEmpty(txtOther.Text))
                    responses.Add(txtOther.Text);
                objReferral.JudgeMotions = string.Join("|", responses);

                if (chkYes.Checked)
                {
                    objReferral.Status = (int)Statuses.ReferredToCounsel;
                    ctl.UpdateReferral(objReferral);
                    SendToCounsel(objReferral);
                }
                else
                {
                    objReferral.Status = (int)Statuses.RetainedByJudge;
                    ctl.UpdateReferral(objReferral);
                }
                Response.Redirect(HomeUrl);
            }
        }

        private void SendToCounsel(JudgeReferralInfo objReferral)
        {
            var objJudge = UserController.GetUserById(PortalId, objReferral.JudgeId);
            if (objJudge == null) return;
            string emailFrom = objJudge.Email;
            string toEmail = CourtCounselEmail;
            string subject = "Judicial Referral Request";
            string body = string.Format(
                "<p>Please review the <a href='{0}'>Judicial Referral Request</a> for case number {1}.</p>",
                EditUrl("rid", objReferral.ReferralId.ToString(), "editlog"),
                objReferral.CaseNumber);
            Mail.SendEmail(emailFrom, toEmail, subject, body);
        }

        protected void cmdComplete_Click(object sender, EventArgs e)
        {
            var objReferral = ctl.GetReferral(ReferralID);
            if (objReferral == null) return;

            if (objReferral.Status == (int)Statuses.Completed)
            {
                objReferral.Status = objReferral.CounselAssistance
                    ? (int)Statuses.ReferredToCounsel
                    : (int)Statuses.RetainedByJudge;
            }
            else
            {
                objReferral.Status = (int)Statuses.Completed;
            }
            ctl.UpdateStatus(objReferral.ReferralId, objReferral.Status);
            Response.Redirect(HomeUrl);
        }

        private void PopulateCaseNumberFields(string caseNumber)
        {
            if (string.IsNullOrEmpty(caseNumber)) return;
            var parts = caseNumber.Split('-');
            if (parts.Length >= 1)
            {
                var item = drpCountyLetter.Items.FindByValue(parts[0]);
                if (item != null) drpCountyLetter.SelectedValue = parts[0];
            }
            if (parts.Length >= 2) txtCaseYear.Text = parts[1];
            if (parts.Length >= 3) txtCaseType.Text = parts[2];
            if (parts.Length >= 4) txtCaseSequence.Text = parts[3];
            if (parts.Length >= 5) txtDefendantSuffix.Text = string.Join("-", parts.Skip(4));
        }

        private string GetCaseNumber()
        {
            string county = drpCountyLetter.SelectedValue ?? string.Empty;
            string year = (txtCaseYear.Text ?? string.Empty).Trim();
            string type = (txtCaseType.Text ?? string.Empty).Trim().ToUpper();
            string sequence = (txtCaseSequence.Text ?? string.Empty).Trim();
            string suffix = (txtDefendantSuffix.Text ?? string.Empty).Trim().ToUpper();

            int seqInt;
            if (int.TryParse(new string(sequence.Where(char.IsDigit).ToArray()), out seqInt))
                sequence = seqInt.ToString("000000");

            string result = string.Format("{0}-{1}-{2}-{3}", county, year, type, sequence);
            if (!string.IsNullOrWhiteSpace(suffix))
                result += "-" + suffix;
            return result;
        }
    }
}
