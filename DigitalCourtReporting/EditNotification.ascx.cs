/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Abstractions;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using tjc.Modules.DigitalCourtReporting.Components;

namespace tjc.Modules.DigitalCourtReporting
{
    /// -----------------------------------------------------------------------------
    /// <summary>   
    /// The Edit class is used to manage content
    /// 
    /// Typically your edit control would be used to create new content, or edit existing content within your module.
    /// The ControlKey for this control is "Edit", and is defined in the manifest (.dnn) file.
    /// 
    /// Because the control inherits from DigitalCourtReportingModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class EditNotification : DigitalCourtReportingModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        #endregion

        #region Methods
        public EditNotification()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Implement your edit logic for your module
                if (!Page.IsPostBack)
                {
                    try
                    {
                        lnkCancel.NavigateUrl = string.Format("{0}/searchText/{1}/cid/{2}", NotificationUrl, SearchText, CountyId);
                        if (ProceedingId != Null.NullInteger)
                        {
                            var ctl = new ProceedingController();
                            ProceedingListItem proceeding = ctl.GetProceedingListItem(ProceedingId);

                            {
                                if (proceeding.RequestedDate != Null.NullDate)
                                    txtRequestedDate.Text = proceeding.RequestedDate.ToString("MMMM dd, yyyy");
                                if (proceeding.Involvement == "State Attorney" | proceeding.Involvement == "Public Defender")
                                {
                                    rblNotification.Items[0].Value = "Uploaded";
                                    rblNotification.Items.RemoveAt(4);
                                    rblNotification.Items.RemoveAt(3);
                                    rblNotification.Items.RemoveAt(2);
                                    dvCalledBy.Visible = false;
                                    valCalledMailedBy.Visible = false;
                                    dvCalledPerson.Visible = false;
                                    valCalledPerson.Visible = false;
                                   dvDatePickedUp.Visible = false;
                                    dvRecipient.Visible = false;
                                }
                                txtAddress.Text = proceeding.Address;
                                txtCaseName.Text = proceeding.CaseName;
                                txtCaseNumber.Text = proceeding.CaseNumber;
                                txtCdPreference.Text = proceeding.CDPreference;
                                txtEmail.Text = proceeding.Email;
                                txtPresidingJudge.Text = proceeding.Judge;
                                txtFax.Text = proceeding.Fax;
                                txtCaseInvolvment.Text = proceeding.Involvement;
                                txtJurisdiction.Text = proceeding.Jurisdiction;
                                txtPhone.Text = proceeding.Phone;
                                txtProceedingDate.Text = proceeding.ProceedingDate;
                                txtProceedingTime.Text = proceeding.ProceedingTime;
                                txtLocation.Text = proceeding.Location;
                                txtRequestorName.Text = proceeding.Requestor;
                                txtProceddingType.Text = proceeding.ProceedingType;
                                ltNotes.Text = proceeding.Instructions;
                                txtCityStateZip.Text = string.Format("{0}, {1} {2}", proceeding.City, proceeding.State, proceeding.Zip);
                                var aCtl = new AccountController();
                                Account account = aCtl.GetAccountByProceeding(proceeding.ProceedingID);
                                if (account != null)
                                {
                                    txtPaymentReceived.Text = account.PaymentDate;
                                    txtCheckMo.Text = account.CheckNumber;
                                    txtPaymentAmount.Text = string.Format("{0:c}", account.Payment);
                                    txtReceivedBy.Text = account.ReceivedBy;
                                    txtAccountingNotes.Text = account.Notes;
                                }
                                else
                                    fsAccounting.Visible = false;
                                var adCtl = new AudioController();
                                Audio audio = adCtl.GetAudiosByProceeding(proceeding.ProceedingID).FirstOrDefault();
                                if (audio != null)
                                {
                                    txtJuvenile.Text = audio.Juvenile;
                                    txtIndigentCertAttach.Text = audio.Indigence;
                                    txtCdType.Text = audio.CDType;
                                    txtProcessedBy.Text = audio.Employee;
                                    txtTrackingNumber.Text = audio.Tracking;
                                    if (audio.CDBurnDate.HasValue)
                                        txtDateBurned.Text = audio.CDBurnDate.Value.ToShortDateString();
                                    txtTotalMinutes.Text = audio.TotalMinutes.ToString();
                                    txtCDProvided.Text = audio.CDCount.ToString();
                                    txtNotesDCR.Text = audio.Notes;
                                }
                                else
                                    fsDCR.Visible = false;
                                var nCtl = new NotificationController();
                                Notification notification = nCtl.GetNotificationByProceeding(proceeding.ProceedingID);
                                if (notification != null)
                                {
                                    txtCalledBy.Text = notification.Responsible;
                                    txtCalledPerson.Text = notification.PersonCalled;
                                    txtDateCalled.Text = notification.DateCalled;
                                    rblNotification.SelectedValue = notification.Description;
                                    txtDatePickedUp.Text = notification.PickupDate;
                                    txtRecipient.Text = notification.ReceivedBy;
                                    txtCourtAdminNotes.Text = notification.Notes;
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        Exceptions.ProcessModuleLoadException(this, exc);
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
                var ctl = new NotificationController();
                var pCtl = new ProceedingController();
                Notification notification = ctl.GetNotificationByProceeding(ProceedingId);
                Proceeding proceeding = pCtl.GetProceeding(ProceedingId);
                if (proceeding != null)
                {
                    if (notification != null && notification.DeliveryID != Null.NullInteger)
                    {
                        notification.ReceivedBy = txtRecipient.Text;
                        notification.PickupDate = txtDatePickedUp.Text;
                        ctl.UpdateNotification(notification);
                    }
                    else
                    {
                        notification = new Notification
                        {
                            ProceedingID = ProceedingId,
                            Description = rblNotification.SelectedValue,
                            DateCalled = txtDateCalled.Text,
                            Responsible = txtCalledBy.Text,
                            PersonCalled = txtCalledPerson.Text,
                            ReceivedBy = txtRecipient.Text,
                            PickupDate = txtDatePickedUp.Text,
                            Notes = txtCourtAdminNotes.Text
                        };
                        ctl.CreateNotification(notification);
                    }

                    if (txtDatePickedUp.Text != "" | rblNotification.SelectedValue == "Mailed" | rblNotification.SelectedValue == "Delivered" | rblNotification.SelectedValue == "Inter-Officed" | rblNotification.SelectedValue == "Uploaded")
                        proceeding.Closed = true;
                    else
                        proceeding.Closed = false;
                    pCtl.UpdateProceeding(proceeding);
                    string returnUrl = string.Format("{0}/searchText/{1}/cid/{2}", NotificationUrl, SearchText, CountyId);
                    Response.Redirect(returnUrl, false);
                }
                else
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Proceeding Not Found", "Could not retrieve procedding record", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void valCalledPerson_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (rblNotification.SelectedValue == "Called" && string.IsNullOrEmpty(txtCalledPerson.Text))
            {
                args.IsValid = false;
                valCalledPerson.ErrorMessage = "Person Called is required when Notification is Called.";
            }
            else
            {
                args.IsValid = true;
            }
        }
        #endregion
    }
}