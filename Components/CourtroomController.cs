using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
namespace tjc.Modules.jacs.Components
{
    internal class CourtroomController
    {
        private const string CONN_JACS = "jacs"; //Connection

        #region Courtroom Methods
        public void CreateCourtroom(Courtroom t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Courtroom>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }
        public void DeleteCourtroom(long courtroomId)
        {
            var t = GetCourtroom(courtroomId);
            DeleteCourtroom(t);
        }
        public void DeleteCourtroom(Courtroom t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Courtroom>();
                rep.Delete(t);
            }
        }
        public IEnumerable<Courtroom> GetCourtrooms()
        {
            IEnumerable<Courtroom> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Courtroom>();
                t = rep.Get();
            }
            return t;
        }
        public List<KeyValuePair<long, string>> GetCourtroomDropDownItems(string searchTerm)
        {
            try
            {
                // Normalize search term
                searchTerm = string.IsNullOrWhiteSpace(searchTerm) ? string.Empty : searchTerm.Trim();

                using (IDataContext ctx = DataContext.Instance("jacs"))
                {
                    var rep = ctx.GetRepository<Courtroom>();
                    var results = rep.Find("WHERE description LIKE @0", $"%{searchTerm}%")
                        .Select(c => new KeyValuePair<long, string>(c.id, c.description)).OrderBy(c => c.Value).ToList();
                    return results ?? new List<KeyValuePair<long, string>>();
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return new List<KeyValuePair<long, string>>();
            }
        }

        /// <summary>
        /// Returns only courtrooms that have an xref entry for the given county,
        /// ensuring the timeslot courtroom dropdown is scoped to the correct county.
        /// </summary>
        public List<KeyValuePair<long, string>> GetCourtroomDropDownItemsByCounty(long countyId)
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance(CONN_JACS))
                {
                    return ctx.ExecuteQuery<CourtroomDropDownItem>(
                        System.Data.CommandType.Text,
                        "SELECT c.id AS Id, c.description AS Description " +
                        "FROM courtrooms c " +
                        "INNER JOIN courtroom_clerk_xref x ON x.courtroom_id = c.id " +
                        "WHERE x.county_id = @0 " +
                        "ORDER BY c.description", countyId)
                        .Select(r => new KeyValuePair<long, string>(r.Id, r.Description))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return new List<KeyValuePair<long, string>>();
            }
        }

        private class CourtroomDropDownItem
        {
            public long Id { get; set; }
            public string Description { get; set; }
        }
        public Courtroom GetCourtroom(long courtroomId)
        {
            Courtroom t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Courtroom>();
                t = rep.GetById(courtroomId);
            }
            return t;
        }
        public void UpdateCourtroom(Courtroom t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Courtroom>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }
        public IEnumerable<Courtroom> GetCourtroomPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<Courtroom>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_courtroom_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }
        public int GetCourtroomCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_courtroom_count",
                    searchTerm ?? string.Empty
                );
            }
        }
        public Dictionary<long, int> GetCourtroomXrefCounts()
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rows = ctx.ExecuteQuery<CourtroomXrefCountRow>(
                    CommandType.Text,
                    "SELECT courtroom_id, COUNT(*) AS cnt FROM courtroom_clerk_xref GROUP BY courtroom_id");
                return rows.ToDictionary(r => r.courtroom_id, r => r.cnt);
            }
        }

        internal class CourtroomXrefCountRow
        {
            public long courtroom_id { get; set; }
            public int cnt { get; set; }
        }
        #endregion // end Courtroom Methods

        #region Courtroom Clerk Xref Methods
        public List<KeyValuePair<long, string>> GetCourtroomXrefDropDownItemsByCounty(long countyId)
        {
            try
            {
                var countyCtl = new CountyController();
                var county = countyCtl.GetCounty(countyId);

                if (county == null || string.IsNullOrWhiteSpace(county.auth_end_point_url))
                {
                    Exceptions.LogException(new Exception($"County not found or missing auth endpoint for county ID {countyId}"));
                    return new List<KeyValuePair<long, string>>();
                }

                // Use the STATIC token stored in the county table (already decrypted)
                string token = county.decrypted_token;
                if (string.IsNullOrWhiteSpace(token))
                {
                    Exceptions.LogException(new Exception($"Static token is missing/empty for county ID {countyId} ({county.name})"));
                    return new List<KeyValuePair<long, string>>();
                }

                var apiCtl = new ApiEndpointController();
                var api = apiCtl.GetApiEndpointByCountyAndType(county.id, (int)ApiEndpointType.GetClerkCourtrooms);

                if (api == null)
                {
                    Exceptions.LogException(new Exception($"No API endpoint configured for GetClerkCourtrooms in county ID {countyId} ({county.name})"));
                    return new List<KeyValuePair<long, string>>();
                }

                try
                {
                    var response = apiCtl.CallExternalApi(api, token, null, HttpMethod.Get).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = response.Content.ReadAsStringAsync().Result;
                        Exceptions.LogException(new Exception(
                            $"External Clerk Courtroom API failed (static token). CountyID: {countyId}, Status: {response.StatusCode}, Response: {errorContent}"));
                        return new List<KeyValuePair<long, string>>();
                    }

                    var json = response.Content.ReadAsStringAsync().Result;
                    var courtrooms = JsonConvert.DeserializeObject<List<CourtroomXrefItem>>(json);

                    return courtrooms?.Select(c => new KeyValuePair<long, string>(c.CourtRoomId, c.CourtroomName))
                                     .OrderBy(c => c.Value)
                                     .ToList() ?? new List<KeyValuePair<long, string>>();
                }
                catch (Exception ex)
                {
                    Exceptions.LogException(new Exception($"Error calling external Courtroom API (static token) for county {countyId} ({county.name})", ex));
                    return new List<KeyValuePair<long, string>>();
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(new Exception($"Unexpected error in GetCourtroomXrefDropDownItemsByCounty for county {countyId}", ex));
                return new List<KeyValuePair<long, string>>();
            }
        }
        public List<KeyValuePair<long, string>> GetDummyCourtroomDropDownItemsForCounty(long countyId)
        {
            // Temporary stub – return dummy courtroom records based on countyId
            // In the future this will call the county-specific REST API with JWT
            var ctl=new CountyController();
            County county = ctl.GetCounty(countyId);
            var dummyCourtrooms = new List<KeyValuePair<long, string>>();

            // You can make this as realistic as you want – use real-looking courtroom   IDs/names
            switch (county.code)
            {
                case "41": // Example: Manatee County
                    dummyCourtrooms.Add(new KeyValuePair<long, string>(1001, ""));
                    dummyCourtrooms.Add(new KeyValuePair<long, string>(1002, "2A"));
                    dummyCourtrooms.Add(new KeyValuePair<long, string>(1003, "3B"));
                    break;

                case "58": // Example: Sarasota County
                    dummyCourtrooms.Add(new KeyValuePair<long, string>(2001, "6E"));
                    dummyCourtrooms.Add(new KeyValuePair<long, string>(2002, "6F"));
                    dummyCourtrooms.Add(new KeyValuePair<long, string>(2003, "6G"));
                    break;

                case "14": // Example: DeSoto County 
                    dummyCourtrooms.Add(new KeyValuePair<long, string>(3001, "8A"));
                    dummyCourtrooms.Add(new KeyValuePair<long, string>(3002, "8B"));
                    dummyCourtrooms.Add(new KeyValuePair<long, string>(3003, "8C"));
                    break;

                default:
                    // Unknown county → return empty or a fallback message
                    dummyCourtrooms.Add(new KeyValuePair<long, string>(0, "No courtrooms available for this county yet"));
                    break;
            }

            // Sort alphabetically by name (optional but nice for dropdown)
            return dummyCourtrooms.OrderBy(j => j.Value).ToList();
        }

        public IEnumerable<CourtroomClerkXrefListItem> GetCourtroomXrefByCourtroom(long courtroomId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<CourtroomClerkXrefListItem>(
                    System.Data.CommandType.Text,
                    "SELECT x.*, crt.description as courtroom_name, c.name as county_name " +
                    "FROM courtroom_clerk_xref x " +
                           "join courtrooms crt on crt.id = x.courtroom_id " +
                           "join counties c on c.id = x.county_id " +
                    "WHERE x.courtroom_id=@0", courtroomId);
            }
        }
        public IEnumerable<CourtroomClerkXref> GetCourtroomXref(long courtroomId, long countyId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<CourtroomClerkXref>(
                    System.Data.CommandType.Text,
                    "SELECT x.*, crt.description as courtroom_name, c.name as county_name " +
                    "FROM courtroom_clerk_xref x " +
                           "join courtrooms crt on crt.id = x.courtroom_id " +
                           "join counties c on c.id = x.county_id " +
                    "WHERE x.courtroom_id=@0 AND x.county_id=@1", courtroomId, countyId);
            }
        }
        public void CreateCourtroomClerkXref(CourtroomClerkXref t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var sql = "DELETE FROM courtroom_clerk_xref WHERE courtroom_id=@0 AND county_id=@1";
                ctx.Execute(CommandType.Text, sql, t.courtroom_id, t.county_id);
            }
        }
        public void CreateCourtroomXref(CourtroomClerkXref t)
        {
            using (IDataContext context = DataContext.Instance(CONN_JACS))
            {
                var sql = "Insert Into courtroom_clerk_xref(courtroom_id,county_id,clerk_courtroom_id,clerk_courtroom_name,created_at,updated_at) Values(@0,@1,@2,@3,@4,@5)";
                context.Execute(CommandType.Text, sql, t.courtroom_id, t.county_id, t.clerk_courtroom_id, t.clerk_courtroom_name, t.created_at, t.updated_at);
            }
        }
        public void UpdateCourtroomXref(CourtroomClerkXref t)
        {
            using (IDataContext context = DataContext.Instance(CONN_JACS))
            {
                var sql = "UPDATE courtroom_clerk_xref SET clerk_courtroom_name=@0, clerk_courtroom_id=@1, updated_at=@2 WHERE courtroom_id=@3 AND county_id=@4;";
                context.Execute(CommandType.Text, sql, t.clerk_courtroom_name, t.clerk_courtroom_id, t.updated_at, t.courtroom_id, t.county_id);
            }
        }
        public void DeleteCourtroomXref(long courtroomId, long countyId)
        {
            using (IDataContext context = DataContext.Instance(CONN_JACS))
            {
                var sql = "DELETE FROM courtroom_clerk_xref WHERE courtroom_id=@0 AND county_id=@1";
                context.Execute(CommandType.Text, sql, courtroomId, countyId);
            }
        }

        #endregion
    }
}