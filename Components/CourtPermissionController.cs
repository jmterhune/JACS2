using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Components
{
    internal class CourtPermissionController
    {
        private const string CONN_JACS = "jacs";

        public void CreateCourtPermission(CourtPermission t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtPermission>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }

        public void DeleteCourtPermission(long courtPermissionId)
        {
            var t = GetCourtPermission(courtPermissionId);
            DeleteCourtPermission(t);
        }

        public void DeleteCourtPermission(CourtPermission t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtPermission>();
                rep.Delete(t);
            }
        }

        public IEnumerable<CourtPermission> GetCourtPermissions()
        {
            IEnumerable<CourtPermission> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtPermission>();
                t = rep.Get();
            }
            return t;
        }

        public CourtPermission GetCourtPermission(long courtPermissionId)
        {
            CourtPermission t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtPermission>();
                t = rep.GetById(courtPermissionId);
            }
            return t;
        }
        public IEnumerable<CourtPermission> GetCourtPermission(int userId)
        {
            IEnumerable<CourtPermission> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtPermission>();
                t = rep.Find("Where user_id=@0 and active=1",userId);
            }
            return t;
        }

        public void UpdateCourtPermission(CourtPermission t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtPermission>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }

        public IEnumerable<CourtPermissionViewModel> GetCourtPermissionsPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            IEnumerable<CourtPermission> permissions;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                permissions = ctx.ExecuteQuery<CourtPermission>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_permission_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }

            var viewModels = new List<CourtPermissionViewModel>();
            foreach (var permission in permissions)
            {
                var user = GetUserById(permission.user_id);
                var judge = GetJudgeById(permission.judge_id);
                viewModels.Add(new CourtPermissionViewModel(
                    permission,
                    user?.name ?? "Unknown",
                    judge?.name ?? "Unknown"
                ));
            }
            return viewModels;
        }

        public int GetCourtPermissionsCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_permission_count",
                    searchTerm ?? string.Empty
                );
            }
        }

        private tjc.Modules.jacs.Components.User GetUserById(long userId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<tjc.Modules.jacs.Components.User>();
                return rep.GetById(userId);
            }
        }

        private Judge GetJudgeById(long judgeId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Judge>();
                return rep.GetById(judgeId);
            }
        }

        public IEnumerable<KeyValuePair<long, string>> GetUsersForDropdown()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<tjc.Modules.jacs.Components.User>();
                return rep.Get().Select(u => new KeyValuePair<long, string>(u.id, u.name)).ToList();
            }
        }

        public IEnumerable<KeyValuePair<long, string>> GetJudgesForDropdown()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Judge>();
                return rep.Get().Select(j => new KeyValuePair<long, string>(j.id, j.name)).ToList();
            }
        }
    }
}