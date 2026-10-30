/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using tjc.Modules.CourtCalendar.Components;

namespace tjc.Modules.CourtCalendar
{
    public partial class ManageJudges : CourtCalendarModuleBase
    {
        private JudgeController Judges { get { return new JudgeController(_hostSettings); } }

        private string JudgeRoleName { get { return Settings.Contains(Settings_JudgeRole) ? Convert.ToString(Settings[Settings_JudgeRole]) : string.Empty; } }
        private string JaRoleName { get { return Settings.Contains(Settings_JaRole) ? Convert.ToString(Settings[Settings_JaRole]) : string.Empty; } }
        private const string Settings_JudgeRole = "JudgeRole";
        private const string Settings_JaRole = "JaRole";

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
            txtJudgeUser.Attributes["list"] = "dlJudgeUsers_" + ModuleId;
            txtJaUser.Attributes["list"] = "dlJaUsers_" + ModuleId;
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

            BindUserDatalists();
            CheckRolesConfigured();

            if (judgeId > 0)
            {
                var j = Judges.GetJudge(judgeId);
                if (j == null) { BindList(); return; }
                txtFullName.Text = j.FullName;
                txtJudgeUser.Text = LabelForUser(j.JudgeUserId);
                txtJaUser.Text = LabelForUser(j.JaUserId);
                txtJaName.Text = j.JaName;
                txtPhone.Text = j.Phone;
                chkActive.Checked = j.Active;
            }
            else
            {
                txtFullName.Text = txtJudgeUser.Text = txtJaUser.Text = txtJaName.Text = txtPhone.Text = string.Empty;
                chkActive.Checked = true;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsAdmin) return;

            UserInfo judgeUser;
            if (!ResolveUser(txtJudgeUser.Text, JudgeRoleName, out judgeUser))
            {
                ltMessage.Text = Alert("Select a judge from the list.", "danger");
                ShowEdit(ParseId(hfJudgeId.Value));
                return;
            }
            UserInfo jaUser;
            if (!ResolveUser(txtJaUser.Text, JaRoleName, out jaUser))
            {
                ltMessage.Text = Alert("Select a JA from the list.", "danger");
                ShowEdit(ParseId(hfJudgeId.Value));
                return;
            }

            int judgeId = ParseId(hfJudgeId.Value);
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

        // ---- User pickers (by role) ----

        private IEnumerable<UserInfo> RoleMembers(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName)) return Enumerable.Empty<UserInfo>();
            var members = RoleController.Instance.GetUsersByRole(PortalId, roleName);
            if (members == null) return Enumerable.Empty<UserInfo>();
            return members.Cast<UserInfo>().OrderBy(u => u.DisplayName, StringComparer.CurrentCultureIgnoreCase);
        }

        private void BindUserDatalists()
        {
            ltJudgeUsers.Text = BuildOptions(RoleMembers(JudgeRoleName));
            ltJaUsers.Text = BuildOptions(RoleMembers(JaRoleName));
        }

        private static string BuildOptions(IEnumerable<UserInfo> users)
        {
            var sb = new StringBuilder();
            foreach (var u in users)
                sb.AppendFormat("<option value=\"{0}\"></option>", HttpUtility.HtmlAttributeEncode(UserLabel(u)));
            return sb.ToString();
        }

        /// <summary>
        /// Resolves the typed label back to a user within the given role. Returns
        /// true when the text is empty (no selection) or matches a role member;
        /// false when the text is non-empty but matches no one.
        /// </summary>
        private bool ResolveUser(string text, string roleName, out UserInfo user)
        {
            user = null;
            if (string.IsNullOrWhiteSpace(text)) return true;
            var target = text.Trim();
            user = RoleMembers(roleName).FirstOrDefault(u => string.Equals(UserLabel(u), target, StringComparison.CurrentCultureIgnoreCase));
            return user != null;
        }

        private string LabelForUser(int userId)
        {
            if (userId <= 0) return string.Empty;
            var u = UserController.GetUserById(PortalId, userId);
            return u != null ? UserLabel(u) : string.Empty;
        }

        private static string UserLabel(UserInfo u)
        {
            return u.DisplayName + " (" + u.Username + ")";
        }

        private void CheckRolesConfigured()
        {
            if (string.IsNullOrWhiteSpace(JudgeRoleName) || string.IsNullOrWhiteSpace(JaRoleName))
                ltMessage.Text = Alert("Set the Judge and JA roles in this module's Settings so the pickers can list users.", "info");
        }

        private static int ParseId(string value)
        {
            int v; int.TryParse(value, out v); return v;
        }

        private static string Alert(string message, string type)
        {
            return string.Format("<div class=\"alert alert-{0}\">{1}</div>", type,
                System.Web.HttpUtility.HtmlEncode(message));
        }
    }
}
