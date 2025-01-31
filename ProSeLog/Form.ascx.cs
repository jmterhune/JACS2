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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Tokens;
using DotNetNuke.UI.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.SqlTypes;
using System.Web.UI.WebControls;
using tjc.Modules.ProSeLog.Components;

namespace tjc.Modules.ProSeLog
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from ProSeLogModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Form : ProSeLogModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion
        #region Methods
        public Form()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private string GetCaseNumber()
        {
            return String.Format("{0}-{1}-{2}-{3}", drpCountyLetter.SelectedValue, txtCaseYear.Text, txtCaseType.Text, txtCaseSequence.Text.PadLeft(6,'0')).ToUpper();
        }
        private void PopulateDropDowns()
        {
            var cCtl=new CaseTypeController();
            var lCtl=new CountyController();
            var tCtl=new ContactController();
            drpCaseType.DataSource = cCtl.GetCaseTypes();
            drpCaseType.DataBind();
            drpInitialContact.DataSource=tCtl.GetContacts();
            drpInitialContact.DataBind();
            drpLocation.DataSource=lCtl.GetCounties();
            drpLocation.DataBind();
        }
        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (IsAdmin)
                    {
                        lnkManage.Visible = true;
                        lnkManage.NavigateUrl = CaseTypeListUrl;
                    }
                    PopulateDropDowns();
                    for (var i = 1; i <= 12; i++)
                    {
                        string month = DateTime.Parse(i.ToString() + "/1/2007").ToString("MMM");
                        drpMonths.Items.Add(new ListItem(month, i.ToString()));
                    }
                    int year = 2004;
                    while (year <= DateTime.Now.Year + 1)
                    {
                        drpYear.Items.Add(new ListItem(year.ToString()));
                        year += 1;
                    }
                    drpMonths.SelectedValue = DateTime.Now.Month.ToString();
                    drpYear.SelectedValue = DateTime.Now.Year.ToString();
                    if (HistoryId > 0)
                    {
                        var ctl = new HistoryController();
                        History history = ctl.GetHistory(HistoryId);
                        if (history != null)
                        {
                            txtCaseName.Text = history.CaseName;
                            string[] caseNumber = history.CaseNumber.Split('-');
                            drpCountyLetter.SelectedValue = caseNumber[0];
                            txtCaseYear.Text = caseNumber[1];
                            txtCaseType.Text = caseNumber[2];
                            txtCaseSequence.Text = caseNumber[3];
                            txtPetitioner.Text = history.Petitioner;
                            txtPhone.Text = history.Phone;
                            txtRespondent.Text = history.Respondent;
                            drpCaseType.SelectedValue = history.CaseTypeID.ToString();
                            drpInitialContact.SelectedValue = history.ContactID.ToString();
                            drpLocation.SelectedValue = history.CountyID.ToString();
                            hdReceivedDate.Value = history.ReceivedDate.ToString();
                            if (!IsCopy)
                            { 
                                if (history.ResolutionDate.HasValue)
                                txtResolutionDate.Text = history.ResolutionDate.Value.ToShortDateString();
                                drpMonths.SelectedValue = history.MonthNumber;
                                drpYear.SelectedValue = history.Year.ToString();
                                chkAppointedPro.Checked = history.AppointedPro;
                                chkAssistedForm.Checked = history.AssistedForms;
                                chkAssistedProcedures.Checked = history.AssistedProcedures;
                                chkNeedsLetter.Checked = history.NeedsLetter;
                                chkOther.Checked = history.Other;
                                chkPreparedOrder.Checked = history.PreparedOrder;
                                chkProvidedForms.Checked = history.ProvidedForms;
                                chkReferralGmMag.Checked = history.ReferralGmMag;
                                chkReferralOther.Checked = history.ReferralOther;
                                chkSetFinalHearing.Checked = history.SetFinalHearing;
                                chkSetOtherHearing.Checked = history.SetOtherHearing;
                                chkSetFinalHearing.Enabled = false;
                                chkSetOtherHearing.Enabled = false;
                                chkReferralOther.Enabled = false;
                                chkReferralGmMag.Enabled = false;
                                chkPreparedOrder.Enabled = false;
                                chkNeedsLetter.Enabled = false;
                                chkOther.Enabled = false;
                                chkProvidedForms.Enabled = false;
                                chkAssistedProcedures.Enabled = false;
                                chkAssistedForm.Enabled = false;
                                chkAppointedPro.Enabled = false;
                                drpYear.Enabled = false;
                                drpMonths.Enabled = false;
                                txtRespondent.Enabled = false;
                                drpCaseType.Enabled = false;
                                drpInitialContact.Enabled = false;
                                drpLocation.Enabled = false;
                                txtCaseName.Enabled = false;
                                drpCountyLetter.Enabled = false;
                                txtDefendantSuffix.Enabled = false;
                                drpYear.Enabled = false;
                                txtCaseType.Enabled = false;
                                txtCaseYear.Enabled = false;
                                txtCaseSequence.Enabled = false;
                                txtPetitioner.Enabled = false;
                                txtPhone.Enabled = false;
                            }
                        }
                        else
                        {
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this,"Unable to Retrieve the Requested Record. Please contact the Help Desk.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                        }
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            var ctl = new HistoryController();
            History history = new History();
            if (HistoryId <= 0 | IsCopy)
            {
                {
                    history.AppointedPro = chkAppointedPro.Checked;
                    history.AssistedForms = chkAssistedForm.Checked;
                    history.AssistedProcedures = chkAssistedProcedures.Checked;
                    history.CaseName = txtCaseName.Text;
                    history.CaseNumber = GetCaseNumber();
                    if (drpCaseType.SelectedIndex > 0)
                        history.CaseTypeID = Int32.Parse(drpCaseType.SelectedValue);
                    if (drpInitialContact.SelectedIndex > 0)
                        history.ContactID = Int32.Parse(drpInitialContact.SelectedValue);
                    if (drpLocation.SelectedIndex > 0)
                        history.CountyID = Int32.Parse(drpLocation.SelectedValue);
                    history.Month = drpMonths.SelectedItem.Text;
                    history.MonthNumber = drpMonths.SelectedValue;
                    history.NeedsLetter = chkNeedsLetter.Checked;
                    history.Other = chkOther.Checked;
                    history.Petitioner = txtPetitioner.Text;
                    history.Phone = txtPhone.Text;
                    history.PreparedOrder = chkPreparedOrder.Checked;
                    history.ProvidedForms = chkProvidedForms.Checked;
                    history.ReferralGmMag = chkReferralGmMag.Checked;
                    if (!string.IsNullOrEmpty(txtResolutionDate.Text))
                        history.ResolutionDate = DateTime.Parse(txtResolutionDate.Text);
                    history.ReferralOther = chkReferralOther.Checked;
                    history.Respondent = txtRespondent.Text;
                    history.SetFinalHearing = chkSetFinalHearing.Checked;
                    history.SetOtherHearing = chkSetOtherHearing.Checked;
                    history.Year = Int32.Parse(drpYear.SelectedValue);
                    history.ReceivedDate = DateTime.Now;
                    history.CreatedDate = DateTime.Now;
                    history.LastModifiedByID = UserId;
                    history.LastModifiedDate = DateTime.Now;
                    history.CreatedByID = UserId;
                    ctl.CreateHistory(history);
                }
            }
            else
            {
                history = ctl.GetHistory(HistoryId);
                history.LastModifiedByID = UserId;
                history.LastModifiedDate = DateTime.Now;
                if (!string.IsNullOrEmpty(txtResolutionDate.Text))
                    history.ResolutionDate = DateTime.Parse(txtResolutionDate.Text);
                ctl.UpdateHistory(history);
            }
                Response.Redirect(EditUrl("case", history.CaseNumber, "case-list"));
        }
        #endregion
    }
}