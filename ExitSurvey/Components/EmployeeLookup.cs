/*
' Copyright (c) 2026  Joe Terhune
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Data;
using System;
using System.Linq;

namespace tjc.Modules.ExitSurvey.Components
{
    // Read-only partial mapping of the EmployeeDB table (tjc_employee) - just the
    // columns the exit survey needs to pre-fill the Personal Info section. Lives in
    // the same DNN database, so it is reachable through the normal DAL2 context.
    [TableName("tjc_employee")]
    [PrimaryKey("EmployeeId", AutoIncrement = true)]
    public class EmployeeLookup
    {
        public int EmployeeId { get; set; }
        public int? UserId { get; set; }
        public int? SupervisorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public string JobTitle { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }

        [IgnoreColumn]
        public string FullName
        {
            get { return ((FirstName ?? "") + " " + (LastName ?? "")).Trim(); }
        }

        // Prefer the human-readable job title, fall back to the position field.
        [IgnoreColumn]
        public string Title
        {
            get { return !string.IsNullOrWhiteSpace(JobTitle) ? JobTitle : Position; }
        }
    }

    public class EmployeeLookupController
    {
        // Finds the employee record for the signed-in user: by DNN UserId first
        // (exact), then by first/last name as the user asked ("search for the
        // logged in user's name in the employee database").
        public EmployeeLookup GetForUser(int userId, string firstName, string lastName)
        {
            EmployeeLookup emp = null;
            if (userId > 0)
                emp = FindOne("WHERE UserId = @0", userId);
            if (emp == null && !string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
                emp = FindOne("WHERE FirstName = @0 AND LastName = @1", firstName.Trim(), lastName.Trim());
            return emp;
        }

        public EmployeeLookup GetById(int employeeId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.GetRepository<EmployeeLookup>().GetById(employeeId);
            }
        }

        private EmployeeLookup FindOne(string condition, params object[] args)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.GetRepository<EmployeeLookup>().Find(condition, args).FirstOrDefault();
            }
        }
    }
}
