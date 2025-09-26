using DotNetNuke.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
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

            long userId = query.ContainsKey("userId") && long.TryParse(query["userId"], out long uid) ? uid : 0;
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
                filteredCount = ctl.GetCourtsCount(userId, searchTerm);
                if (p1 == 0) { recordCount = filteredCount; }
                courts = ctl.GetCourtsPaged(userId, searchTerm, recordOffset, pageSize, sortColumn, sortDirection).ToList();
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
                long userId = -1;
                UserInfo user = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
                if (user != null && user.UserID > 0)
                {
                    if (user.IsAdmin)
                        userId = 0; // Admin can see all courts
                    else
                        userId = user.UserID;
                }
                var ctl = new CourtController();
                courts = ctl.GetCourtDropDownItems(userId, searchTerm);
                return Request.CreateResponse(new ListItemOptionResult { data = courts, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new ListItemOptionResult { data = courts, error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourt(long p1)
        {
            try
            {
                var ctl = new CourtController();
                var court = ctl.GetCourt(p1);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CourtResult { data = null, error = "Court not found" });
                }
                return Request.CreateResponse(new CourtResult { data = new CourtViewModel(court), error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new CourtResult { data = null, error = ex.Message });
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
        public HttpResponseMessage GetTemplates(long p1)
        {
            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(p1);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }

                var templateCtl = new CourtTemplateController();
                var templates = templateCtl.GetCourtTemplatesPaged("", 0, 1000, "name", "asc")
                    .Where(t => t.court_id == p1)
                    .Select(t => new { Key = t.id, Value = t.name })
                    .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, data = templates });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateCourt(JObject p1)
        {
            try
            {
                var court = p1.ToObject<Court>();
                if (string.IsNullOrEmpty(court.description) || court.county_id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Description and county are required." });
                }
                var ctl = new CourtController();
                court.created_at = DateTime.Now;
                court.updated_at = DateTime.Now;
                ctl.CreateCourt(court);

                // Handle motions
                var motionCtl = new CourtMotionController();
                var motions = p1["available_motions"]?.ToObject<List<long>>() ?? new List<long>();
                foreach (var motion in motions)
                {
                    motionCtl.CreateCourtMotion(new CourtMotion { court_id = court.id, motion_id = motion, allowed = true });
                }
                var restrictedMotions = p1["restricted_motions"]?.ToObject<List<long>>() ?? new List<long>();
                foreach (var motion in restrictedMotions)
                {
                    motionCtl.CreateCourtMotion(new CourtMotion { court_id = court.id, motion_id = motion, allowed = false });
                }

                // Handle event types
                var eventTypeCtl = new CourtEventTypeController();
                var eventTypes = p1["available_hearing_types"]?.ToObject<List<long>>() ?? new List<long>();
                foreach (var eventType in eventTypes)
                {
                    eventTypeCtl.CreateCourtEventType(new CourtEventType { court_id = court.id, event_type_id = eventType });
                }

                // Handle templates
                var templateOrderCtl = new CourtTemplateOrderController();
                var templates = p1["templates"]?.ToObject<List<JObject>>() ?? new List<JObject>();
                foreach (var t in templates)
                {
                    var templateId = t["templateId"]?.ToObject<long>();
                    var week = t["week"]?.ToObject<int?>();
                    var dateStr = t["date"]?.ToString();
                    var auto = t["auto"]?.ToObject<bool>() ?? false;
                    DateTime? date = null;

                    if (templateId == null || templateId <= 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid template ID." });
                    }

                    if (auto && week.HasValue)
                    {
                        date = DateTime.UtcNow.Date.AddDays((week.Value - 1) * 7);
                    }
                    else if (!auto && !string.IsNullOrEmpty(dateStr))
                    {
                        if (!DateTime.TryParse(dateStr, out var parsedDate))
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid date format in templates." });
                        }
                        date = parsedDate;
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Week or date required for template." });
                    }

                    var templateCtl = new CourtTemplateController();
                    var template = templateCtl.GetCourtTemplate(templateId.Value);
                    if (template == null || template.court_id != court.id)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = $"Template ID {templateId} not found or does not belong to court {court.id}." });
                    }

                    var order = new CourtTemplateOrder
                    {
                        court_id = court.id,
                        template_id = templateId.Value,
                        date = date,
                        auto = auto,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    templateOrderCtl.CreateCourtTemplateOrder(order);
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court created successfully", data = new CourtViewModel(court) });
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
                var court = p1.ToObject<CourtViewModel>();
                if (court.id <= 0 || string.IsNullOrEmpty(court.description) || court.county_id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court ID, description, and county are required." });
                }
                var ctl = new CourtController();
                var existingCourt = ctl.GetCourt(court.id);
                if (existingCourt == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }
                ctl.UpdateCourt(court);

                // Handle motions
                var motionCtl = new CourtMotionController();
                motionCtl.DeleteCourtMotionsByCourtId(court.id);
                var motions = p1["available_motions"]?.ToObject<List<long>>() ?? new List<long>();
                foreach (var motion in motions)
                {
                    motionCtl.CreateCourtMotion(new CourtMotion { court_id = court.id, motion_id = motion, allowed = true });
                }
                var restrictedMotions = p1["restricted_motions"]?.ToObject<List<long>>() ?? new List<long>();
                foreach (var motion in restrictedMotions)
                {
                    motionCtl.CreateCourtMotion(new CourtMotion { court_id = court.id, motion_id = motion, allowed = false });
                }

                // Handle event types
                var eventTypeCtl = new CourtEventTypeController();
                eventTypeCtl.DeleteCourtEventTypesByCourtId(court.id);
                var eventTypes = p1["available_hearing_types"]?.ToObject<List<long>>() ?? new List<long>();
                foreach (var eventType in eventTypes)
                {
                    eventTypeCtl.CreateCourtEventType(new CourtEventType { court_id = court.id, event_type_id = eventType });
                }

                // Handle templates
                var templateOrderCtl = new CourtTemplateOrderController();
                templateOrderCtl.DeleteCourtTemplateOrdersByCourtId(court.id, court.auto_extension);
                var templates = p1["templates"]?.ToObject<List<JObject>>() ?? new List<JObject>();
                foreach (var t in templates)
                {
                    var newTemplate = t.ToObject<CourtTemplateOrderViewModel>();

                    if (!newTemplate.auto && newTemplate.date == null)
                    {
                        if (newTemplate.date == null)
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid date format in templates." });
                        }
                    }
                    else if(newTemplate.auto && newTemplate.order == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Week required for auto template." });
                    }

                    var templateCtl = new CourtTemplateController();
                    var template = templateCtl.GetCourtTemplate(newTemplate.template_id.Value);
                    if (template != null && template.court_id != court.id)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = $"Template ID {newTemplate.template_id}  does not belong to court {court.id}." });
                    }

                    var courtTemplateOrder = new CourtTemplateOrder
                    {
                        court_id = court.id,
                        template_id = newTemplate.template_id,
                        date = newTemplate.date,
                        order = newTemplate.order,
                        auto = newTemplate.auto,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    templateOrderCtl.CreateCourtTemplateOrder(courtTemplateOrder);
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court updated successfully", data = court });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage TruncateCalendar(JObject p1)
        {
            try
            {
                long courtId = p1["courtId"].ToObject<long>();
                DateTime date = DateTime.Parse(p1["date"].ToString()).Date;
                string filter = p1["filter"].ToString().ToLower();

                var timeslotCtl = new TimeslotController();
                var eventCtl = new EventController();
                var courtCtl = new CourtController();
                var courtTimeslotCtl = new CourtTimeslotController();
                var timeslots = courtTimeslotCtl.GetCourtTimeslotsByCourtId(courtId)
                    .Where(ct => ct.Timeslot.start >= date)
                    .ToList();

                foreach (var ct in timeslots)
                {
                    var ts = ct.Timeslot;
                    if (filter == "all" || (filter == "reserved" && ts.quantity > 0) || (filter == "unreserved" && ts.quantity == 0))
                    {
                        var events = eventCtl.GetEventsByTimeslot(ts.id);
                        foreach (var ev in events)
                        {
                            eventCtl.DeleteEvent(ev.id);
                        }
                        timeslotCtl.DeleteTimeslot(ts.id);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Calendar truncated successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public HttpResponseMessage TruncateTimeslots(TruncateRequest request)
        {
          var timeslotSQL="select [timeslots].*, [court_timeslots].[court_id] as [laravel_through_key] from [timeslots] inner join [court_timeslots] on [court_timeslots].[timeslot_id] = [timeslots].[id] where [court_timeslots].[court_id] = @0 and [start] >= @1";
            try
            {
                var courtCtl = new CourtController();
                courtCtl.TruncateTimeslots(request.CourtId, request.StartDate, request.Filter);
                using (IDataContext ctx = DataContext.Instance("jacs"))
                {
                    string timeslotFilter = string.Empty;
                    bool handleEvents = false;

                    switch (request.Filter)
                    {
                        case "all":
                            handleEvents = true;
                            break;
                        case "hearings":
                            timeslotFilter = " AND NOT EXISTS (SELECT 1 FROM timeslot_events te WHERE te.timeslot_id = ts.id)";
                            break;
                        case "templates":
                            timeslotFilter = " AND ts.template_id IS NOT NULL AND ts.blocked = 0";
                            handleEvents = true; // Added to prevent potential FK issues, even if not in PHP
                            break;
                        case "both":
                            timeslotFilter = " AND ts.template_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM timeslot_events te WHERE te.timeslot_id = ts.id)";
                            break;
                        default:
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid filter");
                    }

                    if (handleEvents)
                    {
                        // Update events
                        ctx.Execute(CommandType.Text,
                            @"UPDATE e SET status_id = 1 
                      FROM events e 
                      INNER JOIN timeslot_events te ON e.id = te.event_id 
                      INNER JOIN timeslots ts ON te.timeslot_id = ts.id 
                      WHERE ts.court_id = @0 AND ts.start >= @1" + timeslotFilter,
                            request.CourtId, request.StartDate);

                        // Delete timeslot_events
                        ctx.Execute(CommandType.Text,
                            @"DELETE te 
                      FROM timeslot_events te 
                      INNER JOIN timeslots ts ON te.timeslot_id = ts.id 
                      WHERE ts.court_id = @0 AND ts.start >= @1" + timeslotFilter,
                            request.CourtId, request.StartDate);
                    }

                    // Delete motions
                    ctx.Execute(CommandType.Text,
                        @"DELETE m 
                  FROM timeslot_motions m 
                  INNER JOIN timeslots ts ON m.timeslot_id = ts.id 
                  WHERE ts.court_id = @0 AND ts.start >= @1" + timeslotFilter,
                        request.CourtId, request.StartDate);

                    // Delete timeslots
                    ctx.Execute(CommandType.Text,
                        @"DELETE ts 
                  FROM timeslots ts 
                  WHERE ts.court_id = @0 AND ts.start >= @1" + timeslotFilter,
                        request.CourtId, request.StartDate);
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = "Truncate Successful" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        public class TruncateRequest
        {
            public int CourtId { get; set; }
            public DateTime StartDate { get; set; }
            public string Filter { get; set; }
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