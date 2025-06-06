using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.jacs.Components
{
    internal class MediationDocumentController
    {
        public void CreateMediationDocument(MediationDocument t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationDocument>();
                rep.Insert(t);
            }
        }
        public void DeleteMediationDocument(int mediationdocumentId)
        {
            var t = GetMediationDocument(mediationdocumentId);
            DeleteMediationDocument(t);
        }
        public void DeleteMediationDocument(MediationDocument t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationDocument>();
                rep.Delete(t);
            }
        }
        public IEnumerable<MediationDocument> GetMediationDocuments()
        {
            IEnumerable<MediationDocument> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationDocument>();
                t = rep.Get();
            }
            return t;
        }
        public MediationDocument GetMediationDocument(int mediationdocumentId)
        {
            MediationDocument t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationDocument>();
                t = rep.GetById(mediationdocumentId);
            }
            return t;
        }
        public void UpdateMediationDocument(MediationDocument t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<MediationDocument>();
                rep.Update(t);
            }
        }
    }
}