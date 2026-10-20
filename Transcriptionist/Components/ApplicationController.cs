using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.Transcriptionist.Components
{
    internal class ApplicationController
    {
        // The application tables live in the jud12 (external) site's database.
        private const string CONN_JUD12 = "Jud12";

        private readonly IHostSettings _hostSettings;

        public ApplicationController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void UpdateApplication(Application item)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.GetRepository<Application>().Update(item);
            }
        }

        // Removes the application and its experience and attachment rows. The attachment files themselves
        // belong to the jud12 site's file system and are not touched.
        public void DeleteApplication(long transcriptionistId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.Execute(System.Data.CommandType.Text, "DELETE FROM tjc_cat_experience WHERE TranscriptionistID = @0", transcriptionistId);
                ctx.Execute(System.Data.CommandType.Text, "DELETE FROM tjc_cat_attachments WHERE TranscriptionistID = @0", transcriptionistId);
                ctx.Execute(System.Data.CommandType.Text, "DELETE FROM tjc_cat_applications WHERE TranscriptionistID = @0", transcriptionistId);
            }
        }

        public Application GetApplication(long transcriptionistId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<Application>().GetById(transcriptionistId);
            }
        }

        public IEnumerable<Application> GetAll()
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<Application>().Get();
            }
        }
    }
}
