/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Views
{
    public partial class DetailsList : EmployeeDBModuleBase
    {
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
                    BindList();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindList()
        {
            IEnumerable<EmployeeListItem> rows;

            using (IDataContext ctx = DataContext.Instance())
            {
                string sql = @"SELECT e.EmployeeId,
                                      e.FirstName,
                                      e.LastName,
                                      e.MiddleInitial,
                                      e.JobTitle,
                                      e.Email,
                                      e.Position,
                                      e.EmploymentType,
                                      e.AgencyOfEmployment,
                                      e.Race,
                                      e.Gender,
                                      e.BirthDate,
                                      e.HireDate,
                                      e.ServiceDate,
                                      e.TerminationDate,
                                      e.SocialSecurityNumber,
                                      e.Salary,
                                      e.AnnualLeaveBalance,
                                      e.SickLeaveBalance,
                                      e.Address,
                                      e.City,
                                      e.State,
                                      e.Zip,
                                      e.PersonalEmail,
                                      e.IsActive,
                                      g.GroupName AS DepartmentName,
                                      jc.ClassName,
                                      jg.Description AS JobGroupName,
                                      loc.Description AS LocationName,
                                      c.CountyName,
                                      COALESCE(sup.LastName + ', ' + sup.FirstName, '') AS SupervisorName
                               FROM tjc_employee e
                               LEFT JOIN tjc_gl_group g ON g.GroupID = e.DepartmentId
                               LEFT JOIN tjc_employee_class jc ON jc.ClassId = e.ClassId
                               LEFT JOIN tjc_employee_job_group jg ON jg.JobGroupId = e.JobGroupId
                               LEFT JOIN tjc_employee_office_location loc ON loc.OfficeLocationId = e.OfficeLocationId
                               LEFT JOIN tjc_gl_counties c ON c.CountyId = e.CountyId
                               LEFT JOIN tjc_employee sup ON sup.EmployeeId = e.SupervisorId
                               WHERE e.IsEmployee = 1
                               ORDER BY e.LastName, e.FirstName";
                rows = ctx.ExecuteQuery<EmployeeListItem>(CommandType.Text, sql).ToList();
            }

            // Gather supplemental fields (Position, EmploymentType etc.) from the base table via a second fast lookup.
            // Same IsEmployee filter as the projection query above.
            var supplemental = new Dictionary<int, EmployeeInfo>();
            using (IDataContext ctx = DataContext.Instance())
            {
                foreach (var emp in ctx.ExecuteQuery<EmployeeInfo>(CommandType.Text, "SELECT * FROM tjc_employee WHERE IsEmployee = 1"))
                {
                    supplemental[emp.EmployeeId] = emp;
                }
            }

            var projected = rows.Select(r =>
            {
                EmployeeInfo baseRow;
                supplemental.TryGetValue(r.EmployeeId, out baseRow);
                return new
                {
                    r.FirstName,
                    r.MiddleInitial,
                    r.LastName,
                    r.JobTitle,
                    MaskedSsn = MaskSsn(baseRow == null ? null : baseRow.SocialSecurityNumber),
                    r.BirthDate,
                    Race = baseRow == null ? null : baseRow.Race,
                    Gender = baseRow == null ? null : baseRow.Gender,
                    r.HireDate,
                    ServiceDate = baseRow == null ? (DateTime?)null : baseRow.ServiceDate,
                    r.SupervisorName,
                    r.DepartmentName,
                    r.ClassName,
                    r.JobGroupName,
                    Position = baseRow == null ? null : baseRow.Position,
                    AgencyOfEmployment = baseRow == null ? null : baseRow.AgencyOfEmployment,
                    r.CountyName,
                    r.LocationName,
                    EmploymentType = baseRow == null ? null : baseRow.EmploymentType,
                    Salary = baseRow == null ? (decimal?)null : baseRow.Salary,
                    SalaryOrder = baseRow == null ? 0 : (baseRow.Salary ?? 0),
                    AnnualLeaveBalance = baseRow == null ? (decimal?)null : baseRow.AnnualLeaveBalance,
                    AnnualLeaveOrder = baseRow == null ? 0 : (baseRow.AnnualLeaveBalance ?? 0),
                    SickLeaveBalance = baseRow == null ? (decimal?)null : baseRow.SickLeaveBalance,
                    SickLeaveOrder = baseRow == null ? 0 : (baseRow.SickLeaveBalance ?? 0),
                    Address = baseRow == null ? null : baseRow.Address,
                    City = baseRow == null ? null : baseRow.City,
                    State = baseRow == null ? null : baseRow.State,
                    Zip = baseRow == null ? null : baseRow.Zip,
                    r.Email,
                    PersonalEmail = baseRow == null ? null : baseRow.PersonalEmail
                };
            }).ToList();

            rptDetails.DataSource = projected;
            rptDetails.DataBind();
        }

        private static string MaskSsn(string ssn)
        {
            if (string.IsNullOrEmpty(ssn)) return "";
            var digits = new string(ssn.Where(char.IsDigit).ToArray());
            if (digits.Length < 4) return "***-**-****";
            return "***-**-" + digits.Substring(digits.Length - 4);
        }
    }
}
