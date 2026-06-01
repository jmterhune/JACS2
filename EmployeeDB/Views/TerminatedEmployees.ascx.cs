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
    public partial class TerminatedEmployees : EmployeeDBModuleBase
    {
        private readonly EmployeeReportController _reports = new EmployeeReportController();
        private readonly GroupController _groups = new GroupController();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    dpEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    dpStartDate.Text = DateTime.Now.AddDays(-90).ToString("yyyy-MM-dd");
                    LoadTerminated();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void LoadTerminated()
        {
            DateTime startDate, endDate;
            if (!DateTime.TryParse(dpStartDate.Text, out startDate) ||
                !DateTime.TryParse(dpEndDate.Text, out endDate))
                return;

            var departmentLookup = _groups.GetAll().ToDictionary(g => g.GroupID, g => g.GroupName);

            var rows = _reports.GetTerminated(startDate, endDate)
                .Select(emp =>
                {
                    var days = 0;
                    if (emp.HireDate.HasValue && emp.TerminationDate.HasValue)
                    {
                        days = (int)(emp.TerminationDate.Value - emp.HireDate.Value).TotalDays;
                    }
                    return new
                    {
                        emp.LastName,
                        emp.FirstName,
                        emp.TerminationDate,
                        emp.JobTitle,
                        emp.HireDate,
                        DepartmentName = emp.DepartmentId.HasValue && departmentLookup.ContainsKey(emp.DepartmentId.Value)
                            ? departmentLookup[emp.DepartmentId.Value]
                            : "",
                        ServiceDays = days,
                        LengthOfService = FormatLengthOfService(days)
                    };
                })
                .ToList();

            rptTerminated.DataSource = rows;
            rptTerminated.DataBind();
        }

        private static string FormatLengthOfService(int days)
        {
            if (days <= 0) return "";
            var years = days / 365;
            var remDays = days % 365;
            var months = remDays / 30;
            if (years > 0 && months > 0) return string.Format("{0}y {1}m", years, months);
            if (years > 0) return string.Format("{0}y", years);
            if (months > 0) return string.Format("{0}m", months);
            return string.Format("{0}d", days);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            LoadTerminated();
        }
    }
}
