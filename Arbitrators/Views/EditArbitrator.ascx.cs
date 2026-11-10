/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using tjc.Modules.Arbitrators.Components;

namespace tjc.Modules.Arbitrators.Views
{
    /// <summary>
    /// Edits the content of an arbitrator record (contact details, areas, qualifications, status), or adds one by hand
    /// when opened with new=1. Approving or denying a pending application, which emails the applicant, stays on the
    /// Detail view; this page sends no email.
    /// </summary>
    public partial class EditArbitrator : ArbitratorsModuleBase
    {
        private const int MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB, same limit as the public application form

        private readonly ArbitratorApplicationController _appCtl;
        private readonly ArbitratorAttachmentController _attCtl;

        public EditArbitrator()
        {
            _appCtl = new ArbitratorApplicationController(_hostSettings);
            _attCtl = new ArbitratorAttachmentController(_hostSettings);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lnkCancel.NavigateUrl = ListUrl;
                lnkNotFoundBack.NavigateUrl = ListUrl;
                lnkNotAuthorizedBack.NavigateUrl = ListUrl;

                // asp:CheckBox renders <span class="form-check"><input/><label/></span>; Bootstrap 5 needs these on the inner elements.
                foreach (var chk in new[] { chkBarMember, chkCertifiedMediator, chkCompletedTraining })
                {
                    chk.InputAttributes["class"] = "form-check-input";
                    chk.LabelAttributes["class"] = "form-check-label";
                }

                if (!IsPostBack)
                    PopulateForm();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PopulateForm()
        {
            foreach (ApplicationStatus status in Enum.GetValues(typeof(ApplicationStatus)))
                ddlStatus.Items.Add(new System.Web.UI.WebControls.ListItem(status.ToString(), ((int)status).ToString()));

            if (IsNewArbitrator)
            {
                // Adding a record by hand: blank form, starts as Pending so it still goes through review.
                ltHeading.Text = "<h2>Add Arbitrator</h2>";
                ddlStatus.SelectedValue = ((int)ApplicationStatus.Pending).ToString();
                return;
            }

            var app = ArbitratorApplicationId > 0 ? _appCtl.GetApplication(ArbitratorApplicationId) : null;
            if (app == null)
            {
                ShowNotFound();
                return;
            }

            ltHeading.Text = string.Format("<h2>Edit Arbitrator #{0} <span class=\"badge {1}\">{2}</span></h2>",
                app.ArbitratorApplicationID, StatusBadgeClass(app.Status), app.Status);

            txtFirstName.Text = app.FirstName;
            txtMiddleName.Text = app.MiddleName;
            txtLastName.Text = app.LastName;
            txtSuffix.Text = app.Suffix;
            txtAddress.Text = app.Address;
            txtPhone.Text = app.Phone;
            txtFax.Text = app.Fax;
            txtEmail.Text = app.Email;
            txtWebsite.Text = app.Website;
            txtPreferredAreas.Text = app.PreferredAreas;
            chkBarMember.Checked = app.FloridaBarMember;
            chkCertifiedMediator.Checked = app.CertifiedMediator;
            chkCompletedTraining.Checked = app.CompletedArbitrationTraining;
            txtExperience.Text = app.ExperienceStatement;
            ddlStatus.SelectedValue = ((int)app.Status).ToString();
            txtStatusNotes.Text = app.StatusNotes;

            rptAttachments.DataSource = _attCtl.GetAttachments(app.ArbitratorApplicationID).ToList();
            rptAttachments.DataBind();
            rptAttachments.Visible = rptAttachments.Items.Count > 0;
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool isNew = IsNewArbitrator;
                ArbitratorApplication app;
                if (isNew)
                {
                    // Entered by staff, not submitted through the public site: there is no applicant portal account
                    // (UserId -1, same as the imported records) and no applicant signature.
                    var now = DateTime.Now;
                    string enteredBy = string.IsNullOrWhiteSpace(UserInfo.DisplayName) ? "staff" : UserInfo.DisplayName;
                    string signedName = "Entered manually by " + enteredBy;
                    app = new ArbitratorApplication
                    {
                        UserId = -1,
                        CreatedByUserId = UserId,
                        SignedName = signedName.Length > 200 ? signedName.Substring(0, 200) : signedName,
                        SignatureDate = now,
                        SubmittedIP = null,
                        CreatedOnDate = now,
                        Status = ApplicationStatus.Pending
                    };
                }
                else
                {
                    app = ArbitratorApplicationId > 0 ? _appCtl.GetApplication(ArbitratorApplicationId) : null;
                    if (app == null)
                    {
                        ShowNotFound();
                        return;
                    }
                }

                var errors = Validate();
                if (isNew && errors.Count == 0 && EmailAlreadyOnFile(txtEmail.Text.Trim()))
                    errors.Add("An application with this email address already exists. Edit that record instead of adding a new one.");
                if (errors.Count > 0)
                {
                    ltErrors.Text = "<ul class=\"mb-0\"><li>" + string.Join("</li><li>", errors) + "</li></ul>";
                    pnlErrors.Visible = true;
                    return;
                }

                var newStatus = (ApplicationStatus)int.Parse(ddlStatus.SelectedValue);
                // A new record that is saved as anything other than Pending counts as reviewed by the person adding it.
                bool statusChanged = isNew ? newStatus != ApplicationStatus.Pending : newStatus != app.Status;

                app.FirstName = txtFirstName.Text.Trim();
                app.MiddleName = NullIfEmpty(txtMiddleName.Text);
                app.LastName = txtLastName.Text.Trim();
                app.Suffix = NullIfEmpty(txtSuffix.Text);
                app.Address = txtAddress.Text.Trim();
                app.Phone = txtPhone.Text.Trim();
                app.Fax = NullIfEmpty(txtFax.Text);
                app.Email = txtEmail.Text.Trim();
                app.Website = NullIfEmpty(txtWebsite.Text);
                app.PreferredAreas = txtPreferredAreas.Text.Trim();
                app.FloridaBarMember = chkBarMember.Checked;
                app.CertifiedMediator = chkCertifiedMediator.Checked;
                app.CompletedArbitrationTraining = chkCompletedTraining.Checked;
                app.ExperienceStatement = txtExperience.Text.Trim();
                app.StatusNotes = NullIfEmpty(txtStatusNotes.Text);

                if (statusChanged)
                {
                    app.Status = newStatus;
                    app.ReviewedByUserId = UserId;
                    app.ReviewedOnDate = DateTime.Now;
                }

                app.LastModifiedByUserId = UserId;
                app.LastModifiedOnDate = DateTime.Now;
                if (isNew)
                    _appCtl.AddApplication(app);
                else
                    _appCtl.UpdateApplication(app);

                SaveUpload(app.ArbitratorApplicationID, fuTrainingCertificate, AttachmentType.TrainingCertificate);
                SaveUpload(app.ArbitratorApplicationID, fuBarCard, AttachmentType.BarCard);

                Response.Redirect(ListUrl, true);
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

        private bool EmailAlreadyOnFile(string email)
        {
            return _appCtl.GetApplications().Any(a =>
                a.Status != ApplicationStatus.Removed &&
                string.Equals((a.Email ?? string.Empty).Trim(), email, StringComparison.OrdinalIgnoreCase));
        }

        private List<string> Validate()
        {
            var errors = new List<string>();
            RequireText(errors, txtFirstName, "First Name");
            RequireText(errors, txtLastName, "Last Name");
            RequireText(errors, txtAddress, "Address");
            RequireText(errors, txtPhone, "Phone");
            RequireText(errors, txtEmail, "Email");
            RequireText(errors, txtPreferredAreas, "Preferred Areas");

            string email = txtEmail.Text.Trim();
            if (email.Length > 0)
            {
                try { new MailAddress(email); }
                catch (FormatException) { errors.Add("Email is not a valid email address."); }
            }

            CheckLength(errors, txtFirstName, 100, "First Name");
            CheckLength(errors, txtMiddleName, 100, "Middle Name");
            CheckLength(errors, txtLastName, 100, "Last Name");
            CheckLength(errors, txtSuffix, 20, "Suffix");
            CheckLength(errors, txtAddress, 500, "Address");
            CheckLength(errors, txtPhone, 30, "Phone");
            CheckLength(errors, txtFax, 30, "Fax");
            CheckLength(errors, txtEmail, 256, "Email");
            CheckLength(errors, txtWebsite, 300, "Website");
            CheckLength(errors, txtPreferredAreas, 500, "Preferred Areas");

            CheckUpload(errors, fuTrainingCertificate, "Training Certificate");
            CheckUpload(errors, fuBarCard, "Florida Bar Card");
            return errors;
        }

        /// <summary>An upload is optional, but if one is chosen it must be a PDF/JPG/PNG whose contents match its extension, 10MB or less.</summary>
        private static void CheckUpload(List<string> errors, System.Web.UI.WebControls.FileUpload upload, string label)
        {
            if (!upload.HasFile) return;

            if (upload.PostedFile.ContentLength <= 0 || upload.PostedFile.ContentLength > MaxFileSizeBytes)
            {
                errors.Add(label + " must be 10 MB or smaller.");
                return;
            }

            string extension = Path.GetExtension(upload.FileName).ToLowerInvariant();
            if (!FileSignature.MatchesExtension(upload.PostedFile.InputStream, extension))
                errors.Add(label + " must be a PDF, JPG or PNG file.");
        }

        private void SaveUpload(int arbitratorApplicationId, System.Web.UI.WebControls.FileUpload upload, AttachmentType type)
        {
            if (!upload.HasFile) return;

            var stream = upload.PostedFile.InputStream;
            // The stored Content-Type comes from the file's own signature, not the browser-supplied header.
            string contentType = FileSignature.Detect(stream) ?? upload.PostedFile.ContentType;

            byte[] data;
            using (var memory = new MemoryStream())
            {
                stream.Position = 0;
                stream.CopyTo(memory);
                data = memory.ToArray();
            }

            string fileName = Path.GetFileName(upload.FileName) ?? string.Empty;
            foreach (char c in Path.GetInvalidFileNameChars())
                fileName = fileName.Replace(c, '_');
            if (fileName.Length > 260) fileName = fileName.Substring(fileName.Length - 260);

            _attCtl.CreateAttachment(new ArbitratorAttachment
            {
                ArbitratorApplicationID = arbitratorApplicationId,
                AttachmentType = type,
                FileName = fileName,
                ContentType = contentType,
                FileSize = data.Length,
                FileData = data,
                UploadedOnDate = DateTime.Now
            });
        }

        private static void RequireText(List<string> errors, System.Web.UI.WebControls.TextBox box, string label)
        {
            if (string.IsNullOrWhiteSpace(box.Text))
                errors.Add(label + " is required.");
        }

        private static void CheckLength(List<string> errors, System.Web.UI.WebControls.TextBox box, int max, string label)
        {
            if (box.Text.Trim().Length > max)
                errors.Add(string.Format("{0} must be {1} characters or fewer.", label, max));
        }

        private static string NullIfEmpty(string value)
        {
            value = (value ?? string.Empty).Trim();
            return value.Length == 0 ? null : value;
        }

        private static string StatusBadgeClass(ApplicationStatus status)
        {
            switch (status)
            {
                case ApplicationStatus.Pending: return "bg-primary";
                case ApplicationStatus.Approved: return "bg-success";
                case ApplicationStatus.Denied: return "bg-warning text-dark";
                case ApplicationStatus.Removed: return "bg-dark";
                default: return "bg-secondary";
            }
        }

        private void ShowNotFound()
        {
            pnlEdit.Visible = false;
            pnlNotFound.Visible = true;
        }
    }
}
