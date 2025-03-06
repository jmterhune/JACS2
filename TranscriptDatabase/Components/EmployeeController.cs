using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.TranscriptDatabase.Services.ViewModels;
namespace tjc.Modules.TranscriptDatabase.Components
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
        public IEnumerable<Employee> GetEmployeesByType( EmployeeTypes employeeType)
        {
            IEnumerable<Employee> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Employee>();
                t = rep.Find("Where EmployeeTypeID = @0",(int)employeeType);
            }
            return t;
        }
        public IEnumerable<DropDownViewModel> GetEmployeeDropDownByType(EmployeeTypes employeeType)
        {
            IEnumerable<DropDownViewModel> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Employee>();
                t = rep.Find("Where EmployeeTypeID = @0", (int)employeeType).Select(emp=> new DropDownViewModel { Id=emp.EmployeeID, Name=emp.EmployeeName}).OrderBy(x => x.Name);
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