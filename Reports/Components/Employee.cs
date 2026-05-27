/*
' Copyright (c) 2023 Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;
using System.Web.Caching;

namespace tjc.Modules.Reports.Components
{
    [TableName("tjc_employee_report")]
    //setup the primary key for table
    [PrimaryKey("EmployeeId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Employees", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public int CountyId { get; set; }
        public int OfficeLocationId { get; set; }
        public int UserId { get; set; }

        public int SupervisorId { get; set; }

        public int DepartmentId { get; set; }

        public int JobGroupId { get; set; }

        public string AgencyOfEmployment { get; set; }
        public DateTime ServiceDate { get; set; }
        public DateTime TerminatedDate { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsActive { get; set; }

    }
    internal class BirthDayEmployees
    {
        public string FirstName { get; set; }

        public string LastName { get; set; } 
        public DateTime BirthDate { get; set; }
    }
    internal class ServiceAwardEmployees {
        public string AgencyOfEmployment { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public DateTime ServiceDate { get; set; }
        public int YearsOfService { get; set; }

    }
    internal class TerminatedEmployees
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime TerminationDate { get; set; }
        public bool IsActive { get; set; }

    }

    /// <summary>Lookup row from tjc_gl_counties — used to populate the
    /// Birthday Report's County dropdown so the form posts CountyId (int)
    /// instead of a county-name string (the SP signature now expects int).</summary>
    [TableName("tjc_gl_counties")]
    [PrimaryKey("CountyId", AutoIncrement = true)]
    internal class CountyLookup
    {
        public int CountyId { get; set; }
        public string CountyName { get; set; }
    }

    /// <summary>
    /// Row shape for the DROP Participants report. Mirrors the columns in
    /// the legacy EmployeeDB\Documentation\DROP Participants.xlsx sheet:
    ///   Employee | DROP Entry Date | Termination Date | Leave Payout
    /// Filter: tjc_employee.DropEntryDate IS NOT NULL.
    /// </summary>
    internal class DropParticipantRow
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DropEntryDate { get; set; }
        public DateTime? DropExitDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public decimal? DropLeavePayout { get; set; }
        public string JobTitle { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// Row shape used by all three "seniority / incentive milestone" reports
    /// (JA Seniority, Staff Attorney Seniority, Certified Interpreter Seniority).
    /// The legacy Excel sheets each have slightly different anchor dates and
    /// milestone columns; this DTO carries the common fields and the reports
    /// compute the role-specific milestones (e.g. anchor + 2y, anchor + 5y)
    /// at render time from StartDate / CertificationDate.
    /// </summary>
    internal class SeniorityRow
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }
        public string ClassName { get; set; }
        public string DepartmentName { get; set; }
        public DateTime? StartDate { get; set; }          // tjc_employee.HireDate (anchor)
        public DateTime? CertificationDate { get; set; }  // for Certified Interpreters
        public bool? IsActive { get; set; }
    }
}
