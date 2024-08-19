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
    public partial class EditDependency : MediationStatisticsModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        private int _regionId = Null.NullInteger;
        private readonly GroupType _caseTypeGroup = GroupType.Dependency;
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
        public EditDependency()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        public string GetAppearanceItems(string eventId)
        {
            string returnValue = "";
            string template = "<fieldset class=\"outline-fieldset\">\r\n<legend class=\"small\">Appearance Record</legend>\r\n{0}\r\n</fieldset>";
            var ctl = new AppearanceController();
            IEnumerable<Appearance> appearances = ctl.GetEventAppearances(Int32.Parse(eventId));
            foreach (Appearance appearance in appearances)
            {
                returnValue += string.Format("{0}; ", appearance.Description);
            }
            if(returnValue !="")
                return string.Format(template, returnValue);
            return "<p><strong>No Appearance Record</strong></p>";
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
        private void PopulateCaseInformation()
        {
            txtFirstName.Text = _currentCase.p1_FirstName;
            txtLastName.Text = _currentCase.p1_LastName;
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
            Session session = _currentCase.GetCurrentSession(CurrentSessionIndex);
            {
                hdSessionId.Value = session.SessionId.ToString();
                drpActionStage.SelectedIndex = -1;
                if (session.StageOfAction.HasValue)
                    drpActionStage.SelectedValue = session.StageOfAction.Value.ToString();
                txtMediationDate.Text = "";
                if (session.MediationDate.HasValue)
                    txtMediationDate.Text = session.MediationDate.Value.ToShortDateString();
                txtOrderReferralDate.Text = "";
                if (session.ReferralDate.HasValue)
                    txtOrderReferralDate.Text = session.ReferralDate.Value.ToShortDateString();
                txtReferralSource.Text = session.ProgramReferralSource;
                chkTelephoneSession.Checked = false;
                chkInmate.Checked = false;
                chkInterpreterRequested.Checked = false;
                chkTelephoneSession.Checked = session.HeldByPhone;
                if (session.Inmate.HasValue)
                    chkInmate.Checked = session.Inmate.Value;
                if (session.Interpreter.HasValue)
                    chkInterpreterRequested.Checked = session.Interpreter.Value;
                txtChildrenInvolved.Text = session.ChildrenInvolved.ToString();
                txtParentsInvolved.Text = session.ParentsInvolved.ToString();
                txtComments.Text = session.Comment;
            }
            PopulateEventInformation();
            UpdateNavigation();
        }
        private void PopulateEventInformation()
        {
            try
            {
                lstEvents.DataSource = _currentCase.GetCurrentSession(CurrentSessionIndex).SessionEvents;
            }
            catch
            {
                lstEvents.DataSource = new List<Event>();
            }

            lstEvents.DataBind();
        }
        private void InitializeDropDowns()
        {
            var ctlSession = new SessionController();
            var ctlActionStage = new StageActionController();
            drpActionStage.DataSource = ctlActionStage.GetStageActions();
            drpActionStage.DataBind();
            IEnumerable<string> referralSources = ctlSession.GetReferralSourceItems();
            foreach (string referralSource in referralSources)
            {
                ltReferralSourceOptions.Text += string.Format("<option>{0}</option>", referralSource);
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
            _currentCase.p1_FirstName = txtFirstName.Text;
            _currentCase.p1_LastName = txtLastName.Text;
            _currentCase.RegionId = _regionId;
            _currentCase.GroupId = (int)_caseTypeGroup;
            _currentCase.CaseNumber = Helper.GetCaseFormatted(txtCaseYear.Text.Trim(), txtCaseType.Text.Trim(), txtCaseSequence.Text.Trim(), txtSuffix.Text.Trim());
            _currentCase.LastModifiedById = UserId;
            _currentCase.LastModifiedDate = DateTime.Now;
            if (CaseID <= 0)
            {
                _currentCase.CreatedById = UserId;
                _currentCase.CreatedDate = DateTime.Now;
                ctl.CreateCase(_currentCase);
                Response.Redirect(EditUrl("cid", _currentCase.CaseId.ToString(), "Dependency"), true);
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
                if (!string.IsNullOrEmpty(txtMediationDate.Text))
                    session.MediationDate = DateTime.Parse(txtMediationDate.Text);
                if (!string.IsNullOrEmpty(txtOrderReferralDate.Text))
                    session.ReferralDate = DateTime.Parse(txtOrderReferralDate.Text);
                if (drpActionStage.SelectedIndex > 0)
                    session.StageOfAction = Int32.Parse(drpActionStage.SelectedValue);
                if (txtChildrenInvolved.Text.Length > 0)
                    session.ChildrenInvolved = Int32.Parse(txtChildrenInvolved.Text);
                if (txtParentsInvolved.Text.Length > 0)
                    session.ParentsInvolved = Int32.Parse(txtParentsInvolved.Text);
                session.Interpreter = chkInterpreterRequested.Checked;
                session.Inmate = chkInmate.Checked;
                session.ProgramReferralSource = txtReferralSource.Text;
                session.HeldByPhone = chkTelephoneSession.Checked;
                session.Comment = txtComments.Text;
                session.LastModifiedById = UserId;
                session.LastModifiedDate = DateTime.Now;
            }
            if (session.SessionId > 0)
            {
                ctlSession.UpdateSession(session);
            }
            else
            {
                ctlSession.CreateSession(session);
            }
        }
        private void DeleteSession()
        {
            var ctl = new SessionController();
            ctl.DeleteSession(_currentCase.GetCurrentSession(CurrentSessionIndex));
            if (_currentCase.CaseSessions.Count() <= 1)
                _currentCase.CaseSessions.Append(new Session());
        }
        private void CheckExistingCase()
        {
            string caseNumber = Helper.GetCaseFormatted(txtCaseYear.Text.Trim(), txtCaseType.Text.Trim(), txtCaseSequence.Text.Trim(), txtSuffix.Text.Trim());
            var ctl = new CaseController();
            if (string.IsNullOrEmpty(caseNumber))
                return;
            var result = ctl.GetExistingCase(caseNumber, "");
            if (result != null && result.Count() > 0)
            {
                int caseid = result.FirstOrDefault().CaseId;
                Response.Redirect(EditUrl("cid", caseid.ToString(), "Dependency"), true);
            }
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
                chkTelephoneSession.InputAttributes.Add("class", "form-check-input");
                chkTelephoneSession.LabelAttributes.Add("class", "form-check-label");
                chkInmate.InputAttributes.Add("class", "form-check-input");
                chkInmate.LabelAttributes.Add("class", "form-check-label");
                chkInterpreterRequested.LabelAttributes.Add("class", "form-check-label");
                chkInterpreterRequested.InputAttributes.Add("class", "form-check-input");
                if (!Page.IsPostBack)
                {
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
                    lnkCancel.NavigateUrl = EditUrl("cid", CaseID.ToString(), "Dependency");
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
                    lnkNew.NavigateUrl = EditUrl("rid", _regionId.ToString(), "Dependency");
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
        protected void pnlSession_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var stringValue = Localization.GetString("Alert.Text", LocalResourceFile.Replace("Dependency", ""));
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
        protected void cmdAddEvent_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hdSessionId.Value))
            {
                FillCase();
                PopulateSessionInformation();
            }
            lstEvents.InsertItemPosition = InsertItemPosition.FirstItem;
            PopulateEventInformation();
        }
        #region Event Events
        protected void lstEvents_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem | e.Item.ItemType == ListViewItemType.InsertItem)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
                LinkButton lnkUpdate = (LinkButton)e.Item.FindControl("lnkUpdate");
                LinkButton lnkDelete = (LinkButton)e.Item.FindControl("lnkDelete");
                LinkButton lnkInsert = (LinkButton)e.Item.FindControl("lnkInsert");
                LinkButton lnkCancel = (LinkButton)e.Item.FindControl("lnkCancel");
                var ctl = new GroupController();
                if (lnkUpdate != null)
                    scriptMan.RegisterAsyncPostBackControl(lnkUpdate);
                if (lnkInsert != null)
                    scriptMan.RegisterAsyncPostBackControl(lnkInsert);
                if (lnkCancel != null)
                    scriptMan.RegisterAsyncPostBackControl(lnkCancel);
                if (lnkDelete != null)
                    scriptMan.RegisterAsyncPostBackControl(lnkDelete);
                if (e.Item.FindControl("cblAppearanceRecord") is CheckBoxList cblAppearanceRecord)
                {
                    IEnumerable<Appearance> appearances = ctl.GetAppearancesByGroup((int)_caseTypeGroup);
                    cblAppearanceRecord.DataSource = appearances;
                    cblAppearanceRecord.DataBind();
                }
                if (e.Item.FindControl("chkMeetingHeld") is CheckBox chkMeetingHeld)
                {
                    chkMeetingHeld.InputAttributes.Add("class", "form-check-input");
                    chkMeetingHeld.LabelAttributes.Add("class", "form-check-label");
                }
                if (e.Item.FindControl("chkSignedMediation") is CheckBox chkSignedMediation)
                {
                    chkSignedMediation.InputAttributes.Add("class", "form-check-input");
                    chkSignedMediation.LabelAttributes.Add("class", "form-check-label");
                }
                if (e.Item.FindControl("chkSignedAfterMediation") is CheckBox chkSignedAfterMediation)
                {
                    chkSignedAfterMediation.InputAttributes.Add("class", "form-check-input");
                    chkSignedAfterMediation.LabelAttributes.Add("class", "form-check-label");
                }
                if (e.Item.FindControl("chkSignedTrial") is CheckBox chkSignedTrial)
                {
                    chkSignedTrial.InputAttributes.Add("class", "form-check-input");
                    chkSignedTrial.LabelAttributes.Add("class", "form-check-label");
                }
                if (e.Item.FindControl("chkAdjournedTimeRemaining") is CheckBox chkAdjournedTimeRemaining)
                {
                    chkAdjournedTimeRemaining.InputAttributes.Add("class", "form-check-input");
                    chkAdjournedTimeRemaining.LabelAttributes.Add("class", "form-check-label");
                }
            }
        }
        protected void lstEvents_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = e.Item as ListViewDataItem;
                Event @event = (Event)dataItem.DataItem;
                if (e.Item.FindControl("lblHoursRemaining") is Label lblHoursRemaining)
                {
                    lblHoursRemaining.Visible = false;
                    if (@event.TimeRemaining.HasValue && @event.TimeRemaining.Value > 0)
                        lblHoursRemaining.Visible = true;
                }

                if (dataItem.DisplayIndex == lstEvents.EditIndex)
                {
                    if (e.Item.FindControl("chkMeetingHeld") is CheckBox chkMeetingHeld && e.Item.FindControl("lblReason") is Label lblReason && e.Item.FindControl("drpReason") is DropDownList drpReason)
                    {
                        if (chkMeetingHeld.Checked)
                        {
                            drpReason.Attributes.CssStyle.Add("display", "none");
                            lblReason.Attributes.CssStyle.Add("display", "none");
                        }
                        if (e.Item.FindControl("cblAppearanceRecord") is CheckBoxList cblAppearanceRecord)
                        {
                            foreach (Appearance appearance in @event.EventAppearances)
                            {
                                ListItem item = cblAppearanceRecord.Items.FindByValue(appearance.AppearanceId.ToString());
                                item.Selected = true;
                            }
                        }
                    }
                }
            }
        }
        protected void lstEvents_ItemInserting(object sender, ListViewInsertEventArgs e)
        {
            try
            {
                var ctlEvent = new EventController();
                Event newEvent = new Event
                {
                    TimeRemaining = null,
                    CreatedDate = DateTime.Now,
                    CreatedById = UserId,
                    LastModifiedById = UserId,
                    LastModifiedDate = DateTime.Now,
                    SessionId = Int32.Parse(hdSessionId.Value),
                    Signed1 = false, Signed2 = false, Signed3 = false,
                    SignedCount1 = 0, SignedCount2 = 0,SignedCount3 = 0,
                };
                if (e.Item.FindControl("chkMeetingHeld") is CheckBox chkMeetingHeld)
                    newEvent.MediationHeld = chkMeetingHeld.Checked;
                if (e.Item.FindControl("rblAgreementType") is RadioButtonList rblAgreementType)
                    newEvent.AgreementType = rblAgreementType.SelectedValue;
                if (e.Item.FindControl("txtEventDate") is TextBox txtEventDate)
                {
                    if (!string.IsNullOrEmpty(txtEventDate.Text))
                        newEvent.EventDate = DateTime.Parse(txtEventDate.Text);
                }
                if (e.Item.FindControl("chkSignedMediation") is CheckBox chkSignedMediation)
                    newEvent.Signed1 = chkSignedMediation.Checked;
                if (e.Item.FindControl("chkSignedAfterMediation") is CheckBox chkSignedAfterMediation)
                    newEvent.Signed2 = chkSignedAfterMediation.Checked;
                if (e.Item.FindControl("chkSignedTrial") is CheckBox chkSignedTrial)
                    newEvent.Signed3 = chkSignedTrial.Checked;
                if (e.Item.FindControl("drpReason") is DropDownList drpReason)
                    newEvent.ReasonNotHeld = drpReason.SelectedValue;
                if (e.Item.FindControl("chkAdjournedTimeRemaining") is CheckBox chkAdjournedTimeRemaining)
                    newEvent.AdjournedTimeRemaining = chkAdjournedTimeRemaining.Checked;
                if (e.Item.FindControl("txtHours") is TextBox txtHours)
                {
                    decimal.TryParse(txtHours.Text, out decimal timeRemaining);
                    if (timeRemaining > 0)
                        newEvent.TimeRemaining = timeRemaining;
                }
                if (e.Item.FindControl("txtSignedMediationCount") is TextBox txtSignedMediationCount)
                {
                    Int32.TryParse(txtSignedMediationCount.Text, out int mediationCount);
                    newEvent.SignedCount1 = mediationCount;
                }
                if (e.Item.FindControl("txtSignedAfterMediationCount") is TextBox txtSignedAfterMediationCount)
                {
                    Int32.TryParse(txtSignedAfterMediationCount.Text, out int afterMediationCount);
                    newEvent.SignedCount2 = afterMediationCount;
                }
                if (e.Item.FindControl("txtSignedTrialCount") is TextBox txtSignedTrialCount)
                {
                    Int32.TryParse(txtSignedTrialCount.Text, out int trialCount);
                    newEvent.SignedCount3 = trialCount;
                }
                ctlEvent.CreateEvent(newEvent);
                if (e.Item.FindControl("cblAppearanceRecord") is CheckBoxList cblAppearanceRecord)
                {
                    foreach (ListItem item in cblAppearanceRecord.Items)
                    {
                        if (item.Selected)
                        {
                            int appearanceId = Int32.Parse(item.Value);
                            ctlEvent.CreateEventAppearance(new EventAppearance { AppearanceId = appearanceId, EventId = newEvent.EventId, CreatedById = UserId, LastModifiedById = UserId, CreatedDate = DateTime.Now, LastModifiedDate = DateTime.Now });
                        }
                    }
                }
                FillCase();
                lstEvents.InsertItemPosition = InsertItemPosition.None;
                PopulateEventInformation();
            }
            catch (Exception ex)
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, ex.Message, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
            }
        }
        protected void lstEvents_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            try
            {
                var ctl = new EventController();
                Event selectedEvent = _currentCase.CaseSessions.ElementAt(CurrentSessionIndex).SessionEvents.ElementAt(e.ItemIndex);
                ctl.DeleteEvent(selectedEvent);
                PopulateEventInformation();
            }
            catch (Exception ex)
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, ex.Message, DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
            }
        }
        protected void lstEvents_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            lstEvents.EditIndex = e.NewEditIndex;
            cmdSave.Enabled = false;
            PopulateEventInformation();
        }
        protected void lstEvents_ItemInserted(object sender, ListViewInsertedEventArgs e)
        {
            if (e.Exception != null)
            {
                if (e.AffectedRows == 0)
                {
                    e.KeepInInsertMode = true;
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "An exception occurred inserting the new Event. " + "Please verify your values and try again.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                }
                else
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "An exception occurred inserting the new Event. " + "Please verify the values in the newly inserted item.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                    PopulateEventInformation();
                }

                e.ExceptionHandled = true;
                cmdSave.Enabled = true;
            }
            else
            {
                Response.Redirect(EditUrl("cid", CaseID.ToString(), "Dependency", "sidx=" + CurrentSessionIndex), true);
            }
        }
        protected void lstEvents_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            if (e.CancelMode == ListViewCancelMode.CancelingInsert)
                lstEvents.InsertItemPosition = InsertItemPosition.None;
            else
                lstEvents.EditIndex = -1;
            PopulateEventInformation();
            cmdSave.Enabled = true;
        }
        protected void lstEvents_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "update")
            {
                var ctl = new EventController();
                Event oldEvent = ctl.GetEvent(Int32.Parse(e.CommandArgument.ToString()));
                oldEvent.LastModifiedById = UserId;
                oldEvent.LastModifiedDate = DateTime.Now;
                oldEvent.TimeRemaining = null;
                if (e.Item.FindControl("chkMeetingHeld") is CheckBox chkMeetingHeld)
                    oldEvent.MediationHeld = chkMeetingHeld.Checked;
                if (e.Item.FindControl("rblAgreementType") is RadioButtonList rblAgreementType)
                    oldEvent.AgreementType = rblAgreementType.SelectedValue;
                if (e.Item.FindControl("chkSignedMediation") is CheckBox chkSignedMediation)
                    oldEvent.Signed1 = chkSignedMediation.Checked;
                if (e.Item.FindControl("chkSignedAfterMediation") is CheckBox chkSignedAfterMediation)
                    oldEvent.Signed2 = chkSignedAfterMediation.Checked;
                if (e.Item.FindControl("chkSignedTrial") is CheckBox chkSignedTrial)
                    oldEvent.Signed3 = chkSignedTrial.Checked;
                if (e.Item.FindControl("drpReason") is DropDownList drpReason)
                    oldEvent.ReasonNotHeld = drpReason.SelectedValue;
                if (e.Item.FindControl("chkAdjournedTimeRemaining") is CheckBox chkAdjournedTimeRemaining)
                    oldEvent.AdjournedTimeRemaining = chkAdjournedTimeRemaining.Checked;
                if (e.Item.FindControl("txtEventDate") is TextBox txtEventDate)
                {
                    if (!string.IsNullOrEmpty(txtEventDate.Text))
                        oldEvent.EventDate = DateTime.Parse(txtEventDate.Text);
                }
                if (e.Item.FindControl("txtHours") is TextBox txtHours)
                {
                    decimal.TryParse(txtHours.Text, out decimal timeRemaining);
                    if (timeRemaining > 0)
                        oldEvent.TimeRemaining = timeRemaining;
                }
                if (e.Item.FindControl("txtSignedMediationCount") is TextBox txtSignedMediationCount)
                {
                    Int32.TryParse(txtSignedMediationCount.Text, out int mediationCount);
                    oldEvent.SignedCount1 = mediationCount;
                }
                if (e.Item.FindControl("txtSignedAfterMediationCount") is TextBox txtSignedAfterMediationCount)
                {
                    Int32.TryParse(txtSignedAfterMediationCount.Text, out int afterMediationCount);
                    oldEvent.SignedCount2 = afterMediationCount;
                }
                if (e.Item.FindControl("txtSignedTrialCount") is TextBox txtSignedTrialCount)
                {
                    Int32.TryParse(txtSignedTrialCount.Text, out int trialCount);
                    oldEvent.SignedCount3 = trialCount;
                }
                ctl.UpdateEvent(oldEvent);
                ctl.DeleteAllEventAppearances(oldEvent.EventId);
                if (e.Item.FindControl("cblAppearanceRecord") is CheckBoxList cblAppearanceRecord)
                {
                    foreach (ListItem item in cblAppearanceRecord.Items)
                    {
                        if (item.Selected)
                        {
                            int appearanceId = Int32.Parse(item.Value);
                            ctl.CreateEventAppearance(new EventAppearance { AppearanceId = appearanceId, EventId = oldEvent.EventId, CreatedById = UserId, LastModifiedById = UserId, CreatedDate = DateTime.Now, LastModifiedDate = DateTime.Now });
                        }
                    }
                }
                FillCase();
            }
        }
        protected void lstEvents_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            lstEvents.EditIndex = -1;
            PopulateEventInformation();
        }
        #endregion //Event Events

        #endregion //Events
    }
}