using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class PermissionController
    {
        private const string CONN_JACS = "jacs"; //Connection

        public void CreatePermission(Permission t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Permission>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }
        public void DeletePermission(long permissionId)
        {
            var t = GetPermission(permissionId);
            DeletePermission(t);
        }
        public void DeletePermission(Permission t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Permission>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Permission> GetPermissions()
        {
            IEnumerable<Permission> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Permission>();
                t = rep.Get();
            }
            return t;
        }
        public Permission GetPermission(long permissionId)
        {
            Permission t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Permission>();
                t = rep.GetById(permissionId);
            }
            return t;
        }
        public void UpdatePermission(Permission t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Permission>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }
        public IEnumerable<Permission> GetPermissionsPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            IEnumerable<Permission> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t = ctx.ExecuteQuery<Permission>(System.Data.CommandType.StoredProcedure, "tjc_jacs_get_permission_paged", searchTerm, rowOffset, pageSize, sortOrder, sortDesc);
            }
            return t;
        }
        public int GetPermissionsCount(string searchTerm)
        {
            int t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                t = ctx.ExecuteScalar<int>(System.Data.CommandType.StoredProcedure, "tjc_jacs_get_permission_count", searchTerm);
            }
            return t;
        }
    }
}