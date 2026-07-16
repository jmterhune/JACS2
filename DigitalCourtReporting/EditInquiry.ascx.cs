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
using System.Text;
using tjc.Modules.DigitalCourtReporting.Components;
using tjc.Modules.Globals;

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
    public partial class EditInquiry : DigitalCourtReportingModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        #endregion

        #region Methods
        public EditInquiry()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void SendEmails(Proceeding proceeding)
        {
            string mailTo = "";
            string[] mailtolist;
            string jurisdiction = GetCountyName(proceeding.JurisdictionID);
            string location = GetCountyName(proceeding.CountyID);
            if (string.IsNullOrEmpty(location))
                location = "Sarasota";
            switch (location)
            {
                case "DeSoto":
                    mailTo = DeSotoReporterEmail;
                    break;
                case "Manatee":
                    mailTo = ManateeReporterEmail;
                    break;
                default:
                    mailTo = SarasotaReporterEmail;
                    break;
            }
            mailtolist = mailTo.Split(';');
            if (mailtolist.Length > 1)
            {
                foreach (var s in mailtolist)
                    DotNetNuke.Services.Mail.Mail.SendEmail(mailTo, mailTo, s, jurisdiction + " Reporting Request", EmailBody(proceeding, location));
            }
            else
                DotNetNuke.Services.Mail.Mail.SendEmail(mailTo, mailTo, mailTo, jurisdiction + " Reporting Request", EmailBody(proceeding, location));
        }
        private string GetCountyName(int countyId)
        {
            var ctl = new CountyController();
            County county = ctl.GetCounty(CountyId);
            if (county != null)
                return county.CountyName;
            return "";
        }
        private string EmailBody(Proceeding proceeding, string location)
        {
            StringBuilder message = new StringBuilder();
            message.Append("<b>Court Proceedings Reporting Request</b> <br />");
            message.Append("<br />************************************************<br />");
            message.Append("<b>Requestor Information</b> <br />");
            message.Append("<br />************************************************<br />");
            message.Append("<b>Representing:</b>" + proceeding.Involvement + " <br />");
            message.Append("<b>Requestor:</b>" + proceeding.Requestor + " <br />");
            message.Append("<b>Address:</b>" + proceeding.Address + " <br />");
            message.Append("<b>City, State Zip:</b>" + proceeding.City + "," + proceeding.State + " " + proceeding.Zip + " <br />");
            message.Append("<b>Phone:</b>" + proceeding.Phone + " <br />");
            message.Append("<b>Email Address:</b>" + proceeding.Email + " <br />");
            message.Append("<br />************************************************<br />");
            message.Append("<b>Proceeding Information</b> <br />");
            message.Append("<br />************************************************<br />");
            message.Append("<b>Case Name:</b>" + proceeding.CaseName + " <br />");
            message.Append("<b>Case Number:</b>" + proceeding.CaseNumber + " <br />");
            message.Append("<b>Presiding Judge:</b>" + proceeding.Judge + " <br />");
            message.Append("<b>Date of Proceeing:</b>" + proceeding.ProceedingDate + " <br />");
            message.Append("<b>Time of Proceeding:</b>" + proceeding.ProceedingTime + " <br />");
            message.Append("<b>Location of Proceeding:</b>" + location + " <br />");
            message.Append("<b>Type of Proceeding:</b>" + proceeding.ProceedingType + " <br />");
            message.Append("<br />************************************************<br />");
            message.Append("<b>Special Instructions</b> <br />");
            message.Append("<br />************************************************<br />");
            message.Append(proceeding.Instructions + " <br />");
            return message.ToString();
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
                Proceeding proceeding = ctl.GetProceeding(ProceedingId);
                if (proceeding != null)
                {
                    proceeding.IsInquiry = false;
                    ctl.UpdateProceeding(proceeding);
                    string returnUrl = string.Format("{0}/searchText/{1}/cid/{2}", InquiryUrl,  SearchText, CountyId);
                    Response.Redirect(returnUrl, false);
                }
                else
                {
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                        "Swal.fire({ title: '" + System.Web.HttpUtility.JavaScriptStringEncode("Proceeding Not Found") + "', html: '" + System.Web.HttpUtility.JavaScriptStringEncode("Could not retrieve procedding record") + "', icon: 'error', confirmButtonText: 'OK' });", true);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdEmail_Click(object sender, EventArgs e)
        {
            try
            {
                var ctl = new ProceedingController();
                Proceeding proceeding = ctl.GetProceeding(ProceedingId);
                if (proceeding != null)
                {
                    proceeding.Closed = true;
                    proceeding.Paid = true;
                    proceeding.CA = true;
                    ctl.UpdateProceeding(proceeding);
                    SendEmails(proceeding);
                    string returnUrl = string.Format("{0}/searchText/{1}/cid/{2}", InquiryUrl, SearchText, CountyId);
                    Response.Redirect(returnUrl, false);
                }
                else
                {
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                        "Swal.fire({ title: '" + System.Web.HttpUtility.JavaScriptStringEncode("Proceeding Not Found") + "', html: '" + System.Web.HttpUtility.JavaScriptStringEncode("Could not retrieve procedding record") + "', icon: 'error', confirmButtonText: 'OK' });", true);
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