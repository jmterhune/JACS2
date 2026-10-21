using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using tjc.Modules.Transcriptionist.Components;

namespace tjc.Modules.Transcriptionist
{
    /// <summary>
    /// Read-only view of one application. A pending application is approved or rejected here, which emails the
    /// applicant; changing the record or its status without an email is done on the Edit page.
    /// </summary>
    public partial class ApplicationDetail : TranscriptionistModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (RedirectIfNoAccess()) return;

                lnkList.NavigateUrl = EditUrl("list");
                lnkNotFoundBack.NavigateUrl = EditUrl("list");

                // asp:CheckBox renders <span class="form-check"><input/><label/></span>; Bootstrap 5 needs these on the
                // inner elements. The box reflects what the applicant submitted, so it can't be toggled here.
                chkJacsVendor.InputAttributes["class"] = "form-check-input";
                chkJacsVendor.LabelAttributes["class"] = "form-check-label";
                chkJacsVendor.InputAttributes["onclick"] = "return false;";
                chkJacsVendor.InputAttributes["onkeydown"] = "return false;";

                if (!IsPostBack)
                    PopulateForm();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private Application LoadApplication()
        {
            return TranscriptionistID > 0 ? new ApplicationController(_hostSettings).GetApplication(TranscriptionistID) : null;
        }

        // Literal.Text is written to the page as it is, so every value the applicant typed is encoded first.
        private static string Enc(string value)
        {
            return HttpUtility.HtmlEncode(value ?? "");
        }

        private static string EncMultiline(string value)
        {
            return Enc(value).Replace("\r\n", "<br />").Replace("\n", "<br />").Replace("\r", "<br />");
        }

        private void PopulateForm()
        {
            var app = LoadApplication();
            if (app == null)
            {
                pnlApplication.Visible = false;
                pnlNotFound.Visible = true;
                return;
            }

            bool pending = app.Status == "Pending";
            pnlReview.Visible = pending;
            cmdApprove.Visible = pending;
            cmdReject.Visible = pending;
            pnlAlreadyReviewed.Visible = !pending;

            lnkEdit.NavigateUrl = EditUrl("tid", app.TranscriptionistID.ToString(), "edit");
            ltHeading.Text = $"<h2>{Enc(app.FullName)} Transcriptionist Application #{app.TranscriptionistID} <span class=\"badge {StatusBadgeClass(app.Status)}\">{app.Status}</span></h2>";

            ltFirstName.Text = Enc(app.FirstName);
            ltMiddleName.Text = Enc(app.MiddleName);
            ltLastName.Text = Enc(app.LastName);
            ltHomeAddress.Text = EncMultiline(app.HomeAddress);
            ltCounty.Text = Enc(app.County);
            ltOfficePhone.Text = Enc(app.OfficePhone);
            ltResidencePhone.Text = Enc(app.ResidencePhone);
            ltCellPhone.Text = Enc(app.CellPhone);
            ltEmployer.Text = Enc(app.Employer);
            ltEmployerAddress.Text = EncMultiline(app.EmployerAddress);
            if (!string.IsNullOrEmpty(app.EmailAddress))
            {
                lnkEmail.Text = Enc(app.EmailAddress);
                lnkEmail.NavigateUrl = "mailto:" + app.EmailAddress;
            }

            ltTranscriptionTraining.Text = EncMultiline(app.TranscriptionTraining);
            ltCertifications.Text = EncMultiline(app.Certifications);
            ltMonthsExperience.Text = app.MonthsExperience.HasValue
                ? $"{app.MonthsExperience.Value / 12} years, {app.MonthsExperience.Value % 12} months"
                : "N/A";
            ltSoftwareExperience.Text = EncMultiline(app.SoftwareExperience);
            ltTurnAroundTime.Text = EncMultiline(app.TurnAroundTime);
            ltRuleFamiliarity.Text = EncMultiline(app.RuleFamiliarity);
            chkJacsVendor.Checked = app.JacsVendor == true;

            ltSubmitted.Text = app.CreatedDate?.ToString("MM/dd/yyyy") ?? "N/A";
            ltStatus.Text = app.Status == "Approved" ? $"Approved ({app.ApprovedDate:MM/dd/yyyy})"
                : app.Status == "Rejected" ? $"Rejected ({app.RejectedDate:MM/dd/yyyy})"
                : "Pending";
            pnlRejectionReason.Visible = app.Status == "Rejected";
            ltRejectionReason.Text = EncMultiline(app.RejectionReason);

            var experiences = new ExperienceController(_hostSettings).GetByTranscriptionistId(app.TranscriptionistID)
                .OrderByDescending(x => x.StartDate)
                .ToList();
            rptExperiences.DataSource = experiences;
            rptExperiences.DataBind();
            rptExperiences.Visible = experiences.Any();
            lblNoExperience.Visible = !experiences.Any();

            var attachments = new AttachmentController(_hostSettings).GetByTranscriptionistId(app.TranscriptionistID).ToList();
            rptAttachments.DataSource = attachments;
            rptAttachments.DataBind();
            rptAttachments.Visible = attachments.Any();
            lblNoAttachments.Visible = !attachments.Any();
            lblNoExternalUrl.Visible = attachments.Any() && ExternalAppUrl == null;
        }

