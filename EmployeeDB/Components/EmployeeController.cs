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
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<EmployeeListItem> GetEmployeeListItems(bool isActive)
        {
            IEnumerable<EmployeeListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmployeeListItem>();
                t = rep.Find("Where IsActive=@0",isActive);
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

    }
}
