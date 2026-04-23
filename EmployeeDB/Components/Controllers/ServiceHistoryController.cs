using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    public class ServiceHistoryController
    {
        public ServiceHistoryInfo GetById(int id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ServiceHistoryInfo>();
                return rep.GetById(id);
            }
        }

        public IEnumerable<ServiceHistoryInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ServiceHistoryInfo>();
                return rep.Get();
            }
        }

        public int Create(ServiceHistoryInfo item, int userId = -1)
        {
            item.CreatedDate = DateTime.Now;
            item.CreatedById = userId;
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ServiceHistoryInfo>();
                rep.Insert(item);
            }
            return item.ServiceId;
        }

        public void Update(ServiceHistoryInfo item, int userId = -1)
        {
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ServiceHistoryInfo>();
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
                    var rep = ctx.GetRepository<ServiceHistoryInfo>();
                    rep.Delete(item);
                }
            }
        }

        public IEnumerable<ServiceHistoryInfo> GetForSsn(string ssn)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ServiceHistoryInfo>();
                return rep.Find("WHERE SocialSecurityNumber = @0 ORDER BY HireDate DESC", ssn);
            }
        }
    }
}
