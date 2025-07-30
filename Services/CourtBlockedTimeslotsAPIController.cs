using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class CourtBlockedTimeslotsAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage Index(long courtId)
        {
            try
            {
                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(courtId);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }
                var courtTimeslotCtl = new CourtTimeslotController();
                var timeslots = courtTimeslotCtl.GetCourtTimeslotsByCourtId(courtId)
                    .Where(ct => ct.Timeslot.blocked)
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
                        blocked = t.blocked
                    })
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