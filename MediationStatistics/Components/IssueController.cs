using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.MediationStatistics.Components
{
    internal class IssueController
    {
        private readonly IHostSettings _hostSettings;

        public IssueController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        private IDataContext GetContext()
        {
            return DataContext.Instance(_hostSettings);
        }

        public void CreateIssue(Issue t)
        {
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<Issue>();
                rep.Insert(t);
            }
        }

        public void DeleteIssue(int issueId)
        {
            var t = GetIssue(issueId);
            DeleteIssue(t);
        }

        public void DeleteIssue(Issue t)
        {
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<Issue>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Issue> GetIssues()
        {
            IEnumerable<Issue> t;
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<Issue>();
                t = rep.Get();
            }
            return t;
        }

        public Issue GetIssue(int issueId)
        {
            Issue t;
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<Issue>();
                t = rep.GetById(issueId);
            }
            return t;
        }
        public IEnumerable<Issue> GetIssuesBySession(int sessionId)
        {
            IEnumerable<Issue> t;
            using (IDataContext ctx = GetContext())
            {
                t = ctx.ExecuteQuery<Issue>(System.Data.CommandType.StoredProcedure, "tjc_med_get_issues_by_session", sessionId);
            }
            return t;
        }
        public void UpdateIssue(Issue t)
        {
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<Issue>();
                rep.Update(t);
            }
        }

    }
}
