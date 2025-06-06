using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class PermissionController
    {
        public void CreatePermission(Permission t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Permission>();
                rep.Insert(t);
            }
        }
        public void DeletePermission(int permissionId)
        {
            var t = GetPermission(permissionId);
            DeletePermission(t);
        }
        public void DeletePermission(Permission t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Permission>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Permission> GetPermissions()
        {
            IEnumerable<Permission> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Permission>();
                t = rep.Get();
            }
            return t;
        }
        public Permission GetPermission(int permissionId)
        {
            Permission t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Permission>();
                t = rep.GetById(permissionId);
            }
            return t;
        }
        public void UpdatePermission(Permission t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Permission>();
                rep.Update(t);
            }
        }
    }
}