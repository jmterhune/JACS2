// Filename: TimeslotAPIController.cs
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
                if (timeslot.quantity < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Quantity must be non-negative." });
                }
                if (timeslot.duration <= 0 && !timeslot.allDay)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Duration must be positive for non-all-day timeslots." });
                }
                timeslot.created_at = DateTime.Now;
                timeslot.updated_at = DateTime.Now;
                timeslot.start = DateTime.SpecifyKind(timeslot.start, DateTimeKind.Local);
                timeslot.end = DateTime.SpecifyKind(timeslot.end, DateTimeKind.Local);
                var ctl = new TimeslotController();
                var courtId = p1["courtId"].ToObject<long>();
                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(courtId);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }
                var restrictedMotions = p1["restrictedMotions"]?.ToObject<long[]>() ?? new long[0];
                var courtMotionCtl = new CourtMotionController();
                foreach (var motionId in restrictedMotions)
                {
                    var courtMotion = courtMotionCtl.GetCourtMotionByCourtAndMotion(courtId, motionId);
                    if (courtMotion == null || !courtMotion.allowed)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = $"Invalid or not allowed motion ID: {motionId}" });
                    }
                }
                bool isConcurrent = p1["cattlecall"]?.ToObject<string>() == "1";
                if (timeslot.quantity == 0 && !timeslot.allDay)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Quantity must be positive for non-all-day timeslots." });
                }
                if (timeslot.allDay)
                {
                    timeslot.duration = 480; // 8 hours, as per PHP logic
                    timeslot.quantity = 0;
                }
                if (!isConcurrent && timeslot.quantity > 1)
                {
                    var buffer = timeslot.start;
                    var createdTimeslots = new List<Timeslot>();
                    for (int i = 0; i < timeslot.quantity; i++)
                    {
                        var newTimeslot = new Timeslot
                        {
                            start = buffer,
                            end = buffer.AddMinutes(timeslot.duration),
                            duration = timeslot.duration,
                            quantity = 1,
                            description = timeslot.description,
                            category_id = timeslot.category_id,
                            blocked = timeslot.blocked,
                            public_block = timeslot.public_block,
                            block_reason = timeslot.block_reason,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        };
                        ctl.CreateTimeslot(newTimeslot);
                        var courtTimeslot = new CourtTimeslot
                        {
                            court_id = courtId,
                            timeslot_id = newTimeslot.id,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        };
                        var courtTimeslotCtl = new CourtTimeslotController();
                        courtTimeslotCtl.CreateCourtTimeslot(courtTimeslot);
                        foreach (var motionId in restrictedMotions)
                        {
                            ctl.CreateTimeslotMotion(new TimeslotMotion
                            {
                                timeslotable_type = "Timeslot",
                                timeslotable_id = newTimeslot.id,
                                motion_id = motionId,
                                created_at = DateTime.Now,
                                updated_at = DateTime.Now
                            });
                        }
                        createdTimeslots.Add(newTimeslot);
                        buffer = buffer.AddMinutes(timeslot.duration);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        status = 200,
                        message = "Timeslots created successfully",
                        timeslots = createdTimeslots.Select(t => new
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
                            template_id = t.template_id,
                            created_at = t.created_at,
                            updated_at = t.updated_at,
                            deleted_at = t.deleted_at,
                            restrictedMotions
                        })
                    });
                }
                else
                {
                    ctl.CreateTimeslot(timeslot);
                    var courtTimeslot = new CourtTimeslot
                    {
                        court_id = courtId,
                        timeslot_id = timeslot.id,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    var courtTimeslotCtl = new CourtTimeslotController();
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
                        restrictedMotions
                    });
                }
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
                if (timeslot.quantity < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Quantity must be non-negative." });
                }
                if (timeslot.duration <= 0 && !timeslot.allDay)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Duration must be positive for non-all-day timeslots." });
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
                long courtId = p1["courtId"].ToObject<long>();
                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(courtId);
                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Court not found." });
                }
                var restrictedMotions = p1["restrictedMotions"]?.ToObject<long[]>() ?? new long[0];
                var courtMotionCtl = new CourtMotionController();
                foreach (var motionId in restrictedMotions)
                {
                    var courtMotion = courtMotionCtl.GetCourtMotionByCourtAndMotion(courtId, motionId);
                    if (courtMotion == null || !courtMotion.allowed)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = $"Invalid or not allowed motion ID: {motionId}" });
                    }
                }
                ctl.UpdateTimeslot(timeslot);
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
                    restrictedMotions
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
                var timeslot = ctl.GetTimeslot(p1);
                if (timeslot == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Timeslot not found." });
                }
                var courtTimeslotCtl = new CourtTimeslotController();
                var courtTimeslot = courtTimeslotCtl.GetCourtTimeslotByTimeslotId(p1);
                if (courtTimeslot != null)
                {
                    courtTimeslotCtl.DeleteCourtTimeslot(courtTimeslot.id);
                }
                var timeslotMotions = ctl.GetTimeslotMotions(p1);
                foreach (var motion in timeslotMotions)
                {
                    ctl.DeleteTimeslotMotion(motion.id);
                }
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
                    restrictedMotions
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
        public HttpResponseMessage CreateTimeslotMotion(JObject p1)
        {
            try
            {
                var timeslotMotion = p1.ToObject<TimeslotMotion>();
                if (string.IsNullOrWhiteSpace(timeslotMotion.timeslotable_type) || timeslotMotion.timeslotable_id <= 0 || timeslotMotion.motion_id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Timeslotable type, ID, and Motion ID are required." });
                }
                var ctl = new TimeslotController();
                timeslotMotion.created_at = DateTime.Now;
                timeslotMotion.updated_at = DateTime.Now;
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
                var timeslotMotion = ctl.GetTimeslotMotion(p1);
                if (timeslotMotion == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Timeslot Motion not found." });
                }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage DestroyMulti(JArray p1)
        {
            try
            {
                var timeslotIds = p1.ToObject<List<long>>();
                if (timeslotIds == null || !timeslotIds.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "At least one timeslot ID is required." });
                }
                var ctl = new TimeslotController();
                var courtTimeslotCtl = new CourtTimeslotController();
                var deletedIds = new List<long>();
                foreach (var id in timeslotIds)
                {
                    var timeslot = ctl.GetTimeslot(id);
                    if (timeslot == null)
                    {
                        continue; // Skip non-existent timeslots
                    }
                    var timeslotMotions = ctl.GetTimeslotMotions(id);
                    foreach (var motion in timeslotMotions)
                    {
                        ctl.DeleteTimeslotMotion(motion.id);
                    }
                    var courtTimeslot = courtTimeslotCtl.GetCourtTimeslotByTimeslotId(id);
                    if (courtTimeslot != null)
                    {
                        courtTimeslotCtl.DeleteCourtTimeslot(courtTimeslot.id);
                    }
                    ctl.DeleteTimeslot(id);
                    deletedIds.Add(id);
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    message = "Timeslots deleted successfully",
                    deletedIds
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
        public HttpResponseMessage Copy(JArray p1)
        {
            try
            {
                var timeslotIds = p1.ToObject<List<long>>();
                if (timeslotIds == null || !timeslotIds.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "At least one timeslot ID is required." });
                }
                var ctl = new TimeslotController();
                var courtTimeslotCtl = new CourtTimeslotController();
                var newTimeslots = new List<Timeslot>();
                foreach (var id in timeslotIds)
                {
                    var original = ctl.GetTimeslot(id);
                    if (original == null)
                    {
                        continue; // Skip non-existent timeslots
                    }
                    var newTimeslot = new Timeslot
                    {
                        start = original.start,
                        end = original.end,
                        duration = original.duration,
                        quantity = original.quantity,
                        allDay = original.allDay,
                        description = original.description + " (Copy)",
                        category_id = original.category_id,
                        blocked = original.blocked,
                        public_block = original.public_block,
                        block_reason = original.block_reason,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    ctl.CreateTimeslot(newTimeslot);
                    var courtTimeslot = courtTimeslotCtl.GetCourtTimeslotByTimeslotId(id);
                    if (courtTimeslot != null)
                    {
                        var newCourtTimeslot = new CourtTimeslot
                        {
                            court_id = courtTimeslot.court_id,
                            timeslot_id = newTimeslot.id,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        };
                        courtTimeslotCtl.CreateCourtTimeslot(newCourtTimeslot);
                    }
                    var timeslotMotions = ctl.GetTimeslotMotions(id);
                    foreach (var motion in timeslotMotions)
                    {
                        ctl.CreateTimeslotMotion(new TimeslotMotion
                        {
                            timeslotable_type = motion.timeslotable_type,
                            timeslotable_id = newTimeslot.id,
                            motion_id = motion.motion_id,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        });
                    }
                    newTimeslots.Add(newTimeslot);
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    message = "Timeslots copied successfully",
                    timeslots = newTimeslots.Select(t => new
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
                        template_id = t.template_id,
                        created_at = t.created_at,
                        updated_at = t.updated_at,
                        deleted_at = t.deleted_at
                    })
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
        public HttpResponseMessage TempCopy(JArray p1)
        {
            try
            {
                var timeslotIds = p1.ToObject<List<long>>();
                if (timeslotIds == null || !timeslotIds.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "At least one timeslot ID is required." });
                }
                var ctl = new TimeslotController();
                var courtTimeslotCtl = new CourtTimeslotController();
                var newTimeslots = new List<Timeslot>();
                foreach (var id in timeslotIds)
                {
                    var original = ctl.GetTimeslot(id);
                    if (original == null)
                    {
                        continue; // Skip non-existent timeslots
                    }
                    var newTimeslot = new Timeslot
                    {
                        start = original.start,
                        end = original.end,
                        duration = original.duration,
                        quantity = original.quantity,
                        allDay = original.allDay,
                        description = original.description + " (Temp Copy)",
                        category_id = original.category_id,
                        blocked = original.blocked,
                        public_block = original.public_block,
                        block_reason = original.block_reason,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    ctl.CreateTimeslot(newTimeslot);
                    var courtTimeslot = courtTimeslotCtl.GetCourtTimeslotByTimeslotId(id);
                    if (courtTimeslot != null)
                    {
                        var newCourtTimeslot = new CourtTimeslot
                        {
                            court_id = courtTimeslot.court_id,
                            timeslot_id = newTimeslot.id,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        };
                        courtTimeslotCtl.CreateCourtTimeslot(newCourtTimeslot);
                    }
                    var timeslotMotions = ctl.GetTimeslotMotions(id);
                    foreach (var motion in timeslotMotions)
                    {
                        ctl.CreateTimeslotMotion(new TimeslotMotion
                        {
                            timeslotable_type = motion.timeslotable_type,
                            timeslotable_id = newTimeslot.id,
                            motion_id = motion.motion_id,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        });
                    }
                    newTimeslots.Add(newTimeslot);
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    message = "Timeslots temporarily copied successfully",
                    timeslots = newTimeslots.Select(t => new
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
                        template_id = t.template_id,
                        created_at = t.created_at,
                        updated_at = t.updated_at,
                        deleted_at = t.deleted_at
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