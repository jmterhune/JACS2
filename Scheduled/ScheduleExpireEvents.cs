using DotNetNuke.Services.Scheduling;
using System;
using System.Linq;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Scheduled
{
    public class ScheduleExpireEvents : SchedulerClient
    {
        public ScheduleExpireEvents(ScheduleHistoryItem scheduleHistoryItem)
        {
            ScheduleHistoryItem = scheduleHistoryItem;
        }

        public override void DoWork()
        {
            try
            {
                ScheduleHistoryItem.AddLogNote("Starting event expiration...");
                var estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); // America/New_York
                var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, estZone);
                var statusCtl = new EventStatusController(); // Assuming exists; else add below
                var activeStatuses = statusCtl.GetEventActiveStatusIds();
                if (activeStatuses.Any())
                {
                    var pastStatusId = statusCtl.GetEventPastStatusId();
                    if (pastStatusId <= 0)
                    {
                        ScheduleHistoryItem.AddLogNote("Past status not found.");
                        ScheduleHistoryItem.Succeeded = false;
                        return;
                    }
                    var evtCtl = new EventController();
                    var events = evtCtl.GetEventsByStatus(activeStatuses);
                    var tsCtl = new TimeslotController();
                    foreach (var evt in events)
                    {
                        var timeslot = tsCtl.GetTimeslotByEventId(evt.id);
                        if (timeslot != null)
                        {
                            var endTime = timeslot.end; // Assume stored in local time; adjust if UTC
                            if (endTime < now)
                            {
                                evt.status_id = pastStatusId;
                                evt.owner_type = "App\\Models\\User"; // Adjust to DNN user model
                                evtCtl.UpdateEvent(evt,false);
                            }
                        }
                        else
                        {
                            ScheduleHistoryItem.AddLogNote($"Event ID {evt.id} has no timeslot."); // Log missing timeslot
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