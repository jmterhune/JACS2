using DotNetNuke.Data;
using DotNetNuke.Services.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.Purchasing.Components
{
    internal class AttachmentController
    {

        #region Supply Order Attachments
        public void CreateSupplyAttachment(SupplyOrderAttachment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrderAttachment>();
               var file= DotNetNuke.Services.FileSystem.FileManager.Instance.GetFile(t.FileID);
                t.FileName=file.FileName;
                t.Path=file.RelativePath;
                rep.Insert(t);
            }
        }

        public void DeleteSupplyAttachment(int attachmentId)
        {
            var t = GetSupplyAttachment(attachmentId);
            DeleteSupplyAttachment(t);
        }
        public void DeleteSupplyAttachmentByFileId(int fileId)
        {
            var t = GetSupplyAttachmentByFileId(fileId);
            DeleteSupplyAttachment(t);
        }
        public void DeleteSupplyAttachmentByFormId(int formId)
        {
            var at = GetSupplyAttachmentsByFormId(formId);
            foreach (SupplyOrderAttachment a in at)
            {
                DeleteSupplyAttachment(a);
            }
        }
        public void DeleteSupplyAttachmentByOrderId(int orderId)
        {
            var at = GetSupplyAttachmentsByOrderId(orderId);
            foreach (SupplyOrderAttachment a in at)
            {
                DeleteSupplyAttachment(a);
            }
        }
        public void DeleteSupplyAttachment(SupplyOrderAttachment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrderAttachment>();
                DeleteSupplyFile(t.FileID);
                rep.Delete(t);
            }
        }
        private void DeleteSupplyFile(int fileId)
        {
            FileManager objFile = new FileManager();
            var file = objFile.GetFile(fileId);
            objFile.DeleteFile(file);
        }
        public IEnumerable<SupplyOrderAttachment> GetSupplyAttachments()
        {
            IEnumerable<SupplyOrderAttachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrderAttachment>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<SupplyOrderAttachment> GetSupplyAttachmentsByFormId(int formId)
        {
            IEnumerable<SupplyOrderAttachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrderAttachment>();
                t = rep.Find("Where FormID=@0", formId);
            }
            return t;
        }
        public IEnumerable<SupplyOrderAttachment> GetSupplyAttachmentsByOrderId(int orderId)
        {
            IEnumerable<SupplyOrderAttachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrderAttachment>();
                t = rep.Find("Where OrderID=@0", orderId);
            }
            return t;
        }
        public SupplyOrderAttachment GetSupplyAttachment(int attachmentId)
        {
            SupplyOrderAttachment t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrderAttachment>();
                t = rep.GetById(attachmentId);
            }
            return t;
        }
        public SupplyOrderAttachment GetSupplyAttachmentByFileId(int fileId)
        {
            SupplyOrderAttachment t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrderAttachment>();
                t = rep.Find("Where FileID=@0", fileId).FirstOrDefault();
            }
            return t;
        }

        public void UpdateSupplyAttachment(SupplyOrderAttachment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrderAttachment>();
                rep.Update(t);
            }
        }

        internal object GetSupplyAttachmentsByOrder(int orderId)
        {
            IEnumerable<SupplyOrderAttachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<SupplyOrderAttachment>();
                t = rep.Find("Where OrderID=@0", orderId);
            }
            return t;
        }
        #endregion


        #region Stamp Order Attachments
        public void CreateStampAttachment(StampOrderAttachment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrderAttachment>();
                rep.Insert(t);
            }
        }

        public void DeleteStampAttachment(int attachmentId)
        {
            var t = GetStampAttachment(attachmentId);
            DeleteStampAttachment(t);
        }
        public void DeleteStampAttachmentByFileId(int fileId)
        {
            var t = GetStampAttachmentByFileId(fileId);
            DeleteStampAttachment(t);
        }
        public void DeleteStampAttachmentByFormId(int formId)
        {
            var at = GetStampAttachmentsByFormId(formId);
            foreach (StampOrderAttachment a in at)
            {
                DeleteStampAttachment(a);
            }
        }
        public void DeleteStampAttachmentByOrderId(int orderId)
        {
            var at = GetStampAttachmentsByOrderId(orderId);
            foreach (StampOrderAttachment a in at)
            {
                DeleteStampAttachment(a);
            }
        }
        public void DeleteStampAttachment(StampOrderAttachment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrderAttachment>();
                DeleteStampFile(t.FileID);
                rep.Delete(t);
            }
        }
        private void DeleteStampFile(int fileId)
        {
            FileManager objFile = new FileManager();
            var file = objFile.GetFile(fileId);
            objFile.DeleteFile(file);
        }
        public IEnumerable<StampOrderAttachment> GetStampAttachments()
        {
            IEnumerable<StampOrderAttachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrderAttachment>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<StampOrderAttachment> GetStampAttachmentsByFormId(int formId)
        {
            IEnumerable<StampOrderAttachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrderAttachment>();
                t = rep.Find("Where FormID=@0", formId);
            }
            return t;
        }
        public IEnumerable<StampOrderAttachment> GetStampAttachmentsByOrderId(int orderId)
        {
            IEnumerable<StampOrderAttachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrderAttachment>();
                t = rep.Find("Where OrderID=@0", orderId);
            }
            return t;
        }
        public StampOrderAttachment GetStampAttachment(int attachmentId)
        {
            StampOrderAttachment t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrderAttachment>();
                t = rep.GetById(attachmentId);
            }
            return t;
        }
        public StampOrderAttachment GetStampAttachmentByFileId(int fileId)
        {
            StampOrderAttachment t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrderAttachment>();
                t = rep.Find("Where FileID=@0", fileId).FirstOrDefault();
            }
            return t;
        }

        public void UpdateStampAttachment(StampOrderAttachment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrderAttachment>();
                rep.Update(t);
            }
        }

        internal object GetStampAttachmentsByOrder(int orderId)
        {
            IEnumerable<StampOrderAttachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<StampOrderAttachment>();
                t = rep.Find("Where OrderID=@0", orderId);
            }
            return t;
        }
        #endregion

        #region Form Order Attachments
        public void CreateFormAttachment(FormOrderAttachment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderAttachment>();
                rep.Insert(t);
            }
        }

        public void DeleteFormAttachment(int attachmentId)
        {
            var t = GetFormAttachment(attachmentId);
            DeleteFormAttachment(t);
        }
        public void DeleteFormAttachmentByFileId(int fileId)
        {
            var t = GetFormAttachmentByFileId(fileId);
            DeleteFormAttachment(t);
        }
        public void DeleteFormAttachmentByFormId(int formId)
        {
            var at = GetFormAttachmentsByFormId(formId);
            foreach (FormOrderAttachment a in at)
            {
                DeleteFormAttachment(a);
            }
        }
        public void DeleteFormAttachmentByOrderId(int orderId)
        {
            var at = GetFormAttachmentsByOrderId(orderId);
            foreach (FormOrderAttachment a in at)
            {
                DeleteFormAttachment(a);
            }
        }
        public void DeleteFormAttachment(FormOrderAttachment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderAttachment>();
                DeleteFormFile(t.FileID);
                rep.Delete(t);
            }
        }
        private void DeleteFormFile(int fileId)
        {
            FileManager objFile = new FileManager();
            var file = objFile.GetFile(fileId);
            objFile.DeleteFile(file);
        }
        public IEnumerable<FormOrderAttachment> GetFormAttachments()
        {
            IEnumerable<FormOrderAttachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderAttachment>();
                t = rep.Get();
            }
            return t;
        }
        public IEnumerable<FormOrderAttachment> GetFormAttachmentsByFormId(int formId)
        {
            IEnumerable<FormOrderAttachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderAttachment>();
                t = rep.Find("Where FormID=@0", formId);
            }
            return t;
        }
        public IEnumerable<FormOrderAttachment> GetFormAttachmentsByOrderId(int orderId)
        {
            IEnumerable<FormOrderAttachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderAttachment>();
                t = rep.Find("Where OrderID=@0", orderId);
            }
            return t;
        }
        public FormOrderAttachment GetFormAttachment(int attachmentId)
        {
            FormOrderAttachment t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderAttachment>();
                t = rep.GetById(attachmentId);
            }
            return t;
        }
        public FormOrderAttachment GetFormAttachmentByFileId(int fileId)
        {
            FormOrderAttachment t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderAttachment>();
                t = rep.Find("Where FileID=@0", fileId).FirstOrDefault();
            }
            return t;
        }

        public void UpdateFormAttachment(FormOrderAttachment t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderAttachment>();
                rep.Update(t);
            }
        }

        internal object GetFormAttachmentsByOrder(int orderId)
        {
            IEnumerable<FormOrderAttachment> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FormOrderAttachment>();
                t = rep.Find("Where OrderID=@0", orderId);
            }
            return t;
        }
     
        #endregion

    }
}
