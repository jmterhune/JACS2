using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Controllers
{
    /// <summary>
    /// Persists submitted New Hire IT Worksheet requests. Each request is a
    /// snapshot of the form at submission time, with one tjc_nhit_request_item
    /// child row per checked item (item name + category captured verbatim so
    /// later catalog edits don't mutate history).
    /// </summary>
    public class NhitRequestController
    {
        private readonly NhitItemController _items = new NhitItemController();

        public NhitRequestInfo GetById(int id)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.GetRepository<NhitRequestInfo>().GetById(id);
            }
        }

        public IEnumerable<NhitRequestInfo> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.GetRepository<NhitRequestInfo>()
                    .Find("ORDER BY SubmittedDate DESC");
            }
        }

        public IEnumerable<NhitRequestItemInfo> GetItemsForRequest(int requestId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                return ctx.GetRepository<NhitRequestItemInfo>()
                    .Find("WHERE NhitRequestId = @0 ORDER BY ItemSnapshotCategory, ItemSnapshotName", requestId);
            }
        }

        public int Create(NhitRequestInfo item, int userId = -1)
        {
            item.SubmittedDate = DateTime.Now;
            item.SubmittedById = userId;

            // Pre-resolve checked item rows from the catalog so we can
            // snapshot their (Name, Category) on the request_item rows.
            // Doing this BEFORE inserting the parent keeps the work in one
            // place and lets us bail out early if the IDs are bogus.
            var catalogLookup = item.SelectedItemIds == null || !item.SelectedItemIds.Any()
                ? new Dictionary<int, NhitItemInfo>()
                : _items.GetAll()
                    .Where(i => item.SelectedItemIds.Contains(i.NhitItemId))
                    .ToDictionary(i => i.NhitItemId);

            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.GetRepository<NhitRequestInfo>().Insert(item);

                if (item.SelectedItemIds != null)
                {
                    var rep = ctx.GetRepository<NhitRequestItemInfo>();
                    foreach (var itemId in item.SelectedItemIds.Distinct())
                    {
                        catalogLookup.TryGetValue(itemId, out var cat);
                        rep.Insert(new NhitRequestItemInfo
                        {
                            NhitRequestId = item.NhitRequestId,
                            NhitItemId = itemId,
                            ItemSnapshotName = cat == null ? null : cat.Name,
                            ItemSnapshotCategory = cat == null ? null : cat.Category,
                            IsChecked = true
                        });
                    }
                }
            }
            return item.NhitRequestId;
        }

        /// <summary>Update only the email-status columns (called after the
        /// helpdesk send attempt). Doesn't touch the form fields — those are
        /// immutable once a request is filed.</summary>
        public void UpdateEmailStatus(int requestId, string sentTo, bool success, string errorMessage)
        {
            var existing = GetById(requestId);
            if (existing == null) return;
            existing.EmailSentTo = sentTo;
            existing.EmailSentDate = DateTime.Now;
            existing.EmailSuccess = success;
            existing.EmailErrorMessage = errorMessage;
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.GetRepository<NhitRequestInfo>().Update(existing);
            }
        }
    }
}
