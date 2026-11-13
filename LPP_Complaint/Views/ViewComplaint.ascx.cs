/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using System;
using System.Web;
using System.Web.UI.WebControls;
using tjc.Modules.LPP_Complaint.Components;

namespace tjc.Modules.LPP_Complaint
{
    /// <summary>
    /// One LPP complaint in full, with the handling status and notes that users with
    /// module edit permission can change, and a delete.
    /// </summary>
    public partial class ViewComplaint : LPP_ComplaintModuleBase
    {
        private LppComplaint _complaint;

        /// <summary>Exposed to the markup for the status badge in the heading.</summary>
        protected string StatusBadge
        {
            get { return SubmissionStatus.BadgeClass(_complaint == null ? null : _complaint.Status); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lnkBack.NavigateUrl = ListUrl;
                _complaint = new LppComplaintController(_hostSettings).GetComplaint(ComplaintIdParam);

                if (_complaint == null)
                {
                    pnlComplaint.Visible = false;
                    pnlMissing.Visible = true;
                    return;
                }

                pnlManage.Visible = CanManage;

                if (!IsPostBack)
                {
                    BindComplaint();
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
                if (!CanManage || _complaint == null) return;

                bool saved = new LppComplaintController(_hostSettings)
                    .SetStatus(_complaint.LppComplaintId, drpStatus.SelectedValue, txtNotes.Text.Trim(), UserId);

                if (!saved)
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this,
                        "That complaint has since been deleted, so the status was not saved.",
                        DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning);
                    return;
                }

                Response.Redirect(ComplaintUrl(_complaint.LppComplaintId), false);
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
                if (!CanManage || _complaint == null) return;

                new LppComplaintController(_hostSettings).DeleteComplaint(_complaint.LppComplaintId);
                Response.Redirect(ListUrl, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindComplaint()
        {
            BindHeading();

            ltReferrerName.Text = Encode(_complaint.ReferrerName);
            ltReferrerEmail.Text = MailTo(_complaint.ReferrerEmail);
            ltReferrerAddress.Text = Address(_complaint.ReferrerAddress, _complaint.ReferrerCity,
                _complaint.ReferrerState, _complaint.ReferrerZip);
            ltReferrerPhone.Text = Encode(_complaint.ReferrerPhone);
            ltRelationship.Text = Encode(_complaint.Relationship);

            ltAttorneyName.Text = Encode(_complaint.AttorneyName);
            ltBarNumber.Text = Encode(_complaint.AttorneyBarNumber);
            ltAttorneyAddress.Text = Address(_complaint.AttorneyAddress, _complaint.AttorneyCity,
                _complaint.AttorneyState, _complaint.AttorneyZip);
            ltAttorneyPhone.Text = Encode(_complaint.AttorneyPhone);
            ltAttorneyEmail.Text = MailTo(_complaint.AttorneyEmail);

            bool hasNatures = _complaint.Natures != null && _complaint.Natures.Count > 0;
            pnlNoNatures.Visible = !hasNatures;
            if (hasNatures)
            {
                rptNatures.DataSource = _complaint.Natures;
                rptNatures.DataBind();
            }

            ltViolationDate.Text = Encode(_complaint.ViolationDate);
            ltCounty.Text = Encode(_complaint.County);
            // The description is typed into a multi-line box, so its line breaks have to
            // survive being rendered into HTML.
            ltIncident.Text = Encode(_complaint.IncidentDescription).Replace("\n", "<br />");
            ltSigned.Text = Encode(_complaint.SignedBy);

            drpStatus.Items.Clear();
            foreach (string status in SubmissionStatus.All)
            {
                drpStatus.Items.Add(new ListItem(status, status));
            }
            ListItem current = drpStatus.Items.FindByValue(_complaint.Status ?? SubmissionStatus.New);
            if (current != null) current.Selected = true;

            txtNotes.Text = _complaint.StatusNotes;
        }

        private void BindHeading()
        {
            ltComplaintId.Text = _complaint.LppComplaintId.ToString();
            ltStatus.Text = Encode(_complaint.Status);
            ltSubmitted.Text = _complaint.CreatedOnDate.ToString("MMMM d, yyyy 'at' h:mm tt");
            ltStatusChanged.Text = StatusChangedText();
        }

        /// <summary>
        /// "Last updated by ... on ..." when someone has touched the status. The user id
        /// is an account on this intranet site, so it resolves against this portal.
        /// </summary>
        private string StatusChangedText()
        {
            if (!_complaint.StatusChangedOnDate.HasValue) return string.Empty;

            string who = "someone";
            if (_complaint.StatusChangedByUserId.HasValue)
            {
                UserInfo user = UserController.GetUserById(PortalId, _complaint.StatusChangedByUserId.Value);
                if (user != null) who = user.DisplayName;
            }

            return string.Format("Last updated by {0} on {1:MMMM d, yyyy 'at' h:mm tt}.",
                Encode(who), _complaint.StatusChangedOnDate.Value);
        }

        private static string Address(string street, string city, string state, string zip)
        {
            string cityLine = string.Format("{0}, {1} {2}", city, state, zip).Trim(' ', ',');
            if (string.IsNullOrEmpty(street)) return Encode(cityLine);
            if (string.IsNullOrEmpty(cityLine)) return Encode(street);
            return Encode(street) + "<br />" + Encode(cityLine);
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
