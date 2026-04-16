/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.JudicialReferral.Components.Controllers;
using tjc.Modules.JudicialReferral.Components.Models;

namespace tjc.Modules.JudicialReferral.Views
{
    public partial class Referral : JudicialReferralModuleBase
    {
        private readonly JudgeReferralController ctl = new JudgeReferralController();
        private readonly AttachmentController attCtl = new AttachmentController();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (UserId <= 0)
                    {
                        Response.Redirect(HomeUrl);
                        return;
                    }
                    if (!UserInfo.IsInRole(JaRole) && !UserInfo.IsInRole(JudgeRole))
                    {
                        Response.Redirect(HomeUrl);
                        return;
                    }
                    if (UserInfo.IsInRole(JudgeRole))
                    {
                        hdJudge.Value = "1";
                    }
                    cmdCancel.NavigateUrl = HomeUrl;
                    PopulateJudgeList();
                    EnsureTargetFolder();
                    txtYear.Attributes.Add("placeholder", DateTime.Now.Year.ToString());
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void EnsureTargetFolder()
        {
            var dCtl = FolderManager.Instance;
            if (!dCtl.FolderExists(PortalId, TargetFolder))
            {
                dCtl.AddFolder(PortalId, TargetFolder);
            }
        }

        private void PopulateJudgeList()
        {
            var rCtl = new RoleController();
            var judgeList = rCtl.GetUsersByRole(PortalId, JudgeRole);
            var judges = new List<UserInfo>();
            foreach (UserInfo j in judgeList)
            {
                judges.Add(j);
            }
            drpJudge.AppendDataBoundItems = true;
            drpJudge.DataTextField = "DisplayName";
            drpJudge.DataValueField = "UserId";
            drpJudge.DataSource = judges.OrderBy(j => j.DisplayName).ToList();
            drpJudge.DataBind();
            if (UserInfo.IsInRole(JudgeRole))
            {
                drpJudge.SelectedValue = UserId.ToString();
                cmdSave.Text = "Submit";
            }
        }

        private void UploadFiles(int referralId)
        {
            try
            {
                if (fuAttachments.HasFiles)
                {
                    var fCtl = FileManager.Instance;
                    var dCtl = FolderManager.Instance;
                    var folder = dCtl.GetFolder(PortalId, TargetFolder);

                    foreach (var f in fuAttachments.PostedFiles)
                    {
                        try
                        {
                            var fileInfo = fCtl.AddFile(folder, System.IO.Path.GetFileName(f.FileName), f.InputStream);
                            var objAtt = new AttachmentInfo
                            {
                                FileID = fileInfo.FileId,
                                FileName = System.IO.Path.GetFileName(f.FileName),
                                ReferralID = referralId,
                                Path = fileInfo.RelativePath
                            };
                            attCtl.AddAttachment(objAtt);
                        }
                        catch
                        {
                            // swallow per original behavior
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            DateTime motionDate = DateTime.MinValue;
            DateTime parsed;
            if (DateTime.TryParse(txtMotionDate.Text, out parsed))
                motionDate = parsed;

            int caseInt = 0;
            int.TryParse(txtCaseNumber.Text, out caseInt);

            string caseNumber = string.Format("{0}-{1}-{2}-{3:000000}",
                drpCounty.SelectedValue, txtYear.Text, txtCaseType.Text.ToUpper(), caseInt);

            var objReferral = new JudgeReferralInfo
            {
                JaID = UserId,
                JudgeId = int.Parse(drpJudge.SelectedValue),
                JaCreatedDate = DateTime.Now,
                CaseParties = txtCaseParties.Text,
                CaseNumber = caseNumber,
                MotionTitle = txtMotionTitle.Text,
                MotionDate = motionDate == DateTime.MinValue ? (DateTime?)null : motionDate,
                Status = (int)Statuses.NewReferral
            };

            int referralId = ctl.AddReferral(objReferral);
            UploadFiles(referralId);
            objReferral.ReferralId = referralId;

            if (UserInfo.IsInRole(JudgeRole))
            {
                Response.Redirect(EditUrl("rid", referralId.ToString(), "review"));
            }
            else
            {
                SendToJudge(objReferral);
                Response.Redirect(HomeUrl);
            }
        }

        private void SendToJudge(JudgeReferralInfo objReferral)
        {
            const string emailFrom = "noreply.intranet@jud12.flcourts.org";
            var user = UserController.GetUserById(PortalId, objReferral.JudgeId);
            if (user == null) return;
            string toEmail = user.Email;
            string subject = "New Judicial Referral Request";
            string body = string.Format(
                "<p>Please review the <a href='https://intranet{0}'>Judicial Referral Request</a> for case number {1}.</p>",
                EditUrl("rid", objReferral.ReferralId.ToString(), "review"),
                objReferral.CaseNumber);
            Mail.SendEmail(emailFrom, toEmail, subject, body);
        }
    }
}
