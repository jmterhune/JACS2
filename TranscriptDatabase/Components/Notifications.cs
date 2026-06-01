using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UserInfo = DotNetNuke.Entities.Users.UserInfo;

namespace tjc.Modules.TranscriptDatabase.Components
{
    public static class Notifications
    {
        private static UserInfo userinfo=UserController.Instance.GetCurrentUserInfo();
        private static void NotifiyRecordingManager(EventListItem evt, EventListItem evtOld, int sequence, int portalId, string displayName, string managerRole, string county)
        {
            string subject = string.Format("Event Update to Designation - {0}", displayName);
            StringBuilder body = new StringBuilder();
            if (evt.HearingDate.HasValue)
                body.Append(HighlightChange("Hearing Date: ", evt.HearingDate.Value.ToShortDateString(), evtOld.HearingDate.Value.ToShortDateString()));
            else
                body.Append("Hearing Date: No Hearing Date");

            body.Append(HighlightChange("Presiding Judge: ", evt.PresidingJudgeName, evtOld.PresidingJudgeName));
            body.Append(HighlightChange("Hearing Type: ", evt.HearingType, evtOld.HearingType));
            body.Append(HighlightChange("Court Reporter: ", evt.CourtReporterName, evtOld.CourtReporterName));
            body.Append(HighlightChange("Estimated Pages: ", evt.Pages.ToString(), evtOld.Pages.ToString()));
            body.Append(HighlightChange("Scopist: ", evt.ScopistName, evtOld.ScopistName));
            body.Append(HighlightChange("Scopist Begin Date: ", evt.ScopSent.Value.ToShortDateString(), evtOld.ScopSent.Value.ToShortDateString()));
            body.Append(HighlightChange("Scopist Pages Started: ", evt.ScopPagesIn.ToString(), evtOld.ScopPagesIn.ToString()));
            body.Append(HighlightChange("Scopist Completed Date: ", evt.ScopReturned.Value.ToShortDateString(), evtOld.ScopReturned.Value.ToShortDateString()));
            body.Append(HighlightChange("Scopist Pages Completed: ", evt.ScopPagesOut.ToString(), evtOld.ScopPagesOut.ToString()));
            body.Append(HighlightChange("Transcriptionist: ", evt.TranscriptionistName, evtOld.TranscriptionistName));
            body.Append(HighlightChange("Transcription Begin Date: ", evt.TransSent.Value.ToShortDateString(), evtOld.TransSent.Value.ToShortDateString()));
            body.Append(HighlightChange("Transcription Pages Started: ", evt.TransPagesIn.ToString(), evtOld.TransPagesIn.ToString()));
            body.Append(HighlightChange("Transcription Completed Date: ", evt.TransReturned.Value.ToShortDateString(), evtOld.TransReturned.Value.ToShortDateString()));
            body.Append(HighlightChange("Transcription Pages Completed: ", evt.TransPagesOut.ToString(), evtOld.TransPagesOut.ToString()));
            body.Append(HighlightChange("Editor: ", evt.EditorName, evtOld.EditorName));
            body.Append(HighlightChange("Editing Begin Date: ", evt.EditSent.Value.ToShortDateString(), evtOld.EditSent.Value.ToShortDateString()));
            body.Append(HighlightChange("Editing Pages Started: ", evt.EditPagesIn.ToString(), evtOld.EditPagesIn.ToString()));
            body.Append(HighlightChange("Editing Completed Date: ", evt.EditReturned.Value.ToShortDateString(), evtOld.EditReturned.Value.ToShortDateString()));
            body.Append(HighlightChange("Editing Pages Completed: ", evt.EditPagesOut.ToString(), evtOld.EditPagesOut.ToString()));
            body.Append(HighlightChange("Proofer: ", evt.ProoferName, evtOld.ProoferName));
            body.Append(HighlightChange("Proofing Begin Date: ", evt.ProofSent.Value.ToShortDateString(), evtOld.ProofSent.Value.ToShortDateString()));
            body.Append(HighlightChange("Proofing Pages Started: ", evt.ProofPagesIn.ToString(), evtOld.ProofPagesIn.ToString()));
            body.Append(HighlightChange("Proofing Completed Date: ", evt.ProofReturned.Value.ToShortDateString(), evtOld.ProofReturned.Value.ToShortDateString()));
            body.Append(HighlightChange("Proofing Pages Completed: ", evt.ProofPagesOut.ToString(), evtOld.ProofPagesOut.ToString()));
            body.Append(HighlightChange("Completed By: ", evt.CompletedByName, evtOld.CompletedByName));
            body.Append(HighlightChange("Completed Date: ", evt.Completed.Value.ToShortDateString(), evtOld.Completed.Value.ToShortDateString()));
            body.Append(HighlightChange("Completed Pages: ", evt.CompletedPages.ToString(), evtOld.CompletedPages.ToString()));
            IList<UserInfo> lstManager = RoleController.Instance.GetUsersByRole(portalId, managerRole);
            string fromEmail = "dcrgrpsar@jud12.flcourts.org";
            if (county.ToLower() == "manatee")
                fromEmail = "dcrgrpman@jud12.flcourts.org";
            foreach (UserInfo objuser in lstManager)
            {
                Mail.SendEmail(fromEmail, objuser.Email, subject, body.ToString());
            }
        }

