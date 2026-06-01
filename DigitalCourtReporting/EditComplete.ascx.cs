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
    public partial class EditComplete : DigitalCourtReportingModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        #endregion

        #region Methods
        public EditComplete()
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
                        lnkCancel.NavigateUrl = string.Format("{0}/searchText/{1}/cid/{2}", CompleteUrl, SearchText, CountyId);
                        if (ProceedingId != Null.NullInteger)
                        {
                            var ctl = new ProceedingController();
                            ProceedingListItem proceeding = ctl.GetProceedingListItem(ProceedingId);

                            {
                                if (proceeding.RequestedDate != Null.NullDate)
                                    txtRequestedDate.Text = proceeding.RequestedDate.ToString("MMMM dd, yyyy");
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
                                    txtNotification.Text = notification.Description;
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
                var ctl = new ProceedingController();
                Proceeding proceeding=ctl.GetProceeding(ProceedingId);
                if (proceeding != null)
                {
                        ctl.DeleteCompletedRecords(ProceedingId);
                        proceeding.CA = false;
                        proceeding.Closed = false;
                        proceeding.Paid = false;
                        ctl.UpdateProceeding(proceeding);
                    string returnUrl = string.Format("{0}/searchText/{1}/cid/{2}", CompleteUrl, SearchText, CountyId);
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
        #endregion
    }
}