using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    public class EmergencyContactController
    {
        public EmergencyContactInfo GetById(int id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmergencyContactInfo>();
                return rep.GetById(id);
            }
        }

        public IEnumerable<EmergencyContactInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmergencyContactInfo>();
                return rep.Get();
            }
        }

        public int Create(EmergencyContactInfo item, int userId = -1)
        {
            item.CreatedDate = DateTime.Now;
            item.CreatedById = userId;
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmergencyContactInfo>();
                rep.Insert(item);
            }
            return item.ContactId;
        }

        public void Update(EmergencyContactInfo item, int userId = -1)
        {
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmergencyContactInfo>();
                rep.Update(item);
            }
        }

        public void Delete(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                using (IDataContext ctx = DataContext.Instance())
                {
                    var rep = ctx.GetRepository<EmergencyContactInfo>();
                    rep.Delete(item);
                }
            }
        }

        public IEnumerable<EmergencyContactInfo> GetForEmployee(int employeeId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<EmergencyContactInfo>();
                return rep.Find("WHERE EmployeeId = @0 ORDER BY CallOrder", employeeId);
            }
        }
    }
}
