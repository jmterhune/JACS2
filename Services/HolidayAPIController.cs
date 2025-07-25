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
    public class HolidayAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetHolidays(int p1)
        {
            List<HolidayViewModel> holidays = new List<HolidayViewModel>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            string searchTerm = !query.ContainsKey("searchText") ? "" : query["searchText"].ToString();
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "name"; // Default sort column
            string sortDirection = "asc"; // Default sort direction

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new HolidayController();
                filteredCount = ctl.GetHolidaysCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                holidays = ctl.GetHolidaysPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).Select(holiday => new HolidayViewModel(holiday)).ToList();
                return Request.CreateResponse(new HolidaySearchResult { data = holidays, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new HolidaySearchResult { data = holidays, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteHoliday(long p1)
        {
            try
            {
                var ctl = new HolidayController();
                ctl.DeleteHoliday(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Holiday deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetHoliday(long p1)
        {
            try
            {
                var ctl = new HolidayController();
                Holiday holiday = ctl.GetHoliday(p1);
                if (holiday == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new HolidayResult { data = null, error = "Holiday not found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new HolidayResult { data = holiday, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new HolidayResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateHoliday(JObject p1)
        {
            try
            {
                var ctl = new HolidayController();
                var holiday = p1.ToObject<Holiday>();
                if (string.IsNullOrWhiteSpace(holiday.name) || holiday.date == default)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name and Date are required." });
                }
                holiday.created_at = DateTime.Now;
                holiday.updated_at = DateTime.Now;
                ctl.CreateHoliday(holiday);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Holiday created successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateHoliday(JObject p1)
        {
            try
            {
                var ctl = new HolidayController();
                var holiday = p1.ToObject<Holiday>();
                if (holiday.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Holiday ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(holiday.name) || holiday.date == default)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name and Date are required." });
                }
                var existingHoliday = ctl.GetHoliday(holiday.id);
                if (existingHoliday == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Holiday not found." });
                }
                holiday.updated_at = DateTime.Now;
                ctl.UpdateHoliday(holiday);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Holiday updated successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        internal class HolidaySearchResult
        {
            public List<HolidayViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class HolidayResult
        {
            public Holiday data { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            string fieldName = "name";
            switch (columnIndex)
            {
                case 2:
                    fieldName = "name";
                    break;
                case 3:
                    fieldName = "date";
                    break;
                default:
                    fieldName = "name";
                    break;
            }
            return fieldName;
        }
    }
}