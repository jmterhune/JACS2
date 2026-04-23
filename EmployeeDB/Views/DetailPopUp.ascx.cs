/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using System;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Controllers;

namespace tjc.Modules.EmployeeDB.Views
{
    public partial class DetailPopUp : EmployeeDBModuleBase
    {
        private readonly EmployeeController _employees = new EmployeeController();
        private readonly PhoneController _phones = new PhoneController();
        private readonly GroupController _groups = new GroupController();
        private readonly OfficeLocationController _locations = new OfficeLocationController();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    LoadEmployee();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void LoadEmployee()
        {
            var eid = -1;
            var qs = Request.QueryString["eid"];
            if (!string.IsNullOrEmpty(qs) && int.TryParse(qs, out int parsed))
                eid = parsed;

            if (eid <= 0)
            {
                pnlNotFound.Visible = true;
                return;
            }

            var emp = _employees.GetEmployee(eid);
            if (emp == null)
            {
                pnlNotFound.Visible = true;
                return;
            }

            ltName.Text = string.Format("{0} {1} {2}",
                emp.FirstName,
                string.IsNullOrEmpty(emp.MiddleInitial) ? "" : emp.MiddleInitial + ".",
                emp.LastName).Replace("  ", " ").Trim();

            ltTitle.Text = emp.JobTitle;

            if (emp.DepartmentId.HasValue)
            {
                var dept = _groups.GetById(emp.DepartmentId.Value);
                if (dept != null) ltDepartment.Text = dept.GroupName;
            }

            var locName = emp.LocationName;
            if (string.IsNullOrEmpty(locName) && emp.OfficeLocationId.HasValue)
            {
                var loc = _locations.GetById(emp.OfficeLocationId.Value);
                if (loc != null) locName = loc.Description;
            }
            ltLocation.Text = locName;

            if (!string.IsNullOrEmpty(emp.Email))
            {
                ltEmail.Text = string.Format("<a href=\"mailto:{0}\">{0}</a>", emp.Email);
            }

            // Photo
            if (emp.PhotoFileId.HasValue && emp.PhotoFileId.Value > 0)
            {
                var file = FileManager.Instance.GetFile(emp.PhotoFileId.Value);
                if (file != null)
                {
                    imgPhoto.ImageUrl = FileManager.Instance.GetUrl(file);
                }
                else
                {
                    imgPhoto.Visible = false;
                }
            }
            else
            {
                imgPhoto.Visible = false;
            }

            // Work phones
            var phones = _phones.GetWorkPhonesForEmployee(emp.EmployeeId)
                .OrderByDescending(p => p.IsMain)
                .Select(p => new
                {
                    p.PhoneType,
                    p.PhoneNumber,
                    DisplayNumber = string.IsNullOrEmpty(p.Extension)
                        ? p.PhoneNumber
                        : p.PhoneNumber + " x" + p.Extension
                })
                .ToList();

            rptPhones.DataSource = phones;
            rptPhones.DataBind();
        }
    }
}
