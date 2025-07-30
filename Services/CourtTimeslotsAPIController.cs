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

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class CourtTimeslotsAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage Show(long courtId)
        {
            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                DateTime.TryParse(query.ContainsKey("start") ? query["start"] : DateTime.Now.ToString(), out DateTime start);
                DateTime.TryParse(query.ContainsKey("end") ? query["end"] : null, out DateTime end);
                if (end == DateTime.MinValue)
                {
                    end = start.AddDays(7);
                }
                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(courtId);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }
                var courtTimeslotCtl = new CourtTimeslotController();
                var timeslots = courtTimeslotCtl.GetCourtTimeslotsByCourtId(courtId)
                    .Where(ct => ct.Timeslot.start >= start.AddDays(-1) && ct.Timeslot.start <= end)
                    .Select(ct => ct.Timeslot)
                    .ToList();
                var holidayCtl = new HolidayController();
                var holidays = holidayCtl.GetHolidays();
                var result = timeslots.Concat(holidays.Select(h => new Timeslot
                {
                    id = h.id,
                    start = h.date,
                    end = h.date.AddDays(1),
                    allDay = true,
                    blocked = true,
                    description = h.name
                })).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    timeslots = result.Select(t => new
                    {
                        id = t.id,
                        start = t.start.ToString("yyyy-MM-ddTHH:mm"),
                        end = t.end.ToString("yyyy-MM-ddTHH:mm"),
                        allDay = t.allDay,
                        description = t.description,
                        quantity = t.quantity,
                        duration = t.duration,
                        blocked = t.blocked,
                        public_block = t.public_block,
                        block_reason = t.block_reason,
                        category_id = t.category_id
                    })
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage AvailableTimeslots(long courtId)
        {
            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                DateTime.TryParse(query.ContainsKey("end") ? query["end"] : DateTime.Now.AddDays(7).ToString(), out DateTime end);
                Int32.TryParse(query.ContainsKey("duration") ? query["duration"] : "0", out int duration);
                Int32.TryParse(query.ContainsKey("category") ? query["category"] : "0", out int category);
                Int32.TryParse(query.ContainsKey("motion") ? query["motion"] : "0", out int motion);

                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(courtId);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }
                var courtTimeslotCtl = new CourtTimeslotController();
                var timeslotCtl = new TimeslotController();
                var queryTimeslots = courtTimeslotCtl.GetCourtTimeslotsByCourtId(courtId)
                    .Where(ct => !ct.Timeslot.blocked && ct.Timeslot.start >= DateTime.Today && ct.Timeslot.start <= end)
                    .AsQueryable();
                if (duration > 0)
                {
                    queryTimeslots = queryTimeslots.Where(ct => ct.Timeslot.duration >= duration);
                }
                if (category > 0)
                {
                    queryTimeslots = queryTimeslots.Where(ct => ct.Timeslot.category_id == category);
                }
                if (motion > 0)
                {
                    queryTimeslots = queryTimeslots.Where(ct => !ct.Timeslot.Motions.Any() || ct.Timeslot.Motions.Any(m => m.motion_id == motion));
                }
                var timeslots = queryTimeslots.ToList()
                    .Where(ct => ct.Timeslot.quantity > timeslotCtl.GetEventCountForTimeslot(ct.Timeslot.id))
                    .Select(ct => ct.Timeslot)
                    .ToList();
                var holidayCtl = new HolidayController();
                var holidays = holidayCtl.GetHolidays();
                var result = timeslots.Concat(holidays.Select(h => new Timeslot
                {
                    id = h.id,
                    start = h.date,
                    end = h.date.AddDays(1),
                    allDay = true,
                    blocked = true,
                    description = h.name
                })).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    timeslots = result.Select(t => new
                    {
                        id = t.id,
                        start = t.start.ToString("yyyy-MM-ddTHH:mm"),
                        end = t.end.ToString("yyyy-MM-ddTHH:mm"),
                        allDay = t.allDay,
                        description = t.description,
                        quantity = t.quantity,
                        duration = t.duration,
                        blocked = t.blocked,
                        public_block = t.public_block,
                        block_reason = t.block_reason,
                        category_id = t.category_id
                    })
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage Month(long courtId)
        {
            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                DateTime.TryParse(query.ContainsKey("month") ? query["month"] : DateTime.Now.ToString(), out DateTime month);
                var start = new DateTime(month.Year, month.Month, 1);
                var end = start.AddMonths(1).AddDays(-1);

                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(courtId);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }
                var courtTimeslotCtl = new CourtTimeslotController();
                var timeslots = courtTimeslotCtl.GetCourtTimeslotsByCourtId(courtId)
                    .Where(ct => ct.Timeslot.start >= start && ct.Timeslot.start <= end)
                    .Select(ct => ct.Timeslot)
                    .ToList();
                var result = new List<object>();
                foreach (var timeslot in timeslots.OrderBy(t => t.start))
                {
                    var events = timeslot.TimeslotEvents?.Select(te => new
                    {
                        case_num = te.Event?.case_num,
                        plaintiff = te.Event?.plaintiff,
                        defendant = te.Event?.defendant,
                        motion = te.Event?.Motion?.description
                    }).ToList();
                    result.Add(new
                    {
                        id = timeslot.id,
                        start = timeslot.start.ToString("yyyy-MM-ddTHH:mm"),
                        end = timeslot.end.ToString("yyyy-MM-ddTHH:mm"),
                        allDay = timeslot.allDay,
                        description = timeslot.description,
                        quantity = timeslot.quantity,
                        duration = timeslot.duration,
                        blocked = timeslot.blocked,
                        public_block = timeslot.public_block,
                        block_reason = timeslot.block_reason,
                        category_id = timeslot.category_id,
                        events
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    timeslots = result
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage PrintPDF(long courtId, string startDate, string endDate)
        {
            try
            {
                if (!DateTime.TryParse(startDate, out DateTime start) || !DateTime.TryParse(endDate, out DateTime end))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Valid start and end dates are required." });
                }
                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(courtId);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }
                var courtTimeslotCtl = new CourtTimeslotController();
                var timeslots = courtTimeslotCtl.GetCourtTimeslotsByCourtId(courtId)
                    .Where(ct => ct.Timeslot.start >= start && ct.Timeslot.start <= end)
                    .Select(ct => ct.Timeslot)
                    .ToList();
                var holidayCtl = new HolidayController();
                var holidays = holidayCtl.GetHolidays();
                var result = timeslots.Concat(holidays.Select(h => new Timeslot
                {
                    id = h.id,
                    start = h.date,
                    end = h.date.AddDays(1),
                    allDay = true,
                    blocked = true,
                    description = h.name
                })).ToList();
                var responseData = result.Select(t => new
                {
                    id = t.id,
                    start = t.start.ToString("yyyy-MM-ddTHH:mm"),
                    end = t.end.ToString("yyyy-MM-ddTHH:mm"),
                    allDay = t.allDay,
                    description = t.description,
                    quantity = t.quantity,
                    duration = t.duration,
                    blocked = t.blocked,
                    public_block = t.public_block,
                    block_reason = t.block_reason,
                    category_id = t.category_id,
                    events = t.TimeslotEvents?.Select(te => new
                    {
                        case_num = te.Event?.case_num,
                        plaintiff = te.Event?.plaintiff,
                        defendant = te.Event?.defendant,
                        motion = te.Event?.Motion?.description
                    }).ToList()
                });
                // Placeholder for PDF generation; PHP uses a PDF library (not specified)
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    message = "PDF generation not fully implemented; data prepared.",
                    timeslots = responseData
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }
    }
}