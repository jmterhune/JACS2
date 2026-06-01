using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Helpers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    public class PhoneController
    {
        public PhoneInfo GetById(long id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PhoneInfo>();
                return rep.GetById(id);
            }
        }

        public IEnumerable<PhoneInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PhoneInfo>();
                return rep.Get();
            }
        }

        public long Create(PhoneInfo item, int userId = -1)
        {
            ModelNormalizer.Normalize(item);
            item.CreatedDate = DateTime.Now;
            item.CreatedById = userId;
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PhoneInfo>();
                rep.Insert(item);
            }
            return item.PhoneId;
        }

        public void Update(PhoneInfo item, int userId = -1)
        {
            ModelNormalizer.Normalize(item);
            // Preserve the audit columns from the existing row (the JSON-bound
            // payload from the API layer comes in with DateTime.MinValue / 0
            // for these, which SQL Server datetime rejects).
            var existing = GetById(item.PhoneId);
            if (existing != null)
            {
                item.CreatedDate = existing.CreatedDate;
                item.CreatedById = existing.CreatedById;
            }
            else
            {
                item.CreatedDate = DateTime.Now;
                item.CreatedById = userId;
            }
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PhoneInfo>();
                rep.Update(item);
            }
        }

        public void Delete(long id)
        {
            var item = GetById(id);
            if (item != null)
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<PhoneInfo>();
                    rep.Delete(item);
                }
            }
        }

        public IEnumerable<PhoneInfo> GetForEmployee(int employeeId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PhoneInfo>();
                return rep.Find("WHERE EmployeeId = @0", employeeId);
            }
        }

        public IEnumerable<PhoneInfo> GetWorkPhonesForEmployee(int employeeId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<PhoneInfo>();
                return rep.Find("WHERE EmployeeId = @0 AND PhoneType LIKE 'Work%'", employeeId);
            }
        }

        public void DeleteForEmployee(int employeeId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(CommandType.Text,
                    "DELETE FROM tjc_employee_phone WHERE EmployeeId = @0",
                    employeeId);
            }
        }
    }
}
