/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using System;
using System.Web;
using System.Web.UI.WebControls;
using tjc.Modules.InterpreterRequest.Components;

namespace tjc.Modules.InterpreterRequest
{
    /// <summary>
    /// One interpreter request in full, with the handling status and notes that
    /// users with module edit permission can change, and a delete.
    /// </summary>
    public partial class ViewRequest : InterpreterRequestModuleBase
    {
        private InterpreterRequestItem _request;

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
                _request = new InterpreterRequestItemController(_hostSettings).GetRequest(RequestIdParam);

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

                bool saved = new InterpreterRequestItemController(_hostSettings)
                    .SetStatus(_request.InterpreterRequestId, drpStatus.SelectedValue, txtNotes.Text.Trim(), UserId);

                if (!saved)
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this,
                        "That request has since been deleted, so the status was not saved.",
                        DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
                    return;
                }

                Response.Redirect(RequestUrl(_request.InterpreterRequestId), false);
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

                new InterpreterRequestItemController(_hostSettings).DeleteRequest(_request.InterpreterRequestId);
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

            ltLanguage.Text = Encode(_request.InterpreterLanguage);
            ltCaseNumber.Text = Encode(_request.CaseNumber);
            ltPersonNeeding.Text = Encode(_request.PersonNeeding);
            ltPersonName.Text = Encode(_request.PersonName);
            ltEventType.Text = Encode(_request.EventType);
            ltDateNeeded.Text = Encode(_request.DateNeeded);
            ltTimeNeeded.Text = Encode(_request.TimeNeeded);
            ltDuration.Text = Encode(_request.DurationNeeded);
            ltCounty.Text = Encode(_request.County);
            ltCourtroom.Text = Encode(_request.Courtroom);
            ltAppearanceMethod.Text = Encode(_request.AppearanceMethod);
            ltJudge.Text = Encode(_request.JudgeName);
            ltRequestorName.Text = Encode(_request.RequestorName);
            ltRequestorPhone.Text = Encode(_request.RequestorPhone);
            ltRequestorEmail.Text = string.IsNullOrEmpty(_request.RequestorEmail)
                ? string.Empty
                : string.Format("<a href=\"mailto:{0}\">{0}</a>", Encode(_request.RequestorEmail));
            ltIsAttorney.Text = _request.IsAttorney ? "Yes" : "No";

            drpStatus.Items.Clear();
            foreach (string status in SubmissionStatus.All)
            {
                drpStatus.Items.Add(new ListItem(status, status));
            }
            ListItem current = drpStatus.Items.FindByValue(_request.Status ?? SubmissionStatus.New);
            if (current != null) current.Selected = true;

            txtNotes.Text = _request.StatusNotes;
        }

        private void BindHeading()
        {
            ltRequestId.Text = _request.InterpreterRequestId.ToString();
            ltStatus.Text = Encode(_request.Status);
            ltSubmitted.Text = _request.CreatedOnDate.ToString("MMMM d, yyyy 'at' h:mm tt");
            ltFormLanguage.Text = _request.IsSpanishForm ? "Spanish" : "English";
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

        private static string Encode(string value)
        {
            return HttpUtility.HtmlEncode(value ?? string.Empty);
        }
    }
}
