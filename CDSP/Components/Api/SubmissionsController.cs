/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using tjc.Modules.CDSPAdmin.Components.Controllers;
using tjc.Modules.CDSPAdmin.Components.Models;

namespace tjc.Modules.CDSPAdmin.Components.Api
{
    /// <summary>
    /// AJAX endpoints for the submission list: fetch a detail fragment for the
    /// modal, and toggle the Completed flag. Both verbs require module View
    /// access and a valid anti-forgery token (sent by the JS layer).
    /// </summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class SubmissionsController : DnnApiController
    {
        private readonly SubmissionController _ctrl = new SubmissionController();

        /// <summary>Returns the submission's detail as a ready-to-inject HTML
        /// fragment (server-encoded) plus its id/completed state for the modal.</summary>
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            try
            {
                var s = _ctrl.GetSubmission(id);
                if (s == null) return Request.CreateResponse(HttpStatusCode.NotFound);
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    id = s.SubmissionID,
                    completed = s.Completed,
                    html = BuildDetailHtml(s)
                });
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>The single update call used by both the row checkbox and the
        /// modal button. Sets Completed (+ audit columns) and echoes the new state.</summary>
        [HttpPost]
        public HttpResponseMessage SetCompleted(SetCompletedDto dto)
        {
            if (dto == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required.");
            try
            {
                _ctrl.SetCompleted(dto.Id, dto.Completed, UserInfo.UserID);
                return Request.CreateResponse(HttpStatusCode.OK, new { id = dto.Id, completed = dto.Completed });
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        public class SetCompletedDto
        {
            public int Id { get; set; }
            public bool Completed { get; set; }
        }

        // ---- detail fragment builder -------------------------------------

        private static string BuildDetailHtml(SubmissionInfo s)
        {
            string status = s.Completed
                ? "<span class=\"badge bg-success\">Completed</span>"
                : "<span class=\"badge bg-warning text-dark\">Open</span>";
            string submitted = s.CreatedDate.HasValue ? s.CreatedDate.Value.ToString("MM/dd/yyyy h:mm tt") : string.Empty;

            var sb = new StringBuilder();

            sb.Append("<div class=\"cdsp-section\"><div class=\"cdsp-section-title\">Submission</div><div class=\"row\">");
            sb.Append(Field("Submitted", Enc(submitted), "col-md-3"));
            sb.Append(Field("Division", Enc(s.Division), "col-md-3"));
            sb.Append(Field("County", Enc(s.County), "col-md-3"));
            sb.Append(Field("Status", status, "col-md-3"));
            sb.Append("</div></div>");

            sb.Append("<div class=\"cdsp-section\"><div class=\"cdsp-section-title\">Complainant</div><div class=\"row\">");
            sb.Append(Field("Name", Enc(s.ComplainantName), "col-md-4"));
            sb.Append(Field("Phone", Enc(s.Phone), "col-md-4"));
            sb.Append(Field("Email", Enc(s.Email), "col-md-4"));
            sb.Append(Field("Address", EncMl(s.Address), "col-md-12"));
            sb.Append("</div></div>");

            sb.Append("<div class=\"cdsp-section\"><div class=\"cdsp-section-title\">Respondent</div><div class=\"row\">");
            sb.Append(Field("Name", Enc(s.RespondentName), "col-md-4"));
            sb.Append(Field("Phone", Enc(s.RespondentPhone), "col-md-4"));
            sb.Append(Field("Email", Enc(s.RespondentEmail), "col-md-4"));
            sb.Append(Field("Address", EncMl(s.RespondentAddress), "col-md-12"));
            sb.Append("</div></div>");

            sb.Append("<div class=\"cdsp-section\"><div class=\"cdsp-section-title\">Details</div><div class=\"row\">");
            sb.Append(Field("Children involved", s.ChildrenInvolved ? "Yes" : "No", "col-md-3"));
            sb.Append(Field("How did you hear about the program?", EncMl(s.HowDidYouHear), "col-md-9"));
            sb.Append(Field("Description of issues", EncMl(s.Comments), "col-md-12"));
            sb.Append("</div></div>");

            return sb.ToString();
        }

        private static string Field(string label, string valueHtml, string colClass)
        {
            return "<div class=\"" + colClass + "\"><dl><dt>" + HttpUtility.HtmlEncode(label) +
                   "</dt><dd>" + (string.IsNullOrEmpty(valueHtml) ? "&nbsp;" : valueHtml) + "</dd></dl></div>";
        }

        private static string Enc(string value)
        {
            return HttpUtility.HtmlEncode(value ?? string.Empty);
        }

        private static string EncMl(string value)
        {
            return Enc(value).Replace("\r\n", "\n").Replace("\n", "<br />");
        }
    }
}
