using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System.Collections.Generic;

namespace tjc.Modules.Transcriptionist.Components
{
    internal class ExperienceController
    {
        private const string CONN_JUD12 = "Jud12";

        private readonly IHostSettings _hostSettings;

        public ExperienceController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        public void CreateExperience(Experience item)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.GetRepository<Experience>().Insert(item);
            }
        }

        public void UpdateExperience(Experience item)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.GetRepository<Experience>().Update(item);
            }
        }

        public void DeleteExperience(long experienceId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                ctx.Execute(System.Data.CommandType.Text, "DELETE FROM tjc_cat_experience WHERE ExperienceID = @0", experienceId);
            }
        }

        public Experience GetExperience(long experienceId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<Experience>().GetById(experienceId);
            }
        }

        public IEnumerable<Experience> GetByTranscriptionistId(long transcriptionistId)
        {
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                return ctx.GetRepository<Experience>().Find("WHERE TranscriptionistID = @0", transcriptionistId);
            }
        }
    }
}
