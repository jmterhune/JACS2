using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class UserDefinedFieldAPIController : DnnApiController
    {
        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage GetUserDefinedFields(int p1)
        {
            List<UserDefinedField> userDefinedFields = new List<UserDefinedField>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            string searchTerm = query.ContainsKey("searchText") ? query["searchText"].ToString() : "";
            string courtIdStr = query.ContainsKey("courtId") ? query["courtId"].ToString() : "0";
            long courtId = long.Parse(courtIdStr);
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "field_name"; // Default sort column
            string sortDirection = "asc"; // Default sort direction

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new UserDefinedFieldController();
                filteredCount = ctl.GetUserDefinedFieldsCount(courtId, searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                var udfsPaged = ctl.GetUserDefinedFieldsPaged(courtId, searchTerm, recordOffset, pageSize, sortColumn, sortDirection);
                if (udfsPaged == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new UserDefinedFieldSearchResult
                    {
                        data = userDefinedFields,
                        draw = draw,
                        recordsFiltered = filteredCount,
                        recordsTotal = recordCount,
                        error = "No user defined fields found."
                    });
                }
                userDefinedFields = udfsPaged.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new UserDefinedFieldSearchResult
                {
                    data = userDefinedFields,
                    draw = draw,
                    recordsFiltered = filteredCount,
                    recordsTotal = recordCount,
                    error = null
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new UserDefinedFieldSearchResult
                {
                    data = userDefinedFields,
                    draw = draw,
                    recordsFiltered = filteredCount,
                    recordsTotal = recordCount,
                    error = $"Failed to retrieve user defined fields: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage DeleteUserDefinedField(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid user defined field ID." });
                }
                var ctl = new UserDefinedFieldController();
                var udf = ctl.GetUserDefinedField(p1);
                if (udf == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "User defined field not found." });
                }
                ctl.DeleteUserDefinedField(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "User defined field deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage GetUserDefinedField(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new UserDefinedFieldResult { data = null, error = "Invalid user defined field ID." });
                }
                var ctl = new UserDefinedFieldController();
                var udf = ctl.GetUserDefinedField(p1);
                if (udf == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new UserDefinedFieldResult { data = null, error = "User defined field not found." });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new UserDefinedFieldResult { data = udf, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new UserDefinedFieldResult { data = null, error = $"Failed to retrieve user defined field: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateUserDefinedField(JObject p1)
        {
            try
            {
                var udf = p1.ToObject<UserDefinedField>();
                if (string.IsNullOrWhiteSpace(udf.field_name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Field name is required." });
                }
                var ctl = new UserDefinedFieldController();
                udf.created_at = DateTime.Now;
                udf.updated_at = DateTime.Now;
                ctl.CreateUserDefinedField(udf);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "User defined field created successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to create user defined field: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateUserDefinedField(JObject p1)
        {
            try
            {
                var udf = p1.ToObject<UserDefinedField>();
                if (udf.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "User defined field ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(udf.field_name))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Field name is required." });
                }
                var ctl = new UserDefinedFieldController();
                var existingUdf = ctl.GetUserDefinedField(udf.id);
                if (existingUdf == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "User defined field not found." });
                }
                udf.updated_at = DateTime.Now;
                ctl.UpdateUserDefinedField(udf);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "User defined field updated successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to update user defined field: {ex.Message}" });
            }
        }

        internal class UserDefinedFieldSearchResult
        {
            public List<UserDefinedField> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class UserDefinedFieldResult
        {
            public UserDefinedField data { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            switch (columnIndex)
            {
                case 2: return "field_name";
                case 3: return "field_type";
                case 4: return "alignment";
                case 5: return "default_value";
                case 6: return "required";
                case 7: return "yes_answer_required";
                case 8: return "display_on_docket";
                case 9: return "display_on_schedule";
                case 10: return "use_in_attorany_scheduling";
                default: return "field_name";
            }
        }
    }
}