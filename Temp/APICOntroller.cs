// ApiController.cs (DNN Web API Controller)
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;

namespace CourtCalendarModule.Controllers
{
    [SupportedModules("CourtCalendarModule")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)] // Adjust security as needed
    public class ApiController : DnnApiController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage ExtendCalendar(ExtendRequest model)
        {
            try
            {
                var calendar = DataController.GetCourt(model.CourtId);
                if (calendar == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Court not found");

                var holidays = DataController.GetHolidays().ToList();
                var orderedTemplates = DataController.GetAutoCourtTemplateOrders(model.CourtId).ToList();
                if (!orderedTemplates.Any()) return Request.CreateResponse(HttpStatusCode.BadRequest, "No auto templates found");

                var lastTemplateTimeslot = DataController.GetLastTemplateTimeslot(model.CourtId);

                DateTime startWeek;
                if (lastTemplateTimeslot != null)
                {
                    if (model.StartDate.Date == lastTemplateTimeslot.start.Date)
                    {
                        startWeek = GetStartOfWeek(lastTemplateTimeslot.start.AddDays(7));
                    }
                    else
                    {
                        startWeek = GetStartOfWeek(model.StartDate);
                    }
                }
                else
                {
                    startWeek = GetStartOfWeek(DateTime.Now);
                }

                int currentOrder = model.StartTemplate;
                DateTime currentWeekStart = startWeek;

                for (int x = 0; x < model.Weeks; x++)
                {
                    var cto = DataController.GetAutoCourtTemplateOrderByOrder(model.CourtId, currentOrder);
                    if (cto == null)
                    {
                        currentOrder = 1;
                        cto = DataController.GetAutoCourtTemplateOrderByOrder(model.CourtId, currentOrder);
                    }
                    if (cto == null || !cto.template_id.HasValue) continue;

                    var template = DataController.GetTemplate(cto.template_id.Value);
                    var templateTimeslots = DataController.GetTemplateTimeslots(template.id).ToList();

                    DateTime currentDay = currentWeekStart;
                    for (int y = 0; y < 5; y++)
                    {
                        DateTime dayDate = currentDay.Date;
                        bool isHoliday = holidays.Any(h => h.date.Date == dayDate);

                        var dayTimeslots = templateTimeslots.Where(tt => tt.day == y + 1);
                        foreach (var tts in dayTimeslots)
                        {
                            DateTime start = dayDate.Add(tts.start.TimeOfDay);
                            DateTime end = dayDate.Add(tts.end.TimeOfDay);

                            if (!isHoliday)
                            {
                                var newTs = new Timeslot
                                {
                                    start = start,
                                    end = end,
                                    description = tts.description,
                                    allDay = tts.allDay,
                                    duration = tts.duration,
                                    quantity = tts.quantity,
                                    blocked = tts.blocked,
                                    block_reason = tts.block_reason,
                                    public_block = tts.public_block,
                                    category_id = tts.category_id,
                                    template_id = template.id
                                };

                                int newTsId = DataController.CreateTimeslot(newTs);

                                var newCts = new CourtTimeslot
                                {
                                    court_id = model.CourtId,
                                    timeslot_id = newTsId
                                };
                                DataController.CreateCourtTimeslot(newCts);
                            }
                        }
                        currentDay = currentDay.AddDays(1);
                    }

                    currentOrder++;
                    currentWeekStart = currentWeekStart.AddDays(7);
                }

                return Request.CreateResponse(HttpStatusCode.OK, "Extending Successful");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private DateTime GetStartOfWeek(DateTime dt)
        {
            int diff = (7 + ((int)dt.DayOfWeek - (int)DayOfWeek.Monday)) % 7;
            return dt.AddDays(-diff).Date;
        }
    }
}