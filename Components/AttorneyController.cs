using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public List<AttorneyDropDownItem> GetAttorneyDropDownItems(string term)
        {
            IEnumerable<Attorney> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Attorney>();
                t = rep.Find("Where name like @0 OR bar_num like @1", string.Format("%{0}%",term), string.Format("{0}%", term));
            }
            return t.Select(a => new AttorneyDropDownItem
            {
                id = a.id,
                bar_num = a.bar_num,
                name = a.name,
                label = string.Format("{0} - {1}", a.name, a.bar_num)
            }).OrderBy(a => a.label).ToList();
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

        public Attorney GetAttorneyByBarNumber(string barNumber)
        {
            if (string.IsNullOrWhiteSpace(barNumber)) return null;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Attorney>();
                return rep.Find("Where bar_num=@0", barNumber.Trim()).FirstOrDefault();
            }
        }

        // Returns the existing JACS attorney row for the bar number if present;
        // otherwise fetches from the Florida Bar API, inserts a row, and returns it.
        // Returns null if the bar number is empty, the API rejects it, or the API
        // is unreachable — the caller treats those cases as "no local row created."
        public async Task<Attorney> EnsureAttorneyByBarNumberAsync(string barNumber)
        {
            if (string.IsNullOrWhiteSpace(barNumber)) return null;

            string normalized = barNumber.Trim().TrimStart('0');
            if (string.IsNullOrEmpty(normalized)) normalized = barNumber.Trim();

            Attorney existing = GetAttorneyByBarNumber(normalized);
            if (existing != null) return existing;

            FloridaBarMember member = await FloridaBarApiClient.FetchAsync(normalized).ConfigureAwait(false);
            if (member == null) return null;

            var attorney = new Attorney
            {
                UserId = 0,
                name = string.IsNullOrWhiteSpace(member.DisplayName) ? normalized : member.DisplayName,
                bar_num = normalized,
                phone = member.Phone,
                scheduling = false,
                enabled = member.Eligible && member.IsInGoodStanding,
                emails = !string.IsNullOrEmpty(member.Email)
                    ? new List<string> { member.Email }
                    : null
            };

            CreateAttorney(attorney);
            return attorney;
        }

        public KeyValuePair<long, string> GetAttorneyListItem(long attorneyId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var attorney = ctx.GetRepository<Attorney>().GetById(attorneyId);
                return attorney != null
                    ? new KeyValuePair<long, string>(attorney.id,attorney.name)
                    : new KeyValuePair<long, string>();
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