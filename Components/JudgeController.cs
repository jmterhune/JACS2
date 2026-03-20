using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public IEnumerable<Judge> GetFilteredJudges(long userId)
        {
            IEnumerable<Judge> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Judge>();
                t = rep.Find("Where court_id IN (select court_id from dbo.getUserCourtViewPermissions(@0))",userId);
            }
            return t;
        }
        public List<KeyValuePair<long, string>> GetJudgeCourtDropDownItems(long userId)
        {
            using (IDataContext ctx = DataContext.Instance("jacs"))
            {
                var rep = ctx.GetRepository<Judge>();
                var results = rep.Find("Where court_id in (Select court_id from dbo.getUserCourtViewPermissions(@0))",userId)
                       .Select(j => new KeyValuePair<long, string>(j.court_id.Value, j.name)).OrderBy(j=>j.Value).ToList();
                return results ?? new List<KeyValuePair<long, string>>();
            }
        }
        public List<KeyValuePair<long, string>> GetJudgeDropDownItems()
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance("jacs"))
                {
                    var rep = ctx.GetRepository<Judge>();
                    var results = rep.Get()
                        .Select(j => new KeyValuePair<long, string>(j.id, j.name)).OrderBy(j => j.Value).ToList();
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

        #region Judge Clerk Xref
        public List<KeyValuePair<long, string>> GetDummyJudgeDropDownItemsForCounty(long countyId)
        {
            // Temporary stub – return dummy clerk judge records based on countyId
            // In the future this will call the county-specific REST API with JWT            var ctl=new CountyController();
            var ctl=new CountyController();
            County county = ctl.GetCounty(countyId);

            var dummyJudges = new List<KeyValuePair<long, string>>();

            // You can make this as realistic as you want – use real-looking clerk judge IDs/names
            switch (county.code)
            {
                case "41": // Example: Manatee County 
                    dummyJudges.Add(new KeyValuePair<long, string>(1001, "Hon. Jane Doe - Manatee Circuit"));
                    dummyJudges.Add(new KeyValuePair<long, string>(1002, "Hon. Robert Smith - Manatee County"));
                    dummyJudges.Add(new KeyValuePair<long, string>(1003, "Hon. Maria Gonzalez - Family Division"));
                    break;

                case "58": // Example: Sarasota County
                    dummyJudges.Add(new KeyValuePair<long, string>(2001, "Hon. William Johnson - Sarasota Circuit"));
                    dummyJudges.Add(new KeyValuePair<long, string>(2002, "Hon. Emily Chen - Probate"));
                    dummyJudges.Add(new KeyValuePair<long, string>(2003, "Hon. David Lee - County Court"));
                    break;

                case "14": // Example: DeSoto County 
                    dummyJudges.Add(new KeyValuePair<long, string>(3001, "Hon. Thomas Brown - Hillsborough Criminal"));
                    dummyJudges.Add(new KeyValuePair<long, string>(3002, "Hon. Sarah Miller - Civil Division"));
                    dummyJudges.Add(new KeyValuePair<long, string>(3003, "Hon. Michael Rivera - Juvenile"));
                    break;

                default:
                    // Unknown county → return empty or a fallback message
                    dummyJudges.Add(new KeyValuePair<long, string>(0, "No judges available for this county yet"));
                    break;
            }

            // Sort alphabetically by name (optional but nice for dropdown)
            return dummyJudges.OrderBy(j => j.Value).ToList();
        }
        public List<KeyValuePair<long, string>> GetJudgeDropDownItemsForCounty(long countyId)
        {
            var county = new CountyController().GetCounty(countyId);
            if (county == null || string.IsNullOrEmpty(county.auth_end_point_url))
                return new List<KeyValuePair<long, string>>();

            // Example: call external REST API with JWT
            var client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetJwtForCounty(county)); // your JWT logic

            var response = client.GetAsync($"{county.auth_end_point_url}/judges").Result;
            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                // Parse JSON → map to List<KeyValuePair<long, string>>
                // e.g. using Newtonsoft.Json or System.Text.Json
                return JsonConvert.DeserializeObject<List<KeyValuePair<long, string>>>(json);
            }

            return new List<KeyValuePair<long, string>>();
        }
        public IEnumerable<JudgeClerkXrefListItem> GetJudgeXrefByJudge(long judgeId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<JudgeClerkXrefListItem>(
                    System.Data.CommandType.Text,
                    "SELECT x.*,j.name as judge_name,c.name as county_name FROM judge_clerk_xref x "+ 
                        "join judges j on j.id=x.judge_id " +
                        "join counties c on c.id=x.county_id " +
                    "WHERE x.judge_id=@0", judgeId);
            }
        }
        public IEnumerable<JudgeClerkXref> GetJudgeXref(long judgeId,long countyId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<JudgeClerkXref>(
                    System.Data.CommandType.Text,
                    "SELECT x.*,j.name as judge_name,c.name as county_name FROM judge_clerk_xref x " +
                        "join judges j on j.id=x.judge_id " +
                        "join counties c on c.id=x.county_id " +
                    "WHERE x.judge_id=@0 AND x.county_id=@1", judgeId,countyId);
            }
        }
        public void CreateJudgeClerkXref(JudgeClerkXref t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var sql = "DELETE FROM judge_clerk_xref WHERE judge_id=@0 AND county_id=@1";
                ctx.Execute(CommandType.Text, sql, t.judge_id, t.county_id);
            }
        }
        public void CreateJudgeXref(JudgeClerkXref t)
        {
            using (IDataContext context = DataContext.Instance(CONN_JACS))
            {
                var sql = "Insert Into judge_clerk_xref(judge_id,county_id,clerk_judge_id,clerk_judge_name,created_at,updated_at) Values(@0,@1,@2,@3,@4,@5)";
                context.Execute(CommandType.Text, sql, t.judge_id, t.county_id, t.clerk_judge_id, t.clerk_judge_name, t.created_at, t.updated_at);
            }
        }
        public void UpdateJudgeXref(JudgeClerkXref t)
        {
            using (IDataContext context = DataContext.Instance(CONN_JACS))
            {
                var sql = "UPDATE judge_clerk_xref SET clerk_judge_name=@0, clerk_judge_id=@1, updated_at=@2 WHERE judge_id=@3 AND county_id=@4;";
                context.Execute(CommandType.Text, sql, t.clerk_judge_id, t.clerk_judge_name, t.updated_at, t.judge_id, t.county_id);
            }
        }
        public void DeleteJudgeXref(long judgeId, long countyId)
        {
            using (IDataContext context = DataContext.Instance(CONN_JACS))
            {
                var sql = "DELETE FROM judge_clerk_xref WHERE judge_id=@0 AND county_id=@1";
                context.Execute(CommandType.Text, sql, judgeId, countyId);
            }
        }

        #endregion
    }
}