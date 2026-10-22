using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.Transcriptionist.Components
{
    internal class AttachmentController
    {
        private const string CONN_JUD12 = "Jud12";

        private readonly IHostSettings _hostSettings;

        public AttachmentController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public IEnumerable<Attachment> GetByTranscriptionistId(long transcriptionistId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<Attachment>().Find("WHERE TranscriptionistID = @0", transcriptionistId);
            }
        }
    }
}
