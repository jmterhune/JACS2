using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Helpers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    public class RaceController
    {
        public RaceInfo GetById(int id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RaceInfo>();
                return rep.GetById(id);
            }
        }

        public IEnumerable<RaceInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RaceInfo>();
                return rep.Get();
            }
        }

        public int Create(RaceInfo item, int userId = -1)
        {
            ModelNormalizer.Normalize(item);
            item.CreatedDate = DateTime.Now;
            item.CreatedById = userId;
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<RaceInfo>();
                rep.Insert(item);
            }
            return item.RaceId;
        }

        public void Update(RaceInfo item, int userId = -1)
        {
            ModelNormalizer.Normalize(item);
            // Preserve audit columns from the existing row (JSON payloads come
            // in with DateTime.MinValue / 0 which SQL Server datetime rejects).
            var existing = GetById(item.RaceId);
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
                var rep = ctx.GetRepository<RaceInfo>();
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
                    var rep = ctx.GetRepository<RaceInfo>();
                    rep.Delete(item);
                }
            }
        }
    }
}
