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
    public partial class EditDcr : DigitalCourtReportingModuleBase
    {
        #region Properties
        private readonly INavigationManager _navigationManager;
        #endregion

        #region Methods
        public EditDcr()
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
                                if (proceeding.Involvement == "State Attorney" | proceeding.Involvement == "Public Defender")
                                {
                                    rblClerkCertAttach.SelectedValue = "N/A";
                                    rblCourOrderAttach.SelectedValue= "N/A";
                                    dvCourOrderAttach.Visible = false;
                                    dvClerkCertAttach.Visible = false;
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
                var aCtl = new AudioController();
                Audio audio = aCtl.GetAudiosByProceeding(ProceedingId).FirstOrDefault() ?? new Audio { ProceedingID = ProceedingId };
                audio.Juvenile = rblCourOrderAttach.SelectedValue;
                audio.Indigence = rblClerkCertAttach.SelectedValue;
                audio.CDType = rblCDType.SelectedValue;
                audio.Employee = txtProcessedBy.Text;
                audio.Tracking = txtTrackingNumber.Text;
                if (DateTime.TryParse(txtDateBurned.Text, out DateTime dateBurned))
                    audio.CDBurnDate = dateBurned;
                if (Int32.TryParse(txtTotalMinutes.Text, out int totalMinutes))
                    audio.TotalMinutes = totalMinutes;
                if (Int32.TryParse(txtCdsProvided.Text, out int cdCount))
                    audio.CDCount = cdCount;
                audio.Notes = txtDCRNotes.Text;
                if (audio.AudioID > 0)
                    aCtl.UpdateAudio(audio);
                else
                    aCtl.CreateAudio(audio);
                var pCtl = new ProceedingController();
                Proceeding proceeding = pCtl.GetProceeding(ProceedingId);
                proceeding.CA = true;
                pCtl.UpdateProceeding(proceeding);
                string returnUrl = string.Format("{0}/searchText/{1}/cid/{2}", DCRUrl, SearchText, CountyId);
                Response.Redirect(returnUrl, false);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
     #endregion
    }
}