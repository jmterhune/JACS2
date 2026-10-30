/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using tjc.Modules.CourtCalendar.Components;

namespace tjc.Modules.CourtCalendar
{
    public partial class ManageDivisions : CourtCalendarModuleBase
    {
        private DivisionController Divisions { get { return new DivisionController(_hostSettings); } }
        private JudgeController Judges { get { return new JudgeController(_hostSettings); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsAdmin)
            {
                pnlList.Visible = false;
                pnlEdit.Visible = false;
                ltMessage.Text = Alert("You do not have permission to manage divisions.", "warning");
                return;
            }
            lnkHome.NavigateUrl = HomeUrl;
            if (!IsPostBack)
            {
                if (DivisionIdParam > 0) ShowEdit(DivisionIdParam);
                else BindList();
            }
        }

        protected string LayoutName(int typeId)
        {
            return typeId == (int)CalendarType.RowHeader ? "Row headers" : "Weekday headers";
        }

        private void BindList()
        {
            pnlList.Visible = true;
            pnlEdit.Visible = false;
            rptDivisions.DataSource = Divisions.GetDivisions();
            rptDivisions.DataBind();
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            ShowEdit(0);
        }

        protected void rptDivisions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            switch (e.CommandName)
            {
                case "edit":
                    ShowEdit(id);
                    break;
                case "categories":
                    Response.Redirect(ManageCategoriesUrl(id), false);
                    break;
                case "delete":
                    Divisions.DeleteDivision(id);
                    ltMessage.Text = Alert("Division deleted.", "success");
                    BindList();
                    break;
            }
        }

        private void BindJudges(int selectedJudgeId)
        {
            ddlJudge.Items.Clear();
            ddlJudge.Items.Add(new ListItem("-- none --", "0"));
            foreach (var j in Judges.GetJudges())
                ddlJudge.Items.Add(new ListItem(j.FullName, j.JudgeId.ToString()));
            var sel = ddlJudge.Items.FindByValue(selectedJudgeId.ToString());
            if (sel != null) ddlJudge.SelectedValue = selectedJudgeId.ToString();
        }

        private void SelectCounty(string county)
        {
            var item = ddlCounty.Items.FindByValue(county ?? string.Empty);
            ddlCounty.SelectedValue = item != null ? (county ?? string.Empty) : string.Empty;
        }

        private void ShowEdit(int divisionId)
        {
            pnlList.Visible = false;
            pnlEdit.Visible = true;
            hfDivisionId.Value = divisionId.ToString();
            ltEditTitle.Text = divisionId > 0 ? "Edit division" : "Add division";

            if (divisionId > 0)
            {
                var d = Divisions.GetDivision(divisionId);
                if (d == null) { BindList(); return; }
                BindJudges(d.JudgeId);
                txtName.Text = d.Name;
                SelectCounty(d.County);
                ddlType.SelectedValue = d.CalendarTypeId.ToString();
                txtCourtroom.Text = d.Courtroom;
                txtSlug.Text = d.Slug;
                txtZoomId.Text = d.ZoomMeetingId;
                txtZoomPwd.Text = d.ZoomPassword;
                txtJaPhone.Text = d.JaPhone;
                txtEmail.Text = d.DivisionEmail;
                txtSortOrder.Text = d.SortOrder.ToString();
                chkActive.Checked = d.Active;
            }
            else
            {
                BindJudges(0);
                txtName.Text = txtCourtroom.Text = txtSlug.Text = string.Empty;
                SelectCounty(string.Empty);
                txtZoomId.Text = txtZoomPwd.Text = txtJaPhone.Text = txtEmail.Text = string.Empty;
                txtSortOrder.Text = "0";
                ddlType.SelectedValue = "1";
                chkActive.Checked = true;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsAdmin) return;
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ltMessage.Text = Alert("Division name is required.", "danger");
                return;
            }

            int divisionId;
            int.TryParse(hfDivisionId.Value, out divisionId);
            var d = divisionId > 0 ? Divisions.GetDivision(divisionId) : new Division();
            if (d == null) d = new Division();

            int judgeId;
            int.TryParse(ddlJudge.SelectedValue, out judgeId);
            var judge = judgeId > 0 ? Judges.GetJudge(judgeId) : null;

            d.Name = txtName.Text.Trim();
            d.County = ddlCounty.SelectedValue;
            d.JudgeId = judgeId;
            d.JudgeName = judge != null ? judge.FullName : string.Empty;
            int typeId; int.TryParse(ddlType.SelectedValue, out typeId);
            d.CalendarTypeId = typeId == 0 ? 1 : typeId;
            d.Courtroom = txtCourtroom.Text.Trim();
            d.ZoomMeetingId = txtZoomId.Text.Trim();
            d.ZoomPassword = txtZoomPwd.Text.Trim();
            d.JaPhone = txtJaPhone.Text.Trim();
            d.DivisionEmail = txtEmail.Text.Trim();
            int sort; int.TryParse(txtSortOrder.Text, out sort);
            d.SortOrder = sort;
            d.Active = chkActive.Checked;

            var slug = string.IsNullOrWhiteSpace(txtSlug.Text) ? Slugify(d.Name) : Slugify(txtSlug.Text);
            if (!Divisions.IsSlugAvailable(slug, divisionId))
            {
                ltMessage.Text = Alert("That URL slug is already in use. Choose another.", "danger");
                ShowEdit(divisionId);
                txtSlug.Text = slug;
                return;
            }
            d.Slug = slug;

            if (divisionId > 0) Divisions.UpdateDivision(d);
            else Divisions.CreateDivision(d); // also seeds built-in categories

            ltMessage.Text = Alert("Division saved.", "success");
            BindList();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            BindList();
        }

        private static string Slugify(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            var s = value.Trim().ToLowerInvariant();
            s = Regex.Replace(s, "[^a-z0-9]+", "-");
            s = s.Trim('-');
            return s;
        }

        private static string Alert(string message, string type)
        {
            return string.Format("<div class=\"alert alert-{0}\">{1}</div>", type,
                System.Web.HttpUtility.HtmlEncode(message));
        }
    }
}
