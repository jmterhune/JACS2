using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.UI.WebControls.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;

namespace tjc.Modules.jacs.Components
{
    internal class MotionController
    {
        private const string CONN_JACS = "jacs";

        #region Motion Methods
        public void CreateMotion(Motion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Motion>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }

        public void DeleteMotion(long motionId)
        {
            var t = GetMotion(motionId);
            DeleteMotion(t);
        }

        public void DeleteMotion(Motion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Motion>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Motion> GetMotions()
        {
            IEnumerable<Motion> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Motion>();
                t = rep.Get();
            }
            return t;
        }

        public List<KeyValuePair<long, string>> GetMotionDropDownItems()
        {
            try
            {
                using (IDataContext ctx = DataContext.Instance("jacs"))
                {
                    var rep = ctx.GetRepository<Motion>();
                    var results = rep.Get()
                        .Select(m => new KeyValuePair<long, string>(m.id, m.description)).OrderBy(v => v.Value).ToList();
                    return results ?? new List<KeyValuePair<long, string>>();
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return new List<KeyValuePair<long, string>>();
            }
        }

        public Motion GetMotion(long motionId)
        {
            Motion t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Motion>();
                t = rep.GetById(motionId);
            }
            return t;
        }

        public void UpdateMotion(Motion t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<Motion>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }

        public IEnumerable<Motion> GetMotionsPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<Motion>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_motion_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }

        public int GetMotionsCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_motion_count",
                    searchTerm ?? string.Empty
                );
            }
        }
        #endregion

        #region Motion Clerk Xref Methods
        public List<KeyValuePair<long, string>> GetMotionXrefDropDownItemsByCounty(long countyId)
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

                string token = county.decrypted_token;
                if (string.IsNullOrWhiteSpace(token))
                {
                    Exceptions.LogException(new Exception($"Static token is missing/empty for county ID {countyId} ({county.name})"));
                    return new List<KeyValuePair<long, string>>();
                }

                var apiCtl = new ApiEndpointController();
                var api = apiCtl.GetApiEndpointByCountyAndType(county.id, (int)ApiEndpointType.GetClerkMotions);

                if (api == null)
                {
                    Exceptions.LogException(new Exception($"No API endpoint configured for GetClerkMotions in county ID {countyId} ({county.name})"));
                    return new List<KeyValuePair<long, string>>();
                }

                try
                {
                    var response = apiCtl.CallExternalApi(api, token, null, HttpMethod.Get).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = response.Content.ReadAsStringAsync().Result;
                        Exceptions.LogException(new Exception(
                            $"External Clerk Motion API failed (static token). CountyID: {countyId}, Status: {response.StatusCode}, Response: {errorContent}"));
                        return new List<KeyValuePair<long, string>>();
                    }

                    var json = response.Content.ReadAsStringAsync().Result;
                    var motions = JsonConvert.DeserializeObject<List<MotionXrefItem>>(json);

                    return motions?.Select(m => new KeyValuePair<long, string>(m.EventTypeId, m.EventTypeName))
                                  .OrderBy(m => m.Value)
                                  .ToList() ?? new List<KeyValuePair<long, string>>();
                }
                catch (Exception ex)
                {
                    Exceptions.LogException(new Exception($"Error calling external Motion API (static token) for county {countyId} ({county.name})", ex));
                    return new List<KeyValuePair<long, string>>();
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(new Exception($"Unexpected error in GetMotionXrefDropDownItemsByCounty for county {countyId}", ex));
                return new List<KeyValuePair<long, string>>();
            }
        }
        public List<KeyValuePair<long, string>> GetDummyMotionDropDownItemsForCounty(long countyId)
        {
            var ctl = new CountyController();
            County county = ctl.GetCounty(countyId);
            var dummyMotions = new List<KeyValuePair<long, string>>();

            switch (county.code)
            {
                case "41": // Manatee
                    dummyMotions.Add(new KeyValuePair<long, string>(1001, "Motion 1A"));
                    dummyMotions.Add(new KeyValuePair<long, string>(1002, "Motion 2B"));
                    dummyMotions.Add(new KeyValuePair<long, string>(1003, "Motion 3C"));
                    break;
                case "58": // Sarasota
                    dummyMotions.Add(new KeyValuePair<long, string>(2001, "Motion 4D"));
                    dummyMotions.Add(new KeyValuePair<long, string>(2002, "Motion 5E"));
                    break;
                case "14": // DeSoto
                    dummyMotions.Add(new KeyValuePair<long, string>(3001, "Motion 6F"));
                    dummyMotions.Add(new KeyValuePair<long, string>(3002, "Motion 7G"));
                    break;
                default:
                    dummyMotions.Add(new KeyValuePair<long, string>(0, "No motions available for this county yet"));
                    break;
            }
            return dummyMotions.OrderBy(j => j.Value).ToList();
        }

        public IEnumerable<MotionClerkXrefListItem> GetMotionXrefByMotion(long motionId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<MotionClerkXrefListItem>(
                    System.Data.CommandType.Text,
                    "SELECT x.*, m.description as motion_name, c.name as county_name " +
                    "FROM motion_clerk_xref x " +
                    "JOIN motions m ON m.id = x.motion_id " +
                    "JOIN counties c ON c.id = x.county_id " +
                    "WHERE x.motion_id=@0", motionId);
            }
        }

        public IEnumerable<MotionClerkXref> GetMotionXref(long motionId, long countyId)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<MotionClerkXref>(
                    System.Data.CommandType.Text,
                    "SELECT x.*, m.description as motion_name, c.name as county_name " +
                    "FROM motion_clerk_xref x " +
                    "JOIN motions m ON m.id = x.motion_id " +
                    "JOIN counties c ON c.id = x.county_id " +
                    "WHERE x.motion_id=@0 AND x.county_id=@1", motionId, countyId);
            }
        }

        public void CreateMotionXref(MotionClerkXref t)
        {
            using (IDataContext context = DataContext.Instance(CONN_JACS))
            {
                var sql = "INSERT INTO motion_clerk_xref(motion_id,county_id,clerk_motion_id,clerk_motion_name,created_at,updated_at) VALUES(@0,@1,@2,@3,@4,@5)";
                context.Execute(CommandType.Text, sql, t.motion_id, t.county_id, t.clerk_motion_id, t.clerk_motion_name, t.created_at, t.updated_at);
            }
        }

        public void DeleteMotionXref(long motionId, long countyId)
        {
            using (IDataContext context = DataContext.Instance(CONN_JACS))
            {
                var sql = "DELETE FROM motion_clerk_xref WHERE motion_id=@0 AND county_id=@1";
                context.Execute(CommandType.Text, sql, motionId, countyId);
            }
        }
        #endregion
    }
}