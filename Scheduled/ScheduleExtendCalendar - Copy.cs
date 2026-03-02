using DotNetNuke.Services.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Scheduled
{
    public class ScheduleExtendCalendar : SchedulerClient
    {
        public ScheduleExtendCalendar(ScheduleHistoryItem scheduleHistoryItem)
        {
            ScheduleHistoryItem = scheduleHistoryItem;
        }

        public override void DoWork()
        {
            try
            {
                ScheduleHistoryItem.AddLogNote("Starting auto extension...");

                var courtCtl = new CourtController();
                var courts = courtCtl.GetCourts().Where(c => c.auto_extension).ToList();

                var holidayCtl = new HolidayController();
                var holidays = holidayCtl.GetHolidays().ToList();
                var templateCtl = new CourtTemplateController(); // Assuming exists; else add
                var tsCtl = new TimeslotController();
                var ctoCtl = new CourtTemplateOrderController();
                var crtTmpCtl = new CourtTemplateController();
                foreach (var court in courts)
                {
                    var today = DateTime.Now.Date;
                    var end = today.AddDays(7 * court.calendar_weeks);

                    var lastTimeslot = tsCtl.GetLastTemplateTimeslot(court.id);

                    // Log for debugging
                    ScheduleHistoryItem.AddLogNote($"Processing court: {court.description}");

                    if (lastTimeslot == null) continue;

                    var startTemplate = templateCtl.GetCourtTemplate(lastTimeslot.template_id ?? 0);
                    if (startTemplate == null) continue;

                    var orderItem = ctoCtl.GetAutoExtendCourtTemplateOrders(court.id, startTemplate.id);
                    if (orderItem == null) continue;

                    var dates = new List<DateTime>();
                    for (var date = lastTimeslot.start.Date; date <= end; date = date.AddDays(1))
                    {
                        if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                        {
                            dates.Add(date);
                        }
                    }

                    int orderOfTemplate = orderItem.order.Value;
                    for (int key = 0; key < dates.Count; key++)
                    {
                        var date = dates[key];

                        Components.CourtTemplate templateMask;
                        if (key == 0)
                        {
                            templateMask = crtTmpCtl.GetCourtTemplateToExtend(court.id, orderOfTemplate);
                        }
                        else
                        {
                            var courtTemplateOrder = ctoCtl.GetCourtTemplateOrderToExtend(court.id, orderOfTemplate);
                            if (courtTemplateOrder != null)
                            {
                                templateMask = courtTemplateOrder.template;
                            }
                            else
                            {
                                orderOfTemplate = 1;
                                templateMask = ctoCtl.GetCourtTemplateOrderToExtend(court.id, orderOfTemplate)?.template;
                            }
                        }

                        if (templateMask == null) continue;

                        var currentTimeslots = tsCtl.GetTimeslotsByCourtAndDate(court.id, date).ToList();

                        var templateTimeslots = templateCtl.GetTemplateTimeslots(templateMask.id).Where(tt => tt.day == (int)date.DayOfWeek).ToList();

                        foreach (var templateTimeslot in templateTimeslots)
                        {
                            var startDt = date.Date.Add(templateTimeslot.start.TimeOfDay);
                            var endDt = date.Date.Add(templateTimeslot.end.TimeOfDay);

                            if (!holidays.Any(h => h.date.Date == date.Date))
                            {
                                var match = currentTimeslots
                                    .Where(ct => ct.start <= startDt && ct.end >= endDt)
                                    .FirstOrDefault();

                                bool noOldCreated = !currentTimeslots.Any(ct => ct.created_at < DateTime.Now.Date);

                                if (match == null && noOldCreated)
                                {
                                    // Log for debugging
                                   // ScheduleHistoryItem.AddLogNote($"Creating timeslot for {startDt} - {endDt}");

                                    var newTimeslot = new Timeslot
                                    {
                                        start = startDt,
                                        end = endDt,
                                        allDay = templateTimeslot.allDay,
                                        duration = templateTimeslot.duration,
                                        quantity = templateTimeslot.quantity,
                                        blocked = templateTimeslot.blocked,
                                        public_block = templateTimeslot.public_block,
                                        block_reason = string.IsNullOrEmpty(templateTimeslot.block_reason) ? null : templateTimeslot.block_reason,
                                        category_id = templateTimeslot.category_id,
                                        template_id = templateTimeslot.court_template_id,
                                        court_template_order_id = orderItem.id,
                                        description = templateTimeslot.description,
                                        created_at = DateTime.Now,
                                        updated_at = DateTime.Now
                                    };

                                    tsCtl.CreateTimeslot(newTimeslot);
                                    long newTimeslotId = newTimeslot.id;
                                    var ctCtl = new CourtTimeslotController();
                                    var newCourtTimeslot = new CourtTimeslot
                                    {
                                        court_id = court.id,
                                        timeslot_id = newTimeslotId,
                                        created_at = DateTime.Now,
                                        updated_at = DateTime.Now
                                    };
                                    ctCtl.CreateCourtTimeslot(newCourtTimeslot);
                                }
                                else if (match != null)
                                {
                                    match.template_id = templateTimeslot.court_template_id;
                                    tsCtl.UpdateTimeslot(match);
                                }
                            }
                        }

                        if (date.DayOfWeek == DayOfWeek.Friday)
                        {
                            orderOfTemplate++;
                        }
                    }
                }

                ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                ScheduleHistoryItem.Succeeded = false;
                ScheduleHistoryItem.AddLogNote($"Error: {ex.Message}");
                Errored(ref ex);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
            }
        }
    }
}