        protected void cmdApprove_Click(object sender, EventArgs e)
        {
            ReviewApplication(approve: true);
        }

        protected void cmdReject_Click(object sender, EventArgs e)
        {
            ReviewApplication(approve: false);
        }

        private void ReviewApplication(bool approve)
        {
            try
            {
                if (RedirectIfNoAccess()) return;

                var app = LoadApplication();
                if (app == null || app.Status != "Pending") return;

                string notes = txtReviewNotes.Text.Trim();
                if (!approve && notes.Length == 0)
                {
                    ltErrors.Text = "A reason is required to reject an application. Enter it in the Review Notes box.";
                    pnlErrors.Visible = true;
                    return;
                }

                if (approve)
                {
                    app.ApprovedDate = DateTime.UtcNow;
                    app.ApprovedByUserID = IntranetUserId;
                    app.RejectedDate = null;
                    app.RejectedByUserID = null;
                    app.RejectionReason = null;
                }
                else
                {
                    app.RejectedDate = DateTime.UtcNow;
                    app.RejectedByUserID = IntranetUserId;
                    app.RejectionReason = Limit(notes, 4000);
                    app.ApprovedDate = null;
                    app.ApprovedByUserID = null;
                }
                app.LastModifiedDate = DateTime.UtcNow;
                app.LastModifiedByUserID = IntranetUserId;
                new ApplicationController(_hostSettings).UpdateApplication(app);

                SendStatusEmail(app, approve, notes);
                Response.Redirect(EditUrl("list"), true);
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private static void SendStatusEmail(Application app, bool approved, string notes)
        {
            if (string.IsNullOrWhiteSpace(app.EmailAddress)) return;

            string subject = approved
                ? "Your Transcriptionist Application Has Been Approved"
                : "Your Transcriptionist Application Has Been Rejected";

            string firstName = Enc(app.FirstName);
            string notesHtml = EncMultiline(notes);
            string body = approved
                ? $"Dear {firstName},<br /><br />Congratulations! Your application to become a court-approved transcriptionist for the 12th Judicial Circuit has been approved.<br /><br />"
                    + (notes.Length > 0 ? notesHtml + "<br /><br />" : "") + "Thank you."
                : $"Dear {firstName},<br /><br />We regret to inform you that your application to become a court-approved transcriptionist for the 12th Judicial Circuit has been rejected.<br /><br /><strong>Reason:</strong> {notesHtml}<br /><br />If you have questions, please contact our office.<br /><br />Thank you.";

            string toAddress = app.EmailAddress;

            // Fire-and-forget: an unreachable/slow SMTP server must never block this postback.
            // A synchronous SendEmail call here can stall the SmtpClient for its full connect
            // timeout, and under load that stalls the whole worker process's thread pool.
            Task.Run(() =>
            {
                try
                {
                    DotNetNuke.Services.Mail.Mail.SendEmail("webhelp@jud12.flcourts.org", toAddress, subject, body);
                }
                catch
                {
                    // Notification failure must not affect the approve/reject action itself.
                }
            });
        }

        private static string StatusBadgeClass(string status)
        {
            switch (status)
            {
                case "Approved": return "bg-success";
                case "Rejected": return "bg-danger";
                default: return "bg-secondary";
            }
        }
    }
}
