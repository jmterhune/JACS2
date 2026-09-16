using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tjc.Modules.jacs.Components
{
    /// <summary>
    /// The substitute-recipient switch for event notification email.
    ///
    /// When it is on, every notice <see cref="EventNotificationMailer"/> sends goes to
    /// one configured address instead of the real attorneys, so a non-production site
    /// can exercise the whole mail path without contacting anybody outside the office.
    ///
    /// Stored in DNN HostSettings rather than the JACS database on purpose: the mailer
    /// runs from places with no module or portal context (the truncate path through
    /// ClerkEventService, scheduled tasks), HostSettings is resolvable from all of them,
    /// and because each environment has its own DNN database the setting cannot follow a
    /// deployment from test into production.
    /// </summary>
    internal static class NotificationSettings
    {
        internal const string EnabledKey = "JACS_SubstituteEmailEnabled";
        internal const string AddressKey = "JACS_SubstituteEmailAddress";

        public static bool Enabled
        {
            get
            {
                try
                {
                    // Read the raw value and parse it here rather than leaning on
                    // GetBoolean: DNN's own host booleans are mostly "Y"/"N" but not
                    // uniformly ("DebugMode" is "False"), and both sides of this
                    // setting are ours, so an explicit parse keeps them in step.
                    string raw = (HostController.Instance.GetString(EnabledKey, string.Empty) ?? string.Empty).Trim();
                    return raw.Equals("Y", StringComparison.OrdinalIgnoreCase)
                        || raw.Equals("True", StringComparison.OrdinalIgnoreCase)
                        || raw == "1";
                }
                catch { return false; }
            }
        }

        public static string Address
        {
            get
            {
                try { return HostController.Instance.GetString(AddressKey, string.Empty) ?? string.Empty; }
                catch { return string.Empty; }
            }
        }

        /// <summary>
        /// Saves both values together. An address is required whenever the switch is
        /// being turned on, so it can never be left in a state that would silently
        /// swallow mail.
        /// </summary>
        public static void Save(bool enabled, string address)
        {
            string trimmed = (address ?? string.Empty).Trim();

            if (enabled && !IsValidAddress(trimmed))
                throw new ArgumentException("A valid substitute email address is required to turn substitution on.");

            HostController.Instance.Update(AddressKey, trimmed, true);
            HostController.Instance.Update(EnabledKey, enabled ? "Y" : "N", true);
        }

        /// <summary>
        /// The address every notice should be redirected to, or null when mail should go
        /// to its real recipients. A blank or unparseable stored address counts as off,
        /// so a half-finished configuration can never black-hole notifications.
        /// </summary>
        public static string GetSubstituteRecipient()
        {
            if (!Enabled)
                return null;

            string address = Address.Trim();
            if (!IsValidAddress(address))
            {
                Exceptions.LogException(new Exception(
                    "Substitute email is enabled but the stored address is missing or invalid " +
                    $"('{address}'); sending to the real recipients instead."));
                return null;
            }

            return address;
        }

        public static bool IsValidAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return false;

            try
            {
                // MailAddress is the same parser the send path will use.
                var parsed = new System.Net.Mail.MailAddress(address.Trim());
                return string.Equals(parsed.Address, address.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Sends "Hearing Created" and "Hearing Cancellation" notices to the
    /// attorneys on an event. Recipients come from the emails xref table for
    /// the event's attorney_id and opp_attorney_id (emailable_type =
    /// "App\Models\Attorney"). The body matches the court-approved
    /// confirmation/cancellation format.
    ///
    /// Send failures are logged but never thrown — the underlying event
    /// create/cancel has already succeeded by the time we run.
    /// </summary>
    internal class EventNotificationMailer
    {
        private const string AttorneyEmailableType = "App\\Models\\Attorney";

        private enum NoticeKind
        {
            Created,
            Cancellation,
        }

        /// <summary>
        /// Notify attorneys that a new hearing was created. Called from
        /// EventAPIController.CreateEvent after the local insert + timeslot
        /// link land. Safe to call unconditionally — silently no-ops if
        /// there are no attorney emails on file.
        /// </summary>
        public void SendCreated(Event evt) => Send(evt, NoticeKind.Created, reason: null);

        /// <summary>
        /// Notify attorneys that a hearing was cancelled. Called from every
        /// cancel path (ClerkEventService.CancelEventAsync used by truncate,
        /// plus the single-event UI flow in EventAPIController.CancelEvent).
        /// </summary>
        public void SendCancellation(Event evt, string reason) => Send(evt, NoticeKind.Cancellation, reason);

        private void Send(Event evt, NoticeKind kind, string reason)
        {
            try
            {
                if (evt == null) return;

                // The body needs a date/time/duration — pull the timeslot.
                var timeslot = new TimeslotController().GetTimeslotByEventId(evt.id);
                if (timeslot == null) return;

                long courtId = new EventController().GetCourtIdByEventId(evt.id);
                if (courtId <= 0) return;

                var court = new CourtController().GetCourt(courtId);
                if (court == null) return;

                // Court-level opt-out: when "Email Confirmations" is unchecked
                // on the court edit screen, no event-related emails go out for
                // ANY of that court's events or timeslots — creation,
                // cancellation, future notice types alike. The flag is the
                // single switch covering all of this court's notifications.
                if (!court.email_confirmations) return;

                var county = new CountyController().GetCounty(court.county_id);
                var judge = new JudgeController().GetJudgeByCourt(courtId);

                // Resolve motion description (id 221 is "Other" — use custom_motion).
                string motionDesc;
                if (evt.motion_id == 221)
                    motionDesc = evt.custom_motion ?? string.Empty;
                else if (evt.motion_id.HasValue)
                    motionDesc = new MotionController().GetMotion(evt.motion_id.Value)?.description ?? string.Empty;
                else
                    motionDesc = string.Empty;

                // Courtroom name (pulled from the timeslot's courtroom_id).
                string courtroomName = string.Empty;
                if (timeslot.courtroom_id.HasValue && timeslot.courtroom_id.Value > 0)
                    courtroomName = new CourtroomController().GetCourtroom(timeslot.courtroom_id.Value)?.description ?? string.Empty;

                // Attorney names reformatted from stored "First Middle Last [Suffix]"
                // to "Last, First Middle [Suffix]" matching docket-print convention.
                Attorney attorney = null, opposing = null;
                if (evt.attorney_id.HasValue)
                    attorney = new AttorneyController().GetAttorney(evt.attorney_id.Value);
                if (evt.opp_attorney_id.HasValue)
                    opposing = new AttorneyController().GetAttorney(evt.opp_attorney_id.Value);

                string attorneyDisplay = ReformatNameLastFirst(attorney?.name);
                string oppDisplay = ReformatNameLastFirst(opposing?.name);

                // Collect attorney recipient emails from the xref table.
                var emailCtl = new EmailController();
                var attorneyRecipients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var oppRecipients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (attorney != null) AddAttorneyEmails(emailCtl, attorney.id, attorneyRecipients);
                if (opposing != null) AddAttorneyEmails(emailCtl, opposing.id, oppRecipients);

                if (attorneyRecipients.Count == 0 && oppRecipients.Count == 0) return;

                string countyName = county?.name ?? "";
                string judgeName = judge?.name?.ToUpper() ?? "";
                string courtDesc = (court.description ?? "").ToUpper();
                if (!string.IsNullOrEmpty(countyName))
                    courtDesc = string.IsNullOrEmpty(courtDesc) ? countyName.ToUpper() : courtDesc + ", " + countyName.ToUpper();

                string startDate = timeslot.start.ToString("MM/dd/yyyy");
                string startTime = timeslot.start.ToString("h:mm tt").ToLower();

                string body = BuildBody(
                    kind: kind,
                    countyName: countyName,
                    courtDesc: courtDesc,
                    judgeName: judgeName,
                    startDate: startDate,
                    startTime: startTime,
                    duration: timeslot.duration,
                    caseNum: evt.case_num ?? "",
                    motion: motionDesc,
                    courtroom: courtroomName,
                    attorneyDisplay: attorneyDisplay,
                    plaintiff: evt.plaintiff ?? "",
                    oppDisplay: oppDisplay,
                    defendant: evt.defendant ?? "",
                    confirmationNum: evt.clerk_event_id,
                    reason: reason ?? "");

                string subjectVerb = kind == NoticeKind.Created ? "Hearing Created" : "Hearing Cancellation";
                string subject = $"{subjectVerb} - Case {evt.case_num} on {startDate}";

                string fromAddr = ResolveFromAddress();
                if (string.IsNullOrWhiteSpace(fromAddr))
                {
                    Exceptions.LogException(new Exception(
                        $"EventNotificationMailer: cannot resolve a from-address; skipping {kind} email for event {evt.id}."));
                    return;
                }

                // Send one message per attorney bucket so opposing parties don't
                // see each other's addresses on the To line.
                if (attorneyRecipients.Count > 0)
                    Deliver(fromAddr, string.Join(",", attorneyRecipients), subject, body, evt.id);
                if (oppRecipients.Count > 0)
                    Deliver(fromAddr, string.Join(",", oppRecipients), subject, body, evt.id);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(new Exception(
                    $"EventNotificationMailer: unhandled exception sending {kind} email for event {evt?.id}.", ex));
            }
        }

        private static void Deliver(string from, string to, string subject, string body, long eventIdForLog)
        {
            // Substitution is applied here, at the single point every notice passes
            // through, so no caller can bypass it. Declared outside the try so the
            // failure logs below name where the mail was actually addressed. The real
            // recipients go to the log because the message itself is a court-approved
            // template we do not alter.
            string recipients = to;

            try
            {
                string substitute = NotificationSettings.GetSubstituteRecipient();
                if (!string.IsNullOrWhiteSpace(substitute))
                {
                    Exceptions.LogException(new Exception(
                        "EventNotificationMailer: substitute email is ON — redirecting the notice for " +
                        $"event {eventIdForLog} from '{to}' to '{substitute}'."));
                    recipients = substitute;
                }

                // Use the full string-args overload of Mail.SendMail so we route
                // through portal-default SMTP settings (the simpler 4-arg overload
                // expects DNN user ids, not addresses).
                string err = DotNetNuke.Services.Mail.Mail.SendMail(
                    mailFrom: from,
                    mailTo: recipients,
                    cc: "",
                    bcc: "",
                    priority: DotNetNuke.Services.Mail.MailPriority.Normal,
                    subject: subject,
                    bodyFormat: DotNetNuke.Services.Mail.MailFormat.Text,
                    bodyEncoding: Encoding.UTF8,
                    body: body,
                    attachments: new string[0],
                    smtpServer: "",
                    smtpAuthentication: "",
                    smtpUsername: "",
                    smtpPassword: "",
                    smtpEnableSSL: false);
                if (!string.IsNullOrWhiteSpace(err))
                {
                    Exceptions.LogException(new Exception(
                        $"EventNotificationMailer: Mail.SendMail returned '{err}' for recipient '{recipients}' (event {eventIdForLog})."));
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(new Exception(
                    $"EventNotificationMailer: Mail.SendMail threw for recipient '{recipients}' (event {eventIdForLog}).", ex));
            }
        }

        private static string BuildBody(
            NoticeKind kind,
            string countyName, string courtDesc, string judgeName,
            string startDate, string startTime, int duration,
            string caseNum, string motion, string courtroom,
            string attorneyDisplay, string plaintiff,
            string oppDisplay, string defendant,
            long confirmationNum, string reason)
        {
            // The two notice variants share header, fields, and the "do not
            // reply" footer. The cancellation adds a "Reason :" line, an
            // explanatory sentence, and the confidentiality block. The
            // creation notice stops at "Please do not reply" (matches the
            // template the court approved).
            string verb = kind == NoticeKind.Created ? "Hearing Created" : "Hearing Cancellation";

            var sb = new StringBuilder();
            sb.AppendLine($"***************  12th Judicial Circuit JACS Court Scheduling - {countyName} County  ***************");
            sb.AppendLine();
            sb.AppendLine($"                {courtDesc}");
            sb.AppendLine();
            sb.AppendLine($"                JUDGE {judgeName}");
            sb.AppendLine();
            sb.AppendLine("**********************************************************************************************");
            sb.AppendLine();
            sb.AppendLine(" ");
            sb.AppendLine();
            sb.AppendLine($"{verb} on {startDate} at {startTime} for {duration} minutes");
            sb.AppendLine();
            sb.AppendLine(" ");
            sb.AppendLine();
            sb.AppendLine($"        Case # : {caseNum}");
            sb.AppendLine();
            sb.AppendLine($"        Motion : {motion}");
            sb.AppendLine();
            sb.AppendLine($"        Courtroom : {courtroom}");
            sb.AppendLine();
            sb.AppendLine($"        Attorney : {attorneyDisplay}");
            sb.AppendLine();
            sb.AppendLine($"        Plaintiff : {plaintiff}");
            sb.AppendLine();
            sb.AppendLine($"        Opposing Attorney : {oppDisplay}");
            sb.AppendLine();
            sb.AppendLine($"        Defendant : {defendant}");
            sb.AppendLine();
            sb.AppendLine($"        Confirmation # : {confirmationNum}");

            if (kind == NoticeKind.Cancellation)
            {
                sb.AppendLine();
                sb.AppendLine($"        Reason : {reason}");
                sb.AppendLine();
                sb.AppendLine(" ");
                sb.AppendLine();
                sb.AppendLine("The hearing referenced above has been cancelled and will not take place.");
                sb.AppendLine();
                sb.AppendLine(" ");
                sb.AppendLine();
                sb.AppendLine(" ");
            }

            sb.AppendLine();
            sb.AppendLine("   *** Please do not reply to this email ***");

            if (kind == NoticeKind.Cancellation)
            {
                sb.AppendLine();
                sb.AppendLine(" ");
                sb.AppendLine();
                sb.AppendLine("----------------------------------------------------------------");
                sb.AppendLine();
                sb.AppendLine("This email may contain confidential and/or privileged information.");
                sb.AppendLine();
                sb.AppendLine("If you are not the intended recipient (or have received this email in error) please destroy this email.");
                sb.AppendLine();
                sb.AppendLine("----------------------------------------------------------------");
                sb.AppendLine();
                sb.AppendLine(" ");
                sb.AppendLine();
                sb.AppendLine("----------");
            }

            return sb.ToString();
        }

        private static void AddAttorneyEmails(EmailController emailCtl, long attorneyId, HashSet<string> bucket)
        {
            foreach (var e in emailCtl.GetEmails(attorneyId))
            {
                if (string.IsNullOrWhiteSpace(e?.email)) continue;
                if (!string.Equals(e.emailable_type, AttorneyEmailableType, StringComparison.OrdinalIgnoreCase)) continue;
                bucket.Add(e.email.Trim());
            }
        }

        private static string ResolveFromAddress()
        {
            // Prefer the current portal's admin email. Falls back to portal 0
            // for non-HTTP contexts (future scheduled-task callers).
            try
            {
                var settings = PortalSettings.Current;
                if (settings != null && !string.IsNullOrWhiteSpace(settings.Email))
                    return settings.Email;
            }
            catch { /* fall through */ }

            try
            {
                var portal = PortalController.Instance.GetPortal(0);
                if (portal != null && !string.IsNullOrWhiteSpace(portal.Email))
                    return portal.Email;
            }
            catch { /* fall through */ }

            return null;
        }

        /// <summary>
        /// Converts "First [Middle…] Last [Suffix]" into "Last, First [Middle…] [Suffix]".
        /// Common suffixes (Jr/Sr/II/III/IV/V/VI/Esq) are detected so
        /// "John Q Smith Jr" → "Smith, John Q Jr".
        /// </summary>
        private static string ReformatNameLastFirst(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";
            var parts = raw.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return raw;

            var suffixes = new[] { "jr", "jr.", "sr", "sr.", "ii", "iii", "iv", "v", "vi", "esq", "esq." };
            string lastPart = parts.Last();
            string suffix = "";
            string lastName = lastPart;
            if (suffixes.Contains(lastPart.ToLowerInvariant()))
            {
                suffix = lastPart;
                lastName = parts.Length >= 3 ? parts[parts.Length - 2] : parts[0];
            }

            int trimFromEnd = suffix.Length > 0 ? 2 : 1;
            var firstAndMiddle = parts.Take(parts.Length - trimFromEnd).ToList();
            string firstPart = firstAndMiddle.FirstOrDefault() ?? "";
            string middleParts = string.Join(" ", firstAndMiddle.Skip(1));

            string result = lastName;
            if (!string.IsNullOrEmpty(firstPart))
            {
                result += ", " + firstPart;
                if (!string.IsNullOrEmpty(middleParts)) result += " " + middleParts;
            }
            if (!string.IsNullOrEmpty(suffix)) result += " " + suffix;
            return result;
        }
    }
}
