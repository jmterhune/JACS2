using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Helpers;
using tjc.Modules.EmployeeDB.Components.SWN;

namespace tjc.Modules.EmployeeDB.Components.Api
{
    /// <summary>
    /// REST endpoints for the SWN sync buttons on EmployeeList.
    ///
    /// These were originally postback LinkButtons on EmployeeList.ascx, but
    /// the postback path was polluting the URL with junk segments (DNN's
    /// BreadCrumb get_GroupId() then blew up trying to int.Parse them) and
    /// there was no good place to surface progress while the long-running
    /// Sync was working its way through ~600 contacts.
    ///
    /// AJAX endpoints solve both: the page never reloads, the URL stays
    /// clean, and the JS layer can put up a "syncing... please wait"
    /// overlay while the request is in flight.
    /// </summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class SwnController : DnnApiController
    {
        // --------------- Module settings → SWN credentials ---------------
        // Credential reading lives in SwnSettings so PhonesController (and any
        // future SWN-aware endpoint) can share the exact same logic. Keys are
        // the same as what EmployeeDBModuleBase reads on the page side.
        private SwnSettings.Credentials SwnCreds =>
            SwnSettings.Read(ActiveModule?.ModuleSettings);

        private string SwnUsername => SwnCreds.Username;
        private string SwnPassword => SwnCreds.Password;

        // --------------- Response shape ---------------
        // success  → drives Noty (success) vs SweetAlert (error/details) on the JS side.
        // title    → modal/banner heading.
        // html     → ready-to-render HTML body (already encoded server-side).
        public class SwnResult
        {
            public bool Success { get; set; }
            public string Title { get; set; }
            public string Html { get; set; }
        }

        private static SwnResult Build(string title, string html, bool success)
            => new SwnResult { Title = title, Html = html, Success = success };

        // --------------- Endpoints ---------------

        /// <summary>List active employees with no matching SWN contact.</summary>
        [HttpGet]
        [ActionName("MissingContacts")]
        public HttpResponseMessage MissingContacts()
        {
            try
            {
                var swn = new SWNServiceRequests(SwnUsername, SwnPassword);
                var swnIds = swn.GetContactIds() ?? new List<int>();
                var swnSet = new HashSet<int>(swnIds);

                var active = new EmployeeController().GetActive().ToList();
                var missing = active
                    .Where(x => !swnSet.Contains(x.EmployeeId))
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .ToList();

                if (missing.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        Build("Missing SWN Contacts",
                              "All active employees have corresponding SWN contacts.",
                              success: true));
                }

                var sb = new StringBuilder();
                sb.Append("<p><strong>")
                  .Append(missing.Count)
                  .Append(missing.Count == 1 ? " employee is" : " employees are")
                  .Append(" missing from SWN:</strong></p>")
                  // Bootstrap's list-unstyled drops the bullet markers and the
                  // default ul left-padding, so the names align flush with the
                  // headline above them.
                  .Append("<ul class=\"list-unstyled mb-0\">");
                foreach (var m in missing)
                    sb.Append("<li>").Append(HttpUtility.HtmlEncode(m.DisplayName)).Append("</li>");
                sb.Append("</ul>");

                return Request.CreateResponse(HttpStatusCode.OK,
                    Build("Missing SWN Contacts", sb.ToString(), success: false));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>Long-running: pushes every active employee into SWN
        /// (creates / updates) and deletes contacts that are no longer active.</summary>
        [HttpPost]
        [ActionName("Sync")]
        public HttpResponseMessage Sync()
        {
            try
            {
                var swn = new SWNServiceRequests(SwnUsername, SwnPassword);
                var response = swn.BlockUpdateContacts();
                return Request.CreateResponse(HttpStatusCode.OK,
                    FormatSwnResponse("SWN Sync", response));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>Push every Department flagged IsSwnGroup into SWN as a Group.</summary>
        [HttpPost]
        [ActionName("AddAllGroups")]
        public HttpResponseMessage AddAllGroups()
        {
            try
            {
                var swn = new SWNServiceRequests(SwnUsername, SwnPassword);
                var response = swn.AddAllGroups();
                return Request.CreateResponse(HttpStatusCode.OK,
                    FormatSwnResponse("Add All Groups", response));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>Adds every active employee that doesn't already have an
        /// SWN contact. Targeted alternative to a full Sync — only touches
        /// rows that need to be created and leaves existing contacts alone.</summary>
        [HttpPost]
        [ActionName("AddMissing")]
        public HttpResponseMessage AddMissing()
        {
            try
            {
                var swn = new SWNServiceRequests(SwnUsername, SwnPassword);
                var swnIds = new HashSet<int>(swn.GetContactIds() ?? new List<int>());

                var missing = new EmployeeController().GetActive()
                    .Where(e => !swnIds.Contains(e.EmployeeId))
                    .OrderBy(e => e.LastName)
                    .ThenBy(e => e.FirstName)
                    .ToList();

                if (missing.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        Build("Add Missing SWN Contacts",
                              "All active employees already have an SWN contact &mdash; nothing to add.",
                              success: true));
                }

                var response = swn.BlockAddMissing(missing);
                var formatted = FormatSwnResponse("Add Missing SWN Contacts", response);
                // Prepend a count header so the user sees the scope at a glance.
                formatted.Html = "<p><strong>" + missing.Count
                    + (missing.Count == 1 ? " employee" : " employees")
                    + " added to SWN:</strong></p>" + formatted.Html;
                return Request.CreateResponse(HttpStatusCode.OK, formatted);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>Returns the pipe-delimited SWN contact export as a
        /// downloadable text file. Mirrors the format the legacy
        /// SWN-List.aspx page produced. The browser-side handler triggers
        /// this via window.location so the user sees a normal download.</summary>
        [HttpGet]
        [ActionName("Export")]
        public HttpResponseMessage Export()
        {
            try
            {
                var result = SwnExportBuilder.Build();

                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(result.Content, Encoding.UTF8, "text/plain")
                };
                resp.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "SWN_Export_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt"
                    };
                return resp;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        // --------------- Helpers ---------------

        private static SwnResult FormatSwnResponse(string title, SWNResponse response)
        {
            if (response == null)
                return Build(title, HttpUtility.HtmlEncode(title) + ": no response returned.", false);

            var sb = new StringBuilder();
            if (response.MessageList != null && response.MessageList.Count > 0)
            {
                sb.Append("<ul class=\"list-unstyled mb-0\">");
                foreach (var m in response.MessageList)
                {
                    sb.Append("<li>[")
                      .Append(HttpUtility.HtmlEncode(m.MessageType.ToString()))
                      .Append("] ")
                      .Append(HttpUtility.HtmlEncode(m.MessageText ?? ""))
                      .Append("</li>");
                }
                sb.Append("</ul>");
            }
            else
            {
                sb.Append("<p>").Append(HttpUtility.HtmlEncode(title)).Append(" completed.</p>");
            }

            return Build(title, sb.ToString(), success: !response.HasErrors);
        }
    }
}
