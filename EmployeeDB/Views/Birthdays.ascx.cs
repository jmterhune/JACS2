/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Controllers;

namespace tjc.Modules.EmployeeDB.Views
{
    public partial class Birthdays : EmployeeDBModuleBase
    {
        private readonly EmployeeReportController _reports = new EmployeeReportController();
        private readonly CountyController _counties = new CountyController();
        private readonly GroupController _groups = new GroupController();
        private readonly OfficeLocationController _locations = new OfficeLocationController();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    PopulateCounties();
                    drpMonth.SelectedValue = DateTime.Now.Month.ToString();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PopulateCounties()
        {
            drpCounty.Items.Clear();
            foreach (var c in _counties.GetAll().OrderBy(x => x.CountyName))
            {
                drpCounty.Items.Add(new System.Web.UI.WebControls.ListItem(c.CountyName, c.CountyId.ToString()));
            }
        }

        protected void cmdReport_Click(object sender, EventArgs e)
        {
            try
            {
                int month = int.Parse(drpMonth.SelectedValue);
                int countyId;
                if (!int.TryParse(drpCounty.SelectedValue, out countyId))
                    return;

                var departmentLookup = _groups.GetAll().ToDictionary(g => g.GroupID, g => g.GroupName);
                var locationLookup = _locations.GetAll().ToDictionary(l => l.OfficeLocationId, l => l.Description);

                var rows = _reports.GetBirthdays(month, countyId)
                    .OrderBy(emp => emp.BirthDate.HasValue ? emp.BirthDate.Value.Day : 99)
                    .Select(emp => new
                    {
                        emp.FirstName,
                        emp.LastName,
                        emp.BirthDate,
                        BirthOrder = emp.BirthDate.HasValue ? emp.BirthDate.Value.ToString("MMdd") : "9999",
                        DepartmentName = emp.DepartmentId.HasValue && departmentLookup.ContainsKey(emp.DepartmentId.Value)
                            ? departmentLookup[emp.DepartmentId.Value]
                            : "",
                        LocationName = !string.IsNullOrEmpty(emp.LocationName)
                            ? emp.LocationName
                            : (emp.OfficeLocationId.HasValue && locationLookup.ContainsKey(emp.OfficeLocationId.Value)
                                ? locationLookup[emp.OfficeLocationId.Value]
                                : "")
                    })
                    .ToList();

                rptBirthdays.DataSource = rows;
                rptBirthdays.DataBind();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
