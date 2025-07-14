using DotNetNuke.Data;
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

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class TimeslotAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCourtTimeslots(long p1)
        {
            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                DateTime.TryParse(query.ContainsKey("start") ? query["start"] : DateTime.Now.ToString(), out DateTime start);
                DateTime.TryParse(query.ContainsKey("end") ? query["end"] : null, out DateTime end);
                if (end == DateTime.MinValue)
                {
                    end = start.AddDays(7); // Default to next 7 days if parsing fails
                }
                var ctl = new TimeslotController();
                var timeslots = ctl.GetTimeslotsByCourtId(p1, start, end);
                var events = timeslots.Select(t => new
                {
                    id = t.id,
                    title = t.description,
                    start = t.start.ToString("o"),
                    end = t.end.ToString("o"),
                    allDay = t.allDay,
                    extendedProps = new
                    {
                        availableSlots = t.blocked || t.public_block ? 0 : t.quantity - t.eventCount,
                        blockReason = t.block_reason,
                        blocked = t.blocked,
                        publicBlock = t.public_block,
                    }
                }).ToList();
                return Request.CreateResponse(events);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateTimeslot(JObject data)
        {
            try
            {
                var timeslot = data.ToObject<Timeslot>();
                timeslot.created_at = DateTime.Now;
                timeslot.updated_at = DateTime.Now;
                var ctl = new TimeslotController();
                ctl.CreateTimeslot(timeslot);
                long courtId = data["courtId"].ToObject<long>();
                using (IDataContext ctx = DataContext.Instance("jacs"))
                {
                    ctx.Execute(System.Data.CommandType.Text, "insert into court_timeslots (court_id, timeslot_id) values (@0, @1)", courtId, timeslot.id);
                }
                return Request.CreateResponse(timeslot);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateTimeslot(JObject timeslotData)
        {
            try
            {
                var timeslot = timeslotData.ToObject<Timeslot>();
                timeslot.updated_at = DateTime.Now;
                var ctl = new TimeslotController();
                ctl.UpdateTimeslot(timeslot);
                return Request.CreateResponse(timeslot);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteTimeslot(long p1)
        {
            try
            {
                var ctl = new TimeslotController();
                ctl.DeleteTimeslot((int)p1);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetTimeslot(long p1)
        {
            try
            {
                var ctl = new TimeslotController();
                var timeslot = ctl.GetTimeslot(p1);
                return Request.CreateResponse(timeslot);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetOverlappingTimeslots(long p1)
        {
            try
            {
                var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                DateTime.TryParse(query.ContainsKey("start") ? query["start"] : DateTime.Now.ToString(), out DateTime start);
                DateTime.TryParse(query.ContainsKey("end") ? query["end"] : null, out DateTime end);
                if (end == DateTime.MinValue)
                {
                    end = start.AddDays(7); // Default to next 7 days if parsing fails
                }

                var ctl = new TimeslotController();
                var timeslots = ctl.GetOverlappingTimeslots(p1, start, end);
                return Request.CreateResponse(timeslots);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }
    }
}