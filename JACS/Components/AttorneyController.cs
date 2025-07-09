using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    internal class AttorneyController
    {
        private const string CONN_JACS = "jacs"; //Connection
        private const string CONN_JUD12 = "Jud12"; //jud12.flcourts.org

        public void CreateAttorney(Attorney t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Attorney>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                if (!t.scheduling.HasValue)
                    t.scheduling = false;
                if (!t.enabled.HasValue)
                    t.enabled = false;
                rep.Insert(t);
                if(t.emails != null && t.emails.Count > 0)
                {
                    var emailCtl = new EmailController();
                    foreach (var email in t.emails)
                    {
                        emailCtl.CreateEmail(new Email { emailable_id = t.id, emailable_type= "App\\Models\\Attorney", email = email });
                    }
                }
            }
        }
        public void DeleteAttorney(long attorneyId)
        {
            var t = GetAttorney(attorneyId);
            DeleteAttorney(t);
        }
        public void DeleteAttorney(Attorney t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Attorney>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Attorney> GetAttorneys()
        {
            IEnumerable<Attorney> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Attorney> GetAttorneyDropDownItems(string term)
        {
            IEnumerable<Attorney> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.Find("Where name like @0",string.Format("%{0}%",term));
            }
            return t;
        }
        public Attorney GetAttorney(long attorneyId)
        {
            Attorney t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.GetById(attorneyId);
            }
            return t;
        }
        public CourtListItem GetAttorneyListItem(long attorneyId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var attorney = ctx.GetRepository<Attorney>().GetById(attorneyId);
                return attorney != null
                    ? new CourtListItem
                    {
                        value = attorney.id,
                        text = attorney.name
                    }
                    : new CourtListItem();
            }
        }

        public void UpdateAttorney(Attorney t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Attorney>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
                if(t.emails != null && t.emails.Count > 0)
                {
                    var emailCtl = new EmailController();
                    emailCtl.DeletAllEmailsByAttorney(t.id);
                    foreach (var email in t.emails)
                    {
                        emailCtl.CreateEmail(new Email { emailable_id = t.id, emailable_type= "App\\Models\\Attorney", email = email });
                    }
                }
            }
        }
        public IEnumerable<Attorney> GetAttorneysPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<Attorney>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_attorney_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }
        public int GetAttorneysCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_attorney_count",
                    searchTerm ?? string.Empty
                );
            }
        }
        public SiteUser GetSiteUser(int portalId,string barNumber)
        {
            IEnumerable<SiteUser> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JUD12))
            {
                t = ctx.ExecuteQuery<SiteUser>(System.Data.CommandType.StoredProcedure, "tjc_jacs_get_user_by_barnumber",portalId, barNumber);
            }
            return t.FirstOrDefault();
        }
    }
}