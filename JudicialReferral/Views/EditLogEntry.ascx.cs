/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.JudicialReferral.Components.Controllers;
using tjc.Modules.JudicialReferral.Components.Models;

namespace tjc.Modules.JudicialReferral.Views
{
    public partial class EditLogEntry : JudicialReferralModuleBase
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
                    BindLists();
                    if (ReferralID > 0)
                    {
                        var objReferral = ctl.GetReferral(ReferralID);
                        if (objReferral != null)
                        {
                            var judgeInfo = UserController.GetUserById(PortalId, objReferral.JudgeId);
                            if (judgeInfo != null)
                                txtJudge.Text = judgeInfo.DisplayName;
                            txtCaseName.Text = objReferral.CaseParties;
                            txtCaseNumber.Text = objReferral.CaseNumber;
                            if (objReferral.MotionDate.HasValue)
                                txtMotionFiled.Text = objReferral.MotionDate.Value.ToString("yyyy-MM-dd");
                            if (objReferral.JudgeResponseDate.HasValue)
                                txtReceived.Text = objReferral.JudgeResponseDate.Value.ToString("yyyy-MM-dd");

                            var files = attCtl.GetAttachmentsByReferral(objReferral.ReferralId).ToList();
                            rptFiles.DataSource = files;
                            rptFiles.DataBind();
                            if (files.Count < 1) rptFiles.Visible = false;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindLists()
        {
            BindSimple(drpCaseType, "SELECT CaseType AS Value FROM tjc_cc_casetype ORDER BY CaseType");
            BindSimple(drpCounty, "SELECT County AS Value FROM tjc_cc_county ORDER BY County");
            BindSimple(drpAction, "SELECT [Action] AS Value FROM tjc_cc_actiontaken ORDER BY [Action]");
            BindActiveInactive(drpRequestor,
                "SELECT RequestorName AS Value, IsActive FROM tjc_cc_requestor ORDER BY IsActive DESC, RequestorName");
            BindActiveInactive(drpAttorney,
                "SELECT AttorneyName AS Value, IsActive FROM tjc_cc_attorney ORDER BY IsActive DESC, AttorneyName");
            BindActiveInactive(drpTimeSpan,
                "SELECT TimeSpan AS Value, IsActive FROM tjc_cc_timespent ORDER BY IsActive DESC, TimeSpan");
        }

        private void BindSimple(DropDownList ddl, string sql)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var items = ctx.ExecuteQuery<StringRow>(CommandType.Text, sql).ToList();
                foreach (var r in items)
                {
                    if (!string.IsNullOrEmpty(r.Value))
                        ddl.Items.Add(new ListItem(r.Value, r.Value));
                }
            }
        }

        private void BindActiveInactive(DropDownList ddl, string sql)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var items = ctx.ExecuteQuery<ActiveRow>(CommandType.Text, sql).ToList();
                foreach (var r in items.Where(x => x.IsActive))
                {
                    ddl.Items.Add(new ListItem(r.Value, r.Value));
                }
                foreach (var r in items.Where(x => !x.IsActive))
                {
                    var li = new ListItem(r.Value, r.Value);
                    li.Attributes["class"] = "disabled";
                    li.Attributes["disabled"] = "disabled";
                    ddl.Items.Add(li);
                }
            }
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid) return;

                var objReferral = ctl.GetReferral(ReferralID);
                if (objReferral == null) return;

                DateTime motionFiled, dateReceived, dateCompleted;
                DateTime.TryParse(txtMotionFiled.Text, out motionFiled);
                DateTime.TryParse(txtReceived.Text, out dateReceived);
                DateTime.TryParse(txtDateCompleted.Text, out dateCompleted);

                string statusName = drpStatus.SelectedValue;
                if (dateReceived > DateTime.Now && ReferralID <= 0) statusName = "";

                if (dateCompleted != DateTime.MinValue && dateCompleted < dateReceived)
                    dateReceived = dateCompleted;

                // Insert into tjc_cc_history
                int logId;
                using (IDataContext ctx = DataContext.Instance())
                {
                    const string sql = @"
INSERT INTO tjc_cc_history
(DateReceived, CaseNumber, PartyName, CaseType, RequestedBy, Responsible, County, [Action],
 DateCompleted, TimeSpent, Comments, StatusName, MotionFiled, LastModifiedDate)
VALUES
(@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, GETDATE());
SELECT SCOPE_IDENTITY();";

                    var args = new object[]
                    {
                        dateReceived == DateTime.MinValue ? (object)DBNull.Value : dateReceived,
                        txtCaseNumber.Text.Trim(),
                        txtCaseName.Text.Trim(),
                        drpCaseType.SelectedValue,
                        drpRequestor.SelectedValue,
                        drpAttorney.SelectedValue,
                        drpCounty.SelectedValue,
                        drpAction.SelectedValue,
                        dateCompleted == DateTime.MinValue ? (object)DBNull.Value : dateCompleted,
                        drpTimeSpan.SelectedValue,
                        txtComments.Text,
                        statusName,
                        motionFiled == DateTime.MinValue ? (object)DBNull.Value : motionFiled
                    };

                    logId = Convert.ToInt32(ctx.ExecuteScalar<decimal>(CommandType.Text, sql, args));
                }

                if (logId > 0)
                {
                    objReferral.CounselReceivedDate = DateTime.Now;
                    objReferral.Status = (int)Statuses.ReceivedAssigned;
                    ctl.UpdateReferral(objReferral);
                    Response.Redirect(HomeUrl);
                }
                else
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Unable to add Log Entry.",
                        DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        // Internal DTOs for reading single/two-column result sets
        public class StringRow
        {
            public string Value { get; set; }
        }

        public class ActiveRow
        {
            public string Value { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
