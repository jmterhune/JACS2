/*
' Copyright (c) 2026  Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using tjc.Modules.CourtRegistry.Components;

namespace tjc.Modules.CourtRegistry
{
    /// <summary>
    /// Court administration's queue of attorneys asking to come off the registry.
    /// Approving archives their application for the requested fiscal year and later,
    /// which is what drops them from the JAC export; denying leaves them on the
    /// registry. Either way the attorney is emailed.
    /// </summary>
    public partial class Withdrawals : CourtRegistryModuleBase
    {
        private readonly IHostSettings _hostSettings;

        public Withdrawals()
        {
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
        }

        /// <summary>Base URL of the application detail screen, used by the grid links.</summary>
        public string ApplicationUrl
        {
            get { return EditUrl("app"); }
        }

        private bool IsApprover
        {
            get
            {
                if (string.IsNullOrEmpty(ApproverUsername) || UserId <= 0)
                    return false;
                return UserInfo.Username.IndexOf(ApproverUsername, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        private int StatusFilter
        {
            get
            {
                int status;
                return int.TryParse(drpStatus.SelectedValue, out status) ? status : (int)WithdrawalStatus.Pending;
            }
        }

        #region Event Handlers
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    BindStatusList();
                    BindList();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void drpStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindList();
        }

        protected void rptWithdrawals_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "review")
            {
                return;
            }
            int withdrawalId;
            if (!int.TryParse(e.CommandArgument.ToString(), out withdrawalId) || withdrawalId <= 0)
            {
                return;
            }
            ShowRequest(withdrawalId);
        }

        protected void cmdApprove_Click(object sender, EventArgs e)
        {
            try
            {
                WithdrawalListItem request = GetSelectedPendingRequest();
                if (request == null)
                {
                    return;
                }
                DateTime effective;
                if (!DateTime.TryParse(txtEffectiveDate.Text, out effective))
                {
                    effective = DateTime.Today;
                }
                var ctl = new WithdrawalController(_hostSettings);
                ctl.Approve(request.WithdrawalId, UserId, txtReviewNote.Text.Trim(), effective);
                NotifyAttorney(request, true, txtReviewNote.Text.Trim(), effective);
                Notify(string.Format("{0} has been removed from the registry effective {1}.",
                    request.AttorneyName, effective.ToShortDateString()), "success");
                BindList();
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
                Notify("The request could not be approved: " + exc.Message, "error");
            }
        }

        protected void cmdDeny_Click(object sender, EventArgs e)
        {
            try
            {
                WithdrawalListItem request = GetSelectedPendingRequest();
                if (request == null)
                {
                    return;
                }
                string note = txtReviewNote.Text.Trim();
                if (note.Length == 0)
                {
                    Notify("Enter a note explaining why the request is denied; the attorney is sent that note.", "warning");
                    ShowRequest(request.WithdrawalId);
                    return;
                }
                var ctl = new WithdrawalController(_hostSettings);
                ctl.Deny(request.WithdrawalId, UserId, note);
                NotifyAttorney(request, false, note, null);
                Notify(string.Format("The request from {0} has been denied.", request.AttorneyName), "success");
                BindList();
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
                Notify("The request could not be denied: " + exc.Message, "error");
            }
        }
        #endregion

        #region Methods
        private void BindStatusList()
        {
            foreach (var status in Enumerations.GetValues<WithdrawalStatus>())
            {
                drpStatus.Items.Add(new ListItem(Enumerations.GetEnumDescription(status), ((int)status).ToString()));
            }
            drpStatus.Items.Insert(0, new ListItem("Every Status", "-1"));
            drpStatus.SelectedValue = ((int)WithdrawalStatus.Pending).ToString();
        }

        private void BindList()
        {
            var ctl = new WithdrawalController(_hostSettings);
            List<WithdrawalListItem> items = ctl.GetWithdrawalRequests(StatusFilter).ToList();
            rptWithdrawals.DataSource = items;
            rptWithdrawals.DataBind();
            pnlEmpty.Visible = items.Count == 0;

            int pending = ctl.GetPendingCount();
            ltPendingCount.Text = pending == 0
                ? "<span class='badge bg-secondary'>No requests waiting</span>"
                : string.Format("<span class='badge bg-warning text-dark'>{0} request{1} waiting</span>",
                    pending, pending == 1 ? string.Empty : "s");

            if (!IsApprover)
            {
                ltMessage.Text = "<div class='alert alert-info'>You can review these requests, but only the approver account can approve or deny them.</div>";
            }
        }

        /// <summary>Fills the review modal and opens it.</summary>
        private void ShowRequest(int withdrawalId)
        {
            var ctl = new WithdrawalController(_hostSettings);
            WithdrawalListItem request = ctl.GetWithdrawalRequest(withdrawalId);
            if (request == null)
            {
                return;
            }

            hdWithdrawalId.Value = request.WithdrawalId.ToString();
            ltName.Text = HttpUtility.HtmlEncode(request.AttorneyName);
            ltBarNumber.Text = request.BarNumber.ToString();
            ltRequested.Text = request.DateRequested.ToString("g");
            ltApplication.Text = request.ApplicationId.HasValue && request.ApplicationId.Value > 0
                ? string.Format("#{0} for fiscal year {1}", request.ApplicationId.Value, request.PeriodYear)
                : "None on file";
            ltSignature.Text = HttpUtility.HtmlEncode(request.Signature);
            ltAcknowledged.Text = request.Acknowledged
                ? "<span class='badge bg-success'>Yes</span>"
                : "<span class='badge bg-danger'>No</span>";
            ltReason.Text = string.IsNullOrEmpty(request.Reason)
                ? "<em class='text-muted'>No reason given</em>"
                : HttpUtility.HtmlEncode(request.Reason).Replace("\n", "<br />");
            ltJacCodes.Text = BuildJacCodeList(ctl, request);

            bool pending = request.IsPending;
            pnlDecision.Visible = pending && IsApprover;
            cmdApprove.Visible = pending && IsApprover;
            cmdDeny.Visible = pending && IsApprover;
            pnlReviewed.Visible = !pending;
            if (!pending)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("<strong>{0}</strong>", request.StatusName);
                if (request.DateReviewed.HasValue)
                {
                    sb.AppendFormat(" on {0}", request.DateReviewed.Value.ToShortDateString());
                }
                if (request.EffectiveDate.HasValue)
                {
                    sb.AppendFormat(", effective {0}", request.EffectiveDate.Value.ToShortDateString());
                }
                if (!string.IsNullOrEmpty(request.ReviewNote))
                {
                    sb.AppendFormat("<br />{0}", HttpUtility.HtmlEncode(request.ReviewNote));
                }
                ltReviewedSummary.Text = sb.ToString();
            }
            if (pending)
            {
                txtEffectiveDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtReviewNote.Text = string.Empty;
            }

            ltModalScript.Text = "<script>(function(){function s(){if(typeof bootstrap!=='undefined'&&bootstrap.Modal){bootstrap.Modal.getOrCreateInstance(document.getElementById('withdrawalModal')).show();}else if(typeof jQuery!=='undefined'){jQuery('#withdrawalModal').modal('show');}}if(document.readyState!=='loading'){s();}else{document.addEventListener('DOMContentLoaded',s);}})();</script>";
        }

        private string BuildJacCodeList(WithdrawalController ctl, WithdrawalListItem request)
        {
            int applicationId = request.ApplicationId ?? 0;
            var codes = ctl.GetAffectedJacCodes(applicationId).ToList();
            if (codes.Count == 0)
            {
                return "<p class='text-muted'>No case categories on file for this application.</p>";
            }
            var sb = new StringBuilder();
            sb.Append("<table class=\"table table-sm table-bordered\"><thead><tr><th>Case Type</th><th>Category</th><th>JAC Code</th><th>Location</th><th>Status</th></tr></thead><tbody>");
            foreach (var code in codes)
            {
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>",
                    HttpUtility.HtmlEncode(code.CaseTypeName),
                    HttpUtility.HtmlEncode(code.Category),
                    code.JacCodeID,
                    HttpUtility.HtmlEncode(code.LocationName),
                    ((CodeStatus)code.Status));
            }
            sb.Append("</tbody></table>");
            return sb.ToString();
        }

        /// <summary>Re-reads the request behind the modal and checks it can still be acted on.</summary>
        private WithdrawalListItem GetSelectedPendingRequest()
        {
            if (!IsApprover)
            {
                Notify("You do not have approval rights.", "error");
                return null;
            }
            int withdrawalId;
            if (!int.TryParse(hdWithdrawalId.Value, out withdrawalId) || withdrawalId <= 0)
            {
                return null;
            }
            var ctl = new WithdrawalController(_hostSettings);
            WithdrawalListItem request = ctl.GetWithdrawalRequest(withdrawalId);
            if (request == null)
            {
                Notify("That request could no longer be found.", "error");
                return null;
            }
            if (!request.IsPending)
            {
                Notify("That request has already been " + request.StatusName.ToLower() + ".", "warning");
                BindList();
                return null;
            }
            return request;
        }

        private void NotifyAttorney(WithdrawalListItem request, bool approved, string note, DateTime? effective)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return;
            }
            var setCtl = new SettingController(_hostSettings);
            Setting appSetting = setCtl.GetSettings().FirstOrDefault();

            var sb = new StringBuilder();
            sb.AppendFormat("<p>Dear {0} {1},</p>", HttpUtility.HtmlEncode(request.FirstName), HttpUtility.HtmlEncode(request.LastName));
            if (approved)
            {
                sb.AppendFormat("<p>Court Administration has approved your request to be removed from the 12<sup>th</sup> Circuit court appointed registry, effective {0}. You will not be considered for new appointments.</p>",
                    (effective ?? DateTime.Today).ToShortDateString());
                sb.AppendFormat("<p><strong>{0}</strong></p>", WithdrawalText.Caveat);
            }
            else
            {
                sb.Append("<p>Court Administration has denied your request to be removed from the 12<sup>th</sup> Circuit court appointed registry. You remain on the registry and will continue to receive appointments.</p>");
            }
            if (!string.IsNullOrEmpty(note))
            {
                sb.AppendFormat("<p><strong>Note from Court Administration:</strong><br />{0}</p>", HttpUtility.HtmlEncode(note));
            }

            string subject = approved
                ? "Your Court Registry Removal Request Has Been Approved"
                : "Your Court Registry Removal Request Has Been Denied";
            DotNetNuke.Services.Mail.Mail.SendEmail("cr.noreply@jud12.flcourts.org", request.Email, subject, sb.ToString());

            string copyTo = appSetting == null ? string.Empty : appSetting.ContactEmail;
            if (!string.IsNullOrEmpty(copyTo))
            {
                sb.Insert(0, string.Format("<p><strong>The following notification was sent on {0}</strong></p><hr>", DateTime.Now.ToShortDateString()));
                DotNetNuke.Services.Mail.Mail.SendEmail("cr.noreply@jud12.flcourts.org", copyTo,
                    "Registry Withdrawal Notification", sb.ToString());
            }
        }

        private void Notify(string message, string type)
        {
            System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                "new Noty({ text: '" + HttpUtility.JavaScriptStringEncode(message) + "', type: '" + type + "', timeout: 4500, layout: 'topRight', theme: 'mint' }).show();", true);
        }
        #endregion
    }
}
