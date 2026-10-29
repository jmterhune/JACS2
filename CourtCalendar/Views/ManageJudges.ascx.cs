/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System;
using DotNetNuke.Entities.Users;
using tjc.Modules.CourtCalendar.Components;

namespace tjc.Modules.CourtCalendar
{
    public partial class ManageJudges : CourtCalendarModuleBase
    {
        private JudgeController Judges { get { return new JudgeController(_hostSettings); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsAdmin)
            {
                pnlList.Visible = false;
                pnlEdit.Visible = false;
                ltMessage.Text = Alert("You do not have permission to manage judges.", "warning");
                return;
            }
            lnkHome.NavigateUrl = HomeUrl;
            if (!IsPostBack)
            {
                if (JudgeIdParam > 0) ShowEdit(JudgeIdParam);
                else BindList();
            }
        }

        private void BindList()
        {
            pnlList.Visible = true;
            pnlEdit.Visible = false;
            rptJudges.DataSource = Judges.GetJudges();
            rptJudges.DataBind();
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            ShowEdit(0);
        }

        protected void rptJudges_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            if (e.CommandName == "edit")
            {
                ShowEdit(id);
            }
            else if (e.CommandName == "delete")
            {
                Judges.DeleteJudge(id);
                ltMessage.Text = Alert("Judge deleted.", "success");
                BindList();
            }
        }

        private void ShowEdit(int judgeId)
        {
            pnlList.Visible = false;
            pnlEdit.Visible = true;
            hfJudgeId.Value = judgeId.ToString();
            ltEditTitle.Text = judgeId > 0 ? "Edit judge" : "Add judge";

            if (judgeId > 0)
            {
                var j = Judges.GetJudge(judgeId);
                if (j == null) { BindList(); return; }
                txtFullName.Text = j.FullName;
                txtJudgeUsername.Text = UsernameFor(j.JudgeUserId);
                txtJaUsername.Text = UsernameFor(j.JaUserId);
                txtJaName.Text = j.JaName;
                txtPhone.Text = j.Phone;
                chkActive.Checked = j.Active;
            }
            else
            {
                txtFullName.Text = txtJudgeUsername.Text = txtJaUsername.Text = txtJaName.Text = txtPhone.Text = string.Empty;
                chkActive.Checked = true;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsAdmin) return;

            var judgeUser = ResolveUser(txtJudgeUsername.Text);
            if (!string.IsNullOrWhiteSpace(txtJudgeUsername.Text) && judgeUser == null)
            {
                ltMessage.Text = Alert("Judge username not found on this site.", "danger");
                return;
            }
            var jaUser = ResolveUser(txtJaUsername.Text);
            if (!string.IsNullOrWhiteSpace(txtJaUsername.Text) && jaUser == null)
            {
                ltMessage.Text = Alert("JA username not found on this site.", "danger");
                return;
            }

            int judgeId;
            int.TryParse(hfJudgeId.Value, out judgeId);
            var j = judgeId > 0 ? Judges.GetJudge(judgeId) : new Judge();
            if (j == null) j = new Judge();

            j.FullName = string.IsNullOrWhiteSpace(txtFullName.Text) && judgeUser != null
                ? judgeUser.DisplayName : txtFullName.Text.Trim();
            j.JudgeUserId = judgeUser != null ? judgeUser.UserID : 0;
            j.JaUserId = jaUser != null ? jaUser.UserID : 0;
            j.JaName = string.IsNullOrWhiteSpace(txtJaName.Text) && jaUser != null
                ? jaUser.DisplayName : txtJaName.Text.Trim();
            j.Phone = txtPhone.Text.Trim();
            j.Active = chkActive.Checked;

            if (judgeId > 0) Judges.UpdateJudge(j); else Judges.CreateJudge(j);

            ltMessage.Text = Alert("Judge saved.", "success");
            BindList();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            BindList();
        }

        private UserInfo ResolveUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            return UserController.GetUserByName(PortalId, username.Trim());
        }

        private string UsernameFor(int userId)
        {
            if (userId <= 0) return string.Empty;
            var u = UserController.GetUserById(PortalId, userId);
            return u != null ? u.Username : string.Empty;
        }

        private static string Alert(string message, string type)
        {
            return string.Format("<div class=\"alert alert-{0}\">{1}</div>", type,
                System.Web.HttpUtility.HtmlEncode(message));
        }
    }
}
