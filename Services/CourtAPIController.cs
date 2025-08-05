// Filename: CourtAPIController.cs
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
    public class CourtAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCourts(int p1)
        {
            List<CourtViewModel> courts = new List<CourtViewModel>();
            int recordCount = p1;
            int filteredCount = 0;
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            string searchTerm = query.ContainsKey("searchText") ? query["searchText"] : string.Empty;
            Int32.TryParse(query.ContainsKey("draw") ? query["draw"] : "0", out int draw);
            Int32.TryParse(query.ContainsKey("length") ? query["length"] : "25", out int pageSize);
            Int32.TryParse(query.ContainsKey("start") ? query["start"] : "0", out int recordOffset);

            string sortColumn = "description";
            string sortDirection = "asc";

            if (query.ContainsKey("order[0].column") && query.ContainsKey("order[0].dir"))
            {
                Int32.TryParse(query["order[0].column"], out int sortIndex);
                sortColumn = GetSortColumn(sortIndex);
                sortDirection = query["order[0].dir"];
            }

            try
            {
                var ctl = new CourtController();
                filteredCount = ctl.GetCourtsCount(searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                courts = ctl.GetCourtsPaged(searchTerm, recordOffset, pageSize, sortColumn, sortDirection).ToList();
                return Request.CreateResponse(new CourtSearchResult { data = courts, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new CourtSearchResult { data = courts, draw = draw, recordsFiltered = filteredCount, recordsTotal = recordCount, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourtDropDownItems()
        {
            List<KeyValuePair<long, string>> courts = new List<KeyValuePair<long, string>>();

            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                string searchTerm = !query.ContainsKey("q") ? "" : query["q"].ToString();

                var ctl = new CourtController();
                courts = ctl.GetCourtDropDownItems(searchTerm);
                return Request.CreateResponse(new ListItemOptionResult { data = courts, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new ListItemOptionResult { data = courts, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourtsUnassigned()
        {
            try
            {
                var courtController = new CourtController();
                var judgeController = new JudgeController();
                var assignedCourtIds = judgeController.GetJudges()
                    .Where(j => j.court_id.HasValue)
                    .Select(j => j.court_id.Value)
                    .Distinct()
                    .ToList();

                var courts = courtController.GetCourts()
                    .Where(c => !assignedCourtIds.Contains(c.id))
                    .Select(c => new
                    {
                        id = c.id,
                        description = c.description
                    }).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { data = courts, error = "" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteCourt(long p1)
        {
            try
            {
                var ctl = new CourtController();
                var court = ctl.GetCourt(p1);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }
                ctl.DeleteCourt(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourt(long p1)
        {
            try
            {
                var ctl = new CourtController();
                Court court = ctl.GetCourt(p1);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CourtResult { data = null, error = "Court not found" });
                }
                CourtViewModel courtViewModel = new CourtViewModel(court);
                return Request.CreateResponse(HttpStatusCode.OK, new CourtResult { data = courtViewModel, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new CourtResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateCourt(JObject p1)
        {
            try
            {
                var ctl = new CourtController();
                var court = p1.ToObject<CourtViewModel>();
                if (string.IsNullOrWhiteSpace(court.description) || court.county_id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Description and County are required." });
                }
                ctl.CreateCourt(court);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court created successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateCourt(JObject p1)
        {
            try
            {
                var ctl = new CourtController();
                var court = p1.ToObject<CourtViewModel>();
                if (court.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(court.description) || court.county_id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Description and County are required." });
                }
                var existingCourt = ctl.GetCourt(court.id);
                if (existingCourt == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }
                ctl.UpdateCourt(court);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court updated successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage ExtendCalendar(long courtId)
        {
            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                DateTime.TryParse(query.ContainsKey("startDate") ? query["startDate"] : DateTime.Now.ToString(), out DateTime startDate);
                Int32.TryParse(query.ContainsKey("weeks") ? query["weeks"] : "0", out int weeks);
                Int32.TryParse(query.ContainsKey("startTemplate") ? query["startTemplate"] : "0", out int startTemplate);

                var courtController = new CourtController();
                var court = courtController.GetCourt(courtId);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }

                var holidayController = new HolidayController();
                var holidays = holidayController.GetHolidays();

                var timeslotController = new TimeslotController();
                var courtTimeslotController = new CourtTimeslotController();
                var templateController = new CourtTemplateController();
                var templateOrderController = new CourtTemplateOrderController();

                var lastTimeslot = courtTimeslotController.GetCourtTimeslotsByCourtId(courtId)
                    .Where(ct => ct.Timeslot.template_id.HasValue)
                    .OrderByDescending(ct => ct.Timeslot.start)
                    .FirstOrDefault();

                if (lastTimeslot == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "No template timeslots found for this court." });
                }

                var startTemplateObj = templateController.GetCourtTemplate(lastTimeslot.Timeslot.template_id.Value);
                if (startTemplateObj == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Starting template not found." });
                }

                var startOrder = templateOrderController.GetCourtTemplateOrdersByCourtId(courtId)
                    .Where(t => t.template_id == startTemplateObj.id && t.auto)
                    .Select(t => t.order)
                    .FirstOrDefault();

                if (startOrder == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "No valid template order found." });
                }

                DateTime endDate = DateTime.Today.AddDays(court.calendar_weeks * 7);
                var period = GetDateRange(lastTimeslot.Timeslot.start, endDate)
                    .Where(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                    .ToList();

                int? templateOrder = startOrder;
                CourtTemplate currentTemplate = startTemplateObj;

                foreach (var date in period)
                {
                    var templateOrderObj = templateOrderController.GetCourtTemplateOrdersByCourtId(courtId)
                        .Where(t => t.order == templateOrder && t.auto)
                        .FirstOrDefault();

                    if (templateOrderObj == null)
                    {
                        templateOrder = 1;
                        templateOrderObj = templateOrderController.GetCourtTemplateOrdersByCourtId(courtId)
                            .Where(t => t.order == templateOrder && t.auto)
                            .FirstOrDefault();
                    }

                    if (templateOrderObj != null)
                    {
                        currentTemplate = templateController.GetCourtTemplate(templateOrderObj.template_id.Value);
                    }

                    if (currentTemplate == null)
                        continue;

                    var templateTimeslots = templateController.GetTemplateTimeslots(currentTemplate.id)
                        .Where(t => t.day == (int)date.DayOfWeek)
                        .ToList();

                    var existingTimeslots = courtTimeslotController.GetCourtTimeslotsByCourtId(courtId)
                        .Where(ct => ct.Timeslot.start.Date == date.Date)
                        .ToList();

                    foreach (var templateTimeslot in templateTimeslots)
                    {
                        var start = new DateTime(date.Year, date.Month, date.Day, templateTimeslot.start.Hour, templateTimeslot.start.Minute, templateTimeslot.start.Second);
                        var end = new DateTime(date.Year, date.Month, date.Day, templateTimeslot.end.Hour, templateTimeslot.end.Minute, templateTimeslot.end.Second);

                        if (holidays.Any(h => h.date.Date == date.Date))
                            continue;

                        var matchingTimeslots = existingTimeslots
                            .Where(ct => ct.Timeslot.start <= start && ct.Timeslot.end >= end)
                            .ToList();

                        if (!matchingTimeslots.Any() && !existingTimeslots.Any(ct => ct.Timeslot.created_at < DateTime.Today))
                        {
                            var newTimeslot = new Timeslot
                            {
                                start = start,
                                end = end,
                                allDay = templateTimeslot.allDay,
                                duration = templateTimeslot.duration,
                                quantity = templateTimeslot.quantity,
                                blocked = templateTimeslot.blocked,
                                public_block = templateTimeslot.public_block,
                                block_reason = string.IsNullOrEmpty(templateTimeslot.block_reason) ? null : templateTimeslot.block_reason,
                                category_id = templateTimeslot.category_id,
                                template_id = templateTimeslot.court_template_id,
                                description = templateTimeslot.description,
                                created_at = DateTime.Now,
                                updated_at = DateTime.Now
                            };

                            timeslotController.CreateTimeslot(newTimeslot);

                            courtTimeslotController.CreateCourtTimeslot(new CourtTimeslot
                            {
                                court_id = courtId,
                                timeslot_id = newTimeslot.id,
                                created_at = DateTime.Now,
                                updated_at = DateTime.Now
                            });
                        }
                        else if (matchingTimeslots.Any())
                        {
                            var duplicate = matchingTimeslots.First();
                            duplicate.Timeslot.template_id = templateTimeslot.court_template_id;
                            timeslotController.UpdateTimeslot(duplicate.Timeslot);
                        }
                    }

                    if (date.DayOfWeek == DayOfWeek.Friday)
                        templateOrder++;
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Calendar extended successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage ExtendCalendarManual(long courtId, JObject p2)
        {
            try
            {
                var courtController = new CourtController();
                var court = courtController.GetCourt(courtId);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }

                var holidayController = new HolidayController();
                var holidays = holidayController.GetHolidays();

                var templateOrderController = new CourtTemplateOrderController();
                var templates = templateOrderController.GetCourtTemplateOrdersByCourtId(courtId)
                    .Where(t => !t.auto)
                    .ToList();

                var timeslotController = new TimeslotController();
                var courtTimeslotController = new CourtTimeslotController();
                var templateController = new CourtTemplateController();

                foreach (var courtTemplateOrder in templates)
                {
                    var template = templateController.GetCourtTemplate(courtTemplateOrder.template_id.Value);
                    if (template == null)
                        continue;

                    var weekStart = courtTemplateOrder.date;
                    var timeslots = templateController.GetTemplateTimeslots(template.id);

                    for (int i = 0; i < 5; i++)
                    {
                        var day = weekStart.Value.AddDays(i);
                        var dayTimeslots = timeslots.Where(t => t.day == (int)day.DayOfWeek).ToList();

                        foreach (var timeslot in dayTimeslots)
                        {
                            if (holidays.Any(h => h.date.Date == day.Date))
                                continue;

                            var start = new DateTime(day.Year, day.Month, day.Day, timeslot.start.Hour, timeslot.start.Minute, timeslot.start.Second);
                            var end = new DateTime(day.Year, day.Month, day.Day, timeslot.end.Hour, timeslot.end.Minute, timeslot.end.Second);

                            var existingTimeslots = courtTimeslotController.GetCourtTimeslotsByCourtId(courtId)
                                .Where(ct => ct.Timeslot.start.Date == start.Date)
                                .ToList();

                            var match = existingTimeslots.FirstOrDefault(ct => ct.Timeslot.start == start && ct.Timeslot.template_id == template.id);

                            if (match == null)
                            {
                                var newTimeslot = new Timeslot
                                {
                                    start = start,
                                    end = end,
                                    description = timeslot.description,
                                    allDay = timeslot.allDay,
                                    duration = timeslot.duration,
                                    quantity = timeslot.quantity,
                                    blocked = timeslot.blocked,
                                    block_reason = timeslot.block_reason,
                                    public_block = timeslot.public_block,
                                    category_id = timeslot.category_id,
                                    template_id = template.id,
                                    created_at = DateTime.Now,
                                    updated_at = DateTime.Now
                                };

                                timeslotController.CreateTimeslot(newTimeslot);

                                courtTimeslotController.CreateCourtTimeslot(new CourtTimeslot
                                {
                                    court_id = courtId,
                                    timeslot_id = newTimeslot.id,
                                    created_at = DateTime.Now,
                                    updated_at = DateTime.Now
                                });
                            }
                        }
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Manual calendar extension completed successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UploadData(long courtId, JObject p2)
        {
            try
            {
                // Placeholder for upload data logic; requires implementation for handling file uploads (e.g., iCal or CSV)
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Data upload not fully implemented." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        private IEnumerable<DateTime> GetDateRange(DateTime start, DateTime end)
        {
            for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
                yield return date;
        }

        internal class CourtSearchResult
        {
            public List<CourtViewModel> data { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public int draw { get; set; }
            public string error { get; set; }
        }

        internal class CourtResult
        {
            public CourtViewModel data { get; set; }
            public string error { get; set; }
        }

        private string GetSortColumn(int columnIndex)
        {
            switch (columnIndex)
            {
                case 2:
                    return "description";
                case 3:
                    return "judge_name";
                case 4:
                    return "county_name";
                default:
                    return "description";
            }
        }
    }
}