using DotNetNuke.Data;
using System.Collections.Generic;
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