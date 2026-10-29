/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using tjc.Modules.CourtCalendar.Components;

namespace tjc.Modules.CourtCalendar
{
    public partial class ManageCategories : CourtCalendarModuleBase
    {
        private static readonly Regex HexColor = new Regex("^#[0-9a-fA-F]{6}$", RegexOptions.Compiled);
        private CategoryController Categories { get { return new CategoryController(_hostSettings); } }

        private Division _division;
        private Division CurrentDivision
        {
            get
            {
                if (_division == null && DivisionIdParam > 0)
                    _division = new DivisionController(_hostSettings).GetDivision(DivisionIdParam);
                return _division;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var division = CurrentDivision;
            if (division == null || !CanManageDivision(division))
            {
                pnlMain.Visible = false;
                ltMessage.Text = Alert("You do not have permission to manage categories for this division.", "warning");
                return;
            }
            lnkHome.NavigateUrl = HomeUrl;
            ltDivision.Text = HttpUtility.HtmlEncode(division.Name);
            if (!IsPostBack) BindList();
        }

        protected string ColorSwatch(string color, bool isDefault)
        {
            if (isDefault || string.IsNullOrWhiteSpace(color))
                return "<span class=\"cc-swatch cc-swatch-default\" title=\"Light grey\"></span> <span class=\"text-muted\">Default</span>";
            var safe = HttpUtility.HtmlAttributeEncode(color.Trim());
            return string.Format("<span class=\"cc-swatch\" style=\"background:{0};\"></span> <code>{1}</code>",
                safe, HttpUtility.HtmlEncode(color.Trim()));
        }

        private void BindList()
        {
            pnlEdit.Visible = false;
            rptCategories.DataSource = Categories.GetCategories(DivisionIdParam);
            rptCategories.DataBind();
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            ShowEdit(0);
        }

        protected void rptCategories_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            if (e.CommandName == "edit") ShowEdit(id);
            else if (e.CommandName == "delete")
            {
                Categories.DeleteCategory(id);
                ltMessage.Text = Alert("Category removed.", "success");
                BindList();
            }
        }

        private void ShowEdit(int categoryId)
        {
            pnlEdit.Visible = true;
            hfCategoryId.Value = categoryId.ToString();
            bool isDefault = false;
            if (categoryId > 0)
            {
                var c = Categories.GetCategory(categoryId);
                if (c == null) { BindList(); return; }
                txtName.Text = c.Name;
                txtColor.Text = c.Color;
                chkActive.Checked = c.Active;
                txtName.Enabled = !c.IsSystem; // built-in names are fixed
                isDefault = c.IsDefault;
            }
            else
            {
                txtName.Text = txtColor.Text = string.Empty;
                txtName.Enabled = true;
                chkActive.Checked = true;
            }
            txtColor.Enabled = !isDefault;
            defaultNote.Visible = isDefault;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var division = CurrentDivision;
            if (division == null || !CanManageDivision(division)) return;

            int categoryId;
            int.TryParse(hfCategoryId.Value, out categoryId);
            var c = categoryId > 0 ? Categories.GetCategory(categoryId) : new Category { DivisionId = DivisionIdParam, Active = true };
            if (c == null) { BindList(); return; }

            var color = txtColor.Text.Trim();
            if (!c.IsDefault && color.Length > 0 && !HexColor.IsMatch(color))
            {
                ltMessage.Text = Alert("Color must be a hex value like #cfe8ff.", "danger");
                ShowEdit(categoryId);
                txtColor.Text = color;
                return;
            }

            if (!c.IsSystem)
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    ltMessage.Text = Alert("Category name is required.", "danger");
                    ShowEdit(categoryId);
                    return;
                }
                c.Name = txtName.Text.Trim();
            }
            c.Color = c.IsDefault ? null : (color.Length == 0 ? null : color);
            c.Active = chkActive.Checked;
            c.DivisionId = DivisionIdParam;

            if (categoryId > 0) Categories.UpdateCategory(c); else Categories.CreateCategory(c);
            ltMessage.Text = Alert("Category saved.", "success");
            BindList();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            BindList();
        }

        private static string Alert(string message, string type)
        {
            return string.Format("<div class=\"alert alert-{0}\">{1}</div>", type, HttpUtility.HtmlEncode(message));
        }
    }
}
