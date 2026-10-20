using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using System.Web;
using tjc.Modules.Transcriptionist.Components;

namespace tjc.Modules.Transcriptionist
{
    /// <summary>Read-only view of one application.</summary>
    public partial class ApplicationDetail : TranscriptionistModuleBase
    {
        private Application _app;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (RedirectIfNoAccess()) return;

                if (TranscriptionistID > 0)
                {
                    _app = new ApplicationController(_hostSettings).GetApplication(TranscriptionistID);
                    if (_app == null)
                    {
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Application not found.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                        return;
                    }
                }
                else
                {
                    Response.Redirect(EditUrl("list"), true);
                    return;
                }

                if (!IsPostBack)
                {
                    lnkEdit.NavigateUrl = EditUrl("tid", TranscriptionistID.ToString(), "edit");
                    lnkList.NavigateUrl = EditUrl("list");
                    PopulateFields();
                    BindExperiences();
                    BindAttachments();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        // Label.Text is written to the page as it is, so every value the applicant typed is encoded first.
        private static string Enc(string value)
        {
            return HttpUtility.HtmlEncode(value ?? "");
        }

        private static string EncMultiline(string value)
        {
            return Enc(value).Replace("\r\n", "<br />").Replace("\n", "<br />").Replace("\r", "<br />");
        }

        private void PopulateFields()
        {
            lblFirstName.Text = Enc(_app.FirstName);
            lblMiddleName.Text = Enc(_app.MiddleName);
            lblLastName.Text = Enc(_app.LastName);
            lblHomeAddress.Text = EncMultiline(_app.HomeAddress);
            lblCounty.Text = Enc(_app.County);
            lblOfficePhone.Text = Enc(_app.OfficePhone);
            lblResidencePhone.Text = Enc(_app.ResidencePhone);
            lblCellPhone.Text = Enc(_app.CellPhone);
            lblEmployer.Text = Enc(_app.Employer);
            lblEmployerAddress.Text = EncMultiline(_app.EmployerAddress);
            lblEmailAddress.Text = Enc(_app.EmailAddress);
            lblTranscriptionTraining.Text = EncMultiline(_app.TranscriptionTraining);
            lblCertifications.Text = EncMultiline(_app.Certifications);
            lblMonthsExperience.Text = _app.MonthsExperience.HasValue
                ? $"{_app.MonthsExperience.Value / 12} years, {_app.MonthsExperience.Value % 12} months"
                : "N/A";
            lblSoftwareExperience.Text = EncMultiline(_app.SoftwareExperience);
            lblTurnAroundTime.Text = EncMultiline(_app.TurnAroundTime);
            lblRuleFamiliarity.Text = EncMultiline(_app.RuleFamiliarity);
            lblJacsVendor.Text = _app.JacsVendor == true ? "Yes" : "No";
            lblCreatedDate.Text = _app.CreatedDate?.ToString("MM/dd/yyyy") ?? "N/A";
            lblStatus.Text = _app.Status == "Approved" ? $"Approved ({_app.ApprovedDate:MM/dd/yyyy})"
                : _app.Status == "Rejected" ? $"Rejected ({_app.RejectedDate:MM/dd/yyyy})"
                : "Pending";
            pnlRejectionReason.Visible = _app.Status == "Rejected";
            lblRejectionReason.Text = EncMultiline(_app.RejectionReason);
        }

        private void BindExperiences()
        {
            rptExperiences.DataSource = new ExperienceController(_hostSettings).GetByTranscriptionistId(TranscriptionistID)
                .OrderByDescending(x => x.StartDate)
                .ToList();
            rptExperiences.DataBind();
        }

        private void BindAttachments()
        {
            var attachments = new AttachmentController(_hostSettings).GetByTranscriptionistId(TranscriptionistID).ToList();
            rptAttachments.DataSource = attachments;
            rptAttachments.DataBind();
            lblNoAttachments.Visible = !attachments.Any();
            lblNoExternalUrl.Visible = attachments.Any() && ExternalAppUrl == null;
        }
    }
}
