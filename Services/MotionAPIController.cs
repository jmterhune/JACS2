using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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

            string searchTerm = !query.ContainsKey("searchText") ? "" : query["searchText"].ToString();
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
                motions = ctl.GetMotionsPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).Select(motion => new MotionViewModel(motion)).ToList();
                return Request.CreateResponse(new MotionSearchResult { data = motions, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new MotionSearchResult { data = motions, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetMotionDropDownItems()
        {
            List<MotionViewModel> motions = new List<MotionViewModel>();
            var ctl = new MotionController();
            motions = ctl.GetMotions().Select(motion => new MotionViewModel(motion)).ToList();
            return Request.CreateResponse(new MotionSearchResult { data = motions, error = null });
        }

        [HttpGet]
        public HttpResponseMessage DeleteMotion(long p1)
        {
            try
            {
                var ctl = new MotionController();
                ctl.DeleteMotion(p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetMotion(long p1)
        {
            try
            {
                var ctl = new MotionController();
                Motion motion = ctl.GetMotion(p1);
                return Request.CreateResponse(new MotionResult { data = motion, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new MotionResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateMotion(JObject p1)
        {
            try
            {
                var ctl = new MotionController();
                var motion = p1.ToObject<Motion>();
                ctl.CreateMotion(motion);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateMotion(JObject p1)
        {
            try
            {
                var ctl = new MotionController();
                var motion = p1.ToObject<Motion>();
                ctl.UpdateMotion(motion);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message);
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
            string fieldName = "description";
            switch (columnIndex)
            {
                case 2:
                    fieldName = "description";
                    break;
                default:
                    fieldName = "description";
                    break;
            }
            return fieldName;
        }
    }
}