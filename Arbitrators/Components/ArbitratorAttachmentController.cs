/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;

namespace tjc.Modules.Arbitrators.Components
{
    /// <summary>
    /// Data access for arbitrator application attachments. Same "Jud12"
    /// cross-database connection as ArbitratorApplicationController - see that
    /// class for why.
    /// </summary>
    public class ArbitratorAttachmentController
    {
        private const string CONN_JUD12 = "Jud12";
        private readonly IHostSettings _hostSettings;

        public ArbitratorAttachmentController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        private IDataContext GetContext()
        {
            return DataContext.Instance(_hostSettings, CONN_JUD12);
        }

        public IEnumerable<ArbitratorAttachment> GetAttachments(int arbitratorApplicationId)
        {
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<ArbitratorAttachment>();
                return rep.Find("WHERE ArbitratorApplicationID = @0", arbitratorApplicationId)
                    .OrderBy(a => a.AttachmentType)
                    .ToList();
            }
        }

        /// <summary>
        /// Returns the attachment including its FileData blob, for streaming
        /// by AttachmentViewer.ashx. Callers that only need the list (name,
        /// size, type) should use GetAttachments instead to avoid pulling
        /// every blob back at once.
        /// </summary>
        public ArbitratorAttachment GetAttachment(int attachmentId)
        {
            using (IDataContext ctx = GetContext())
            {
                var rep = ctx.GetRepository<ArbitratorAttachment>();
                return rep.GetById(attachmentId);
            }
        }
    }
}
