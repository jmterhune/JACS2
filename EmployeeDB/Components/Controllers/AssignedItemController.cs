using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    public class AssignedItemController
    {
        public AssignedItemInfo GetById(int id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AssignedItemInfo>();
                return rep.GetById(id);
            }
        }

        public IEnumerable<AssignedItemInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AssignedItemInfo>();
                return rep.Get();
            }
        }

        public int Create(AssignedItemInfo item, int userId = -1)
        {
            item.CreatedDate = DateTime.Now;
            item.CreatedById = userId;
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AssignedItemInfo>();
                rep.Insert(item);
            }
            return item.ItemId;
        }

        public void Update(AssignedItemInfo item, int userId = -1)
        {
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AssignedItemInfo>();
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
                    var rep = ctx.GetRepository<AssignedItemInfo>();
                    rep.Delete(item);
                }
            }
        }

        public IEnumerable<AssignedItemInfo> GetForEmployee(int employeeId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AssignedItemInfo>();
                return rep.Find("WHERE EmployeeId = @0", employeeId);
            }
        }
    }
}
