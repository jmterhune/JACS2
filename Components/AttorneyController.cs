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
        /// <summary>
        /// The most matches a type-ahead will return. The attorneys table holds the whole
        /// Florida Bar (~106k rows), so an unbounded search on a short term materialised
        /// and serialised thousands of records — slow enough that the browser abandoned
        /// the request on the next keystroke. Ordering and limiting in SQL lets the
        /// engine stop early instead of sorting everything in memory.
        /// </summary>
        private const int DropDownResultLimit = 50;

        /// <summary>
        /// True when a search hit <see cref="DropDownResultLimit"/>, i.e. there are
        /// probably more matches the caller is not seeing.
        /// </summary>
        internal static bool WasTruncated(int returnedCount) => returnedCount >= DropDownResultLimit;

        private IEnumerable<Attorney> FindAttorneysForDropDown(IDataContext ctx, string term)
        {
            string search = (term ?? string.Empty).Trim();

            // Search one column, not both with an OR. Bar numbers are numeric, so a term
            // containing letters can never match bar_num and a digits-only term is not a
            // name — and an OR across two columns stops SQL Server using either index,
            // which is what left bar-number lookups scanning the whole table.
            bool looksLikeBarNumber = search.Length > 0 && search.All(char.IsDigit);

            string sql = looksLikeBarNumber
                // Prefix match, so IX_attorneys_bar_num can seek rather than scan.
                ? "SELECT TOP (" + DropDownResultLimit + ") id, name, bar_num " +
                  "FROM attorneys WHERE bar_num LIKE @0 ORDER BY name"
                // A leading wildcard cannot seek, but IX_attorneys_name is narrow and
                // already in name order, so the engine stops as soon as it has enough.
                : "SELECT TOP (" + DropDownResultLimit + ") id, name, bar_num " +
                  "FROM attorneys WHERE name LIKE @0 ORDER BY name";

            string pattern = looksLikeBarNumber ? search + "%" : "%" + search + "%";

            return ctx.ExecuteQuery<Attorney>(System.Data.CommandType.Text, sql, pattern);
        }

        /// <summary>
        /// Attorney matches carrying id, bar number, name and a combined label. Callers
        /// that need more than a value and a caption use this one — the calendar keys its
        /// control by bar number and still wants the id and the label.
        /// </summary>
        public List<AttorneyDropDownItem> GetExtendedAttorneyDropDownItems(string term)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return FindAttorneysForDropDown(ctx, term)
                    .Select(a => new AttorneyDropDownItem
                    {
                        id = a.id,
                        bar_num = a.bar_num,
                        name = a.name,
                        label = string.Format("{0} - {1}", a.name, a.bar_num)
                    }).ToList();
            }
        }

        /// <summary>
        /// Attorney matches as plain id/name pairs, matching the Key/Value shape every
        /// other dropdown in the module uses (see CountyController.GetCountyDropDownItems).
        /// Key is the attorney id, which is what court_def_attorney_id / opp_attorney_id
        /// store, so a caller can bind the selected value straight through.
        /// </summary>
        public List<KeyValuePair<long, string>> GetAttorneyDropDownItems(string term)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                // The bar number goes in the caption too: with the full Bar loaded there
                // are many duplicate names, and the name alone gives no way to tell them
                // apart once a row is selected.
                return FindAttorneysForDropDown(ctx, term)
                    .Select(a => new KeyValuePair<long, string>(
                        a.id,
                        string.IsNullOrWhiteSpace(a.bar_num) ? a.name : a.name + " - " + a.bar_num))
                    .ToList();
            }
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