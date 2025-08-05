// Updated TemplateAPIController.cs (added CloneTemplate method)
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
    public class TemplateAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCourtTemplate(long p1)
        {
            try
            {
                var ctl = new CourtTemplateController();
                var template = ctl.GetCourtTemplate(p1);
                if (template == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Template not found." });
                }
                var timeslots = ctl.GetTemplateTimeslots(p1).Select(t => new
                {
                    id = t.id,
                    start = t.start.ToString("yyyy-MM-ddTHH:mm"),
                    end = t.end.ToString("yyyy-MM-ddTHH:mm"),
                    duration = t.duration,
                    quantity = t.quantity,
                    allDay = t.allDay,
                    day = t.day,
                    court_template_id = t.court_template_id,
                    description = t.description,
                    category_id = t.category_id,
                    blocked = t.blocked,
                    public_block = t.public_block,
                    block_reason = t.block_reason,
                    color = t.color,
                    title = t.title,
                    total_length = t.total_length,
                });
                return Request.CreateResponse(HttpStatusCode.OK, timeslots);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateTemplateTimeslot(JObject p1)
        {
            try
            {
                var timeslot = p1.ToObject<TemplateTimeslot>();
                if (timeslot.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Template timeslot ID is required for update." });
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
                var ctl = new CourtTemplateController();
                var existingTimeslot = ctl.GetTemplateTimeslot(timeslot.id);
                if (existingTimeslot == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Template timeslot not found." });
                }
                var restrictedMotions = p1["timeslot_motions"]?.ToObject<long[]>() ?? new long[0];
                var courtMotionCtl = new CourtMotionController();
                var template = ctl.GetCourtTemplate(timeslot.court_template_id.Value);
                if (template == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Template not found." });
                }
                foreach (var motionId in restrictedMotions)
                {
                    var courtMotion = courtMotionCtl.GetCourtMotionByCourtAndMotion(template.court_id, motionId);
                    if (courtMotion == null || !courtMotion.allowed)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = $"Invalid or not allowed motion ID: {motionId}" });
                    }
                }
                timeslot.day = (int)timeslot.start.DayOfWeek;
                timeslot.updated_at = DateTime.Now;
                timeslot.start = DateTime.SpecifyKind(timeslot.start, DateTimeKind.Local);
                timeslot.end = DateTime.SpecifyKind(timeslot.end, DateTimeKind.Local);
                ctl.UpdateTemplateTimeslot(timeslot);
                var timeslotMotionCtl = new TimeslotMotionController();
                timeslotMotionCtl.DeleteTimeslotMotions(timeslot.id, "TemplateTimeslot");
                foreach (var motionId in restrictedMotions)
                {
                    timeslotMotionCtl.CreateTimeslotMotion(new TimeslotMotion
                    {
                        timeslotable_type = "TemplateTimeslot",
                        timeslotable_id = timeslot.id,
                        motion_id = motionId,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Template timeslot updated successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateTemplateTimeslotTime(JObject p1)
        {
            try
            {
                var timeslot = p1.ToObject<TemplateTimeslot>();
                if (timeslot.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Template timeslot ID is required for update." });
                }
                if (timeslot.start == default || timeslot.end == default || timeslot.end <= timeslot.start)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Valid start and end times are required." });
                }

                var ctl = new CourtTemplateController();
                var existingTimeslot = ctl.GetTemplateTimeslot(timeslot.id);
                if (existingTimeslot == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Template timeslot not found." });
                }
                existingTimeslot.day = (int)timeslot.start.DayOfWeek;
                existingTimeslot.updated_at = DateTime.Now;
                existingTimeslot.start = DateTime.SpecifyKind(timeslot.start, DateTimeKind.Local);
                existingTimeslot.end = DateTime.SpecifyKind(timeslot.end, DateTimeKind.Local);
                var minutesDifference = (int)(existingTimeslot.end - existingTimeslot.start).TotalMinutes;
                var quantity = minutesDifference / existingTimeslot.duration;
                existingTimeslot.quantity = quantity > 0 ? quantity : 1; // Ensure at least one slot

                ctl.UpdateTemplateTimeslot(existingTimeslot);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Template timeslot updated successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage EditTemplateTimeslot(long p1)
        {
            try
            {
                var ctl = new CourtTemplateController();
                var timeslot = ctl.GetTemplateTimeslot(p1);
                if (timeslot == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Template timeslot not found." });
                }
                var timeslotMotionCtl = new TimeslotMotionController();
                var motions = timeslotMotionCtl.GetTemplateTimeslotMotions(p1, "App\\Models\\TemplateTimeslot");
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    id = timeslot.id,
                    start = timeslot.start.ToString("yyyy-MM-ddTHH:mm"),
                    end = timeslot.end.ToString("yyyy-MM-ddTHH:mm"),
                    duration = timeslot.duration,
                    quantity = timeslot.quantity,
                    allDay = timeslot.allDay,
                    day = timeslot.day,
                    court_template_id = timeslot.court_template_id,
                    description = timeslot.description,
                    category_id = timeslot.category_id,
                    blocked = timeslot.blocked,
                    public_block = timeslot.public_block,
                    block_reason = timeslot.block_reason,
                    motions = motions.Select(m => new { m.id, m.motion_id })
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteTemplateTimeslot(long p1)
        {
            try
            {
                var ctl = new CourtTemplateController();
                var timeslot = ctl.GetTemplateTimeslot(p1);
                if (timeslot == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Template timeslot not found." });
                }
                TimeslotMotionController timeslotMotionCtl = new TimeslotMotionController();
                IEnumerable<TimeslotMotion> timeslotMotion = timeslotMotionCtl.GetTemplateTimeslotMotions(p1, "App\\Models\\TemplateTimeslot");
                foreach (var motion in timeslotMotion)
                {
                    timeslotMotionCtl.DeleteTimeslotMotion(motion.id);
                }
                ctl.DeleteTemplateTimeslot(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Template timeslot deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateTemplateTimeslot(JObject p1)
        {
            try
            {
                var timeslot = p1.ToObject<TemplateTimeslot>();
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
                if (timeslot.court_template_id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court template ID is required." });
                }
                var ctl = new CourtTemplateController();
                var template = ctl.GetCourtTemplate(timeslot.court_template_id.Value);
                if (template == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Template not found." });
                }
                var restrictedMotions = p1["timeslot_motions"]?.ToObject<long[]>() ?? new long[0];
                var courtMotionCtl = new CourtMotionController();
                foreach (var motionId in restrictedMotions)
                {
                    var courtMotion = courtMotionCtl.GetCourtMotionByCourtAndMotion(template.court_id, motionId);
                    if (courtMotion == null || !courtMotion.allowed)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = $"Invalid or not allowed motion ID: {motionId}" });
                    }
                }
                bool isConcurrent = p1["cattlecall"]?.ToObject<string>() == "1";
                if (timeslot.quantity == 0)
                {
                    timeslot.allDay = true;
                    timeslot.duration = 480; // 8 hours, as per PHP logic
                }
                timeslot.day = (int)timeslot.start.DayOfWeek;
                timeslot.created_at = DateTime.Now;
                timeslot.updated_at = DateTime.Now;
                timeslot.start = DateTime.SpecifyKind(timeslot.start, DateTimeKind.Local);
                timeslot.end = DateTime.SpecifyKind(timeslot.end, DateTimeKind.Local);
                if (!isConcurrent && timeslot.quantity > 1)
                {
                    var buffer = timeslot.start;
                    var createdTimeslots = new List<TemplateTimeslot>();
                    for (int i = 0; i < timeslot.quantity; i++)
                    {
                        var newTimeslot = new TemplateTimeslot
                        {
                            start = buffer,
                            end = buffer.AddMinutes(timeslot.duration),
                            duration = timeslot.duration,
                            quantity = 1,
                            day = timeslot.day,
                            court_template_id = timeslot.court_template_id,
                            description = timeslot.description,
                            category_id = timeslot.category_id,
                            blocked = timeslot.blocked,
                            public_block = timeslot.public_block,
                            block_reason = timeslot.block_reason,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        };
                        ctl.CreateTemplateTimeslot(newTimeslot);
                        foreach (var motionId in restrictedMotions)
                        {
                            var timeslotMotionCtl = new TimeslotMotionController();
                            timeslotMotionCtl.CreateTimeslotMotion(new TimeslotMotion
                            {
                                timeslotable_type = "App\\Models\\TemplateTimeslot",
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
                        message = "Template timeslots created successfully",
                        timeslots = createdTimeslots.Select(t => new
                        {
                            id = t.id,
                            start = t.start.ToString("yyyy-MM-ddTHH:mm"),
                            end = t.end.ToString("yyyy-MM-ddTHH:mm"),
                            duration = t.duration,
                            quantity = t.quantity,
                            allDay = t.allDay,
                            day = t.day,
                            court_template_id = t.court_template_id,
                            description = t.description,
                            category_id = t.category_id,
                            blocked = t.blocked,
                            public_block = t.public_block,
                            block_reason = t.block_reason
                        })
                    });
                }
                else
                {
                    ctl.CreateTemplateTimeslot(timeslot);
                    foreach (var motionId in restrictedMotions)
                    {
                        var timeslotMotionCtl = new TimeslotMotionController();
                        timeslotMotionCtl.CreateTimeslotMotion(new TimeslotMotion
                        {
                            timeslotable_type = "App\\Models\\TemplateTimeslot",
                            timeslotable_id = timeslot.id,
                            motion_id = motionId,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        });
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        status = 200,
                        message = "Template timeslot created successfully",
                        id = timeslot.id,
                        start = timeslot.start.ToString("yyyy-MM-ddTHH:mm"),
                        end = timeslot.end.ToString("yyyy-MM-ddTHH:mm"),
                        duration = timeslot.duration,
                        quantity = timeslot.quantity,
                        allDay = timeslot.allDay,
                        day = timeslot.day,
                        court_template_id = timeslot.court_template_id,
                        description = timeslot.description,
                        category_id = timeslot.category_id,
                        blocked = timeslot.blocked,
                        public_block = timeslot.public_block,
                        block_reason = timeslot.block_reason
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
        public HttpResponseMessage DeleteTemplateTimeslots(JArray p1) //Delete multiple template timeslots
        {
            try
            {
                UserInfo currentUser = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
                if (currentUser == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new { status = 403, message = "You do not have permission to delete this timeslot." });
                }
                var timeslotIds = p1.ToObject<List<long>>();
                if (timeslotIds == null || !timeslotIds.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "At least one timeslot ID is required." });
                }
                var ctl = new CourtTemplateController();
                var timeslotMotionCtl = new TimeslotMotionController();
                var permissionController = new CourtPermissionController();
                var firstTemplateTimeslot = ctl.GetTemplateTimeslot(timeslotIds.First());
                var template = ctl.GetCourtTemplate(firstTemplateTimeslot.court_template_id.Value);
                var court = new CourtController().GetCourt(template.court_id);
                var judge = court.GetJudge();

                foreach (var id in timeslotIds)
                {
                    var templateTimeslot = ctl.GetTemplateTimeslot(id);
                    var hasPermission = permissionController.HasCourtPermission(currentUser.UserID, judge.id);
                    if (!hasPermission && !currentUser.IsAdmin)
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden, new { status = 403, message = "You do not have permission to delete this timeslot." });
                    }
                    foreach (var motion in templateTimeslot.motions)
                    {
                        var timeslotMotions = timeslotMotionCtl.GetTemplateTimeslotMotions(id, "App\\Models\\TemplateTimeslot");
                        foreach (var timeslotMotion in timeslotMotions)
                        {
                            timeslotMotionCtl.DeleteTimeslotMotion(timeslotMotion.id);
                        }
                    }
                    ctl.DeleteTemplateTimeslot(id);
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    message = "Templates deleted successfully"
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
        public HttpResponseMessage CopyTemplateTimeslots(JArray p1)
        {
            try
            {
                UserInfo currentUser = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
                if (currentUser == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new { status = 403, message = "You do not have permission to edit this template." });
                }
                var timeslotIds = p1.ToObject<List<long>>();
                if (timeslotIds == null || !timeslotIds.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "At least one timeslot ID is required." });
                }

                var ctl = new CourtTemplateController();
                var timeslotMotionCtl = new TimeslotMotionController();
                var permissionController = new CourtPermissionController();
                var firstTemplateTimeslot = ctl.GetTemplateTimeslot(timeslotIds.First());
                var template = ctl.GetCourtTemplate(firstTemplateTimeslot.court_template_id.Value);
                var court = new CourtController().GetCourt(template.court_id);
                var judge = court.GetJudge();

                foreach (var id in timeslotIds)
                {
                    var existingTimeslot = ctl.GetTemplateTimeslot(id);
                    var hasPermission = permissionController.HasCourtPermission(currentUser.UserID, judge.id);
                    if (!hasPermission && !currentUser.IsAdmin)
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden, new { status = 403, message = "You do not have permission to edit this template." });
                    }
                    var clonedTimeslot = (TemplateTimeslot)existingTimeslot.Clone();
                    ctl.CreateTemplateTimeslot(clonedTimeslot);
                    if (clonedTimeslot != null)
                    {
                        foreach (var timeslotMotion in existingTimeslot.timeslot_motions)
                        {
                            var clonedTimeslotMotion = (TimeslotMotion)timeslotMotion.Clone();
                            timeslotMotionCtl.CreateTimeslotMotion(clonedTimeslotMotion);
                        }
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    message = "Template Timeslots Copied successfully"
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage CloneTemplate(long p1)
        {
            try
            {
                var ctl = new CourtTemplateController();
                ctl.CloneTemplate(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Template cloned successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }
    }
}