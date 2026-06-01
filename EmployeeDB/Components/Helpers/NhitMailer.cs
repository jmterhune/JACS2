using DotNetNuke.Services.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Helpers
{
    /// <summary>
    /// Sends a submitted New Hire IT Worksheet PDF to the helpdesk. Wraps
    /// DotNetNuke.Services.Mail.Mail.SendEmail so the rest of the codebase
    /// just hands us a request + PDF bytes + recipient and gets back a
    /// success/error tuple.
    ///
    /// The subject line includes the employee name + effective date so the
    /// helpdesk can sort tickets by what they refer to without opening the
    /// attachment.
    /// </summary>
    public static class NhitMailer
    {
        public class Result
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }

        public static Result Send(NhitRequestInfo request, byte[] pdfBytes,
            string fromAddress, string toAddress)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (pdfBytes == null || pdfBytes.Length == 0) throw new ArgumentException("PDF body required", nameof(pdfBytes));
            if (string.IsNullOrWhiteSpace(toAddress)) throw new ArgumentException("Recipient required", nameof(toAddress));

            var fileName = BuildAttachmentFileName(request);
            var subject = "New Hire IT Worksheet — " + (request.EmployeeName ?? "(unnamed)").Trim();
            if (request.EffectiveDate.HasValue)
            {
                subject += " (effective " + request.EffectiveDate.Value.ToString("MM/dd/yyyy") + ")";
            }
            var body = BuildBody(request);

            // DNN's MailAttachment constructor takes (string filename, byte[]),
            // and the matching SendEmail overload accepts an ICollection of
            // them. This is the non-deprecated path in DNN 9.11+.
            try
            {
                var attachments = new List<MailAttachment>
                {
                    new MailAttachment(fileName, pdfBytes)
                };
                var fromOrFallback = string.IsNullOrWhiteSpace(fromAddress) ? toAddress : fromAddress;

                // Positional args — DNN's parameter names have varied across
                // versions and named-arg binding is brittle.
                var result = Mail.SendEmail(
                    fromOrFallback,        // mailFrom
                    fromOrFallback,        // mailSender
                    toAddress,             // mailTo
                    subject,
                    body,
                    (ICollection<MailAttachment>)attachments);

                if (string.IsNullOrEmpty(result))
                {
                    return new Result { Success = true };
                }
                return new Result { Success = false, ErrorMessage = result };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, ErrorMessage = ex.Message };
            }
        }

        // -------- private helpers --------

        private static string BuildAttachmentFileName(NhitRequestInfo r)
        {
            var name = (r.EmployeeName ?? "Employee").Trim();
            // Replace anything Windows / Outlook would choke on. Keep it
            // simple — no unicode, no path separators, no email-spam-flag
            // special characters.
            foreach (var c in Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
            name = name.Replace(' ', '_');

            var datePart = r.EffectiveDate.HasValue
                ? r.EffectiveDate.Value.ToString("yyyy-MM-dd")
                : DateTime.Now.ToString("yyyy-MM-dd");
            return "NewHireIT_" + name + "_" + datePart + ".pdf";
        }

        private static string BuildBody(NhitRequestInfo r)
        {
            // Simple plaintext-with-line-breaks body. The PDF is the
            // authoritative copy; the body is a quick at-a-glance summary
            // for the helpdesk inbox preview.
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("A new hire IT worksheet has been submitted.");
            sb.AppendLine();
            sb.AppendLine("Employee:        " + (r.EmployeeName ?? string.Empty));
            sb.AppendLine("Position:        " + (r.PositionTitle ?? string.Empty));
            sb.AppendLine("Department:      " + (r.DepartmentUnitGroup ?? string.Empty));
            sb.AppendLine("Supervisor:      " + (r.SupervisorName ?? string.Empty));
            sb.AppendLine("Effective Date:  " + (r.EffectiveDate.HasValue ? r.EffectiveDate.Value.ToString("MM/dd/yyyy") : string.Empty));
            sb.AppendLine("Building:        " + (r.BuildingLocation ?? string.Empty));
            sb.AppendLine();
            sb.AppendLine("Full details are in the attached PDF.");
            return sb.ToString();
        }
    }
}
