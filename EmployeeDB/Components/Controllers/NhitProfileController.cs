using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    /// <summary>
    /// CRUD for saved New Hire IT Worksheet profiles. A profile bundles every
    /// non-employee-unique field on the form (defaults + checkbox catalog
    /// state) under a single name; selecting a profile from the dropdown
    /// pre-populates the form with those values.
    /// </summary>
    public class NhitProfileController
    {
        public NhitProfileInfo GetById(int id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var profile = ctx.GetRepository<NhitProfileInfo>().GetById(id);
                if (profile != null) HydrateSelectedItems(profile, ctx);
                return profile;
            }
        }

        /// <summary>Lightweight list for the profile dropdown — Id + Name only.</summary>
        public IEnumerable<NhitProfileInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.GetRepository<NhitProfileInfo>()
                    .Find("ORDER BY ProfileName")
                    .ToList();
            }
        }

        public int Create(NhitProfileInfo item, int userId = -1)
        {
            item.CreatedDate = DateTime.Now;
            item.CreatedById = userId;
            item.LastModifiedDate = DateTime.Now;
            item.LastModifiedById = userId;
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.GetRepository<NhitProfileInfo>().Insert(item);
                ReplaceSelectedItems(item, ctx);
            }
            return item.NhitProfileId;
        }

        public void Update(NhitProfileInfo item, int userId = -1)
        {
            var existing = GetById(item.NhitProfileId);
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
                ctx.GetRepository<NhitProfileInfo>().Update(item);
                ReplaceSelectedItems(item, ctx);
            }
        }

        public void Delete(int id)
        {
            var profile = GetById(id);
            if (profile == null) return;
            using (IDataContext ctx = DataContext.Instance())
            {
                // Wipe child rows first (no FK in schema, but we want clean
                // data — keeps the DB tidy and prevents orphaned pivots).
                var rep = ctx.GetRepository<NhitProfileItemInfo>();
                foreach (var pi in rep.Find("WHERE NhitProfileId = @0", id).ToList())
                {
                    rep.Delete(pi);
                }
                ctx.GetRepository<NhitProfileInfo>().Delete(profile);
            }
        }

        // -------- private helpers --------

        /// <summary>Reads tjc_nhit_profile_item rows for the profile and
        /// stuffs the IDs into <c>SelectedItemIds</c>. Called from GetById
        /// so the JS layer gets everything in one round-trip.</summary>
        private static void HydrateSelectedItems(NhitProfileInfo profile, IDataContext ctx)
        {
            var rep = ctx.GetRepository<NhitProfileItemInfo>();
            profile.SelectedItemIds = rep
                .Find("WHERE NhitProfileId = @0 AND IsChecked = 1", profile.NhitProfileId)
                .Select(pi => pi.NhitItemId)
                .ToList();
        }

        /// <summary>Replace strategy: nuke the old pivot rows for this profile
        /// and insert new ones for everything in <c>SelectedItemIds</c>. Simple
        /// and correct; the volume per profile is small (dozens of items).</summary>
        private static void ReplaceSelectedItems(NhitProfileInfo profile, IDataContext ctx)
        {
            var rep = ctx.GetRepository<NhitProfileItemInfo>();
            foreach (var pi in rep.Find("WHERE NhitProfileId = @0", profile.NhitProfileId).ToList())
            {
                rep.Delete(pi);
            }
            if (profile.SelectedItemIds == null) return;
            foreach (var itemId in profile.SelectedItemIds.Distinct())
            {
                rep.Insert(new NhitProfileItemInfo
                {
                    NhitProfileId = profile.NhitProfileId,
                    NhitItemId = itemId,
                    IsChecked = true
                });
            }
        }
    }
}
