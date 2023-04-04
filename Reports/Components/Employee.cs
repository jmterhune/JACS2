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
    [TableName("Emp_Employees")]
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
        public string County { get; set; }
        public string Location { get; set; }
        public int UserId { get; set; }

        public int SupervisorId { get; set; }

        public int DivisionUnitId { get; set; }

        public int JobCategoryId { get; set; }

        public string StateCounty { get; set; }
        public DateTime ServiceDate { get; set; }
        public DateTime TerminatedDate { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsEmployee { get; set; }
        public bool Active { get; set; }

    }
    internal class BirthDayEmployees
    {
        public string FirstName { get; set; }

        public string LastName { get; set; } 
        public DateTime DateOfBirth { get; set; }
    }
    internal class ServiceAwardEmployees {
        public string StateCounty { get; set; }

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
        public DateTime TerminatedDate { get; set; }
        public bool Active { get; set; }

    }
}
