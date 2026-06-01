using DotNetNuke.Data;
using System.Collections.Generic;
using tjc.Modules.JudicialReferral.Components.Models;

namespace tjc.Modules.JudicialReferral.Components.Controllers
{
    public class AttachmentController
    {
        public int AddAttachment(AttachmentInfo item)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AttachmentInfo>();
                rep.Insert(item);
            }
            return item.AttachmentID;
        }

        public IEnumerable<AttachmentInfo> GetAttachmentsByReferral(int referralId)
        {
            IEnumerable<AttachmentInfo> items;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<AttachmentInfo>();
                items = rep.Find("WHERE ReferralID = @0", referralId);
            }
            return items;
        }

        public void DeleteAttachment(int attachmentId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.Text,
                    "DELETE FROM tjc_jr_attachments WHERE AttachmentID = @0", attachmentId);
            }
        }

        public void DeleteReferralAttachments(int referralId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(System.Data.CommandType.Text,
                    "DELETE FROM tjc_jr_attachments WHERE ReferralID = @0", referralId);
            }
        }
    }
}
