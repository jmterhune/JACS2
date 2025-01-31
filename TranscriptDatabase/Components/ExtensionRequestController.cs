using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.TranscriptDatabase.Components
{
    internal class ExtensionRequestController
    {
        public void CreateExtensionRequest(ExtensionRequest t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ExtensionRequest>();
                rep.Insert(t);
            }
        }
        public void DeleteExtensionRequest(int extensionrequestId)
        {
            var t = GetExtensionRequest(extensionrequestId);
            DeleteExtensionRequest(t);
        }
        public void DeleteExtensionRequest(ExtensionRequest t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ExtensionRequest>();
                rep.Delete(t);
            }
        }
        public IEnumerable<ExtensionRequest> GetExtensionRequests()
        {
            IEnumerable<ExtensionRequest> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ExtensionRequest>();
                t = rep.Get();
            }
            return t;
        }
        public ExtensionRequest GetExtensionRequest(int extensionrequestId)
        {
            ExtensionRequest t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ExtensionRequest>();
                t = rep.GetById(extensionrequestId);
            }
            return t;
        }
        public void UpdateExtensionRequest(ExtensionRequest t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ExtensionRequest>();
                rep.Update(t);
            }
        }
    }
}