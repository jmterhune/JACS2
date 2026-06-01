/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Controllers;

namespace tjc.Modules.EmployeeDB.Views
{
    public partial class SelectUserId : EmployeeDBModuleBase
    {
        private readonly EmployeeController _employees = new EmployeeController();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsHrAdmin)
                {
                    Response.Redirect(HomeUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                if (!IsPostBack)
                {
                    PopulateUsers();
                    LoadEmployee();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PopulateUsers()
        {
            var users = UserController.GetUsers(PortalId)
                .OfType<UserInfo>()
                .Where(u => !u.IsSuperUser && !u.IsDeleted)
                .OrderBy(u => u.DisplayName)
                .ToList();

            drpUsers.AppendDataBoundItems = true;
            drpUsers.DataTextField = "DisplayName";
            drpUsers.DataValueField = "UserID";
            drpUsers.DataSource = users;
            drpUsers.DataBind();
        }

        private void LoadEmployee()
        {
            if (EmployeeId <= 0)
            {
                pnlEmployee.Visible = false;
                return;
            }

            var emp = _employees.GetEmployee(EmployeeId);
            if (emp == null)
            {
                pnlEmployee.Visible = false;
                return;
            }

            ltEmployeeName.Text = string.Format("{0}, {1}", emp.LastName, emp.FirstName);

            if (emp.UserId.HasValue && emp.UserId.Value > 0)
            {
                var item = drpUsers.Items.FindByValue(emp.UserId.Value.ToString());
                if (item != null) item.Selected = true;
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (EmployeeId <= 0)
                {
                    ShowError("No employee context supplied.");
                    return;
                }

                int userId;
                if (!int.TryParse(drpUsers.SelectedValue, out userId) || userId <= 0)
                {
                    ShowError("Please select a user.");
                    return;
                }

                _employees.SetUserId(EmployeeId, userId, UserId);

                var script = "if (window.opener) { window.opener.location.reload(); } window.close();";
                Page.ClientScript.RegisterStartupScript(GetType(), "closeSelectUserId", script, true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            var script = "window.close();";
            Page.ClientScript.RegisterStartupScript(GetType(), "cancelSelectUserId", script, true);
        }

        private void ShowError(string message)
        {
            ltError.Text = message;
            pnlError.Visible = true;
        }
    }
}
