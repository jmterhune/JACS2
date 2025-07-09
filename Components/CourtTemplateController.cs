using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Components
{
    internal class CourtTemplateController
    {
        private const string CONN_JACS = "jacs"; //Connection

        public void CreateCourtTemplate(CourtTemplate t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }

        public void DeleteCourtTemplate(long courttemplateId)
        {
            var t = GetCourtTemplate(courttemplateId);
            DeleteCourtTemplate(t);
        }

        public void DeleteCourtTemplate(CourtTemplate t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                rep.Delete(t);
            }
        }

        public IEnumerable<CourtTemplate> GetCourtTemplates()
        {
            IEnumerable<CourtTemplate> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                t = rep.Get();
            }
            return t;
        }

        public CourtTemplate GetCourtTemplate(long courttemplateId)
        {
            CourtTemplate t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                t = rep.GetById(courttemplateId);
                if (t != null)
                {
                    // Populate court description
                    var court = ctx.GetRepository<Court>().GetById(t.court_id);
                    t.court_description = court?.description;
                    // Populate judge name
                    var judge = ctx.GetRepository<Judge>().Find($"WHERE court_id = {t.court_id}").FirstOrDefault();
                    t.judge_name = judge?.name;
                }
            }
            return t;
        }

        public void UpdateCourtTemplate(CourtTemplate t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<CourtTemplate>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }

        public IEnumerable<CourtTemplateViewModel> GetCourtTemplatesPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<CourtTemplateViewModel>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_template_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }

        public int GetCourtTemplatesCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_court_template_count",
                    searchTerm ?? string.Empty
                );
            }
        }
    }
}