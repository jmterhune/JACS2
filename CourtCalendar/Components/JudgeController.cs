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
    /// Judge roster data access for the intranet authoring app. The calendar
    /// tables live in the public (jud12) database, so every call uses the "Jud12"
    /// connection. JudgeUserId / JaUserId are intranet accounts.
    /// </summary>
    public class JudgeController
    {
        private const string CONN_JUD12 = "Jud12";
        private readonly IHostSettings _hostSettings;

        public JudgeController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public IEnumerable<Judge> GetJudges()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Judge>();
                return rep.Find("WHERE Active = 1 ORDER BY FullName").ToList();
            }
        }

        public Judge GetJudge(int judgeId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<Judge>().GetById(judgeId);
            }
        }

        /// <summary>Judges whose assigned judge-user or JA-user is the given intranet UserId.</summary>
        public IEnumerable<Judge> GetJudgesForUser(int userId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Judge>();
                return rep.Find("WHERE JudgeUserId = @0 OR JaUserId = @0", userId).ToList();
            }
        }

        public void CreateJudge(Judge j)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.GetRepository<Judge>().Insert(j);
            }
        }

        public void UpdateJudge(Judge j)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.GetRepository<Judge>().Update(j);
            }
        }

        public void DeleteJudge(int judgeId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var rep = ctx.GetRepository<Judge>();
                var j = rep.GetById(judgeId);
                if (j != null) rep.Delete(j);
            }
        }
    }
}
