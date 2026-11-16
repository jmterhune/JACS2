/*
' Copyright (c) 2017  12th Judicial Circuit
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/

using System;
using System.Text.RegularExpressions;
using tjc.Modules.JCID.Components;
using DotNetNuke.Services.Exceptions;
namespace tjc.Modules.JCID
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Review / edit a single JCID application: update the applicant's details and the workflow dates, mark it
    /// picked up, or reject it. Reads and writes the jud12 (public) database through the Jud12 connection.
    /// The applicant photo and the reject-correction link point back to the public site via the External URL
    /// module setting.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Edit : JCIDModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    lnkCancel.NavigateUrl = EditUrl();
                    if (RequestId > 0)
                    {
                        var ctl = new JCIDController(_hostSettings);
                        var objJCID = ctl.GetItem(RequestId);
                        if (objJCID != null)
                        {
                            if(objJCID.IsReplacement)
                                ltReplacementMessage.Visible= true;
                            txtDOB.Text = objJCID.BirthDate;
                            txtPhone.Text = objJCID.BusinessPhone;
                            txtCity.Text = objJCID.City;
                            txtEMail.Text = objJCID.Email;
                            txtRejectReason.Text = objJCID.RejectReason;
                            txtAddress.Text = objJCID.MailingAddress;
                            rblPhotoType.SelectedValue = objJCID.PhotoType;
                            rblPhotoTaken.SelectedValue = objJCID.PickupLocation;
                            drpRace.SelectedValue = objJCID.Race;
                            if (!string.IsNullOrEmpty(objJCID.PhotoUrl))
                            {
                                // The photo file lives on the jud12 site; build an absolute URL to it.
                                string photo = ExternalPhotoUrl(objJCID.PhotoUrl);
                                imgPhoto.ImageUrl = photo;
                                imgZoom.ImageUrl = photo;
                            }
                            txtFirstName.Text = objJCID.FirstName;
                            txtLastName.Text = objJCID.LastName;
                            rblSex.SelectedValue = objJCID.Gender;
                            txtSign.Text = objJCID.Signature;

                            // Decrypt and normalize to ###-##-#### so the jQuery mask accepts it
                            txtSSN.Text = FormatSsn(EncryptionHelper.DecryptSSN(objJCID.SSN));

                            txtState.Text = objJCID.State;
                            txtBarNumber.Text = objJCID.StateBarNumber;
                            txtZip.Text = objJCID.Zip;
                            if (objJCID.CriminalHistoryCompleted.HasValue)
                                txtCriminalHistoryCompleted.Text = objJCID.CriminalHistoryCompleted.Value.ToShortDateString();
                            if (objJCID.CardCreated.HasValue)
                                txtCardCreated.Text = objJCID.CardCreated.Value.ToShortDateString();
                            if (objJCID.ApplicantNotified.HasValue)
                                txtApplicantNotified.Text = objJCID.ApplicantNotified.Value.ToShortDateString();
                            if (objJCID.CompletedDate.HasValue)
                                txtPickedUp.Text = objJCID.CompletedDate.Value.ToShortDateString();

                            chkWaiver.Checked = objJCID.Waiver;
                            if (!string.IsNullOrEmpty(objJCID.CompletedBy))
                                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "This Application is Completed.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo);
                        }
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var ctl = new JCIDController(_hostSettings);
                JCIDInfo objJCID = ctl.GetItem(RequestId);
                objJCID.BirthDate = txtDOB.Text;
                objJCID.BusinessPhone = txtPhone.Text;
                objJCID.City = txtCity.Text;
                objJCID.Email = txtEMail.Text;
                objJCID.MailingAddress = txtAddress.Text;
                objJCID.PhotoType = rblPhotoType.SelectedValue;
                objJCID.Race = drpRace.SelectedValue;
                objJCID.PickupLocation = rblPhotoTaken.SelectedValue;
                objJCID.FirstName = txtFirstName.Text;
                objJCID.LastName = txtLastName.Text;
                objJCID.Gender = rblSex.SelectedValue;
                // Strip mask formatting before encrypting — store as ###-##-####
                objJCID.SSN = EncryptionHelper.EncryptSSN(FormatSsn(txtSSN.Text.Trim()));
                objJCID.State = txtState.Text;
                objJCID.StateBarNumber = txtBarNumber.Text;
                objJCID.Zip = txtZip.Text;
                DateTime.TryParse(txtCriminalHistoryCompleted.Text, out DateTime historyCompleted);
                if (historyCompleted> DateTime.MinValue)
                    objJCID.CriminalHistoryCompleted = historyCompleted;
                if (string.IsNullOrEmpty(txtCriminalHistoryCompleted.Text))
                    objJCID.CriminalHistoryCompleted = null;
                DateTime.TryParse(txtCardCreated.Text, out DateTime cardCompleted);
                if (cardCompleted > DateTime.MinValue)
                    objJCID.CardCreated = cardCompleted;
                if(string.IsNullOrEmpty(txtCardCreated.Text))
                    objJCID.CardCreated = null;
                DateTime.TryParse(txtApplicantNotified.Text, out DateTime notificationCompleted);
                if (notificationCompleted > DateTime.MinValue)
                    objJCID.ApplicantNotified = notificationCompleted;
                if (string.IsNullOrEmpty(txtApplicantNotified.Text))
                    objJCID.ApplicantNotified = null;
                DateTime.TryParse(txtPickedUp.Text, out DateTime pickedUp);
                if (pickedUp > DateTime.MinValue && (!objJCID.CompletedDate.HasValue || objJCID.CompletedDate.Value != pickedUp))
                {
                    objJCID.CompletedDate = pickedUp;
                    objJCID.CompletedBy = UserInfo.Username;
                }
                if(string.IsNullOrEmpty(txtPickedUp.Text))
                {
                    objJCID.CompletedDate = null;
                    objJCID.CompletedBy = null;
                }
                objJCID.Waiver = chkWaiver.Checked;
                ctl.UpdateItem(objJCID);
                Response.Redirect(EditUrl());
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdReject_Click(object sender, EventArgs e)
        {
            var ctl = new JCIDController(_hostSettings);
            JCIDInfo objJCID = ctl.GetItem(RequestId);
            objJCID.Rejected = true;
            objJCID.RejectReason = txtRejectReason.Text;
            ctl.UpdateItem(objJCID);
            string emailTo = txtEMail.Text;
            string emailFrom = "noreply.jcid@jud12.flcourts.org";
            string subject = "JCID Application Rejected";
            // The correction link opens the applicant form on the public (jud12) site. Only include it when the
            // External URL setting is configured, so the email never carries a broken link.
            string fixUrl = ExternalFormFixUrl(RequestId);
            string correction = fixUrl != null
                ? string.Format("<p>Please use the following link to correct your application.<br /><a href='{0}'>{0}</a></p>", fixUrl)
                : string.Empty;
            string body = string.Format("<p>Your application has been rejected for the following reason</p><p><strong>Reject:</strong>&nbsp;{0}</p>{1}", txtRejectReason.Text, correction);
            DotNetNuke.Services.Mail.Mail.SendEmail(emailFrom, emailTo, subject, body);
            Response.Redirect(EditUrl());
        }

        /// <summary>
        /// Normalizes an SSN to ###-##-#### format regardless of whether it arrives
        /// as 9 raw digits, already formatted, or with other separators.
        /// Returns the original string unchanged if it can't be normalized.
        /// </summary>
        private static string FormatSsn(string ssn)
        {
            if (string.IsNullOrEmpty(ssn))
                return ssn;

            // Strip everything except digits
            string digits = Regex.Replace(ssn, @"\D", "");

            if (digits.Length == 9)
                return $"{digits.Substring(0, 3)}-{digits.Substring(3, 2)}-{digits.Substring(5, 4)}";

            // Already formatted or unrecognized — return as-is
            return ssn;
        }
    }
}
