using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class RoleHasPermissionController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateRoleHasPermission(RoleHasPermission t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<RoleHasPermission>();
                rep.Insert(t);
            }
        }
        public void DeleteRoleHasPermission(int rolehaspermissionId)
        {
            var t = GetRoleHasPermission(rolehaspermissionId);
            DeleteRoleHasPermission(t);
        }
        public void DeleteRoleHasPermission(RoleHasPermission t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<RoleHasPermission>();
                rep.Delete(t);
            }
        }
        public IEnumerable<RoleHasPermission> GetRoleHasPermissions()
        {
            IEnumerable<RoleHasPermission> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<RoleHasPermission>();
                t = rep.Get();
            }
            return t;
        }
        public RoleHasPermission GetRoleHasPermission(int rolehaspermissionId)
        {
            RoleHasPermission t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<RoleHasPermission>();
                t = rep.GetById(rolehaspermissionId);
            }
            return t;
        }
        public void UpdateRoleHasPermission(RoleHasPermission t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<RoleHasPermission>();
                rep.Update(t);
            }
        }
    }
}