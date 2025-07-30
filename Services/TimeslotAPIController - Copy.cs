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
                    end = start.AddDays(7);
                }
                var ctl = new TimeslotController();
                var timeslots = ctl.GetTimeslotsByCourtId(p1, start, end);
                var events = timeslots.Select(t => new
                {
                    id = t.id,
                    title = t.description,
                    start = t.start.ToString("yyyy-MM-ddTHH:mm"),
                    end = t.end.ToString("yyyy-MM-ddTHH:mm"),
                    allDay = t.allDay,
                    extendedProps = new
                    {
                        availableSlots = t.blocked || t.public_block ? 0 : t.quantity - t.eventCount,
                        blockReason = t.block_reason,
                        blocked = t.blocked,
                        publicBlock = t.public_block
                    }
                }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, events);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateTimeslot(JObject p1)
        {
            try
            {
                var timeslot = p1.ToObject<Timeslot>();
                if (timeslot.start == default || timeslot.end == default || timeslot.end <= timeslot.start)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Valid start and end times are required." });
                }
                timeslot.created_at = DateTime.Now;
                timeslot.updated_at = DateTime.Now;
                timeslot.start = DateTime.SpecifyKind(timeslot.start, DateTimeKind.Local);
                timeslot.end = DateTime.SpecifyKind(timeslot.end, DateTimeKind.Local);
                var ctl = new TimeslotController();
                ctl.CreateTimeslot(timeslot);
                long courtId = p1["courtId"].ToObject<long>();
                var restrictedMotions = p1["restrictedMotions"]?.ToObject<long[]>() ?? new long[0];
                var courtMotionCtl = new CourtMotionController();
                foreach (var motionId in restrictedMotions)
                {
                    var courtMotion = courtMotionCtl.GetCourtMotionByCourtAndMotion(courtId, motionId);
                    if (courtMotion == null || !courtMotion.allowed)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid or not allowed motion ID" });
                    }
                }
                var courtTimeslotCtl = new CourtTimeslotController();
                var courtTimeslot = new CourtTimeslot
                {
                    court_id = courtId,
                    timeslot_id = timeslot.id,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                courtTimeslotCtl.CreateCourtTimeslot(courtTimeslot);
                foreach (var motionId in restrictedMotions)
                {
                    ctl.CreateTimeslotMotion(new TimeslotMotion
                    {
                        timeslotable_type = "Timeslot",
                        timeslotable_id = timeslot.id,
                        motion_id = motionId,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
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
                    template_id = timeslot.template_id,
                    created_at = timeslot.created_at,
                    updated_at = timeslot.updated_at,
                    deleted_at = timeslot.deleted_at,
                    restrictedMotions = restrictedMotions
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateTimeslot(JObject p1)
        {
            try
            {
                var timeslot = p1.ToObject<Timeslot>();
                if (timeslot.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Timeslot ID is required for update." });
                }
                if (timeslot.start == default || timeslot.end == default || timeslot.end <= timeslot.start)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Valid start and end times are required." });
                }
                timeslot.updated_at = DateTime.Now;
                timeslot.start = DateTime.SpecifyKind(timeslot.start, DateTimeKind.Local);
                timeslot.end = DateTime.SpecifyKind(timeslot.end, DateTimeKind.Local);
                var ctl = new TimeslotController();
                var existingTimeslot = ctl.GetTimeslot(timeslot.id);
                if (existingTimeslot == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Timeslot not found." });
                }
                ctl.UpdateTimeslot(timeslot);
                long courtId = p1["courtId"].ToObject<long>();
                var restrictedMotions = p1["restrictedMotions"]?.ToObject<long[]>() ?? new long[0];
                var courtMotionCtl = new CourtMotionController();
                foreach (var motionId in restrictedMotions)
                {
                    var courtMotion = courtMotionCtl.GetCourtMotionByCourtAndMotion(courtId, motionId);
                    if (courtMotion == null || !courtMotion.allowed)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid or not allowed motion ID" });
                    }
                }
                var courtTimeslotCtl = new CourtTimeslotController();
                var courtTimeslot = courtTimeslotCtl.GetCourtTimeslotByCourtAndTimeslot(courtId, timeslot.id);
                if (courtTimeslot != null)
                {
                    courtTimeslot.updated_at = DateTime.Now;
                    courtTimeslotCtl.UpdateCourtTimeslot(courtTimeslot);
                }
                ctl.DeleteTimeslotMotionsForTimeslot(timeslot.id);
                foreach (var motionId in restrictedMotions)
                {
                    ctl.CreateTimeslotMotion(new TimeslotMotion
                    {
                        timeslotable_type = "Timeslot",
                        timeslotable_id = timeslot.id,
                        motion_id = motionId,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
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
                    template_id = timeslot.template_id,
                    created_at = timeslot.created_at,
                    updated_at = timeslot.updated_at,
                    deleted_at = timeslot.deleted_at,
                    restrictedMotions = restrictedMotions
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteTimeslot(long p1)
        {
            try
            {
                var ctl = new TimeslotController();
                ctl.DeleteTimeslot(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Timeslot deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetTimeslot(long p1)
        {
            try
            {
                var ctl = new TimeslotController();
                var timeslot = ctl.GetTimeslot(p1);
                if (timeslot == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { error = "Timeslot not found" });
                }
                var restrictedMotions = ctl.GetRestrictedMotionsForTimeslot(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    id = timeslot.id,
                    start = timeslot.start.ToString("yyyy-MM-ddTHH:mm"),
                    end = timeslot.end.ToString("yyyy-MM-ddTHH:mm"),
                    allDay = timeslot.allDay,
                    description = timeslot.description,
                    quantity = timeslot.quantity,
                    duration = timeslot.duration,
                    blocked = timeslot.blocked,
                    publicBlock = timeslot.public_block,
                    blockReason = timeslot.block_reason,
                    category = timeslot.category_id,
                    restrictedMotions = restrictedMotions
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { error = ex.Message });
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
                    end = start.AddDays(7);
                }
                start = DateTime.SpecifyKind(start, DateTimeKind.Local);
                end = DateTime.SpecifyKind(end, DateTimeKind.Local);
                var ctl = new TimeslotController();
                var timeslots = ctl.GetOverlappingTimeslots(p1, start, end);
                var response = timeslots.Select(t => new
                {
                    id = t.id,
                    start = t.start.ToString("yyyy-MM-ddTHH:mm"),
                    end = t.end.ToString("yyyy-MM-ddTHH:mm"),
                    allDay = t.allDay,
                    description = t.description,
                    quantity = t.quantity,
                    duration = t.duration,
                    blocked = t.blocked,
                    publicBlock = t.public_block,
                    blockReason = t.block_reason,
                    category = t.category_id,
                    restrictedMotions = ctl.GetRestrictedMotionsForTimeslot(t.id)
                }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateCourtTimeslot(JObject p1)
        {
            try
            {
                var courtTimeslot = p1.ToObject<CourtTimeslot>();
                if (courtTimeslot.court_id <= 0 || courtTimeslot.timeslot_id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court ID and Timeslot ID are required." });
                }
                courtTimeslot.created_at = DateTime.Now;
                courtTimeslot.updated_at = DateTime.Now;
                var ctl = new CourtTimeslotController();
                ctl.CreateCourtTimeslot(courtTimeslot);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    message = "Court Timeslot created successfully",
                    id = courtTimeslot.id,
                    court_id = courtTimeslot.court_id,
                    timeslot_id = courtTimeslot.timeslot_id,
                    created_at = courtTimeslot.created_at,
                    updated_at = courtTimeslot.updated_at
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateCourtTimeslot(JObject p1)
        {
            try
            {
                var courtTimeslot = p1.ToObject<CourtTimeslot>();
                if (courtTimeslot.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court Timeslot ID is required for update." });
                }
                if (courtTimeslot.court_id <= 0 || courtTimeslot.timeslot_id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court ID and Timeslot ID are required." });
                }
                var ctl = new CourtTimeslotController();
                var existingCourtTimeslot = ctl.GetCourtTimeslot(courtTimeslot.id);
                if (existingCourtTimeslot == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court Timeslot not found." });
                }
                courtTimeslot.updated_at = DateTime.Now;
                ctl.UpdateCourtTimeslot(courtTimeslot);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    message = "Court Timeslot updated successfully",
                    id = courtTimeslot.id,
                    court_id = courtTimeslot.court_id,
                    timeslot_id = courtTimeslot.timeslot_id,
                    created_at = courtTimeslot.created_at,
                    updated_at = courtTimeslot.updated_at
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteCourtTimeslot(long p1)
        {
            try
            {
                var ctl = new CourtTimeslotController();
                ctl.DeleteCourtTimeslot(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Court Timeslot deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCourtTimeslot(long p1)
        {
            try
            {
                var ctl = new CourtTimeslotController();
                var courtTimeslot = ctl.GetCourtTimeslot(p1);
                if (courtTimeslot == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { error = "Court Timeslot not found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    id = courtTimeslot.id,
                    court_id = courtTimeslot.court_id,
                    timeslot_id = courtTimeslot.timeslot_id,
                    created_at = courtTimeslot.created_at,
                    updated_at = courtTimeslot.updated_at
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateTimeslotMotion(JObject p1)
        {
            try
            {
                var timeslotMotion = p1.ToObject<TimeslotMotion>();
                if (string.IsNullOrWhiteSpace(timeslotMotion.timeslotable_type) || timeslotMotion.timeslotable_id <= 0 || timeslotMotion.motion_id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Timeslotable type, ID, and Motion ID are required." });
                }
                timeslotMotion.created_at = DateTime.Now;
                timeslotMotion.updated_at = DateTime.Now;
                var ctl = new TimeslotController();
                ctl.CreateTimeslotMotion(timeslotMotion);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    message = "Timeslot Motion created successfully",
                    id = timeslotMotion.id,
                    timeslotable_type = timeslotMotion.timeslotable_type,
                    timeslotable_id = timeslotMotion.timeslotable_id,
                    motion_id = timeslotMotion.motion_id,
                    created_at = timeslotMotion.created_at,
                    updated_at = timeslotMotion.updated_at
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateTimeslotMotion(JObject p1)
        {
            try
            {
                var timeslotMotion = p1.ToObject<TimeslotMotion>();
                if (timeslotMotion.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Timeslot Motion ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(timeslotMotion.timeslotable_type) || timeslotMotion.timeslotable_id <= 0 || timeslotMotion.motion_id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Timeslotable type, ID, and Motion ID are required." });
                }
                var ctl = new TimeslotMotionController();
                var existingTimeslotMotion = ctl.GetTimeslotMotion(timeslotMotion.id);
                if (existingTimeslotMotion == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Timeslot Motion not found." });
                }
                timeslotMotion.updated_at = DateTime.Now;
                ctl.UpdateTimeslotMotion(timeslotMotion);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    message = "Timeslot Motion updated successfully",
                    id = timeslotMotion.id,
                    timeslotable_type = timeslotMotion.timeslotable_type,
                    timeslotable_id = timeslotMotion.timeslotable_id,
                    motion_id = timeslotMotion.motion_id,
                    created_at = timeslotMotion.created_at,
                    updated_at = timeslotMotion.updated_at
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteTimeslotMotion(long p1)
        {
            try
            {
                var ctl = new TimeslotController();
                ctl.DeleteTimeslotMotion(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Timeslot Motion deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetTimeslotMotions(long p1)
        {
            try
            {
                var ctl = new TimeslotController();
                var timeslotMotions = ctl.GetTimeslotMotions(p1);
                return Request.CreateResponse(HttpStatusCode.OK, timeslotMotions.Select(m => new
                {
                    id = m.id,
                    timeslotable_type = m.timeslotable_type,
                    timeslotable_id = m.timeslotable_id,
                    motion_id = m.motion_id,
                    created_at = m.created_at,
                    updated_at = m.updated_at
                }).ToList());
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }
    }
}