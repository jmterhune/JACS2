using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Handlers
{
    public class ExportCalendar : IHttpHandler
    {
        public bool IsReusable => false;
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public void ProcessRequest(HttpContext context)
        {
            try
            {

                // Parse parameters
                if (!long.TryParse(context.Request.QueryString["courtId"], out long courtId) ||
                    !DateTime.TryParse(context.Request.QueryString["fromDate"], out DateTime fromDate) ||
                    !DateTime.TryParse(context.Request.QueryString["toDate"], out DateTime toDate))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.Write("Invalid parameters");
                    return;
                }
                toDate = toDate.AddDays(1); // Make end exclusive

                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(courtId);
                if (court == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.Write("Court not found");
                    return;
                }

                var evtCtl = new EventController();
                var events = evtCtl.GetEventsByCourtId(courtId, fromDate, toDate).ToList();
                string iCalContent = GenerateICalContent(events, court);
                string md5 = ComputeMD5(iCalContent);

                context.Response.ContentType = "text/calendar; charset=utf-8";
                context.Response.AddHeader("Content-Disposition", $"attachment; filename=\"{md5}.ics\"");
                context.Response.Write(iCalContent);
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Write(ex.Message);
            }
        }

        private string GenerateICalContent(IEnumerable<Event> events, Court court)
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine("BEGIN:VCALENDAR");
            output.AppendLine("METHOD:PUBLISH");
            output.AppendLine("VERSION:2.0");
            output.AppendLine("PRODID:-//xAI//Grok//EN"); // Adjusted for example
            output.AppendLine($"X-WR-RELCALID:{EscapeString(court.description)}");
            output.AppendLine($"X-WR-CALNAME:{EscapeString(court.description)}");
            output.AppendLine("CALSCALE:GREGORIAN");

            foreach (var evt in events)
            {
                if (evt.timeslot == null) continue;

                string description = BuildEventDescription(evt);

                output.AppendLine("BEGIN:VEVENT");
                output.AppendLine($"SUMMARY:{EscapeString("Hearing of  " + evt.case_num)}");
                output.AppendLine($"DESCRIPTION:{EscapeString(description)}");
                output.AppendLine($"UID:{evt.id}");
                output.AppendLine($"DTSTART:{evt.timeslot.start.ToString("yyyyMMdd\\THHmmss")}");
                output.AppendLine($"DTEND:{evt.timeslot.end.ToString("yyyyMMdd\\THHmmss")}");
                output.AppendLine($"DTSTAMP:{DateTime.Now.ToString("yyyyMMdd\\THHmmss")}");
                output.AppendLine("END:VEVENT");
            }

            output.AppendLine("END:VCALENDAR");
            return output.ToString();
        }

        private string BuildEventDescription(Event evt)
        {
            StringBuilder desc = new StringBuilder();
            desc.AppendLine($"Case Number: {evt.case_num}");
            desc.AppendLine($"Motion: {evt.Motion?.description ?? ""}");
            desc.AppendLine($"Hearing Type: {evt.EventType?.name ?? ""}");
            desc.AppendLine($"Attorney: {evt.attorney_name ?? ""}");
            desc.AppendLine($"Opposing Attorney: {evt.opp_attorney_name?? ""}");
            desc.AppendLine($"Plaintiff: {evt.plaintiff}");
            desc.AppendLine($"Defendant: {evt.defendant}");
            desc.AppendLine($"Plaintiff Email: {evt.plaintiff_email}");
            desc.AppendLine($"Defendant Email: {evt.defendant_email}");

            if (!string.IsNullOrEmpty(evt.template))
            {
                try
                {
                    var customFields = JsonConvert.DeserializeObject<Dictionary<string, string>>(evt.template);
                    foreach (var kvp in customFields)
                    {
                        string key = kvp.Key.Split(new[] { "_|" }, StringSplitOptions.None)[0];
                        desc.AppendLine($"{key}: {kvp.Value}");
                    }
                }
                catch { }
            }

            desc.AppendLine($"Notes: {evt.notes}");
            return desc.ToString();
        }

        private string EscapeString(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input.Replace("\\", "\\\\")
                        .Replace("\n", "\\n")
                        .Replace("\r", "\\r")
                        .Replace(",", "\\,")
                        .Replace(";", "\\;")
                        .Replace("\"", "");
        }

        private string ComputeMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}