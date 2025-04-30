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
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Cryptography;
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
    public partial class EditAccounting : DigitalCourtReportingModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public EditAccounting()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Implement your edit logic for your module
                if (!Page.IsPostBack)
                {
                    lnkCancel.NavigateUrl = _navigationManager.NavigateURL();
                    try
                    {

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
                                var aCtl = new AudioController();
                                Audio audio = aCtl.GetAudiosByProceeding(proceeding.ProceedingID).FirstOrDefault();
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
                var ctl = new AccountController();
                var pCtl = new ProceedingController();
                Account account = ctl.GetAccountByProceeding(ProceedingId) ?? new Account { ProceedingID= ProceedingId };
                account.CheckNumber = txtCheckMo.Text;
                account.ReceivedBy = txtReceivedBy.Text;
                account.Notes = txtAccountingNotes.Text;
                account.LastModifiedByID = UserId;
                account.LastModifiedDate = DateTime.Now;
                if (DateTime.TryParse(txtPaymentReceived.Text, out DateTime paymentDate))
                    account.PaymentDate = paymentDate.ToString();
                Decimal.TryParse(txtPaymentAmount.Text, out Decimal amount);
                account.Payment = amount;
                if (account.AccountID > 0)
                    ctl.UpdateAccount(account);
                else
                {
                    account.CreatedDate = DateTime.Now;
                    account.CreatedByID = UserId;
                    ctl.CreateAccount(account);
                }
                Proceeding proceeding = pCtl.GetProceeding(ProceedingId);
                proceeding.Paid = true;
                proceeding.LastModifiedDate = DateTime.Now;
                proceeding.LastModifiedByID = UserId;
                pCtl.UpdateProceeding(proceeding);
                string returnUrl = string.Format("{0}/listtype/{1}/searchType/{2}/searchText/{3}/cid/{4}", AccountingUrl, (int)ListType, (int)SearchType, SearchText, CountyId);
                Response.Redirect(returnUrl, false);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}