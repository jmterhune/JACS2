using DotNetNuke.Data;
using System.Collections.Generic;
using System;

namespace tjc.Modules.jacs.Components
{
    internal class RoleController
    {
        private const string CONN_JACS = "jacs"; //Connection

        public void CreateRole(Role t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Role>();
                t.created_at = DateTime.Now;
                t.updated_at = DateTime.Now;
                rep.Insert(t);
            }
        }

        public void DeleteRole(long id)
        {
            var t = GetRole(id);
            DeleteRole(t);
        }

        public void DeleteRole(Role t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Role>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Role> GetRoles()
        {
            IEnumerable<Role> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Role>();
                t = rep.Get();
            }
            return t;
        }

        public Role GetRole(long id)
        {
            Role t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Role>();
                t = rep.GetById(id);
            }
            return t;
        }

        public void UpdateRole(Role t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Role>();
                t.updated_at = DateTime.Now;
                rep.Update(t);
            }
        }

        public IEnumerable<Role> GetRolesPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<Role>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_role_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }

        public int GetRolesCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_role_count",
                    searchTerm ?? string.Empty
                );
            }
        }
    }
}