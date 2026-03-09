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
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class CourtroomAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCourtroomDropDownItems()
        {
            List<KeyValuePair<long, string>> courtrooms = new List<KeyValuePair<long, string>>();
            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                string searchTerm = query.ContainsKey("q") ? query["q"].ToString() : "";

                var ctl = new CourtroomController();
                courtrooms = ctl.GetCourtroomDropDownItems(searchTerm);
                return Request.CreateResponse(HttpStatusCode.OK, new CourtroomListItemResult { data = courtrooms, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new CourtroomListItemResult { data = courtrooms, error = $"Failed to retrieve courtroom dropdown items: {ex.Message}" });
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage GetCourtrooms(int p1)
        {
            List<CourtroomViewModel> courtrooms = new List<CourtroomViewModel>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            string searchTerm = query.ContainsKey("searchText") ? query["searchText"].ToString() : "";
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "description"; // Default sort column
            string sortDirection = "asc"; // Default sort direction

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new CourtroomController();
                filteredCount = ctl.GetCourtroomCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                var courtroomsPaged = ctl.GetCourtroomPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection);
                if (courtroomsPaged == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new CourtroomSearchResult
                    {
                        data = courtrooms,
                        draw = draw,
                        recordsFiltered = filteredCount,
                        recordsTotal = recordCount,
                        error = "No courtrooms found."
                    });
                }
                courtrooms = courtroomsPaged.Select(courtroom => new CourtroomViewModel(courtroom)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new CourtroomSearchResult
                {
                    data = courtrooms,
                    draw = draw,
                    recordsFiltered = filteredCount,
                    recordsTotal = recordCount,
                    error = null
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new CourtroomSearchResult
                {
                    data = courtrooms,
                    draw = draw,
                    recordsFiltered = filteredCount,
                    recordsTotal = recordCount,
                    error = $"Failed to retrieve courtrooms: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage DeleteCourtroom(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid courtroom ID." });
                }
                var ctl = new CourtroomController();
                var courtroom = ctl.GetCourtroom(p1);
                if (courtroom == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Courtroom not found." });
                }
                ctl.DeleteCourtroom(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage GetCourtroom(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new CourtroomResult { data = null, error = "Invalid courtroom ID." });
                }
                var ctl = new CourtroomController();
                var courtroom = ctl.GetCourtroom(p1);
                if (courtroom == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CourtroomResult { data = null, error = "Courtroom not found." });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new CourtroomResult { data = courtroom, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new CourtroomResult { data = null, error = $"Failed to retrieve courtroom: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateCourtroom(JObject p1)
        {
            try
            {
                var courtroom = p1.ToObject<Courtroom>();
                if (string.IsNullOrWhiteSpace(courtroom.description))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Courtroom description is required." });
                }
                var ctl = new CourtroomController();
                courtroom.created_at = DateTime.Now;
                courtroom.updated_at = DateTime.Now;
                ctl.CreateCourtroom(courtroom);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Entity created/updated/deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to create courtroom: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateCourtroom(JObject p1)
        {
            try
            {
                var courtroom = p1.ToObject<Courtroom>();
                if (courtroom.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Courtroom ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(courtroom.description))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Courtroom description is required." });
                }
                var ctl = new CourtroomController();
                var existingCourtroom = ctl.GetCourtroom(courtroom.id);
                if (existingCourtroom == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Courtroom not found." });
                }
                courtroom.updated_at = DateTime.Now;
                ctl.UpdateCourtroom(courtroom);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Entity created/updated/deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to update courtroom: {ex.Message}" });
            }
        }

        internal class CourtroomSearchResult
        {
            public List<CourtroomViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class CourtroomResult
        {
            public Courtroom data { get; set; }
            public string error { get; set; }
        }

        internal class CourtroomListItemResult
        {
            public List<KeyValuePair<long, string>> data { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            switch (columnIndex)
            {
                case 2:
                    return "description";
                default:
                    return "description";
            }
        }
    }
}