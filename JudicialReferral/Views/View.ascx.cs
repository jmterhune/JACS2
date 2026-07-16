/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.JudicialReferral.Components.Controllers;
using tjc.Modules.JudicialReferral.Components.Models;

namespace tjc.Modules.JudicialReferral.Views
{
    public partial class View : JudicialReferralModuleBase
    {
        private readonly JudgeReferralController ctl = new JudgeReferralController();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    cmdCancel.NavigateUrl = HomeUrl;
                    PopulateJudgeList();
                    dpEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    dpStartDate.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");

                    if (UserId > 0)
                    {
                        if ((UserInfo.IsInRole(JaRole) || UserInfo.IsInRole(JudgeRole)) && !UserInfo.IsSuperUser)
                        {
                            divJudge.Visible = false;
                        }
                    }
                    SearchList();
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
            foreach (UserInfo j in judgeList)
            {
                judges.Add(j);
            }
            drpJudge.AppendDataBoundItems = true;
            drpJudge.DataTextField = "DisplayName";
            drpJudge.DataValueField = "UserId";
            drpJudge.DataSource = judges.OrderBy(j => j.DisplayName).ToList();
            drpJudge.DataBind();
        }

        private void SearchList()
        {
            int judgeId = -1;
            string caseNumber = "";
            string motionTitle = "";
            DateTime? startDate = null;
            DateTime? endDate = null;
            int status = -1;

            DateTime parsed;
            if (DateTime.TryParse(dpStartDate.Text, out parsed)) startDate = parsed;
            if (DateTime.TryParse(dpEndDate.Text, out parsed)) endDate = parsed;
            if (!string.IsNullOrEmpty(drpJudge.SelectedValue))
                judgeId = int.Parse(drpJudge.SelectedValue);
            if (!string.IsNullOrEmpty(txtCaseNumber.Text))
                caseNumber = txtCaseNumber.Text;
            if (!string.IsNullOrEmpty(txtMotionTitle.Text))
                motionTitle = txtMotionTitle.Text;
            if (!string.IsNullOrEmpty(drpStatus.SelectedValue))
                status = int.Parse(drpStatus.SelectedValue);

            var list = ctl.GetFilteredReferrals(startDate, endDate, caseNumber, judgeId, motionTitle, status).ToList();

            // Populate JudgeName
            foreach (var r in list)
            {
                var judgeUser = UserController.GetUserById(PortalId, r.JudgeId);
                if (judgeUser != null)
                    r.JudgeName = judgeUser.DisplayName;
            }

            IEnumerable<JudgeReferralInfo> filtered = list;
            if (UserInfo.IsInRole(CounselRole) || UserInfo.IsSuperUser)
            {
                filtered = list;
            }
            else if (UserInfo.IsInRole(JaRole))
            {
                filtered = list.Where(x => x.JaID == UserId);
            }
            else if (UserInfo.IsInRole(JudgeRole))
            {
                filtered = list.Where(x => x.JudgeId == UserId);
            }

            rptReferral.DataSource = filtered.ToList();
            rptReferral.DataBind();
        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            SearchList();
        }

        protected void cmdReferral_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl("referral"));
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            // Ensure jQuery + the __RequestVerificationToken hidden input are emitted
            // so the SweetAlert2 status-update flow can call the WebAPI handler with
            // antiforgery protection.
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

            // Emit ModuleId + TabId for the JS to forward as request headers.
            // Without these, DnnApiController.ActiveModule returns null and the
            // status-update endpoint can't read module settings (CounselRole,
            // CourtCounselEmail).
            string ctx = string.Format(
                "window.__JR_ModuleContext = {{ moduleId: {0}, tabId: {1} }};",
                ModuleId, TabId);
            System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(),
                "JRModuleContext", ctx, true);
        }

        /// <summary>
        /// Emits the in-place "edit status" icon for the status column.
        /// Only rendered for users in the Court Counsel Admin role (or superusers);
        /// returns an empty string for everyone else, so the icon is invisible
        /// to Judge, JA, and regular Counsel users without any client-side hiding.
        /// </summary>
        protected string RenderStatusEditIcon()
        {
            if (UserId <= 0) return string.Empty;
            if (!IsCounselAdmin && !UserInfo.IsSuperUser) return string.Empty;
            return "<a href=\"javascript:void(0)\" class=\"status-edit ms-2\" title=\"Update status\"><i class=\"fas fa-edit\"></i></a>";
        }
    }
}
