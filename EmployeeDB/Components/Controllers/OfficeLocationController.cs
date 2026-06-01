using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Helpers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    public class OfficeLocationController
    {
        public OfficeLocationInfo GetById(int id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<OfficeLocationInfo>();
                return rep.GetById(id);
            }
        }

        public IEnumerable<OfficeLocationInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<OfficeLocationInfo>();
                return rep.Get();
            }
        }

        public int Create(OfficeLocationInfo item, int userId = -1)
        {
            ModelNormalizer.Normalize(item);
            item.CreatedDate = DateTime.Now;
            item.CreatedById = userId;
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<OfficeLocationInfo>();
                rep.Insert(item);
            }
            return item.OfficeLocationId;
        }

        public void Update(OfficeLocationInfo item, int userId = -1)
        {
            ModelNormalizer.Normalize(item);
            // Preserve audit columns from the existing row (JSON payloads come
            // in with DateTime.MinValue / 0 which SQL Server datetime rejects).
            var existing = GetById(item.OfficeLocationId);
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
                var rep = ctx.GetRepository<OfficeLocationInfo>();
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
                    var rep = ctx.GetRepository<OfficeLocationInfo>();
                    rep.Delete(item);
                }
            }
        }

        // Returns 1 if deleted; 0 if the location is still referenced by one or more employees.
        public int DeleteLocation(int id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var inUse = ctx.ExecuteScalar<int>(CommandType.Text,
                    "SELECT COUNT(*) FROM tjc_employee WHERE OfficeLocationId = @0",
                    id);
                if (inUse > 0)
                    return 0;

                var rep = ctx.GetRepository<OfficeLocationInfo>();
                var item = rep.GetById(id);
                if (item != null)
                    rep.Delete(item);
            }
            return 1;
        }

        public void UpdateLocation(int id, string description, int userId = -1)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(CommandType.Text,
                    "UPDATE tjc_employee_office_location SET Description = @0, LastModifiedDate = @1, LastModifiedById = @2 WHERE OfficeLocationId = @3",
                    description, DateTime.Now, userId, id);
            }
        }
    }
}
