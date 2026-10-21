using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using tjc.Modules.Transcriptionist.Components;

namespace tjc.Modules.Transcriptionist
{
    /// <summary>
    /// Edits the content of an application (contact details, qualifications, experience records, status) or deletes it.
    /// Approving or rejecting a pending application, which emails the applicant, stays on the detail page; this page
    /// sends no email. Experience records are handled by Handlers/ExperienceApi.ashx.
    /// </summary>
    public partial class ApplicationEdit : TranscriptionistModuleBase
    {
        private static readonly Regex EmailPattern = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.Compiled);

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

                lnkList.NavigateUrl = EditUrl("list");
                lnkNotFoundBack.NavigateUrl = EditUrl("list");

                _app = TranscriptionistID > 0 ? new ApplicationController(_hostSettings).GetApplication(TranscriptionistID) : null;
                if (_app == null)
                {
                    pnlEdit.Visible = false;
                    pnlNotFound.Visible = true;
                    return;
                }

                if (!IsPostBack)
                {
                    PopulateFields();
                    BindAttachments();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PopulateFields()
        {
            ltHeading.Text = $"<h2>Edit Transcriptionist Application #{_app.TranscriptionistID} <span class=\"badge {StatusBadgeClass(_app.Status)}\">{_app.Status}</span></h2>";

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

            ddlStatus.SelectedValue = _app.Status;
            txtRejectionReason.Text = _app.RejectionReason;
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
            try
            {
                var errors = Validate();
                if (errors.Count > 0)
                {
                    ltErrors.Text = "<ul class=\"mb-0\"><li>" + string.Join("</li><li>", errors) + "</li></ul>";
                    pnlErrors.Visible = true;
                    return;
                }

                ApplyFields();
                ApplyStatus();
                new ApplicationController(_hostSettings).UpdateApplication(_app);
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

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            new ApplicationController(_hostSettings).DeleteApplication(TranscriptionistID);
            Response.Redirect(EditUrl("list"), true);
        }

        private List<string> Validate()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(txtFirstName.Text)) errors.Add("First Name is required.");
            if (string.IsNullOrWhiteSpace(txtLastName.Text)) errors.Add("Last Name is required.");

            string email = txtEmailAddress.Text.Trim();
            if (email.Length > 0 && !EmailPattern.IsMatch(email))
                errors.Add("Email Address is not a valid email address.");

            if (string.IsNullOrWhiteSpace(txtYearsExperience.Text) && string.IsNullOrWhiteSpace(txtMonthsExperience.Text))
                errors.Add("Enter the experience in years or months.");

            if (ddlStatus.SelectedValue == "Rejected" && string.IsNullOrWhiteSpace(txtRejectionReason.Text))
                errors.Add("A rejection reason is required when the status is Rejected.");

            return errors;
        }

        private void ApplyFields()
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
        }

        // Status is kept as dates on the record; moving to another status stamps its date and clears the others.
        private void ApplyStatus()
        {
            string selected = ddlStatus.SelectedValue;
            string reason = Limit(txtRejectionReason.Text.Trim(), 4000);

            if (selected == _app.Status)
            {
                if (selected == "Rejected") _app.RejectionReason = reason;
                return;
            }

            _app.ApprovedDate = null;
            _app.ApprovedByUserID = null;
            _app.RejectedDate = null;
            _app.RejectedByUserID = null;
            _app.RejectionReason = null;

            if (selected == "Approved")
            {
                _app.ApprovedDate = DateTime.UtcNow;
                _app.ApprovedByUserID = IntranetUserId;
            }
            else if (selected == "Rejected")
            {
                _app.RejectedDate = DateTime.UtcNow;
                _app.RejectedByUserID = IntranetUserId;
                _app.RejectionReason = reason;
            }
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
