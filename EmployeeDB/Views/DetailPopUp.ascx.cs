/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using System;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Helpers;

namespace tjc.Modules.EmployeeDB.Views
{
    public partial class DetailPopUp : EmployeeDBModuleBase
    {
        private readonly EmployeeController _employees;
        private readonly PhoneController _phones;
        private readonly GroupController _groups;
        private readonly OfficeLocationController _locations;

        public DetailPopUp()
        {
            _employees = new EmployeeController(_hostSettings);
            _phones = new PhoneController(_hostSettings);
            _groups = new GroupController(_hostSettings);
            _locations = new OfficeLocationController(_hostSettings);
        }

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

            string locName = string.Empty;
            if (emp.OfficeLocationId.HasValue)
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
            if (emp.FileId.HasValue && emp.FileId.Value > 0)
            {
                var file = FileManager.Instance.GetFile(emp.FileId.Value);
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
                    // PhoneNumber is the raw-digits value used for the tel: link;
                    // DisplayNumber is the masked (999) 999-9999 form for humans.
                    p.PhoneNumber,
                    DisplayNumber = DisplayMask.PhoneWithExtension(p.PhoneNumber, p.Extension)
                })
                .ToList();

            rptPhones.DataSource = phones;
            rptPhones.DataBind();
        }
    }
}
