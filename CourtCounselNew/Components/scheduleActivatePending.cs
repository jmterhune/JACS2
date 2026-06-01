using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tjc.Modules.CourtCounsel.Components
{
    public class ScheduleActivatePending : SchedulerClient
    {
        public ScheduleActivatePending(ScheduleHistoryItem oItem) : base()
        {
            this.ScheduleHistoryItem = oItem;
        }

        public override void DoWork()
        {
            try
            {
                // Perform required items for logging
                this.Progressing();
                var ctl = new AssignmentController();
                IEnumerable<Assignment> pendingAssignments = ctl.GetPendingAssignmentsToUpdate();
                foreach (Assignment assignment in pendingAssignments)
                {
                    assignment.StatusTypeId = 0;
                    assignment.ModifiedBy = "Scheduler";
                    assignment.ModifiedDate = DateTime.Now;
                    ctl.UpdateAssignment(assignment);
                    SendNotification(assignment);
                }
                // To log note
                this.ScheduleHistoryItem.AddLogNote("Notifications Processed Successfully");
                // Show success
                this.ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                this.ScheduleHistoryItem.Succeeded = false;
                this.ScheduleHistoryItem.AddLogNote("Exception= " + ex.ToString());
                this.Errored(ref ex);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
            }
        }

        private static void SendNotification(Assignment assignment)
        {
            string subject = "Pending Assignment Activated";
            string body = "";
            var ctl = new MemberController();
            Member member = ctl.GetMember(assignment.CurrentAttorneyId);
            LogEntry logEntry = assignment.logEntry;
            if (logEntry != null)
            {
                body = string.Format("A pending assignment for Case Number: {0} has been activated", logEntry.CaseNumber);
            }
            DotNetNuke.Services.Mail.Mail.SendEmail("ccworkflow@jud12.flcourts.org", member.Email, subject, body);
            DotNetNuke.Services.Mail.Mail.SendEmail("ccworkflow@jud12.flcourts.org", "ccworkflow@jud12.flcourts.org", subject, body);
        }
    }
}