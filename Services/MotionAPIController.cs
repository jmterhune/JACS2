using DotNetNuke.Entities.Users;
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
    public class MotionAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetMotions(int p1)
        {
            List<MotionViewModel> motions = new List<MotionViewModel>();
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
                var ctl = new MotionController();
                filteredCount = ctl.GetMotionsCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                var motionsPaged = ctl.GetMotionsPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection);
                if (motionsPaged == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new MotionSearchResult
                    {
                        data = motions,
                        draw = draw,
                        recordsFiltered = filteredCount,
                        recordsTotal = recordCount,
                        error = "No motions found."
                    });
                }
                motions = motionsPaged.Select(motion => new MotionViewModel(motion)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new MotionSearchResult
                {
                    data = motions,
                    draw = draw,
                    recordsFiltered = filteredCount,
                    recordsTotal = recordCount,
                    error = null
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new MotionSearchResult
                {
                    data = motions,
                    draw = draw,
                    recordsFiltered = filteredCount,
                    recordsTotal = recordCount,
                    error = $"Failed to retrieve motions: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetMotionDropDownItems()
        {
            List<KeyValuePair<long, string>> motions = new List<KeyValuePair<long, string>>();
            try
            {
                var ctl = new MotionController();
                motions = ctl.GetMotions().Select(motion => new KeyValuePair<long, string>(motion.id, motion.description)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new ListItemOptionResult { data = motions, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ListItemOptionResult { data = motions, error = $"Failed to retrieve motion dropdown items: {ex.Message}" });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteMotion(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid motion ID." });
                }
                var ctl = new MotionController();
                var motion = ctl.GetMotion(p1);
                if (motion == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Motion not found." });
                }
                ctl.DeleteMotion(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Motion deleted successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to delete motion: {ex.Message}" });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetMotion(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new MotionResult { data = null, error = "Invalid motion ID." });
                }
                var ctl = new MotionController();
                var motion = ctl.GetMotion(p1);
                if (motion == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new MotionResult { data = null, error = "Motion not found." });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new MotionResult { data = motion, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new MotionResult { data = null, error = $"Failed to retrieve motion: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateMotion(JObject p1)
        {
            try
            {
                var motion = p1.ToObject<Motion>();
                if (string.IsNullOrWhiteSpace(motion.description))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Motion description is required." });
                }
                var ctl = new MotionController();
                motion.created_at = DateTime.Now;
                motion.updated_at = DateTime.Now;
                ctl.CreateMotion(motion);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Motion created successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to create motion: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateMotion(JObject p1)
        {
            try
            {
                var motion = p1.ToObject<Motion>();
                if (motion.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Motion ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(motion.description))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Motion description is required." });
                }
                var ctl = new MotionController();
                var existingMotion = ctl.GetMotion(motion.id);
                if (existingMotion == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Motion not found." });
                }
                motion.updated_at = DateTime.Now;
                ctl.UpdateMotion(motion);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Motion updated successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to update motion: {ex.Message}" });
            }
        }

        internal class MotionSearchResult
        {
            public List<MotionViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class MotionResult
        {
            public Motion data { get; set; }
            public string error { get; set; }
        }


        private string GetSortColumn(int columnIndex)
        {
            switch (columnIndex)
            {
                case 2:
                    return "description";
                case 3:
                    return "lag";
                case 4:
                    return "lead";
                default:
                    return "description";
            }
        }
    }
}