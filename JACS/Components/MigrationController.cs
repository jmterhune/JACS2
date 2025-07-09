using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class MigrationController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateMigration(Migration t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Migration>();
                rep.Insert(t);
            }
        }
        public void DeleteMigration(int migrationId)
        {
            var t = GetMigration(migrationId);
            DeleteMigration(t);
        }
        public void DeleteMigration(Migration t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Migration>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Migration> GetMigrations()
        {
            IEnumerable<Migration> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Migration>();
                t = rep.Get();
            }
            return t;
        }
        public Migration GetMigration(int migrationId)
        {
            Migration t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Migration>();
                t = rep.GetById(migrationId);
            }
            return t;
        }
        public void UpdateMigration(Migration t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Migration>();
                rep.Update(t);
            }
        }
    }
}