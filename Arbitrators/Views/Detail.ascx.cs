/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using System.Text;
using tjc.Modules.Arbitrators.Components;

namespace tjc.Modules.Arbitrators.Views
{
    public partial class Detail : ArbitratorsModuleBase
    {
        private readonly ArbitratorApplicationController _appCtl;
        private readonly ArbitratorAttachmentController _attCtl;

        public Detail()
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
            if (ArbitratorApplicationId <= 0)
            {
                ShowNotFound();
                return;
            }

            var app = _appCtl.GetApplication(ArbitratorApplicationId);
            if (app == null)
            {
                ShowNotFound();
                return;
            }

            bool canReview = IsEditable;
            cmdApprove.Visible = canReview;
            cmdDeny.Visible = canReview;
            txtStatusNotes.Enabled = canReview;

            ltHeading.Text = string.Format("<h2>{0} Arbitrator Application #{1} <span class=\"badge {2}\">{3}</span></h2>",
                System.Web.HttpUtility.HtmlEncode(app.FullName), app.ArbitratorApplicationID, StatusBadgeClass(app.Status), app.Status);

            ltAddress.Text = System.Web.HttpUtility.HtmlEncode(app.Address);
            ltPhone.Text = System.Web.HttpUtility.HtmlEncode(app.Phone) +
                (string.IsNullOrEmpty(app.Fax) ? string.Empty : " / " + System.Web.HttpUtility.HtmlEncode(app.Fax));

            if (!string.IsNullOrEmpty(app.Email))
            {
                lnkEmail.Text = app.Email;
                lnkEmail.NavigateUrl = "mailto:" + app.Email;
            }

            ltWebsite.Text = System.Web.HttpUtility.HtmlEncode(app.Website);
            ltPreferredAreas.Text = System.Web.HttpUtility.HtmlEncode(app.PreferredAreas);

            chkBarMember.Checked = app.FloridaBarMember;
            chkCertifiedMediator.Checked = app.CertifiedMediator;
            chkCompletedTraining.Checked = app.CompletedArbitrationTraining;

            ltExperience.Text = System.Web.HttpUtility.HtmlEncode(app.ExperienceStatement).Replace("\n", "<br />");

            ltSigned.Text = string.Format("{0} ({1:MM/dd/yyyy})", System.Web.HttpUtility.HtmlEncode(app.SignedName), app.SignatureDate);
            ltSubmitted.Text = app.CreatedOnDate.ToString("MM/dd/yyyy h:mm tt");
            ltSubmittedIP.Text = System.Web.HttpUtility.HtmlEncode(app.SubmittedIP);

            txtStatusNotes.Text = app.StatusNotes;

            var attachments = _attCtl.GetAttachments(app.ArbitratorApplicationID).ToList();
            rptAttachments.DataSource = attachments;
            rptAttachments.DataBind();
            rptAttachments.Visible = attachments.Count > 0;
            ltNoAttachments.Visible = attachments.Count == 0;
        }

        private void ShowNotFound()
        {
            pnlApplication.Visible = false;
            pnlNotFound.Visible = true;
        }

        protected void cmdApprove_Click(object sender, EventArgs e)
        {
            ReviewApplication(ApplicationStatus.Approved);
        }

        protected void cmdDeny_Click(object sender, EventArgs e)
        {
            ReviewApplication(ApplicationStatus.Denied);
        }

        private void ReviewApplication(ApplicationStatus newStatus)
        {
            if (!IsEditable) return;

            var app = _appCtl.GetApplication(ArbitratorApplicationId);
            if (app == null) return;

            app.Status = newStatus;
            app.StatusNotes = txtStatusNotes.Text;
            app.ReviewedByUserId = UserId;
            app.ReviewedOnDate = DateTime.Now;
            app.LastModifiedByUserId = UserId;
            app.LastModifiedOnDate = DateTime.Now;
            _appCtl.UpdateApplication(app);

            SendNotification(app);
            Response.Redirect(ListUrl, true);
        }

        private void SendNotification(ArbitratorApplication app)
        {
            string toEmail = _appCtl.GetApplicantNotificationEmail(app);
            if (string.IsNullOrEmpty(toEmail)) return;

            bool approved = app.Status == ApplicationStatus.Approved;
            string subject = approved
                ? "Your Arbitrator Application Has Been Approved"
                : "Your Arbitrator Application Has Been Denied";

            var sb = new StringBuilder();
            sb.AppendFormat("<p>Dear {0},</p>", System.Web.HttpUtility.HtmlEncode(app.FullName));
            sb.Append(approved
                ? "<p>Congratulations - your application to serve as an arbitrator for the 12<sup>th</sup> Judicial Circuit has been approved.</p>"
                : "<p>Your application to serve as an arbitrator for the 12<sup>th</sup> Judicial Circuit has been denied.</p>");

            if (!string.IsNullOrEmpty(app.StatusNotes))
                sb.AppendFormat("<p>{0}</p>", System.Web.HttpUtility.HtmlEncode(app.StatusNotes).Replace("\n", "<br />"));

            sb.Append("<p>Thank you.</p>");

            DotNetNuke.Services.Mail.Mail.SendEmail(NotificationFromEmail, toEmail, subject, sb.ToString());

            if (!string.IsNullOrEmpty(NotificationCcEmail))
            {
                sb.Insert(0, string.Format("<p><strong>The following notification was sent to {0} on {1}</strong></p><hr>",
                    System.Web.HttpUtility.HtmlEncode(toEmail), DateTime.Now.ToShortDateString()));
                DotNetNuke.Services.Mail.Mail.SendEmail(NotificationFromEmail, NotificationCcEmail, subject, sb.ToString());
            }
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
    }
}