        public static void NotifiyRecordingManager(string defendant, string url, int sequence, string managerRole, int portalId, string county)
        {
            string subject = "Designation Event Added Notification";
            StringBuilder body = new StringBuilder();

            if (sequence > 0)
            {
                body.Append("Event Number ");
                body.Append(sequence);
                body.Append(" for ");
                body.Append(defendant);
                body.Append(", has been completed.");
            }
            else
            {
                subject = string.Format("New Event Added for {0}", defendant);
                body.Append(string.Format("A new event has been added for <a href='{0}'>{1}</a>.  Please fill in your pages and days.", url, defendant));
            }

            IList<UserInfo> lstManager = RoleController.Instance.GetUsersByRole(portalId, managerRole);
            string fromEmail = "dcrgrpsar@jud12.flcourts.org";
            if (county.ToLower() == "manatee")
                fromEmail = "dcrgrpman@jud12.flcourts.org";
            foreach (UserInfo objuser in lstManager)
            {
                Mail.SendEmail(fromEmail, objuser.Email, subject, body.ToString());
            }
        }
        public static void NotifiyRecordingManager(int portalId, int designationId, string email, string managerRole, string displayName, string county)
        {
            string subject = string.Format("New Designation - {0}", displayName);
            string body = string.Format("A new Designation has been added for {0}\n\nDesignationId: {1}", displayName, designationId);
            IList<UserInfo> lstManager = RoleController.Instance.GetUsersByRole(portalId, managerRole);
            string fromEmail = "dcrgrpsar@jud12.flcourts.org";
            if (county.ToLower() == "manatee")
                fromEmail = "dcrgrpman@jud12.flcourts.org";
            foreach (UserInfo objuser in lstManager)
            {
                Mail.SendEmail(fromEmail, objuser.Email, subject, body.ToString());
            }
        }
        public static void NotifiyRecordingManager(string subject, string body, int portalId, string managerRole, string county)
        {
            IList<UserInfo> lstManager = RoleController.Instance.GetUsersByRole(portalId, managerRole);
            string fromEmail = "dcrgrpsar@jud12.flcourts.org";
            if (county.ToLower() == "manatee")
                fromEmail = "dcrgrpman@jud12.flcourts.org";
            foreach (UserInfo objuser in lstManager)
            {
                Mail.SendEmail(fromEmail, objuser.Email, subject, body.ToString());
            }
        }

        public static void SendCourtReporterNotification(int ReporterId, string defendant, string url, int sequence, int portalId, UserInfo user, string county)
        {
            string toemail = UserController.GetUserById(portalId, ReporterId).Email;
            string fromEmail = "dcrgrpsar@jud12.flcourts.org";
            if (county.ToLower() == "manatee")
                fromEmail = "dcrgrpman@jud12.flcourts.org";
            if (toemail != "")
            {
                string body = string.Format("You have been assigned to complete event information for <strong>Defendant:</strong> {0} <strong>Event:</strong> {1}<br /><a href='{2}'>Click Here to View</a>", defendant, sequence + 1, url);
                string subject = "Designation Event Assignment Notification";
                Mail.SendEmail(fromEmail, toemail, subject, body.ToString());
            }
        }

