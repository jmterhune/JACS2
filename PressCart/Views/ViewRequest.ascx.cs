/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using tjc.Modules.PressCart.Components;

namespace tjc.Modules.PressCart
{
    /// <summary>
    /// One presentation cart request in full, with the handling status and notes that
    /// users with module edit permission can change, and a delete.
    /// </summary>
    public partial class ViewRequest : PressCartModuleBase
    {
        private Components.PressCart _request;

        /// <summary>Exposed to the markup for the status badge in the heading.</summary>
        protected string StatusBadge
        {
            get { return SubmissionStatus.BadgeClass(_request == null ? null : _request.Status); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lnkBack.NavigateUrl = ListUrl;
                _request = new PressCartController(_hostSettings).GetRequest(PressCartIdParam);

                if (_request == null)
                {
                    pnlRequest.Visible = false;
                    pnlMissing.Visible = true;
                    return;
                }

                pnlManage.Visible = CanManage;

                if (!IsPostBack)
                {
                    BindRequest();
                }
                else
                {
                    // The badge and heading are rebound on every postback so they
                    // reflect a status that was just saved.
                    BindHeading();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdSaveStatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CanManage || _request == null) return;

                bool saved = new PressCartController(_hostSettings)
                    .SetStatus(_request.PressCartId, drpStatus.SelectedValue, txtNotes.Text.Trim(), UserId);

                if (!saved)
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this,
                        "That request has since been deleted, so the status was not saved.",
                        DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
                    return;
                }

                Response.Redirect(RequestUrl(_request.PressCartId), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CanManage || _request == null) return;

                new PressCartController(_hostSettings).DeleteRequest(_request.PressCartId);
                Response.Redirect(ListUrl, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindRequest()
        {
            BindHeading();

            ltRequestorName.Text = Encode(_request.RequestorName);
            ltEmail.Text = MailTo(_request.Email);
            ltPhone.Text = Encode(_request.Phone);

            ltContactName.Text = Encode(_request.ContactName);
            ltContactEmail.Text = MailTo(_request.ContactEmail);
            ltContactPhone.Text = Encode(_request.ContactPhone);

            ltCourtDate.Text = Encode(_request.CourtDate);
            ltCourtTime.Text = Encode(_request.CourtTime);
            ltCounty.Text = Encode(_request.County);
            ltJudge.Text = Encode(_request.Judge);
            ltCourthouse.Text = Encode(_request.Courthouse);
            ltCourtroom.Text = Encode(_request.Courtroom);

            // Only some Sarasota divisions ask for this, so null or zero both mean
            // "not asked" rather than "zero days".
            phTrialLength.Visible = _request.TrialLength.GetValueOrDefault() > 0;
            ltTrialLength.Text = _request.TrialLength.GetValueOrDefault().ToString();

            ltCartType.Text = Encode(_request.CartType);
            ltTraining.Text = _request.TrainingRequired.GetValueOrDefault() ? "Yes" : "No";
            // Special instructions come from a multi-line box, so its line breaks have
            // to survive being rendered into HTML.
            ltComments.Text = Encode(_request.Comments).Replace("\n", "<br />");

            BindEquipment();

            drpStatus.Items.Clear();
            foreach (string status in SubmissionStatus.All)
            {
                drpStatus.Items.Add(new ListItem(status, status));
            }
            ListItem current = drpStatus.Items.FindByValue(_request.Status ?? SubmissionStatus.New);
            if (current != null) current.Selected = true;

            txtNotes.Text = _request.StatusNotes;
        }

        /// <summary>
        /// The per-item equipment flags predate the cart-type question and the current
        /// form no longer sets them, so the section is shown only for the older rows
        /// that still carry one.
        /// </summary>
        private void BindEquipment()
        {
            var flagged = new List<string>();
            if (_request.TelevisionCombo == true) flagged.Add("Television combo");
            if (_request.Laptop == true) flagged.Add("Laptop");
            if (_request.Projector == true) flagged.Add("Projector");
            if (_request.DVD == true) flagged.Add("DVD");
            if (_request.VCR == true) flagged.Add("VCR");
            if (_request.Cassette == true) flagged.Add("Cassette");
            if (_request.DocumentCamera == true) flagged.Add("Document camera");
            if (_request.XRay == true) flagged.Add("X-ray");
            if (_request.Zoom == true) flagged.Add("Zoom");

            pnlEquipment.Visible = flagged.Count > 0;
            ltEquipment.Text = Encode(string.Join(", ", flagged));
        }

        private void BindHeading()
        {
            ltRequestId.Text = _request.PressCartId.ToString();
            ltStatus.Text = Encode(_request.Status);
            ltSubmitted.Text = _request.CreatedOnDate.HasValue
                ? _request.CreatedOnDate.Value.ToString("MMMM d, yyyy 'at' h:mm tt")
                : "(date not recorded)";
            ltStatusChanged.Text = StatusChangedText();
        }

        /// <summary>
        /// "Last updated by ... on ..." when someone has touched the status. The user id
        /// is an account on this intranet site, so it resolves against this portal.
        /// </summary>
        private string StatusChangedText()
        {
            if (!_request.StatusChangedOnDate.HasValue) return string.Empty;

            string who = "someone";
            if (_request.StatusChangedByUserId.HasValue)
            {
                UserInfo user = UserController.GetUserById(PortalId, _request.StatusChangedByUserId.Value);
                if (user != null) who = user.DisplayName;
            }

            return string.Format("Last updated by {0} on {1:MMMM d, yyyy 'at' h:mm tt}.",
                Encode(who), _request.StatusChangedOnDate.Value);
        }

        private static string MailTo(string email)
        {
            return string.IsNullOrEmpty(email)
                ? string.Empty
                : string.Format("<a href=\"mailto:{0}\">{0}</a>", Encode(email));
        }

        private static string Encode(string value)
        {
            return HttpUtility.HtmlEncode(value ?? string.Empty);
        }
    }
}
