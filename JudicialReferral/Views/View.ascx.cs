/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
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
                    txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    txtStartDate.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");

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
            if (DateTime.TryParse(txtStartDate.Text, out parsed))
                startDate = parsed;
            if (DateTime.TryParse(txtEndDate.Text, out parsed))
                endDate = parsed;
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

        protected void rptReferral_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "complete")
            {
                int rid = Convert.ToInt32(e.CommandArgument);
                if (rid > 0)
                {
                    ctl.UpdateStatus(rid, (int)Statuses.Completed);
                    SearchList();
                }
            }
        }

        protected void rptReferral_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var req = (JudgeReferralInfo)e.Item.DataItem;
                if (req == null) return;

                var lnkReview = (HyperLink)e.Item.FindControl("lnkReview");
                var cmdComplete = (LinkButton)e.Item.FindControl("cmdComplete");

                if (lnkReview != null)
                {
                    lnkReview.NavigateUrl = EditUrl("rid", req.ReferralId.ToString(), "review");
                }

                if (UserInfo.IsInRole(CounselRole))
                {
                    if (cmdComplete != null) cmdComplete.Visible = true;
                    if (lnkReview != null)
                    {
                        lnkReview.NavigateUrl = EditUrl("rid", req.ReferralId.ToString(), "editlog");
                        if (req.Status != (int)Statuses.ReferredToCounsel && req.Status != (int)Statuses.ReceivedAssigned)
                        {
                            lnkReview.Visible = false;
                            if (cmdComplete != null) cmdComplete.Visible = false;
                        }
                    }
                }
            }
        }
    }
}
