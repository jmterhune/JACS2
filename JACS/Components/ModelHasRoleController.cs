using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class ModelHasRoleController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateModelHasRole(ModelHasRole t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ModelHasRole>();
                rep.Insert(t);
            }
        }
        public void DeleteModelHasRole(int modelhasroleId)
        {
            var t = GetModelHasRole(modelhasroleId);
            DeleteModelHasRole(t);
        }
        public void DeleteModelHasRole(ModelHasRole t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ModelHasRole>();
                rep.Delete(t);
            }
        }
        public IEnumerable<ModelHasRole> GetModelHasRoles()
        {
            IEnumerable<ModelHasRole> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ModelHasRole>();
                t = rep.Get();
            }
            return t;
        }
        public ModelHasRole GetModelHasRole(int modelhasroleId)
        {
            ModelHasRole t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ModelHasRole>();
                t = rep.GetById(modelhasroleId);
            }
            return t;
        }
        public void UpdateModelHasRole(ModelHasRole t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ModelHasRole>();
                rep.Update(t);
            }
        }
    }
}