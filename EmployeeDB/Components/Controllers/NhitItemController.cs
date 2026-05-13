using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    /// <summary>
    /// CRUD for the New Hire IT Worksheet's checkbox catalog. Items belong
    /// to one of three categories (Software / Intranet / Judicial) and can
    /// be added / renamed / re-categorized / deactivated by HR Admin without
    /// requiring a code deploy.
    /// </summary>
    public class NhitItemController
    {
        public NhitItemInfo GetById(int id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.GetRepository<NhitItemInfo>().GetById(id);
            }
        }

        /// <summary>Active items only, ordered by category then sort order.</summary>
        public IEnumerable<NhitItemInfo> GetActive()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.GetRepository<NhitItemInfo>()
                    .Find("WHERE IsActive = 1 ORDER BY Category, SortOrder, Name");
            }
        }

        /// <summary>Includes inactive — for the Manage Items admin screen.</summary>
        public IEnumerable<NhitItemInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.GetRepository<NhitItemInfo>()
                    .Find("ORDER BY Category, SortOrder, Name");
            }
        }

        public IEnumerable<NhitItemInfo> GetByCategory(string category)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.GetRepository<NhitItemInfo>()
                    .Find("WHERE Category = @0 AND IsActive = 1 ORDER BY SortOrder, Name", category);
            }
        }

        public int Create(NhitItemInfo item, int userId = -1)
        {
            item.CreatedDate = DateTime.Now;
            item.CreatedById = userId;
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            // Default IsActive = true for new items so they appear on the
            // form immediately — admin can deactivate later if needed.
            // (DB DEFAULT also sets it, but DAL2's Insert serialises every
            // property regardless of default, so we set it here too.)
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.GetRepository<NhitItemInfo>().Insert(item);
            }
            return item.NhitItemId;
        }

        public void Update(NhitItemInfo item, int userId = -1)
        {
            // Preserve audit columns from the existing row so a JSON-bound
            // payload from the API doesn't overwrite CreatedDate with
            // DateTime.MinValue (rejected by SQL Server).
            var existing = GetById(item.NhitItemId);
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
                ctx.GetRepository<NhitItemInfo>().Update(item);
            }
        }

        /// <summary>Soft delete — flips IsActive off so historical request
        /// rows that reference the item still resolve cleanly.</summary>
        public void Deactivate(int id, int userId = -1)
        {
            var item = GetById(id);
            if (item == null) return;
            item.IsActive = false;
            Update(item, userId);
        }

        /// <summary>Hard delete — only used by the Manage Items admin screen
        /// for items that have never been included in a submission. Use
        /// Deactivate instead when historical rows reference the item.</summary>
        public void Delete(int id)
        {
            var item = GetById(id);
            if (item == null) return;
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.GetRepository<NhitItemInfo>().Delete(item);
            }
        }
    }
}
