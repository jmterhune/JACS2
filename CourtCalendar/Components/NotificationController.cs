/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace tjc.Modules.CourtCalendar.Components
{
    /// <summary>
    /// Sends calendar change notifications to subscribers. Subscriptions and the
    /// subscriber accounts both live on the public (jud12) portal, so this reads
    /// them through the "Jud12" connection and resolves each subscriber's email
    /// from that portal's Users table (the Arbitrator app's cross-portal pattern).
    /// </summary>
    public class NotificationController
    {
        private const string CONN_JUD12 = "Jud12";
        private readonly IHostSettings _hostSettings;

        public NotificationController(IHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
        }

        /// <summary>Distinct, non-empty subscriber email addresses for a calendar.</summary>
        public IList<string> GetSubscriberEmails(int calendarId)
        {
            var emails = new List<string>();
            using (IDataContext ctx = DataContext.Instance(_hostSettings, CONN_JUD12))
            {
                var subs = ctx.GetRepository<Subscription>()
                    .Find("WHERE CalendarId = @0", calendarId).ToList();
                foreach (var s in subs)
                {
                    try
                    {
                        var email = ctx.ExecuteScalar<string>(CommandType.Text,
                            "SELECT Email FROM Users WHERE UserID = @0", s.UserId);
                        if (!string.IsNullOrWhiteSpace(email)) emails.Add(email.Trim());
                    }
                    catch (Exception exc)
                    {
                        DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
                    }
                }
            }
            return emails.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        /// <summary>
        /// Emails every subscriber that a calendar changed. Returns the number of
        /// messages sent. Each subscriber is emailed individually so addresses are
        /// not disclosed to one another.
        /// </summary>
        public int SendUpdateNotification(Calendar cal, Division division, string fromEmail, string calendarUrl, string senderName)
        {
            var emails = GetSubscriberEmails(cal.CalendarId);
            if (emails.Count == 0) return 0;

            var divisionName = division != null ? division.Name : "court";
            var period = cal.MonthName + " " + cal.Year;
            var subject = string.Format("Calendar updated: {0} – {1}", divisionName, period);

            var link = string.IsNullOrWhiteSpace(calendarUrl)
                ? string.Empty
                : string.Format("<p>View the calendar: <a href=\"{0}\">{0}</a></p>", calendarUrl);
            var by = string.IsNullOrWhiteSpace(senderName) ? string.Empty : " by " + senderName;
            var body = string.Format(
                "<p>The {0} calendar for {1} has been updated{2}.</p>{3}" +
                "<p>You are receiving this because you subscribed to updates for this calendar.</p>",
                System.Web.HttpUtility.HtmlEncode(divisionName),
                System.Web.HttpUtility.HtmlEncode(period),
                System.Web.HttpUtility.HtmlEncode(by),
                link);

            var from = string.IsNullOrWhiteSpace(fromEmail) ? "noreply@jud12.flcourts.org" : fromEmail;
            int sent = 0;
            foreach (var email in emails)
            {
                try
                {
                    DotNetNuke.Services.Mail.Mail.SendEmail(from, email, subject, body);
                    sent++;
                }
                catch (Exception exc)
                {
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
                }
            }
            return sent;
        }
    }
}
