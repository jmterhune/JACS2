using DotNetNuke.Data;
using DotNetNuke.Services.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.Purchasing.Components
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
        public void DeleteAttachmentByFileId(int fileId)
        {
            var t = GetAttachmentByFileId(fileId);
            DeleteAttachment(t);
        }
        public void DeleteAttachmentByFormId(int moduleId, int formId)
        {
            var at = GetAttachmentsByFormId(moduleId, formId);
            foreach (Attachment a in at)
            {
                DeleteAttachment(a);
            }
        }
        public void DeleteAttachmentByOrderId(int moduleId, int orderId)
        {
            var at = GetAttachmentsByOrderId(moduleId, orderId);
            foreach (Attachment a in at)
            {
                DeleteAttachment(a);
            }
        }
        public void DeleteAttachment(Attachment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attachment>();
                DeleteFile(t.FileID);
                rep.Delete(t);
            }
        }
        private void DeleteFile(int fileId)
        {
            FileManager objFile = new FileManager();
            var file = objFile.GetFile(fileId);
            objFile.DeleteFile(file);
        }
        public IEnumerable<Attachment> GetAttachments(int moduleId)
        {
            IEnumerable<Attachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attachment>();
                t = rep.Get(moduleId);
            }
            return t;
        }
        public IEnumerable<Attachment> GetAttachmentsByFormId(int moduleId, int formId)
        {
            IEnumerable<Attachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attachment>();
                t = rep.Find("Where ModuleID=@0 AND FormID=@1", moduleId, formId);
            }
            return t;
        }
        public IEnumerable<Attachment> GetAttachmentsByOrderId(int moduleId, int orderId)
        {
            IEnumerable<Attachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attachment>();
                t = rep.Find("Where ModuleID=@0 AND OrderID=@1", moduleId, orderId);
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
        public Attachment GetAttachmentByFileId(int fileId)
        {
            Attachment t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attachment>();
                t = rep.Find("Where FileID=@0", fileId).FirstOrDefault();
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

        internal object GetAttachmentsByOrder(int orderId)
        {
            IEnumerable<Attachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Attachment>();
                t = rep.Find("Where OrderID=@0", orderId);
            }
            return t;
        }
        public IEnumerable<AttachmentListItem> GetAttachmentsByFormId(int formId)
        {
            IEnumerable<AttachmentListItem> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                t = ctx.ExecuteQuery<AttachmentListItem>(System.Data.CommandType.StoredProcedure, "tjc_purchasing_get_attachments_by_formId", formId);
            }
            return t;
        }
    }
}
