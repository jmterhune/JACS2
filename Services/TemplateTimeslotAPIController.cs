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
    public class TemplateTimeslotAPIController : DnnApiController
    {
        private readonly TemplateTimeslotController _timeslotController = new TemplateTimeslotController();
        private readonly CourtTemplateController _courtTemplateController = new CourtTemplateController();
        private readonly TimeslotMotionController _motionController = new TimeslotMotionController();

        [HttpGet]
        public HttpResponseMessage GetTemplateTimeslots(long p1)
        {
            try
            {
                var timeslots = _courtTemplateController.GetTemplateTimeslots(p1)
                    .Select(t => new
                    {
                        id = t.id,
                        start = t.start.ToString("yyyy-MM-ddTHH:mm:ss"),
                        end = t.end.ToString("yyyy-MM-ddTHH:mm:ss"),
                        title = t.title,
                        color = t.color,
                        total_length = t.duration + " minutes",
                        update_url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Host}/DesktopModules/JACS/API/TemplateTimeslotAPI/UpdateTemplateTimeslot",
                        edit_url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Host}/DesktopModules/JACS/API/TemplateTimeslotAPI/GetTemplateTimeslot/{t.id}",
                        delete_url = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Host}/DesktopModules/JACS/API/TemplateTimeslotAPI/DeleteTemplateTimeslot/{t.id}"
                    });
                return Request.CreateResponse(HttpStatusCode.OK, timeslots);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetTemplateTimeslot(long p1)
        {
            try
            {
                var timeslot = _timeslotController.GetTemplateTimeslot((int)p1);
                if (timeslot == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { error = "Timeslot not found" });
                }
                var motions = _motionController.GetMotionsByTemplateTimeslot(p1).Select(m => new { motion_id = m.id });
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    timeslot.id,
                    timeslot.start,
                    timeslot.end,
                    timeslot.duration,
                    timeslot.quantity,
                    timeslot.allDay,
                    timeslot.day,
                    timeslot.court_template_id,
                    timeslot.description,
                    timeslot.category_id,
                    timeslot.blocked,
                    timeslot.public_block,
                    timeslot.block_reason,
                    motions
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
        public HttpResponseMessage CreateTemplateTimeslot(JObject p1)
        {
            try
            {
                var timeslot = p1.ToObject<TemplateTimeslot>();
                var motions = p1["timeslot_motions"]?.ToObject<List<long>>() ?? new List<long>();
                var isConcurrent = p1["cattlecall"]?.ToString() == "1";

                if (string.IsNullOrEmpty(timeslot.start.ToString()) || string.IsNullOrEmpty(timeslot.end.ToString()))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Start and End times are required." });
                }
                if (timeslot.blocked && string.IsNullOrEmpty(timeslot.block_reason))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Block Reason is required when Block is checked." });
                }
                if (isConcurrent && timeslot.quantity < 1)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Quantity must be at least 1 for concurrent timeslots." });
                }

                timeslot.day = timeslot.start.DayOfWeek.GetHashCode();
                if (isConcurrent)
                {
                    _timeslotController.CreateTemplateTimeslot(timeslot);
                    foreach (var motionId in motions)
                    {
                        _motionController.CreateTimeslotMotion(new TimeslotMotion
                        {
                            timeslotable_id = timeslot.id,
                            timeslotable_type = "App\\Models\\TemplateTimeslot",
                            motion_id = motionId
                        });
                    }
                }
                else // Consecutive timeslots
                {
                    var start = timeslot.start;
                    var end = start.AddMinutes(timeslot.duration);
                    for (int i = 0; i < timeslot.quantity; i++)
                    {
                        var newTimeslot = new TemplateTimeslot
                        {
                            start = start,
                            end = end,
                            day = start.DayOfWeek.GetHashCode(),
                            court_template_id = timeslot.court_template_id,
                            duration = timeslot.duration,
                            quantity = 1,
                            description = timeslot.description,
                            category_id = timeslot.category_id,
                            blocked = timeslot.blocked,
                            public_block = timeslot.public_block,
                            block_reason = timeslot.block_reason
                        };
                        _timeslotController.CreateTemplateTimeslot(newTimeslot);
                        foreach (var motionId in motions)
                        {
                            _motionController.CreateTimeslotMotion(new TimeslotMotion
                            {
                                timeslotable_id = newTimeslot.id,
                                timeslotable_type = "App\\Models\\TemplateTimeslot",
                                motion_id = motionId
                            });
                        }
                        start = end;
                        end = start.AddMinutes(timeslot.duration);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Timeslot created successfully", id = timeslot.id });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateTemplateTimeslot(JObject p1)
        {
            try
            {
                var timeslot = p1.ToObject<TemplateTimeslot>();
                var motions = p1["timeslot_motions"]?.ToObject<List<long>>() ?? new List<long>();

                if (timeslot.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Timeslot ID is required." });
                }
                if (string.IsNullOrEmpty(timeslot.start.ToString()) || string.IsNullOrEmpty(timeslot.end.ToString()))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Start and End times are required." });
                }
                if (timeslot.blocked && string.IsNullOrEmpty(timeslot.block_reason))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Block Reason is required when Block is checked." });
                }

                var existingTimeslot = _timeslotController.GetTemplateTimeslot((int)timeslot.id);
                if (existingTimeslot == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Timeslot not found." });
                }

                timeslot.day = timeslot.start.DayOfWeek.GetHashCode();
                _timeslotController.UpdateTemplateTimeslot(timeslot);

                // Update motions
                var existingMotions = _motionController.GetMotionsByTemplateTimeslot(timeslot.id).Select(m => m.id).ToList();
                foreach (var existingMotionId in existingMotions)
                {
                    if (!motions.Contains(existingMotionId))
                    {
                        _motionController.DeleteTimeslotMotion(existingMotionId, timeslot.id, "App\\Models\\TemplateTimeslot");
                    }
                }
                foreach (var motionId in motions)
                {
                    if (!existingMotions.Contains(motionId))
                    {
                        _motionController.CreateTimeslotMotion(new TimeslotMotion
                        {
                            timeslotable_id = timeslot.id,
                            timeslotable_type = "App\\Models\\TemplateTimeslot",
                            motion_id = motionId
                        });
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Timeslot updated successfully" });
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
                var timeslot = _timeslotController.GetTemplateTimeslot((int)p1);
                if (timeslot == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "Timeslot not found." });
                }
                var motions = _motionController.GetMotionsByTemplateTimeslot(p1);
                foreach (var motion in motions)
                {
                    _motionController.DeleteTimeslotMotion(motion.id, p1, "App\\Models\\TemplateTimeslot");
                }
                _timeslotController.DeleteTemplateTimeslot((int)p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Timeslot deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage DeleteMultiple(JArray p1)
        {
            try
            {
                foreach (var id in p1.ToObject<List<long>>())
                {
                    var timeslot = _timeslotController.GetTemplateTimeslot((int)id);
                    if (timeslot != null)
                    {
                        var motions = _motionController.GetMotionsByTemplateTimeslot(id);
                        foreach (var motion in motions)
                        {
                            _motionController.DeleteTimeslotMotion(motion.id, id, "App\\Models\\TemplateTimeslot");
                        }
                        _timeslotController.DeleteTemplateTimeslot((int)id);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Timeslots deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CopyMultiple(JArray p1)
        {
            try
            {
                foreach (var id in p1.ToObject<List<long>>())
                {
                    var timeslot = _timeslotController.GetTemplateTimeslot((int)id);
                    if (timeslot != null)
                    {
                        var newTimeslot = new TemplateTimeslot
                        {
                            start = timeslot.start,
                            end = timeslot.end,
                            duration = timeslot.duration,
                            quantity = timeslot.quantity,
                            allDay = timeslot.allDay,
                            day = timeslot.day,
                            court_template_id = timeslot.court_template_id,
                            description = timeslot.description + " (Copy)",
                            category_id = timeslot.category_id,
                            blocked = timeslot.blocked,
                            public_block = timeslot.public_block,
                            block_reason = timeslot.block_reason
                        };
                        _timeslotController.CreateTemplateTimeslot(newTimeslot);
                        var motions = _motionController.GetMotionsByTemplateTimeslot(id);
                        foreach (var motion in motions)
                        {
                            _motionController.CreateTimeslotMotion(new TimeslotMotion
                            {
                                timeslotable_id = newTimeslot.id,
                                timeslotable_type = "App\\Models\\TemplateTimeslot",
                                motion_id = motion.id
                            });
                        }
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "Timeslots copied successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetOverlappingTemplateTimeslots(long p1, string p2, string p3)
        {
            try
            {
                DateTime startDate = DateTime.Parse(p2);
                DateTime endDate = DateTime.Parse(p3);
                var timeslots = _timeslotController.GetTemplateTimeslots()
                    .Where(t => t.court_template_id == p1 &&
                                t.start < endDate &&
                                t.end > startDate)
                    .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, timeslots);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { error = ex.Message });
            }
        }
    }
}
