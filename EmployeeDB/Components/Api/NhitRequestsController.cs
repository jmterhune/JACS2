using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Helpers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Api
{
    /// <summary>
    /// REST endpoint for submitting a New Hire IT Worksheet. POST writes
    /// the request to tjc_nhit_request, builds a PDF via NhitPdfBuilder,
    /// and emails it to the helpdesk via NhitMailer.
    ///
    /// The recipient defaults to helpdesk@jud12.flcourts.org but can be
    /// overridden per-module via the Nhit_HelpdeskEmail module setting
    /// (so dev/test environments can route mail to a sandbox inbox).
    /// </summary>
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [ValidateAntiForgeryToken]
    public class NhitRequestsController : DnnApiController
    {
        private const string DefaultHelpdeskEmail = "helpdesk@jud12.flcourts.org";

        private readonly NhitRequestController _requests = new NhitRequestController();
        private readonly NhitItemController _items = new NhitItemController();

        public class SubmitResult
        {
            public int NhitRequestId { get; set; }
            public bool EmailSuccess { get; set; }
            public string EmailMessage { get; set; }
            public string EmailSentTo { get; set; }
        }

        [HttpPost]
        [ActionName("Submit")]
        public HttpResponseMessage Submit(NhitRequestInfo item)
        {
            if (item == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Body required");
            if (string.IsNullOrWhiteSpace(item.EmployeeName))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Employee Name is required");

            try
            {
                // 1) Persist the request snapshot first so even if PDF build
                //    or email send fails we still have a DB record of what
                //    was submitted (the form fields aren't lost).
                _requests.Create(item, UserInfo == null ? -1 : UserInfo.UserID);

                // 2) Build the PDF using the catalog as it stands NOW. Items
                //    that have since been deactivated still appear if they
                //    were in SelectedItemIds — that's why the Create call
                //    snapshots Name+Category onto the junction row.
                var allItems = _items.GetActive().ToList();
                var pdfBytes = NhitPdfBuilder.Build(item, allItems, item.SelectedItemIds);

                // 3) Send the helpdesk email.
                var to = HelpdeskEmail();
                var from = ResolveFromAddress();
                var sendResult = NhitMailer.Send(item, pdfBytes, from, to);

                // 4) Record the email outcome on the request row.
                _requests.UpdateEmailStatus(item.NhitRequestId, to, sendResult.Success, sendResult.ErrorMessage);

                var result = new SubmitResult
                {
                    NhitRequestId = item.NhitRequestId,
                    EmailSuccess = sendResult.Success,
                    EmailMessage = sendResult.Success ? "Worksheet sent to " + to : sendResult.ErrorMessage,
                    EmailSentTo = to
                };
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>Re-generate and download the PDF for a submitted request.
        /// Used by the "Download PDF" link in the request history view.</summary>
        [HttpGet]
        [ActionName("Pdf")]
        public HttpResponseMessage Pdf(int id)
        {
            try
            {
                var request = _requests.GetById(id);
                if (request == null) return Request.CreateResponse(HttpStatusCode.NotFound);

                // Pull the snapshot rows AND the catalog. We render against
                // the catalog so the PDF's section structure stays current,
                // but checked state is taken from the saved request_item rows.
                var savedItems = _requests.GetItemsForRequest(id).ToList();
                request.SelectedItemIds = savedItems.Where(s => s.IsChecked).Select(s => s.NhitItemId).ToList();
                var allItems = _items.GetActive().ToList();

                var pdf = NhitPdfBuilder.Build(request, allItems, request.SelectedItemIds);

                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new System.Net.Http.ByteArrayContent(pdf)
                };
                resp.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                resp.Content.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "NewHireIT_" + id + ".pdf"
                    };
                return resp;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        // -------- helpers --------

        private string HelpdeskEmail()
        {
            // Per-module override, falling back to the production address.
            var settings = ActiveModule == null ? null : ActiveModule.ModuleSettings;
            if (settings != null && settings.Contains("Nhit_HelpdeskEmail"))
            {
                var raw = settings["Nhit_HelpdeskEmail"];
                var v = raw == null ? null : raw.ToString();
                if (!string.IsNullOrWhiteSpace(v)) return v.Trim();
            }
            return DefaultHelpdeskEmail;
        }

        private string ResolveFromAddress()
        {
            // Prefer the logged-in user's email so replies go back to HR
            // rather than into a noreply void. Fall back to the helpdesk
            // address itself when the user has no email on file.
            if (UserInfo != null && !string.IsNullOrWhiteSpace(UserInfo.Email))
                return UserInfo.Email;
            return HelpdeskEmail();
        }
    }
}
