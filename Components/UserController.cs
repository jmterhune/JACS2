using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class UserController
    {
        private const string CONN_JACS = "jacs"; //Connection

        public void CreateUser(User t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<User>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }
        public void DeleteUser(long userId)
        {
            var t = GetUser(userId);
            DeleteUser(t);
        }
        public void DeleteUser(User t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<User>();
                rep.Delete(t);
            }
        }
        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<User>();
                t = rep.Get();
            }
            return t;
        }
        public User GetUser(long userId)
        {
            User t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<User>();
                t = rep.GetById(userId);
            }
            return t;
        }
        public void UpdateUser(User t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<User>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }
        public IEnumerable<User> GetUsersPaged(string searchTerm,int roleId, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<User>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_user_paged",
                    searchTerm ?? string.Empty,
                    roleId,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }
        public int GetUsersCount(string searchTerm,int roleId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_user_count",
                    searchTerm ?? string.Empty,roleId
                );
            }
        }
    }
}