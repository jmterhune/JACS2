using DotNetNuke.Data;
using System;
using System.Collections.Generic;

namespace tjc.Modules.MediationStatistics.Components
{
    internal class SessionController
    {
        public void CreateSession(Session t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Session>();
                rep.Insert(t);
            }
        }
        public void DeleteSession(int sessionId)
        {
            DeleteAllSessionIssues(sessionId);
            var t = GetSession(sessionId);
            DeleteSession(t);
        }
        public void DeleteSession(Session t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Session>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Session> GetSessions()
        {
            IEnumerable<Session> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Session>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Session> GetSessionsByCase(int caseId)
        {
            IEnumerable<Session> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Session>();
                t = rep.Find("Where CaseId = @0",caseId);
            }
            return t;
        }
        public Session GetSession(int sessionId)
        {
            Session t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Session>();
                t = rep.GetById(sessionId);
            }
            return t;
        }

        public void UpdateSession(Session t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Session>();
                rep.Update(t);
            }
        }
        public void CreateSessionIssue(SessionIssue sessionIssue)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_add_session_issue", sessionIssue.SessionId, sessionIssue.IssueId,  sessionIssue.CreatedById);
            }
        }
        public void DeleteSessionIssue(SessionIssue sessionIssue)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_delete_session_issue", sessionIssue.SessionId, sessionIssue.IssueId);
            }
        }
        public void DeleteAllSessionIssues(int sessionId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_delete_all_session_issues", sessionId);
            }
        }
        public IEnumerable<string> GetReferralSourceItems()
        {
            IEnumerable<string> items = new List<string>();
            using (IDataContext ctx = DataContext.Instance())
            {
                items= ctx.ExecuteQuery<string>(System.Data.CommandType.Text, "Select Distinct ProgramReferralSource From tjc_med_sessions Where ProgramReferralSource IS NOT NULL AND ProgramReferralSource <> '' Order by ProgramReferralSource");
            }
            return items;
        }
        public IEnumerable<string> GetReferralSourceItems(DateTime startDate,DateTime endDate)
        {
            IEnumerable<string> items = new List<string>();
            using (IDataContext ctx = DataContext.Instance())
            {
                items = ctx.ExecuteQuery<string>(System.Data.CommandType.Text, "Select Distinct ProgramReferralSource From tjc_med_sessions Where ISNULL(RTRIM(ProgramReferralSource),'') <>'' And (ReferralDate Between @0 And @1) Order by ProgramReferralSource", startDate,endDate);
            }
            return items;
        }

    }
}
