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
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.EmployeeDB.Components
{
    internal class EmployeeController
    {
        public void CreateEmployee(Employee t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Employee>();
                rep.Insert(t);
            }
        }

        public void DeleteEmployee(int employeeId)
        {
            var t = GetEmployee(employeeId);
            DeleteEmployee(t);
        }

        public void DeleteEmployee(Employee t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Employee>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Employee> GetEmployees()
        {
            IEnumerable<Employee> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Employee>();
                t = rep.Find("Where IsEmployee = 1");
            }
            return t;
        }
        public IEnumerable<Employee> GetContacts()
        {
            IEnumerable<Employee> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Employee>();
                t = rep.Find("Where IsEmployee = 0");
            }
            return t;
        }
        public IEnumerable<Employee> GetActiveContacts()
        {
            IEnumerable<Employee> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Employee>();
                t = rep.Find("Where IsActive = 1");
            }
            return t;
        }
        public IEnumerable<EmployeeListItem> GetEmployeeListItems(bool isActive,bool isEmployee)
        {
            IEnumerable<EmployeeListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmployeeListItem>();
                t = rep.Find("Where IsActive=@0 And IsEmployee = @1",isActive,isEmployee);
            }
            return t;
        }
        public IEnumerable<EmployeeListItem> GetContactListItems(bool isActive)
        {
            IEnumerable<EmployeeListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmployeeListItem>();
                t = rep.Find("Where IsActive=@0", isActive);
            }
            return t;
        }
        public IEnumerable<SwnContact> GetSwnContacts()
        {
            IEnumerable<SwnContact> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t= ctx.ExecuteQuery<SwnContact>(System.Data.CommandType.StoredProcedure, "tjc_employee_get_swn_contacts");
            }
            return t;
        }
        public Employee GetEmployee(int employeeId)
        {
            Employee t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Employee>();
                t = rep.GetById(employeeId);
            }
            return t;
        }

        public void UpdateEmployee(Employee t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Employee>();
                rep.Update(t);
            }
        }
        public IEnumerable<DropDownItem> GetEmployeeDropDown(string rolename)
        {
            IEnumerable<DropDownItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<DropDownItem>(System.Data.CommandType.StoredProcedure, "tjc_employee_get_employee_dropdown", rolename);
            }
            return t;
        }

        #region EeoTotals
        public int GetGenderCount(int jobGroupId,string gender,DateTime startDate, DateTime endDate)
        {
            int phoneCount = 0;
            using (IDataContext ctx = DataContext.Instance())
            {
                phoneCount = ctx.ExecuteScalar<int>(System.Data.CommandType.Text, "SELECT Count(Gender) From tjc_employee Where JobGroupId = @0 " +
                    "AND isEmployee = 1 AND Gender = @1 AND AgencyOfEmployment <> 'O' AND (HireDate <= @3 AND (TerminationDate IS NULL OR TerminationDate >= @2))",
                    jobGroupId,gender,startDate,endDate);
            }
            return phoneCount;
        }
        public int GetRaceCount(int jobGroupId, string race, DateTime startDate, DateTime endDate)
        {
            int phoneCount = 0;
            using (IDataContext ctx = DataContext.Instance())
            {
                phoneCount = ctx.ExecuteScalar<int>(System.Data.CommandType.Text, "SELECT Count(Race) From tjc_employee Where JobGroupId = @0 " +
                    "AND isEmployee = 1 AND Race = @1 AND AgencyOfEmployment <> 'O' AND (HireDate <= @3 AND (TerminationDate IS NULL OR TerminationDate >= @2))", 
                    jobGroupId, race, startDate, endDate);
            }
            return phoneCount;
        }
        public int GetGenderHireCount(int jobGroupId, string gender, DateTime startDate, DateTime endDate)
        {
            int phoneCount = 0;
            using (IDataContext ctx = DataContext.Instance())
            {
                phoneCount = ctx.ExecuteScalar<int>(System.Data.CommandType.Text, "SELECT Count(Gender) From tjc_employee Where JobGroupId = @0 " +
                    "AND isEmployee = 1 AND Gender = @1 AND AgencyOfEmployment <> 'O' AND (HireDate <= @3 AND (TerminationDate IS NULL OR TerminationDate >= @2))",
                    jobGroupId, gender, startDate, endDate);
            }
            return phoneCount;
        }
        public int GetRaceHireCount(int jobGroupId, string race, DateTime startDate, DateTime endDate)
        {
            int phoneCount = 0;
            using (IDataContext ctx = DataContext.Instance())
            {
                phoneCount = ctx.ExecuteScalar<int>(System.Data.CommandType.Text, "SELECT Count(Race) From tjc_employee Where JobGroupId = @0 " +
                    "AND isEmployee = 1 AND Race = @1 AND AgencyOfEmployment <> 'O' AND (HireDate <= @3 AND (TerminationDate IS NULL OR TerminationDate >= @2))"
                    , jobGroupId, race, startDate, endDate);
            }
            return phoneCount;
        }
        public int GetRacePromotionTransferCount(int jobGroupId, string race, DateTime startDate, DateTime endDate,string entryType)
        {
            int phoneCount = 0;
            using (IDataContext ctx = DataContext.Instance())
            {
                phoneCount = ctx.ExecuteScalar<int>(System.Data.CommandType.Text, "SELECT Count(Race) From tjc_employee " +
                    "Where JobGroupId = @0 AND isEmployee = 1 AND Race = @1 AND AgencyOfEmployment <> 'O' " +
                    "AND (HireDate <= @3 AND (TerminationDate IS NULL OR TerminationDate >= @2) " +
                    "AND SocialSecurityNumber IN (Select SocialSecurityNumber From tjc_employee_position_history " +
                    "WHERE EntryType=@4 AND startDate >= @2 AND startDate <= @3))", 
                    jobGroupId, race, startDate, endDate,entryType);
            }
            return phoneCount;
        }
        public int GetGenderPromotionTransferCount(int jobGroupId, string gender, DateTime startDate, DateTime endDate, string entryType)
        {
            int phoneCount = 0;
            using (IDataContext ctx = DataContext.Instance())
            {
                phoneCount = ctx.ExecuteScalar<int>(System.Data.CommandType.Text, "SELECT Count(Gender) From tjc_employee " +
                    "Where JobGroupId = @0 AND isEmployee = 1 AND Gender = @1 AND AgencyOfEmployment <> 'O' " +
                    "AND (HireDate <= @3 AND (TerminationDate IS NULL OR TerminationDate >= @2) " +
                    "AND SocialSecurityNumber IN (Select SocialSecurityNumber From tjc_employee_position_history " +
                    "WHERE EntryType=@4 AND startDate >= @2 AND startDate <= @3))",
                    jobGroupId, gender, startDate, endDate, entryType);
            }
            return phoneCount;
        }
        public int GetRaceTerminationCount(int jobGroupId, string race, DateTime startDate, DateTime endDate)
        {
            int phoneCount = 0;
            using (IDataContext ctx = DataContext.Instance())
            {
                phoneCount = ctx.ExecuteScalar<int>(System.Data.CommandType.Text, "SELECT Count(Race) From tjc_employee Where JobGroupId = @0 " +
                    "AND isEmployee = 1 AND Race = @1 AND AgencyOfEmployment <> 'O' AND (TerminationDate <= @3 AND TerminationDate >= @2)"
                    , jobGroupId, race, startDate, endDate);
            }
            return phoneCount;
        }
        public int GetGenderTerminationCount(int jobGroupId, string gender, DateTime startDate, DateTime endDate)
        {
            int phoneCount = 0;
            using (IDataContext ctx = DataContext.Instance())
            {
                phoneCount = ctx.ExecuteScalar<int>(System.Data.CommandType.Text, "SELECT Count(Gender) From tjc_employee Where JobGroupId = @0 " +
                    "AND isEmployee = 1 AND Gender = @1 AND AgencyOfEmployment <> 'O' AND (TerminationDate <= @3 AND TerminationDate >= @2)",
                    jobGroupId, gender, startDate, endDate);
            }
            return phoneCount;
        }
        #endregion
    }
}
