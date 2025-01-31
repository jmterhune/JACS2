/*
' Copyright (c) 2023  12th Judicial Circuit
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
using DotNetNuke.Services.Localization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.MediationStatistics.Components;
namespace tjc.Modules.MediationStatistics
{
    /// -----------------------------------------------------------------------------
    /// <summary>   
    /// The Edit class is used to manage content
    /// 
    /// Typically your edit control would be used to create new content, or edit existing content within your module.
    /// The ControlKey for this control is "Edit", and is defined in the manifest (.dnn) file.
    /// 
    /// Because the control inherits from MediationStatisticsModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class EditCountyClaims : MediationStatisticsModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        private int _regionId = Null.NullInteger;
        private readonly GroupType _caseTypeGroup = GroupType.CountyClaims;
        private Case _currentCase;
        #endregion

        #region Properties
        public string PageTitle { get; set; }
        private int CurrentSessionIndex
        {
            get
            {
                if (ViewState["CurrentSessionIndex"] != null)
                    return Int32.Parse(ViewState["CurrentSessionIndex"].ToString());
                else
                    return Null.NullInteger;
            }
            set
            {
                ViewState["CurrentSessionIndex"] = value;
            }
        }
        private string RegionName
        {
            get
            {
                if (ViewState["RegionName"] != null)
                    return ViewState["RegionName"].ToString();
                else
                {
                    var ctl = new RegionController();
                    Region region = ctl.GetRegion(_regionId);
                    if (region != null)
                        return region.Description;
                }
                return "";
            }
            set
            {
                ViewState["RegionName"] = value;
            }
        }
        private string GroupName
        {
            get
            {
                if (ViewState["GroupName"] != null)
                    return ViewState["GroupName"].ToString();
                else
                {
                    var ctl = new GroupController();
                    Group group = ctl.GetGroup((int)_caseTypeGroup);
                    if (group != null)
                        return group.Description;
                }
                return "";
            }
            set
            {
                ViewState["GroupName"] = value;
            }
        }
        #endregion
        #region Methods
        public EditCountyClaims()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        public string GetAgreementType(string agreementType)
        {
            string returnValue;
            switch (agreementType)
            {
                case "F":
                    {
                        returnValue = "Full";
                        break;
                    }

                case "C":
                    {
                        returnValue = "Partial / Temporary";
                        break;
                    }

                default:
                    {
                        returnValue = "None";
                        break;
                    }
            }
            return returnValue;
        }
        public string GetAppearanceItems(string eventId)
        {
            string returnValue = "";
            var ctl = new AppearanceController();
            IEnumerable<Appearance> appearances = ctl.GetEventAppearances(Int32.Parse(eventId));
            foreach (Appearance appearance in appearances)
            {
                returnValue += string.Format("{0}; ", appearance.Description);
            }
            return returnValue;
        }
        private void PopulateCaseInformation()
        {
            txtBusinessName.Text = _currentCase.p1_business;
            txtFirstName.Text = _currentCase.p1_FirstName;
            txtLastName.Text = _currentCase.p1_LastName;
            txtBusinessName_p2.Text = _currentCase.p2_business;
            txtFirstName_p2.Text = _currentCase.p2_FirstName;
            txtLastName_p2.Text = _currentCase.p2_LastName;
            string delimiter;
            if (_currentCase.CaseNumber.Contains("-")) { delimiter = "-"; } else { delimiter = " "; }
            string[] caseNumber = _currentCase.CaseNumber.Split(char.Parse(delimiter));
            if (caseNumber.Length > 0)
            {
                int count = caseNumber.Length - 1;
                for (var i = 0; i <= count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            {
                                txtCaseYear.Text = caseNumber[i];
                                break;
                            }

                        case 1:
                            {
                                txtCaseType.Text = caseNumber[i];
                                break;
                            }

                        case 2:
                            {
                                txtCaseSequence.Text = caseNumber[i];
                                break;
                            }

                        case 3:
                            {
                                txtSuffix.Text = caseNumber[i];
                                break;
                            }
                    }
                }
            }
            if (_currentCase.CDSPNumber.Contains("-")) { delimiter = "-"; } else { delimiter = " "; }
            string[] cdspNumber = _currentCase.CDSPNumber.Split(char.Parse(delimiter));
            if (cdspNumber.Length > 0)
            {
                int count = cdspNumber.Length - 1;
                for (var i = 0; i <= count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            {
                                drpCDSPType.SelectedValue = cdspNumber[i];
                                break;
                            }

                        case 1:
                            {
                                txtCDSPYear.Text = cdspNumber[i];
                                break;
                            }

                        case 2:
                            {
                                txtCDSPNumber.Text = cdspNumber[i];
                                break;
                            }

                        case 3:
                            {
                                drpCountyLetter.SelectedValue = cdspNumber[i];
                                break;
                            }
                    }
                }
            }
            if (_currentCase.CaseSessions.Count() > 0)
            {
                if (CurrentSessionIndex == Null.NullInteger)
                    CurrentSessionIndex = 0;
                PopulateSessionInformation();
            }
            else
                AddNewSession();
        }
        private void PopulateSessionInformation()
        {
            ClearSession();
            var ctl = new AttorneyController();
            Session session = _currentCase.GetCurrentSession(CurrentSessionIndex);
            {
                hdSessionId.Value = session.SessionId.ToString();
                drpCaseType.SelectedIndex = -1;
                if (session.PrimaryCaseType.HasValue)
                    drpCaseType.SelectedValue = session.PrimaryCaseType.Value.ToString();
                txtMediationDate.Text = "";
                if (session.MediationDate.HasValue)
                    txtMediationDate.Text = session.MediationDate.Value.ToString("yyyy-MM-dd");
                txtOrderReferral.Text = "";
                if (session.ReferralDate.HasValue)
                    txtOrderReferral.Text = session.ReferralDate.Value.ToString("yyyy-MM-dd");
                chkTelephoneSession.Checked = session.HeldByPhone;
                txtComments.Text = session.Comment;
                chkProSePlaintiff.Checked = session.p1_ProSe;
                chkProSeDefendant.Checked = session.p2_ProSe;
                chkOTSC.Checked = false;
                if (session.OTS.HasValue)
                    chkOTSC.Checked = session.OTS.Value;
                chkPlaintiffFta.Checked = false;
                if (session.p1_FTA.HasValue)
                    chkPlaintiffFta.Checked = session.p2_FTA.Value;
                chkDefendantFta.Checked= false;
                if (session.p2_FTA.HasValue)
                    chkDefendantFta.Checked = session.p2_FTA.Value;
                if (session.p1_AttorneyId.HasValue)
                {
                    hdPlaintiffAttorneyId.Value = session.p1_AttorneyId.ToString();
                    Attorney PlaintiffAttorney = ctl.GetAttorney(session.p1_AttorneyId.Value);
                    if (PlaintiffAttorney != null)
                    {
                        txtPlaintiffName.Text = PlaintiffAttorney.FullName;
                        txtPlaintiffEmail.Text = PlaintiffAttorney.Email;
                        txtPlaintiffPhone.Text = PlaintiffAttorney.Phone;
                        txtPlaintiffExtension.Text = PlaintiffAttorney.Extension;
                    }
                    else
                    {
                        txtPlaintiffName.Text = string.Empty;
                        txtPlaintiffEmail.Text = string.Empty;
                        txtPlaintiffPhone.Text = string.Empty;
                        txtPlaintiffExtension.Text = string.Empty;
                    }
                }
                if (session.p2_AttorneyId.HasValue)
                {
                    hdDefendantAttorneyId.Value = session.p2_AttorneyId.ToString();
                    Attorney DefendantAttorney = ctl.GetAttorney(session.p2_AttorneyId.Value);
                    if (DefendantAttorney != null)
                    {
                        txtDefendantName.Text = DefendantAttorney.FullName;
                        txtDefendantEmail.Text = DefendantAttorney.Email;
                        txtDefendantPhone.Text = DefendantAttorney.Phone;
                        txtDefendantExtension.Text = DefendantAttorney.Extension;
                    }
                    else
                    {
                        txtDefendantName.Text = string.Empty;
                        txtDefendantEmail.Text = string.Empty;
                        txtDefendantPhone.Text = string.Empty;
                        txtDefendantExtension.Text = string.Empty;
                    }
                }
                drpFeeAmount.SelectedValue = session.FeeAmount;
                drpDefendantFeesOwed.SelectedValue = session.p2_FeesOwed;
                drpDefendantFeesPaid.SelectedValue = session.p2_FeesPaid;
                drpPlaintiffFeesOwed.SelectedValue = session.p1_FeesOwed;
                drpPlaintiffFeesPaid.SelectedValue = session.p1_FeesPaid;
                chkFeeAgreementEntered.Checked = false;
                if (session.FeeAgreement.HasValue)
                    chkFeeAgreementEntered.Checked = session.FeeAgreement.Value;
                chkFeeJudgmentEntered.Checked=false;
                if (session.FeeJudgement.HasValue)
                    chkFeeJudgmentEntered.Checked = session.FeeJudgement.Value;
                chkDepartmentFeeWaiver.Checked = false;
                if (session.FeeWaiver.HasValue)
                    chkDepartmentFeeWaiver.Checked = session.FeeWaiver.Value;
                chkInterpreterRequested.Checked = false;
                if (session.Interpreter.HasValue)
                    chkInterpreterRequested.Checked = session.Interpreter.Value;
                chkArbitrationReferral.Checked = session.ArbitrationReferral;
                chkCircuitCivilReferal.Checked = session.CircuitCivilReferral;
                clsSecondaryIssues.Items.Clear();
                foreach (ListItem li in clsSecondaryIssues.Items)
                {
                    li.Selected = false;
                }
                foreach (Issue issue in session.SessionIssues)
                {
                    ListItem li = clsSecondaryIssues.Items.FindByValue(issue.IssueId.ToString());
                    li.Selected = true;
                }
            }
            PopulateEventInformation();
            UpdateNavigation();
        }
        private void PopulateEventInformation()
        {
            try
            {
                rptEvent.DataSource = _currentCase.GetCurrentSession(CurrentSessionIndex).SessionEvents;
            }
            catch
            {
                rptEvent.DataSource = new List<Event>();
            }

            rptEvent.DataBind();
        }
        private void InitializeDropDowns()
        {
            var ctlGroup = new GroupController();
            List<CaseType> lstCaseTypes = ctlGroup.GetCaseTypesByGroup((int)_caseTypeGroup).ToList();
            foreach (var c in lstCaseTypes)
            {
                ListItem listItem = new ListItem
                {
                    Text = c.Description,
                    Value = c.CaseTypeId.ToString()
                };
                if (!c.Active)
                    listItem.Attributes.Add("class", "med_invalid");
                drpCaseType.Items.Add(listItem);
            }
            drpCaseType.Items.Insert(0, new ListItem("<Select Case Type>", "-1"));
            IEnumerable<Issue> issues = ctlGroup.GetIssuesByGroup((int)_caseTypeGroup);
            if (issues != null && issues.Count() > 0)
            {
                clsSecondaryIssues.DataSource = issues;
                clsSecondaryIssues.DataTextField = "Description";
                clsSecondaryIssues.DataValueField = "IssueId";
                clsSecondaryIssues.DataBind();
            }
            else
            {
                fsSecondaryIssues.Visible = false;
            }
        }
        private void AddNewSession()
        {
            Session newSession = new Session
            {
                CreatedDate = DateTime.Now,
                CaseId = CaseID,
                CreatedById = UserId,
                LastModifiedById = UserId,
                LastModifiedDate = DateTime.Now
            };
            var ctl = new SessionController();
            ctl.CreateSession(newSession);
            _currentCase.CaseSessions.Append(newSession);
            CurrentSessionIndex = _currentCase.CaseSessions.Count() > 0 ? _currentCase.CaseSessions.Count() - 1 : 0;
        }
        private void ClearSession()
        {
            txtPlaintiffName.Text = string.Empty;
            txtPlaintiffEmail.Text = string.Empty;
            txtPlaintiffPhone.Text = string.Empty;
            txtPlaintiffExtension.Text = string.Empty;
            txtDefendantName.Text = string.Empty;
            txtDefendantEmail.Text = string.Empty;
            txtDefendantPhone.Text = string.Empty;
            txtDefendantExtension.Text = string.Empty;
            drpCaseType.SelectedIndex = 0;
            txtOrderReferral.Text = string.Empty;
            txtMediationDate.Text = string.Empty;
            drpFeeAmount.SelectedIndex = 0;
            drpPlaintiffFeesPaid.SelectedIndex = 0;
            drpDefendantFeesPaid.SelectedIndex = 0;
            drpPlaintiffFeesOwed.SelectedIndex = 0;
            drpDefendantFeesOwed.SelectedIndex = 0;
            chkProSeDefendant.Checked = false;
            chkProSePlaintiff.Checked = false;
            chkPlaintiffFta.Checked = false;
            chkDefendantFta.Checked = false;
            hdDefendantAttorneyId.Value = string.Empty;
            hdPlaintiffAttorneyId.Value = string.Empty;
            chkFeeJudgmentEntered.Checked = false;
            chkFeeAgreementEntered.Checked = false;
            chkInterpreterRequested.Checked = false;
            chkDepartmentFeeWaiver.Checked = false;
            chkTelephoneSession.Checked = false;
            chkCircuitCivilReferal.Checked = false;
            chkArbitrationReferral.Checked = false;
            chkOTSC.Checked = false;
            foreach (ListItem item in clsSecondaryIssues.Items)
            {
                item.Selected = false;
            }
        }
        private void UpdateNavigation()
        {
            int sessionCount = _currentCase.CaseSessions.Count();
            cmdPreviousSession.Enabled = true;
            cmdNextSession.Enabled = true;
            if (CurrentSessionIndex <= 0)
                cmdPreviousSession.Enabled = false;
            if (sessionCount <= 1)
                cmdNextSession.Enabled = false;
            if (CurrentSessionIndex >= sessionCount - 1)
                cmdNextSession.Enabled = false;
            if (sessionCount == 0)
                ltSessionInfo.Text = "";
            else
                ltSessionInfo.Text = " Session " + (CurrentSessionIndex + 1) + " of " + sessionCount;
        }
        private void FillCase()
        {
            var ctl = new CaseController();
            _currentCase.LastModifiedDate = DateTime.Now;
            _currentCase.p1_business = txtBusinessName.Text;
            _currentCase.p1_FirstName = txtFirstName.Text;
            _currentCase.p1_LastName = txtLastName.Text;
            _currentCase.p2_business = txtBusinessName_p2.Text;
            _currentCase.p2_FirstName = txtFirstName_p2.Text;
            _currentCase.p2_LastName = txtLastName_p2.Text;
            _currentCase.RegionId = _regionId;
            _currentCase.GroupId = (int)_caseTypeGroup;
            _currentCase.CaseNumber = Helper.GetCaseFormatted(txtCaseYear.Text.Trim(), txtCaseType.Text.Trim(), txtCaseSequence.Text.Trim(), txtSuffix.Text.Trim());
            _currentCase.CDSPNumber = Helper.GetCDSPFormatted(drpCDSPType.SelectedValue, txtCDSPYear.Text, txtCDSPNumber.Text, drpCountyLetter.SelectedValue);
            _currentCase.LastModifiedById = UserId;
            _currentCase.LastModifiedDate = DateTime.Now;
            if (CaseID <= 0)
            {
                _currentCase.CreatedById = UserId;
                _currentCase.CreatedDate = DateTime.Now;
                ctl.CreateCase(_currentCase);
                Response.Redirect(EditUrl("cid", _currentCase.CaseId.ToString(), "CountyClaims"), true);
            }
            else
            {
                ctl.UpdateCase(_currentCase);
            }
            var ctlSession = new SessionController();
            Session session = new Session();
            if (CurrentSessionIndex == Null.NullInteger)
            {
                session.CreatedById = UserId;
                session.CreatedDate = DateTime.Now;
                session.LastModifiedById = UserId;
                session.LastModifiedDate = DateTime.Now;
                CurrentSessionIndex = 0;
            }
            else
            {
                session = _currentCase.CaseSessions.ElementAt(CurrentSessionIndex);
            }
            if (_currentCase.CaseId >= 0)
            {
                if (!string.IsNullOrEmpty(hdPlaintiffAttorneyId.Value))
                    session.p1_AttorneyId = Int32.Parse(hdPlaintiffAttorneyId.Value);
                if (!string.IsNullOrEmpty(hdDefendantAttorneyId.Value))
                    session.p2_AttorneyId = Int32.Parse(hdDefendantAttorneyId.Value);
                if (drpCaseType.SelectedIndex > 0)
                    session.PrimaryCaseType = Int32.Parse(drpCaseType.SelectedValue);
                if (!string.IsNullOrEmpty(txtMediationDate.Text))
                    session.MediationDate = DateTime.Parse(txtMediationDate.Text);
                if (!string.IsNullOrEmpty(txtOrderReferral.Text))
                    session.ReferralDate = DateTime.Parse(txtOrderReferral.Text);
                session.FeeAgreement = chkFeeAgreementEntered.Checked;
                session.FeeJudgement = chkFeeJudgmentEntered.Checked;
                session.FeeWaiver = chkDepartmentFeeWaiver.Checked;
                session.Interpreter = chkInterpreterRequested.Checked;
                session.Comment = txtComments.Text;
                session.LastModifiedById = UserId;
                session.LastModifiedDate = DateTime.Now;
                session.p1_ProSe = chkProSePlaintiff.Checked;
                session.p2_ProSe = chkProSeDefendant.Checked;
                session.p1_FTA = chkPlaintiffFta.Checked;
                session.p2_FTA = chkDefendantFta.Checked;
                session.OTS = chkOTSC.Checked;
                session.HeldByPhone = chkTelephoneSession.Checked;
                session.ArbitrationReferral = chkArbitrationReferral.Checked;
                session.CircuitCivilReferral = chkCircuitCivilReferal.Checked;
                session.FeeAmount = drpFeeAmount.SelectedValue;
                session.p1_FeesOwed = drpPlaintiffFeesOwed.SelectedValue;
                session.p1_FeesPaid = drpPlaintiffFeesPaid.SelectedValue;
                session.p2_FeesOwed = drpDefendantFeesOwed.SelectedValue;
                session.p2_FeesPaid = drpDefendantFeesPaid.SelectedValue;
            }
            if (session.SessionId > 0)
            {
                ctlSession.UpdateSession(session);
            }
            else
            {
                ctlSession.CreateSession(session);
            }
            hdSessionId.Value = session.SessionId.ToString();
            if (session.SessionId > 0)
            {
                ctlSession.DeleteAllSessionIssues(session.SessionId);
                foreach (ListItem li in clsSecondaryIssues.Items)
                {
                    if (li.Selected)
                    {
                        int issueId = Int32.Parse(li.Value);
                        SessionIssue sessionIssue = new SessionIssue
                        {
                            IssueId = issueId,
                            SessionId = session.SessionId,
                            CreatedById = UserId,
                            LastModifiedById = UserId,
                            LastModifiedDate = DateTime.Now,
                            CreatedDate = DateTime.Now
                        };
                        ctlSession.CreateSessionIssue(sessionIssue);
                    }
                }
            }
        }
        private void DeleteSession()
        {
            var ctl = new SessionController();
            ctl.DeleteSession(_currentCase.GetCurrentSession(CurrentSessionIndex));
            CurrentSessionIndex = 0;
            if (_currentCase.CaseSessions.Count() <= 1)
                _currentCase.CaseSessions.Append(new Session());
        }
        private void CheckExistingCase()
        {
            string caseNumber = Helper.GetCaseFormatted(txtCaseYear.Text.Trim(), txtCaseType.Text.Trim(), txtCaseSequence.Text.Trim(), txtSuffix.Text.Trim());
            string cdspNumber = Helper.GetCDSPFormatted(drpCDSPType.SelectedValue, txtCDSPYear.Text, txtCDSPNumber.Text, drpCountyLetter.SelectedValue);
            var ctl = new CaseController();
            if (string.IsNullOrEmpty(caseNumber) & string.IsNullOrEmpty(cdspNumber))
                return;
            var result = ctl.GetExistingCase(caseNumber, cdspNumber);
            if (result != null && result.Count() > 0)
            {
                int caseid = result.FirstOrDefault().CaseId;
                Response.Redirect(EditUrl("cid", caseid.ToString(), "CountyClaims"), true);
            }
        }
        protected void ClearEventForm()
        {
            hdEventId.Value = string.Empty;
            chkMeetingHeld.Checked = false;
            txtEventDate.Text = string.Empty;
            drpReason.SelectedIndex = -1;
            rblAgreementType.SelectedIndex = -1;
            drpMediatorType.SelectedIndex = -1;
            hdMediatorId.Value = string.Empty;
            txtMediator.Text = string.Empty;
            txtHours.Text = string.Empty;
            chkSubmittedToParties.Checked = false;
            chkAgreementSigned.Checked = false;
            chkPreparedAttorney.Checked = false;
            chkAdjournedTimeRemaining.Checked = false;
            foreach (ListItem li in cblAppearanceRecord.Items) {
                {
                    li.Selected = false;
                }
            }
        }
        private string GetMediatorName(int mediatorId)
        {
            var ctl = new MediatorController();
            Mediator mediator = ctl.GetMediator(mediatorId);
            return mediator.MediatorName;
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (RegionID > 0)
                    _regionId = RegionID;
                if (SessionIndex > 0)
                    CurrentSessionIndex = SessionIndex;
                var tc = new CaseController();
                _currentCase = tc.GetCase(CaseID);
                if (_currentCase != null && _currentCase.RegionId.HasValue)
                {
                    _regionId = _currentCase.RegionId.Value;
                }
                chkProSePlaintiff.InputAttributes.Add("class", "form-check-input");
                chkProSePlaintiff.LabelAttributes.Add("class", "form-check-label");
                chkProSeDefendant.InputAttributes.Add("class", "form-check-input");
                chkProSeDefendant.LabelAttributes.Add("class", "form-check-label");
                chkPlaintiffFta.InputAttributes.Add("class", "form-check-input");
                chkPlaintiffFta.LabelAttributes.Add("class", "form-check-label");
                chkDefendantFta.InputAttributes.Add("class", "form-check-input");
                chkDefendantFta.LabelAttributes.Add("class", "form-check-label");
                chkTelephoneSession.InputAttributes.Add("class", "form-check-input");
                chkTelephoneSession.LabelAttributes.Add("class", "form-check-label");
                chkArbitrationReferral.InputAttributes.Add("class", "form-check-input");
                chkArbitrationReferral.LabelAttributes.Add("class", "form-check-label");
                chkCircuitCivilReferal.InputAttributes.Add("class", "form-check-input");
                chkCircuitCivilReferal.LabelAttributes.Add("class", "form-check-label");
                chkDepartmentFeeWaiver.InputAttributes.Add("class", "form-check-input");
                chkDepartmentFeeWaiver.LabelAttributes.Add("class", "form-check-label");
                chkInterpreterRequested.InputAttributes.Add("class", "form-check-input");
                chkInterpreterRequested.LabelAttributes.Add("class", "form-check-label");
                chkFeeAgreementEntered.InputAttributes.Add("class", "form-check-input");
                chkFeeAgreementEntered.LabelAttributes.Add("class", "form-check-label");
                chkFeeJudgmentEntered.InputAttributes.Add("class", "form-check-input");
                chkFeeJudgmentEntered.LabelAttributes.Add("class", "form-check-label");
                chkOTSC.InputAttributes.Add("class", "form-check-input");
                chkOTSC.LabelAttributes.Add("class", "form-check-label");
                //Event Form Setup
                chkMeetingHeld.InputAttributes.Add("class", "form-check-input");
                chkMeetingHeld.LabelAttributes.Add("class", "form-check-label");
                chkSubmittedToParties.InputAttributes.Add("class", "form-check-input");
                chkSubmittedToParties.LabelAttributes.Add("class", "form-check-label");
                chkAgreementSigned.InputAttributes.Add("class", "form-check-input");
                chkAgreementSigned.LabelAttributes.Add("class", "form-check-label");
                chkPreparedAttorney.InputAttributes.Add("class", "form-check-input");
                chkPreparedAttorney.LabelAttributes.Add("class", "form-check-label");
                chkAdjournedTimeRemaining.InputAttributes.Add("class", "form-check-input");
                chkAdjournedTimeRemaining.LabelAttributes.Add("class", "form-check-label");
                if (!Page.IsPostBack)
                {
                    var aCtl = new GroupController();
                    IEnumerable<Appearance> appearances = aCtl.GetAppearancesByGroup((int)_caseTypeGroup);
                    cblAppearanceRecord.DataSource = appearances;
                    cblAppearanceRecord.DataBind();
                    if (_regionId > 0)
                    {
                        if (RegionName != "" && GroupName != "")
                        {
                            ltHeading.Text = string.Format(ltHeading.Text, GroupName, RegionName);
                        }
                        else { ltHeading.Text = ""; }
                    }
                    else
                    {
                        ltHeading.Text = "";
                    }
                    lnkCancel.NavigateUrl = EditUrl("cid", CaseID.ToString(), "CountyClaims");
                    lnkSearch.NavigateUrl = _navigationManager.NavigateURL();
                    InitializeDropDowns();
                    if (CaseID > 0)
                    {
                        PopulateCaseInformation();
                    }
                    else
                    {
                        pnlSession.Visible = false;
                        cmdDelete.Visible = false;
                    }
                    lnkNew.NavigateUrl = EditUrl("rid", _regionId.ToString(), "CountyClaims");
                    if (UserInfo.IsInRole(AdminRole))
                    {
                        cmdDelete.Visible = false;
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            var ctl = new CaseController();
            ctl.DeleteCase(CaseID);
            Response.Redirect(_navigationManager.NavigateURL());
        }
        protected void pnlSession_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var stringValue = Localization.GetString("Alert.Text", LocalResourceFile.Replace("CountyClaims", ""));
            if (CaseID <= 0)
            {
                CheckExistingCase();
                _currentCase = new Case();
            }
            FillCase();
            PopulateSessionInformation();
            UpdateNavigation();
            ltMessage.Text = string.Format(stringValue, "success", "Success!", "Case and Session Record Saved", "fas fa-thumbs-up");
        }
        protected void cmdNextSession_Click(object sender, EventArgs e)
        {
            CurrentSessionIndex++;
            PopulateSessionInformation();
        }
        protected void cmdDeleteSession_Click(object sender, EventArgs e)
        {
            DeleteSession();
            PopulateSessionInformation();
            UpdateNavigation();
        }
        protected void cmdNewSession_Click(object sender, EventArgs e)
        {
            AddNewSession();
            PopulateSessionInformation();
            UpdateNavigation();
        }
        protected void cmdPreviousSession_Click(object sender, EventArgs e)
        {
            CurrentSessionIndex--;
            PopulateSessionInformation();
        }
        
        #region Event Events
        protected void rptEvent_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "delete")
            {
                var ctl = new EventController();
                Int32.TryParse(e.CommandArgument.ToString(), out int eventId);
                ctl.DeleteEvent(eventId);
                PopulateEventInformation();
            }
            if (e.CommandName == "edit")
            {
                ClearEventForm();
                var ctl = new EventController();
                Int32.TryParse(e.CommandArgument.ToString(), out int eventId);
                Event evt = ctl.GetEvent(eventId);
                hdEventId.Value = eventId.ToString();
                if (evt.MediationHeld.HasValue)
                    chkMeetingHeld.Checked = evt.MediationHeld.Value;
                if (evt.EventDate.HasValue)
                    txtEventDate.Text = evt.EventDate.Value.ToString("yyyy-MM-dd");
                rblAgreementType.SelectedValue = evt.AgreementType;
                drpReason.SelectedValue = evt.ReasonNotHeld;
                if (evt.AgreementSigned.HasValue)
                    chkAgreementSigned.Checked = evt.AgreementSigned.Value;
                drpMediatorType.SelectedValue = evt.MediatorType;
                hdMediatorId.Value = evt.MediatorId.ToString();
                if (evt.MediatorId > 0)
                    txtMediator.Text = GetMediatorName(evt.MediatorId);
                txtHours.Text = evt.TimeRemaining.ToString();
                if (evt.AgreementSubmittedParties.HasValue)
                    chkSubmittedToParties.Checked = evt.AgreementSubmittedParties.Value;
                if (evt.AgreementSigned.HasValue)
                    chkAgreementSigned.Checked = evt.AgreementSigned.Value;
                if (evt.AgreementPreparedAttorney.HasValue)
                    chkPreparedAttorney.Checked = evt.AgreementPreparedAttorney.Value;
                if (evt.AdjournedTimeRemaining.HasValue)
                    chkAdjournedTimeRemaining.Checked = evt.AdjournedTimeRemaining.Value;
                foreach (Appearance appearance in evt.EventAppearances)
                {
                    ListItem item = cblAppearanceRecord.Items.FindByValue(appearance.AppearanceId.ToString());
                    item.Selected = true;
                }
                ScriptManager.RegisterStartupScript(rptEvent, rptEvent.GetType(), "ToggleForm", "ToggleEventForm(true)", true);
            }
        }
        protected void rptEvent_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Event @event = (Event)e.Item.DataItem;
                if (e.Item.FindControl("lblHoursRemaining") is Label lblHoursRemaining)
                {
                    lblHoursRemaining.Visible = false;
                    if (@event.TimeRemaining.HasValue && @event.TimeRemaining.Value > 0)
                        lblHoursRemaining.Visible = true;
                }
            }
        }
        protected void rptEvent_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
            LinkButton lnkUpdate = (LinkButton)e.Item.FindControl("lnkUpdate");
            LinkButton lnkDelete = (LinkButton)e.Item.FindControl("lnkDelete");
            if (lnkUpdate != null)
                scriptMan.RegisterAsyncPostBackControl(lnkUpdate);
            if (lnkDelete != null)
                scriptMan.RegisterAsyncPostBackControl(lnkDelete);
        }
        protected void cmdSaveEvent_Click(object sender, EventArgs e)
        {
            FillCase();
            var ctl = new EventController();
            if (string.IsNullOrEmpty(hdEventId.Value))
            {
                Event newEvent = new Event
                {
                    TimeRemaining = null,
                    CreatedDate = DateTime.Now,
                    CreatedById = UserId,
                    LastModifiedById = UserId,
                    LastModifiedDate = DateTime.Now,
                    SessionId = Int32.Parse(hdSessionId.Value),
                    MediationHeld = chkMeetingHeld.Checked,
                    AgreementType = rblAgreementType.SelectedValue,
                    ReasonNotHeld = drpReason.SelectedValue,
                    AgreementSigned = chkAgreementSigned.Checked,
                    AgreementSubmittedParties = chkSubmittedToParties.Checked,
                    AgreementPreparedAttorney = chkPreparedAttorney.Checked,
                    MediatorType = drpMediatorType.SelectedValue,
                    AdjournedTimeRemaining = chkAdjournedTimeRemaining.Checked
                };
                if (!string.IsNullOrEmpty(txtEventDate.Text))
                    newEvent.EventDate = DateTime.Parse(txtEventDate.Text);
                if (Int32.TryParse(hdMediatorId.Value, out int id))
                    newEvent.MediatorId = id;
                decimal.TryParse(txtHours.Text, out decimal timeRemaining);
                if (timeRemaining > 0)
                    newEvent.TimeRemaining = timeRemaining;
                ctl.CreateEvent(newEvent);

                foreach (ListItem item in cblAppearanceRecord.Items)
                {
                    if (item.Selected)
                    {
                        int appearanceId = Int32.Parse(item.Value);
                        ctl.CreateEventAppearance(new EventAppearance { AppearanceId = appearanceId, EventId = newEvent.EventId, CreatedById = UserId, LastModifiedById = UserId, CreatedDate = DateTime.Now, LastModifiedDate = DateTime.Now });
                    }
                }
            }
            else
            {
                Event oldEvent = ctl.GetEvent(Int32.Parse(hdEventId.Value.ToString()));
                oldEvent.LastModifiedById = UserId;
                oldEvent.LastModifiedDate = DateTime.Now;
                oldEvent.MediationHeld = chkMeetingHeld.Checked;
                oldEvent.AgreementType = rblAgreementType.SelectedValue;
                if (!string.IsNullOrEmpty(txtEventDate.Text))
                    oldEvent.EventDate = DateTime.Parse(txtEventDate.Text);
                oldEvent.AgreementSubmittedParties = chkSubmittedToParties.Checked;
                oldEvent.AgreementSigned = chkAgreementSigned.Checked;
                oldEvent.AgreementPreparedAttorney = chkPreparedAttorney.Checked;
                oldEvent.ReasonNotHeld = drpReason.SelectedValue;
                oldEvent.MediatorType = drpMediatorType.SelectedValue;
                if (Int32.TryParse(hdMediatorId.Value, out int id))
                    oldEvent.MediatorId = id;
                oldEvent.AdjournedTimeRemaining = chkAdjournedTimeRemaining.Checked;
                decimal.TryParse(txtHours.Text, out decimal timeRemaining);
                oldEvent.TimeRemaining = null;
                if (timeRemaining > 0)
                    oldEvent.TimeRemaining = timeRemaining;
                ctl.UpdateEvent(oldEvent);
                ctl.DeleteAllEventAppearances(oldEvent.EventId);
                foreach (ListItem item in cblAppearanceRecord.Items)
                {
                    if (item.Selected)
                    {
                        int appearanceId = Int32.Parse(item.Value);
                        ctl.CreateEventAppearance(new EventAppearance { AppearanceId = appearanceId, EventId = oldEvent.EventId, CreatedById = UserId, LastModifiedById = UserId, CreatedDate = DateTime.Now, LastModifiedDate = DateTime.Now });
                    }
                }
                ClearEventForm();
            }
            PopulateEventInformation();
            ScriptManager.RegisterStartupScript(rptEvent, rptEvent.GetType(), "ToggleForm", "ToggleEventForm(false)", true);
        }
        #endregion //Event Events

        #endregion //Events
    }
}