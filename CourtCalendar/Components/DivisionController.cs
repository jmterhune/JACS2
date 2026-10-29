/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.CourtCalendar.Components
{
    /// <summary>
    /// Division data access for the intranet authoring app (Jud12 connection).
    /// Creating a division also seeds its four built-in categories.
    /// </summary>
    public class DivisionController
    {
        private const string CONN_JUD12 = "Jud12";
        private readonly IHostSettings _hostSettings;

        public DivisionController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public IEnumerable<Division> GetDivisions()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Division>();
                return rep.Find("ORDER BY SortOrder, Name").ToList();
            }
        }

        public Division GetDivision(int divisionId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<Division>().GetById(divisionId);
            }
        }

        /// <summary>Divisions whose assigned judge has the given intranet UserId as judge or JA.</summary>
        public IEnumerable<Division> GetDivisionsForUser(int userId)
        {
            var judgeIds = new HashSet<int>(
                new JudgeController(_hostSettings).GetJudgesForUser(userId).Select(j => j.JudgeId));
            if (judgeIds.Count == 0) return Enumerable.Empty<Division>();
            return GetDivisions().Where(d => judgeIds.Contains(d.JudgeId)).ToList();
        }

        public int CreateDivision(Division d)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.GetRepository<Division>().Insert(d);
            }
            // Seed the four built-in categories for the new division.
            new CategoryController(_hostSettings).SeedDefaults(d.DivisionId);
            return d.DivisionId;
        }

        public void UpdateDivision(Division d)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.GetRepository<Division>().Update(d);
            }
        }

        public void DeleteDivision(int divisionId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Division>();
                var d = rep.GetById(divisionId);
                if (d != null) rep.Delete(d);
            }
        }

        /// <summary>True when a slug is unused by any other division (for uniqueness checks).</summary>
        public bool IsSlugAvailable(string slug, int excludeDivisionId)
        {
            if (string.IsNullOrWhiteSpace(slug)) return false;
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Division>();
                return !rep.Find("WHERE Slug = @0 AND DivisionId <> @1", slug, excludeDivisionId).Any();
            }
        }
    }
}
