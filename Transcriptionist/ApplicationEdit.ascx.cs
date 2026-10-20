using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using tjc.Modules.Transcriptionist.Components;

namespace tjc.Modules.Transcriptionist
{
    /// <summary>Edit, approve, reject or delete one application. Experience records are handled by Handlers/ExperienceApi.ashx.</summary>
    public partial class ApplicationEdit : TranscriptionistModuleBase
    {
        // The *ByUserID columns are shared with the jud12 site, whose user ids mean different people than
        // this site's, so the staff user's id is not written. 0 marks "changed on the intranet".
        private const int IntranetUserId = 0;

        private Application _app;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Bootstrap 5 styling for the checkbox control. CssClass on asp:CheckBox lands
                // on the wrapping <span>, not the <input>, which doubles up borders/outlines.
                chkJacsVendor.InputAttributes["class"] = "form-check-input";
                chkJacsVendor.LabelAttributes["class"] = "form-check-label";

                if (RedirectIfNoAccess()) return;

                if (TranscriptionistID > 0)
                {
                    _app = new ApplicationController(_hostSettings).GetApplication(TranscriptionistID);
                    if (_app == null)
                    {
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Application not found.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                        pnlForm.Visible = false;
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
                    lnkList.NavigateUrl = EditUrl("list");
                    PopulateFields();
                    BindAttachments();
                    btnApprove.Visible = _app.Status != "Approved";
                    btnReject.Visible = _app.Status != "Rejected";
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PopulateFields()
        {
            txtFirstName.Text = _app.FirstName;
            txtMiddleName.Text = _app.MiddleName;
            txtLastName.Text = _app.LastName;
            txtHomeAddress.Text = _app.HomeAddress;
            txtCounty.Text = _app.County;
            txtOfficePhone.Text = _app.OfficePhone;
            txtResidencePhone.Text = _app.ResidencePhone;
            txtCellPhone.Text = _app.CellPhone;
            txtEmployer.Text = _app.Employer;
            txtEmployerAddress.Text = _app.EmployerAddress;
            txtEmailAddress.Text = _app.EmailAddress;
            txtTranscriptionTraining.Text = _app.TranscriptionTraining;
            txtCertifications.Text = _app.Certifications;
            if (_app.MonthsExperience.HasValue)
            {
                txtYearsExperience.Text = (_app.MonthsExperience.Value / 12).ToString();
                txtMonthsExperience.Text = (_app.MonthsExperience.Value % 12).ToString();
            }
            txtSoftwareExperience.Text = _app.SoftwareExperience;
            txtTurnAroundTime.Text = _app.TurnAroundTime;
            txtRuleFamiliarity.Text = _app.RuleFamiliarity;
            chkJacsVendor.Checked = _app.JacsVendor ?? false;

            lblStatusBadge.Text = _app.Status;
            lblStatusBadge.CssClass = "badge " + (_app.Status == "Approved" ? "bg-success" : _app.Status == "Rejected" ? "bg-danger" : "bg-secondary");
            pnlRejectionReason.Visible = _app.Status == "Rejected";
            lblRejectionReason.Text = System.Web.HttpUtility.HtmlEncode(_app.RejectionReason ?? "");
        }

        private void BindAttachments()
        {
            var attachments = new AttachmentController(_hostSettings).GetByTranscriptionistId(TranscriptionistID).ToList();
            rptAttachments.DataSource = attachments;
            rptAttachments.DataBind();
            rptAttachments.Visible = attachments.Any();
            lblNoAttachments.Visible = !attachments.Any();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            UpdateApplication();
            Response.Redirect(EditUrl("list"), true);
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            UpdateApplication();
            _app.ApprovedDate = DateTime.UtcNow;
            _app.ApprovedByUserID = IntranetUserId;
            _app.RejectedDate = null;
            _app.RejectedByUserID = null;
            _app.RejectionReason = null;
            new ApplicationController(_hostSettings).UpdateApplication(_app);
            SendStatusEmail(approved: true);
            Response.Redirect(EditUrl("list"), true);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            string reason = hdRejectionReason.Value.Trim();
            if (reason.Length == 0)
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "A rejection reason is required.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                return;
            }

            UpdateApplication();
            _app.RejectedDate = DateTime.UtcNow;
            _app.RejectedByUserID = IntranetUserId;
            _app.RejectionReason = Limit(reason, 4000);
            _app.ApprovedDate = null;
            _app.ApprovedByUserID = null;
            new ApplicationController(_hostSettings).UpdateApplication(_app);
            SendStatusEmail(approved: false);
            Response.Redirect(EditUrl("list"), true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            new ApplicationController(_hostSettings).DeleteApplication(TranscriptionistID);
            Response.Redirect(EditUrl("list"), true);
        }

        private void SendStatusEmail(bool approved)
        {
            if (string.IsNullOrWhiteSpace(_app.EmailAddress)) return;

            string subject = approved
                ? "Your Transcriptionist Application Has Been Approved"
                : "Your Transcriptionist Application Has Been Rejected";

            string firstName = System.Web.HttpUtility.HtmlEncode(_app.FirstName);
            string body = approved
                ? $"Dear {firstName},<br /><br />Congratulations! Your application to become a court-approved transcriptionist for the 12th Judicial Circuit has been approved.<br /><br />Thank you."
                : $"Dear {firstName},<br /><br />We regret to inform you that your application to become a court-approved transcriptionist for the 12th Judicial Circuit has been rejected.<br /><br /><strong>Reason:</strong> {System.Web.HttpUtility.HtmlEncode(_app.RejectionReason)}<br /><br />If you have questions, please contact our office.<br /><br />Thank you.";

            string toAddress = _app.EmailAddress;

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

        // Column sizes in tjc_cat_applications; a longer value would make the update fail.
        private static string Limit(string value, int max)
        {
            return value.Length > max ? value.Substring(0, max) : value;
        }

        private void UpdateApplication()
        {
            _app.FirstName = Limit(txtFirstName.Text.Trim(), 50);
            _app.MiddleName = Limit(txtMiddleName.Text.Trim(), 50);
            _app.LastName = Limit(txtLastName.Text.Trim(), 50);
            _app.HomeAddress = Limit(txtHomeAddress.Text.Trim(), 4000);
            _app.County = Limit(txtCounty.Text.Trim(), 50);
            _app.OfficePhone = Limit(txtOfficePhone.Text.Trim(), 20);
            _app.ResidencePhone = Limit(txtResidencePhone.Text.Trim(), 20);
            _app.CellPhone = Limit(txtCellPhone.Text.Trim(), 20);
            _app.Employer = Limit(txtEmployer.Text.Trim(), 200);
            _app.EmployerAddress = Limit(txtEmployerAddress.Text.Trim(), 4000);
            _app.EmailAddress = Limit(txtEmailAddress.Text.Trim(), 255);
            _app.TranscriptionTraining = txtTranscriptionTraining.Text.Trim();
            _app.Certifications = txtCertifications.Text.Trim();
            _app.SoftwareExperience = Limit(txtSoftwareExperience.Text.Trim(), 2000);
            _app.TurnAroundTime = Limit(txtTurnAroundTime.Text.Trim(), 2000);
            _app.RuleFamiliarity = Limit(txtRuleFamiliarity.Text.Trim(), 2000);
            _app.JacsVendor = chkJacsVendor.Checked;
            int years = int.TryParse(txtYearsExperience.Text.Trim(), out int y) ? y : 0;
            int months = int.TryParse(txtMonthsExperience.Text.Trim(), out int m) ? m : 0;
            _app.MonthsExperience = (years * 12) + months;
            _app.LastModifiedDate = DateTime.UtcNow;
            _app.LastModifiedByUserID = IntranetUserId;

            new ApplicationController(_hostSettings).UpdateApplication(_app);
        }

        protected void valExperience_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !string.IsNullOrEmpty(txtYearsExperience.Text.Trim()) || !string.IsNullOrEmpty(txtMonthsExperience.Text.Trim());
        }
    }
}
