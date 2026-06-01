using DotNetNuke.Data;
using System.Collections.Generic;
namespace tjc.Modules.TranscriptDatabase.Components
{
    internal class AttachmentController
    {
        public void CreateAttachment(Attachment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attachment>();
                rep.Insert(t);
            }
        }
        public void DeleteAttachment(int attachmentId)
        {
            var t = GetAttachment(attachmentId);
            DeleteAttachment(t);
        }
        public void DeleteAttachment(Attachment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attachment>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Attachment> GetAttachments()
        {
            IEnumerable<Attachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attachment>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<Attachment> GetAttachmentsByDesignation(int designationId)
        {
            IEnumerable<Attachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attachment>();
                t = rep.Find("Where DesignationID = @0",designationId);
            }
            return t;
        }
        public Attachment GetAttachment(int attachmentId)
        {
            Attachment t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attachment>();
                t = rep.GetById(attachmentId);
            }
            return t;
        }
        public void UpdateAttachment(Attachment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attachment>();
                rep.Update(t);
            }
        }
    }
}
