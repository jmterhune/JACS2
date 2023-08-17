using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.MediationStatistics.Components
{
    internal class GroupController
    {
        public void CreateGroup(Group t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                rep.Insert(t);
            }
        }

        public void DeleteGroup(int groupId)
        {
            var t = GetGroup(groupId);
            DeleteGroup(t);
        }

        public void DeleteGroup(Group t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Group> GetGroups()
        {
            IEnumerable<Group> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                t = rep.Get();
            }
            return t;
        }

        public Group GetGroup(int groupId)
        {
            Group t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                t = rep.GetById(groupId);
            }
            return t;
        }

        public void UpdateGroup(Group t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Group>();
                rep.Update(t);
            }
        }
        #region Group Relationships
        public void CreateCaseTypeGroup(CaseTypeGroup caseTypeGroup)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_add_group_case_type", caseTypeGroup.GroupId, caseTypeGroup.CaseTypeId, caseTypeGroup.SortOrder, caseTypeGroup.CreatedById);
            }
        }
        public void UpdateCaseTypeGroup(CaseTypeGroup t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_update_group_case_type", t.GroupId, t.CaseTypeId, t.SortOrder,t.LastModifiedById);
            }
        }
        public void DeleteCaseTypeGroup(CaseTypeGroup caseTypeGroup)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_delete_group_case_type", caseTypeGroup.GroupId, caseTypeGroup.CaseTypeId);
            }
        }
        public IEnumerable<CaseTypeGroup> GetCaseTypeGroups(int groupId)
        {
            IEnumerable<CaseTypeGroup> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseTypeGroup>();
                t = rep.Find("Where GroupId = @0", groupId);
            }
            return t;
        }
        public IEnumerable<CaseType> GetCaseTypesExcludedByGroup(int groupId)
        {
            IEnumerable<CaseType> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<CaseType>(System.Data.CommandType.StoredProcedure, "tjc_med_get_group_case_types_excluded", groupId);
            }
            return t;
        }
        public IEnumerable<CaseType> GetCaseTypesByGroup(int groupId)
        {
            IEnumerable<CaseType> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<CaseType>(System.Data.CommandType.StoredProcedure, "tjc_med_get_case_types_by_group", groupId);
            }
            return t;
        }
        public CaseTypeGroup GetCaseTypeGroup(int groupId, int caseTypeId)
        {
            CaseTypeGroup t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CaseTypeGroup>();
                t = rep.Find("Where GroupId = @0 And CaseTypeId = @1", groupId, caseTypeId).FirstOrDefault();
            }
            return t;
        }
        public void CreateAppearanceGroup(AppearanceGroup appearanceGroup)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_add_group_appearance", appearanceGroup.GroupId, appearanceGroup.AppearanceId, appearanceGroup.SortOrder, appearanceGroup.CreatedById);
            }
        }
        public void UpdateAppearanceGroup(AppearanceGroup t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_update_group_appearance", t.GroupId, t.AppearanceId, t.SortOrder, t.LastModifiedById);
            }
        }
        public void DeleteAppearanceGroup(AppearanceGroup appearanceGroup)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_delete_group_appearance", appearanceGroup.GroupId, appearanceGroup.AppearanceId);
            }
        }
        public IEnumerable<AppearanceGroup> GetAppearanceGroups(int groupId)
        {
            IEnumerable<AppearanceGroup> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AppearanceGroup>();
                t = rep.Find("Where GroupId = @0", groupId);
            }
            return t;
        }
        public AppearanceGroup GetAppearanceGroup(int groupId, int appearanceId)
        {
            AppearanceGroup t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AppearanceGroup>();
                t = rep.Find("Where GroupId = @0 And AppearanceId = @1", groupId, appearanceId).FirstOrDefault();
            }
            return t;
        }
        public IEnumerable<Appearance> GetAppearancesExcludedByGroup(int groupId)
        {
            IEnumerable<Appearance> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<Appearance>(System.Data.CommandType.StoredProcedure, "tjc_med_get_group_appearances_excluded", groupId);
            }
            return t;
        }
        public IEnumerable<Appearance> GetAppearancesByGroup(int groupId)
        {
            IEnumerable<Appearance> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<Appearance>(System.Data.CommandType.StoredProcedure, "tjc_med_get_appearances_by_group", groupId);
            }
            return t;
        }
        public void CreateIssueGroup(IssueGroup issueGroup)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_add_group_issue", issueGroup.GroupId, issueGroup.IssueId, issueGroup.SortOrder, issueGroup.CreatedById);
            }
        }
        public void UpdateIssueGroup(IssueGroup t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_update_group_issue", t.GroupId, t.IssueId, t.SortOrder, t.LastModifiedById);
            }
        }
        public void DeleteIssueGroup(IssueGroup issueGroup)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.StoredProcedure, "tjc_med_delete_group_issue", issueGroup.GroupId, issueGroup.IssueId);
            }
        }
        public IEnumerable<IssueGroup> GetIssueGroups(int groupId)
        {
            IEnumerable<IssueGroup> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<IssueGroup>();
                t = rep.Find("Where GroupId = @0", groupId);
            }
            return t;
        }
        public IssueGroup GetIssueGroup(int groupId, int issueId)
        {
            IssueGroup t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<IssueGroup>();
                t = rep.Find("Where GroupId = @0 And IssueId = @1", groupId, issueId).FirstOrDefault();
            }
            return t;
        }
        public IEnumerable<Issue> GetIssuesExcludedByGroup(int groupId)
        {
            IEnumerable<Issue> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<Issue>(System.Data.CommandType.StoredProcedure, "tjc_med_get_group_issues_excluded", groupId);
            }
            return t;
        }
        public IEnumerable<Issue> GetIssuesByGroup(int groupId)
        {
            IEnumerable<Issue> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<Issue>(System.Data.CommandType.StoredProcedure, "tjc_med_get_issues_by_group", groupId);
            }
            return t;
        }
        #endregion



    }
}
