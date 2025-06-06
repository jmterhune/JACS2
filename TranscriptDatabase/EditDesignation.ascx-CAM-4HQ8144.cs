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
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using tjc.Modules.TranscriptDatabase.Components;
using tjc.Modules.TranscriptDatabase.Services.ViewModels;

namespace tjc.Modules.TranscriptDatabase
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from TranscriptDatabaseModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class EditDesignation : TranscriptDatabaseModuleBase
    {
        #region Properties
        private readonly INavigationManager _navigationManager;
        public string AttorneyArray;
        public string AttorneyList()
        {
            string attorneyList = string.Empty;
            var ctl = new AttorneyController();
            string attorneyIds = string.Empty;
            IEnumerable<AttorneyViewModel> attorneys = ctl.GetDesignationAttorneys(DesignationId);
            foreach (AttorneyViewModel attorney in attorneys)
            {
                attorneyIds += string.Format("{0},", attorney.AttorneyId);
                attorneyList += string.Format("{{id:{0},name:\"{1}\",office:\"{2}\"}},\n", attorney.AttorneyId, attorney.ListName, attorney.OfficeName);
            }
            hdAttorneyIds.Value = attorneyIds.TrimEnd(',');
            return attorneyList.TrimEnd('\n').TrimEnd(',');
        }
        #endregion

        #region Methods
        public EditDesignation()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void UpdateDueDate(Designation designation)
        {
            var ctl = new CalendarController();
            Components.Calendar calendarEvent = ctl.GetCalendarByDesignation(DesignationId);

            if (calendarEvent != null)
            {
                calendarEvent.StartTime = designation.DueDate.Value;
                calendarEvent.EndTime = designation.DueDate.Value;
                txtLastName.Text = designation.dLastName;
                txtFirstName.Text = designation.dFirstName;
                txtMiddleName.Text = designation.dMiddleName;

                ctl.UpdateCalendar(calendarEvent);
            }
            else
                CreateCalendarDueDate(designation);
        }

        private Components.Calendar CreateCalendarDueDate(EventTypes eventTypeId, bool requestOutstanding, Designation designation)
        {
            var ctl = new CalendarController();
            Components.Calendar calendar = new Components.Calendar();
            {
                var withBlock = calendar;
                withBlock.CreatedByUserID = UserId;
                withBlock.CreatedDate = DateTime.Now;
                withBlock.LastModifiedDate = DateTime.Now;
                withBlock.LastModifiedByUserID = UserId;
                withBlock.DesignationID = DesignationId;
                withBlock.StartTime = designation.DueDate.Value;
                withBlock.EndTime = designation.DueDate.Value;
                withBlock.EventTypeID = (int)eventTypeId;
                withBlock.Subject = designation.CalendarName;
                withBlock.RequestOutstanding = requestOutstanding;
            }
            ctl.CreateCalendar(calendar);
            return calendar;
        }

        private Components.Calendar CreateCalendarDueDate(Designation designation)
        {
            return CreateCalendarDueDate(EventTypes.dueDate, false, designation);
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (!IsAdmin)
                        Response.Redirect(_navigationManager.NavigateURL());
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                    lnkCancel.NavigateUrl = _navigationManager.NavigateURL();
                    PopulateForm();
                    AttorneyArray = AttorneyList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        private void PopulateForm()
        {

            hdDesignationId.Value = DesignationId.ToString();
            var ctl = new Components.DesignationController();
            Designation designation = ctl.GetDesignation(DesignationId);
            if (designation.ServiceDate.HasValue)
                txtServiceDate.Text = designation.ServiceDate.Value.ToShortDateString();
            if (designation.DueDate.HasValue)
            {
                txtDueDate.Text = designation.DueDate.Value.ToShortDateString();
                hdOldDueDate.Value = designation.DueDate.Value.ToShortDateString();
                txtDueDateReadonly.Text = designation.DueDate.Value.ToShortDateString();
            }
            if (designation.ReceiptDate.HasValue)
                txtReceiptDate.Text = designation.ReceiptDate.Value.ToShortDateString();
            else
                txtReceiptDate.Text = DateTime.Now.ToShortDateString();
            if (!IsAdmin)
                cmdDelete.Visible = false;
            txtLastName.Text = designation.dLastName;
            txtFirstName.Text = designation.dFirstName;
            txtMiddleName.Text = designation.dMiddleName;
            drpCounty.SelectedValue = designation.County;
            txtTribunalCaseNumber.Text = designation.LowerTribunalCaseNumber;
            txtAppellateCaseNumber.Text = designation.AppellateCaseNumber;
            chkCourtAppointed.Checked = designation.CourtAppointedCounsel;
            chkIndigent.Checked = designation.DeclaredIndigent;
            chkPublicDefender.Checked = designation.PublicDefenderAppointed;
        }
        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            var ctl = new DesignationController();
            var attorneys = hdAttorneyIds.Value.Split(',');
            Designation designation = ctl.GetDesignation(DesignationId);
            if (!string.IsNullOrEmpty(txtServiceDate.Text))
                designation.ServiceDate = DateTime.Parse(txtServiceDate.Text);
            if (!string.IsNullOrEmpty(txtDueDate.Text))
            {
                designation.DueDate = DateTime.Parse(txtDueDate.Text);
                txtDueDateReadonly.Text = designation.DueDate.Value.ToShortDateString();
            }
            if (!string.IsNullOrEmpty(txtReceiptDate.Text))
                designation.ReceiptDate = DateTime.Parse(txtReceiptDate.Text);
            designation.dLastName = txtLastName.Text;
            designation.dFirstName = txtFirstName.Text;
            designation.dMiddleName = txtMiddleName.Text;
            designation.County = drpCounty.SelectedValue;
            designation.LowerTribunalCaseNumber = txtTribunalCaseNumber.Text;
            designation.AppellateCaseNumber = txtAppellateCaseNumber.Text;
            designation.PublicDefenderAppointed = chkPublicDefender.Checked;
            designation.DeclaredIndigent = chkIndigent.Checked;
            designation.CourtAppointedCounsel = chkCourtAppointed.Checked;
            ctl.DeleteDesignationAttorneys(DesignationId);
            foreach (string atty in attorneys)
            {
                ctl.CreateDesignationAttorney(DesignationId, Int32.Parse(atty));
            }
            ctl.UpdateDesignation(designation);
            AttorneyArray = AttorneyList();
            bool isDueDate = DateTime.TryParse(hdOldDueDate.Value, out DateTime newDueDate);
            if (isDueDate && designation.DueDate.HasValue)
                if (designation.DueDate.Value != newDueDate)
                    UpdateDueDate(designation);
            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Update successful", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
        }
        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            var ctl = new DesignationController();
            ctl.DeleteDesignation(DesignationId);
            Response.Redirect(_navigationManager.NavigateURL());
        }
        #endregion
    }
}