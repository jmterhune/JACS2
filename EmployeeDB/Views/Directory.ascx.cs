/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Views
{
    public partial class Directory : EmployeeDBModuleBase
    {
        private readonly EmployeeController _employees = new EmployeeController();
        private readonly GroupController _groups = new GroupController();
        private readonly CountyController _counties = new CountyController();
        private readonly PhoneController _phones = new PhoneController();
        private readonly OfficeLocationController _locations = new OfficeLocationController();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    PopulateDepartments();
                    PopulateCounties();
                    BindActive();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PopulateDepartments()
        {
            var list = _groups.GetAll().OrderBy(g => g.GroupName).ToList();
            drpDepartment.AppendDataBoundItems = true;
            drpDepartment.DataTextField = "GroupName";
            drpDepartment.DataValueField = "GroupID";
            drpDepartment.DataSource = list;
            drpDepartment.DataBind();
        }

        private void PopulateCounties()
        {
            var list = _counties.GetAll().OrderBy(c => c.CountyName).ToList();
            drpCounty.AppendDataBoundItems = true;
            drpCounty.DataTextField = "CountyName";
            drpCounty.DataValueField = "CountyId";
            drpCounty.DataSource = list;
            drpCounty.DataBind();
        }

        private void BindActive()
        {
            var list = _employees.GetActive().ToList();
            BindList(list);
        }

        private void BindList(IEnumerable<EmployeeInfo> employees)
        {
            var departmentLookup = _groups.GetAll().ToDictionary(g => g.GroupID, g => g.GroupName);
            var locationLookup = _locations.GetAll().ToDictionary(l => l.OfficeLocationId, l => l.Description);

            var rows = employees
                .OrderBy(emp => emp.LastName)
                .ThenBy(emp => emp.FirstName)
                .Select(emp =>
                {
                    var workPhones = _phones.GetWorkPhonesForEmployee(emp.EmployeeId)
                        .OrderByDescending(p => p.IsMain)
                        .ToList();

                    var phoneLink = "";
                    var firstPhone = workPhones.FirstOrDefault();
                    if (firstPhone != null && !string.IsNullOrEmpty(firstPhone.PhoneNumber))
                    {
                        var display = firstPhone.PhoneNumber;
                        if (!string.IsNullOrEmpty(firstPhone.Extension))
                            display += " x" + firstPhone.Extension;
                        phoneLink = string.Format("<a href=\"tel:{0}\">{1}</a>",
                            firstPhone.PhoneNumber, display);
                    }

                    string deptName = null;
                    if (emp.DepartmentId.HasValue && departmentLookup.ContainsKey(emp.DepartmentId.Value))
                        deptName = departmentLookup[emp.DepartmentId.Value];

                    string locName = emp.LocationName;
                    if (string.IsNullOrEmpty(locName) && emp.OfficeLocationId.HasValue
                        && locationLookup.ContainsKey(emp.OfficeLocationId.Value))
                    {
                        locName = locationLookup[emp.OfficeLocationId.Value];
                    }

                    return new
                    {
                        emp.EmployeeId,
                        emp.FirstName,
                        emp.LastName,
                        emp.JobTitle,
                        emp.Email,
                        DepartmentName = deptName,
                        LocationName = locName,
                        WorkPhoneLink = phoneLink
                    };
                })
                .ToList();

            rptDirectory.DataSource = rows;
            rptDirectory.DataBind();
        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            int? deptId = null;
            int? countyId = null;
            int parsed;
            if (!string.IsNullOrEmpty(drpDepartment.SelectedValue) &&
                int.TryParse(drpDepartment.SelectedValue, out parsed))
                deptId = parsed;
            if (!string.IsNullOrEmpty(drpCounty.SelectedValue) &&
                int.TryParse(drpCounty.SelectedValue, out parsed))
                countyId = parsed;

            var list = _employees.Search(txtFirstName.Text.Trim(), txtLastName.Text.Trim(), deptId, countyId)
                .Where(emp => (emp.IsActive ?? false))
                .ToList();
            BindList(list);
        }

        protected void cmdReset_Click(object sender, EventArgs e)
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            drpDepartment.ClearSelection();
            drpCounty.ClearSelection();
            BindActive();
        }
    }
}