        public static void SendCourtReporterResetNotification(int reporterId, string displayname, int sequence, string managerRole, int portalId, UserInfo user, string county)
        {
            string body = string.Format("The Court Reporter field for <strong>Designation</strong> {0}, <strong>Event</strong> {1} has been set to blank. Please reassign a court reporter to the event.", displayname, sequence + 1);
            string subject = "Court Reporter Assigned Set to Blank";
            IList<UserInfo> lstManager = RoleController.Instance.GetUsersByRole(portalId, managerRole);
            string fromEmail = "dcrgrpsar@jud12.flcourts.org";
            if (county.ToLower() == "manatee")
                fromEmail = "dcrgrpman@jud12.flcourts.org";
            foreach (UserInfo objuser in lstManager)
            {
                Mail.SendEmail(fromEmail, objuser.Email, subject, body.ToString());
            }
        }

        public static void SendCourtReporterTransferrNotification(int oldReporterId, int ReporterId, string displayname, string url, int sequence, int portalId, string county)
        {
            string toEmail = UserController.GetUserById(portalId, oldReporterId).Email;
            string courtReporterName = UserController.GetUserById(portalId, ReporterId).DisplayName;
            string fromEmail = "dcrgrpsar@jud12.flcourts.org";
            if (county.ToLower() == "manatee")
                fromEmail = "dcrgrpman@jud12.flcourts.org";
            if (toEmail != "")
            {
                string body = string.Format("Designation {0} Event: {1} assigned to you has been re-assigned to {2}\n<a href='{3}'>View Designation</a>", displayname, sequence + 1, courtReporterName, url);
                string subject = "Designation Event Re-Assignment Notification";
                Mail.SendEmail(fromEmail, toEmail, subject, body.ToString());
            }
        }

        public static void SendCourtReporterExtensionNotification(IEnumerable<Event> events, DateTime requestedDate, string displayName, UserInfo user, int portalId)
        {
            string fromEmail = user.Email;
            foreach (Event evt in events)
            {
                UserInfo courtReporterUser = UserController.GetUserById(portalId, evt.CourtReporterID);
                if (courtReporterUser != null)
                {
                    string body = string.Format("The extension request for Designation - {0} has been granted.  The new due date is {1}", displayName, requestedDate.ToShortDateString());
                    string subject = "Extension Request Granted";
                    Mail.SendEmail(fromEmail, courtReporterUser.Email, subject, body.ToString());
                }
            }
        }
        private static string HighlightChange(string label, string newText, string oldText)
        {
            string outputString = "";
            if (newText != oldText)
            {
                if (DateTime.TryParse(oldText, out DateTime oldDate))
                {
                    if (oldDate == Null.NullDate || oldDate == null)
                        outputString = label + newText + " -> none" + Environment.NewLine + Environment.NewLine;
                }
                if (oldText == "")
                    outputString = (label + newText + " -> none" + Environment.NewLine + Environment.NewLine);
                outputString = (label + newText + " -> " + oldText + Environment.NewLine + Environment.NewLine);
            }
            return outputString;
        }
        private static void SendCourtReporterResetNotification(string displayName,int reporterId, int sequence,string managerRole,int portalId, string county)
        {
            string body = "The Court Reporter field for <strong>Designation</strong> " + displayName + ", <strong>Event</strong> " + sequence + " has been set to blank. Please reassign a court reporter to the event.";
            string subject = "Court Reporter Assigned Set to Blank";
            DotNetNuke.Security.Roles.RoleController ctlRole = new DotNetNuke.Security.Roles.RoleController();
            var lstManager = RoleController.Instance.GetUsersByRole(portalId, managerRole);
            string fromEmail = "dcrgrpsar@jud12.flcourts.org";
            if (county.ToLower() == "manatee")
                fromEmail = "dcrgrpman@jud12.flcourts.org";
            foreach (UserInfo objuser in lstManager)
            {
                Mail.SendEmail(fromEmail, objuser.Email, subject, body);
            }
        }

    }
}