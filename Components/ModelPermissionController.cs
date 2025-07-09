using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class ModelPermissionController
    {
        private const string CONN_JACS = "jacs"; //Connection
        public void CreateModelPermission(ModelPermission t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ModelPermission>();
                rep.Insert(t);
            }
        }
        public void DeleteModelPermission(int modelpermissionId)
        {
            var t = GetModelPermission(modelpermissionId);
            DeleteModelPermission(t);
        }
        public void DeleteModelPermission(ModelPermission t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ModelPermission>();
                rep.Delete(t);
            }
        }
        public IEnumerable<ModelPermission> GetModelPermissions()
        {
            IEnumerable<ModelPermission> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ModelPermission>();
                t = rep.Get();
            }
            return t;
        }
        public ModelPermission GetModelPermission(int modelpermissionId)
        {
            ModelPermission t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ModelPermission>();
                t = rep.GetById(modelpermissionId);
            }
            return t;
        }
        public void UpdateModelPermission(ModelPermission t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ModelPermission>();
                rep.Update(t);
            }
        }
    }
}