using DotNetNuke.Abstractions;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.EmployeeDB.Components;
using tjc.Modules.EmployeeDB.Components.Services;

namespace tjc.Modules.EmployeeDB
{
    /// -----------------------------------------------------------------------------
    /// <summary>   
    /// The Edit class is used to manage content
    /// 
    /// Typically your edit control would be used to create new content, or edit existing content within your module.
    /// The ControlKey for this control is "Edit", and is defined in the manifest (.dnn) file.
    /// 
    /// Because the control inherits from EmployeeDBModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class EditContact : EmployeeDBModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public EditContact()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                cmdCancel.NavigateUrl = _navigationManager.NavigateURL();
                chkActive.InputAttributes.Add("class", "form-check-input");
                chkActive.LabelAttributes.Add("class", "form-check-label");

                if (!Page.IsPostBack)
                {
                    JavaScript.RequestRegistration(CommonJs.jQuery);
                    PopulateDropDowns();
                    var ctl = new EmployeeController();
                    Employee employee = ctl.GetEmployee(EmployeeId);
                    if (employee != null)
                    {
                        if (employee.CountyId.HasValue)
                        {
                            drpCounty.SelectedValue = employee.CountyId.ToString();
                        }
                        txtLastName.Text = employee.LocationName;
                        txtAddress.Text = employee.Address;
                        txtCity.Text = employee.City;
                        txtEmail.Text = employee.Email;
                        txtFirstName.Text = employee.FirstName;
                        txtLastName.Text = employee.LastName;
                        txtTitle.Text = employee.JobTitle;
                        txtLocation.Text = employee.LocationName;
                        txtMiddleInitial.Text = employee.MiddleInitial;
                        txtPersonalEmail.Text = employee.PersonalEmail;
                        drpState.SelectedValue = employee.State;
                        txtZip.Text = employee.Zip;
                        if (employee.IsActive.HasValue)
                        {
                            chkActive.Checked = employee.IsActive.Value;
                        }
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdAddGroup_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            List<string> groups = new List<string>();
            LoginRequest loginRequest = new LoginRequest { Password = SwnPassword, Username = SwnUsername };
            string token = SwnInterface.GetToken(SwnServiceIdentifier, SwnSubscriptionKey, loginRequest);
            foreach (ListItem item in lsGroups.Items)
            {
                if (item.Selected)
                {
                    item.Selected = false;
                    Int32.TryParse(item.Value, out int groupId);
                    lsMembership.Items.Add(item);
                    GroupMembership groupMembership = new GroupMembership { EmployeeId = EmployeeId, GroupId = groupId, CreatedById = UserId, CreatedDate = DateTime.Now, LastModifiedById = UserId, LastModifiedDate = DateTime.Now };
                    ctl.CreateGroupMembership(groupMembership);
                    Components.Group group = ctl.GetGroup(groupId);
                    if (group != null && group.IsSwnGroup)
                        groups.Add(groupId.ToString());
                }
            }
            try
            {
                SwnInterface.AddContactToSwnGroup(groups, EmployeeId.ToString(), SwnServiceIdentifier, SwnSubscriptionKey, token);
            }
            catch (Exception exc)
            {
                ltMessage.Text = "<div class='alert alert-danger'><i class='fas fa-exclamation-circle'></i> Failed to Add Contact to SWN Group</div>";
                string process = string.Format("Add groups to EmployeeId:{0} Groups to Add:{1}", EmployeeId, groups.ToString());
                SwnLog swnLog = new SwnLog { CreatedBy = UserId, CreatedDate = DateTime.Now, Exception = exc.InnerException.Message, Process = process };
                var logCtl = new SwnLogController();
                logCtl.CreateSwnLog(swnLog);
            }
            PopulateGroupLists(ctl);
        }

        protected void cmdRemoveGroup_Click(object sender, EventArgs e)
        {
            var ctl = new GroupController();
            LoginRequest loginRequest = new LoginRequest { Password = SwnPassword, Username = SwnUsername };
            string token = SwnInterface.GetToken(SwnServiceIdentifier, SwnSubscriptionKey, loginRequest);
            foreach (ListItem item in lsMembership.Items)
            {
                if (item.Selected)
                {
                    Int32.TryParse(item.Value, out int groupId);
                    item.Selected = false;
                    lsGroups.Items.Add(item);
                    GroupMembership groupMembership = ctl.GetGroupMembership(EmployeeId, groupId);
                    if (groupMembership != null)
                    {
                        ctl.DeleteGroupMembership(groupMembership);
                        Components.Group group = ctl.GetGroup(groupId);
                        try
                        {
                            if (group != null && group.IsSwnGroup)
                                SwnInterface.RemoveContactFromSwnGroup(groupId.ToString(), EmployeeId.ToString(), SwnServiceIdentifier, SwnSubscriptionKey, token);
                        }
                        catch (Exception exc)
                        {
                            string process = string.Format("Remove Group Id:{0} from Contact Id:{1}", groupId, EmployeeId);
                            SwnLog swnLog = new SwnLog { CreatedBy = UserId, CreatedDate = DateTime.Now, Exception = exc.InnerException.Message, Process = process };
                            var logCtl = new SwnLogController();
                            logCtl.CreateSwnLog(swnLog);
                        }
                    }
                }
            }
            PopulateGroupLists(ctl);
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            ltMessage.Text = "";

            var t = new Employee();
            var tc = new EmployeeController();

            if (EmployeeId > 0)
            {
                t = tc.GetEmployee(EmployeeId);
            }
            Int32.TryParse(drpCounty.SelectedValue, out int countyId);
            if (countyId != 0)
            {
                t.CountyId = countyId;
            }
            t.LocationName = txtLocation.Text;
            t.Address = txtAddress.Text;
            t.City = txtCity.Text;
            t.Email = txtEmail.Text;
            t.FirstName = txtFirstName.Text;
            t.LastName = txtLastName.Text;
            t.JobTitle = txtTitle.Text;
            t.LastModifiedById = UserId;
            t.LastModifiedDate = DateTime.Now;
            t.MiddleInitial = txtMiddleInitial.Text;
            t.PersonalEmail = txtPersonalEmail.Text;
            t.State = drpState.SelectedValue;
            t.Zip = txtZip.Text;
            t.IsActive = chkActive.Checked;
            t.IsEmployee = false;
            LoginRequest loginRequest = new LoginRequest { Password = SwnPassword, Username = SwnUsername };
            string token = SwnInterface.GetToken(SwnServiceIdentifier, SwnSubscriptionKey, loginRequest);
            if (EmployeeId <= 0)
            {
                t.CreatedDate = DateTime.Now;
                t.CreatedById = UserId;
                tc.CreateEmployee(t);
            }
            else
            {
                tc.UpdateEmployee(t);
            }
            if (chkActive.Checked)
            {
                try
                {
                    SwnInterface.AddUpdateSwnContact(t, SwnServiceIdentifier, SwnSubscriptionKey, token);
                }
                catch (Exception exc)
                {
                    ltMessage.Text = "<div class='alert alert-danger'><i class='fas fa-exclamation-circle'></i> Failed to sync contact in SWN</div>";
                    string process = string.Format("{1} {0} SWN Contact Information", t.FullName, EmployeeId > 0 ? "Update" : "Add");
                    SwnLog swnLog = new SwnLog { CreatedBy = UserId, CreatedDate = DateTime.Now, Exception = exc.InnerException.Message, Process = process };
                    var logCtl = new SwnLogController();
                    logCtl.CreateSwnLog(swnLog);
                }
            }
            else
            {
                try
                {
                    SwnInterface.DeleteSwnContactById(t.EmployeeId.ToString(), SwnServiceIdentifier, SwnSubscriptionKey, token);
                }
                catch (Exception exc)
                {
                    ltMessage.Text = "<div class='alert alert-danger'><i class='fas fa-exclamation-circle'></i> Failed to removed contact from SWN</div>";
                    string process = string.Format("Remove {0} from SWN", t.FullName);
                    SwnLog swnLog = new SwnLog { CreatedBy = UserId, CreatedDate = DateTime.Now, Exception = exc.InnerException.Message, Process = process };
                    var logCtl = new SwnLogController();
                    logCtl.CreateSwnLog(swnLog);
                }
            }
            if (EmployeeId <= 0)
                Response.Redirect(EditUrl("eid", t.EmployeeId.ToString(), "EditContact"), true);

        }

        #endregion

        #region Methods
        private void PopulateGroupLists(GroupController dCtl)
        {
            lsGroups.DataSource = dCtl.GetGroupsExcludingMembership(EmployeeId);
            lsGroups.DataBind();
            lsMembership.DataSource = dCtl.GetGroupMemberships(EmployeeId);
            lsMembership.DataBind();
        }
        private void PopulateDropDowns()
        {
            var cnCtl = new Globals.CountyController();
            drpCounty.DataSource = cnCtl.GetCounties().OrderBy(x => x.CountyName);
            drpCounty.DataBind();
            PopulateGroupLists(new GroupController());
        }
        #endregion

    }
}