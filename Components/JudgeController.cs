using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Components
{
    internal class JudgeController
    {
        private const string CONN_JACS = "jacs"; // Connection

        public void CreateJudge(Judge t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Judge>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }

        public void DeleteJudge(long judgeId)
        {
            var t = GetJudge(judgeId);
            DeleteJudge(t);
        }

        public void DeleteJudge(Judge t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Judge>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Judge> GetJudges()
        {
            IEnumerable<Judge> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Judge>();
                t = rep.Get();
            }
            return t;
        }
        public List<KeyValuePair<long, string>> GetJudgeDropDownItems()
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance("jacs"))
                {
                    var rep = ctx.GetRepository<Judge>();
                    var results = rep.Get()
                        .Select(j => new KeyValuePair<long, string>(j.id, j.name)).ToList();
                    return results ?? new List<KeyValuePair<long, string>>();
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return new List<KeyValuePair<long, string>>();
            }
        }
        public Judge GetJudge(long judgeId)
        {
            Judge t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Judge>();
                t = rep.GetById(judgeId);
            }
            return t;
        }
        public Judge GetJudgeByCourt(long courtId)
        {
            Judge t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Judge>();
                t = rep.Find("Where court_id=@0",courtId).FirstOrDefault();
            }
            return t;
        }


        public void UpdateJudge(Judge t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Judge>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }

        public IEnumerable<JudgeViewModel> GetJudgesPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<JudgeViewModel>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_judge_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }

        public int GetJudgesCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_judge_count",
                    searchTerm ?? string.Empty
                );
            }
        }
    }
}