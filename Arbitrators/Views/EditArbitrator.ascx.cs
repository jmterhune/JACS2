/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using tjc.Modules.Arbitrators.Components;

namespace tjc.Modules.Arbitrators.Views
{
    /// <summary>
    /// Edits the content of an arbitrator record (contact details, areas, qualifications, status). Approving or
    /// denying a pending application, which emails the applicant, stays on the Detail view; this page sends no email.
    /// </summary>
    public partial class EditArbitrator : ArbitratorsModuleBase
    {
        private readonly ArbitratorApplicationController _appCtl;

        public EditArbitrator()
        {
            _appCtl = new ArbitratorApplicationController(_hostSettings);
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

                if (!CanEditRecords)
                {
                    pnlEdit.Visible = false;
                    pnlNotAuthorized.Visible = true;
                    return;
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
            var app = ArbitratorApplicationId > 0 ? _appCtl.GetApplication(ArbitratorApplicationId) : null;
            if (app == null)
            {
                ShowNotFound();
                return;
            }

            ltHeading.Text = string.Format("<h2>Edit Arbitrator #{0} <span class=\"badge {1}\">{2}</span></h2>",
                app.ArbitratorApplicationID, StatusBadgeClass(app.Status), app.Status);

            foreach (ApplicationStatus status in Enum.GetValues(typeof(ApplicationStatus)))
                ddlStatus.Items.Add(new System.Web.UI.WebControls.ListItem(status.ToString(), ((int)status).ToString()));

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
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CanEditRecords) return;

                var app = ArbitratorApplicationId > 0 ? _appCtl.GetApplication(ArbitratorApplicationId) : null;
                if (app == null)
                {
                    ShowNotFound();
                    return;
                }

                var errors = Validate();
                if (errors.Count > 0)
                {
                    ltErrors.Text = "<ul class=\"mb-0\"><li>" + string.Join("</li><li>", errors) + "</li></ul>";
                    pnlErrors.Visible = true;
                    return;
                }

                var newStatus = (ApplicationStatus)int.Parse(ddlStatus.SelectedValue);
                bool statusChanged = newStatus != app.Status;

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
                _appCtl.UpdateApplication(app);

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
            return errors;
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
