using DotNetNuke.Abstractions;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
    public partial class Edit : EmployeeDBModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public Edit()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                cmdCancel.NavigateUrl = _navigationManager.NavigateURL();
                string ImagePath = "Images/Staff"; // Set default value for folder
                CheckImagePath(ImagePath);

                var userControl = Page.LoadControl("~/controls/filepickeruploader.ascx");
                DotNetNuke.Web.UI.WebControls.DnnFilePickerUploader _fileControl = userControl as DotNetNuke.Web.UI.WebControls.DnnFilePickerUploader;
                _fileControl.ID = "fpUploader";
                _fileControl.Attributes.Add("class", "rounded-3");
                _fileControl.FolderPath = ImagePath;
                _fileControl.EnableViewState = true;
                _fileControl.ViewStateMode = System.Web.UI.ViewStateMode.Enabled;
                _fileControl.UsePersonalFolder = false;
                _fileControl.User = null;
                chkActive.InputAttributes.Add("class", "form-check-input");
                chkActive.LabelAttributes.Add("class", "form-check-label");
                chkManateeAccess.InputAttributes.Add("class", "form-check-input");
                chkManateeAccess.LabelAttributes.Add("class", "form-check-label");

                if (!Page.IsPostBack)
                {
                    JavaScript.RequestRegistration(CommonJs.jQuery);
                    PopulateDropDowns();
                    var ctl = new EmployeeController();
                    Employee employee = ctl.GetEmployee(EmployeeId);
                    if (employee != null)
                    {
                        _fileControl.FileID = employee.PhotoFileId;
                        if (employee.BirthDate.HasValue)
                        {
                            txtBirthDate.Text = employee.BirthDate.Value.ToShortDateString();
                        }
                        if (employee.PhotoFileId > 0)
                        {
                            imgEmployee.ImageUrl = string.Format("/DnnImageHandler.ashx?mode=securefile&fileId={0}&MaxHeight=25", employee.PhotoFileId);
                            lnkThumbnail.NavigateUrl = imgEmployee.ImageUrl;
                        }

                        if (employee.HireDate.HasValue)
                        {
                            txtHireDate.Text = employee.HireDate.Value.ToShortDateString();
                        }
                        if (employee.ServiceDate.HasValue)
                        {
                            txtServiceDate.Text = employee.ServiceDate.Value.ToShortDateString();
                        }
                        if (employee.TerminationDate.HasValue)
                        {
                            txtTerminationDate.Text = employee.TerminationDate.Value.ToShortDateString();
                        }
                        if (employee.ClassId.HasValue)
                        {
                            drpJobClass.SelectedValue = employee.ClassId.ToString();
                        }
                        if (employee.CountyId.HasValue)
                        {
                            drpCounty.SelectedValue = employee.CountyId.ToString();
                        }
                        if (employee.DepartmentId.HasValue)
                        {
                            drpDepartment.SelectedValue = employee.DepartmentId.ToString();
                        }
                        if (employee.JobGroupId.HasValue)
                        {
                            drpJobGroup.SelectedValue = employee.JobGroupId.ToString();
                        }
                        if (employee.OfficeLocationId.HasValue)
                        {
                            drpLocation.SelectedValue = employee.OfficeLocationId.ToString();
                        }
                        if (employee.SupervisorId.HasValue)
                        {
                            drpSupervisor.SelectedValue = employee.SupervisorId.ToString();
                        }
                        if (employee.AnnualLeaveBalance.HasValue)
                        {
                            txtAnnualLeave.Text = employee.AnnualLeaveBalance.Value.ToString("C", CultureInfo.CurrentCulture);
                        }
                        if (employee.Salary.HasValue)
                        {
                            txtSalary.Text = employee.Salary.Value.ToString("C", CultureInfo.CurrentCulture);
                        }
                        if (employee.SickLeaveBalance.HasValue)
                        {
                            txtSickLeave.Text = employee.SickLeaveBalance.Value.ToString("C", CultureInfo.CurrentCulture);
                        }

                        txtAddress.Text = employee.Address;
                        rblWorksFor.SelectedValue = employee.AgencyOfEmployment;
                        txtCity.Text = employee.City;
                        txtDeSotoAccess.Text = employee.DesotoAccess;
                        txtEmail.Text = employee.Email;
                        drpEmploymentType.SelectedValue = employee.EmploymentType;
                        txtFirstName.Text = employee.FirstName;
                        txtLastName.Text = employee.LastName;
                        rblGender.SelectedValue = employee.Gender;
                        txtTitle.Text = employee.JobTitle;
                        if (employee.ManateeAccess.HasValue)
                        {
                            chkManateeAccess.Checked = employee.ManateeAccess.Value;
                        }
                        txtMiddleInitial.Text = employee.MiddleInitial;
                        txtPersonalEmail.Text = employee.PersonalEmail;
                        txtPosition.Text = employee.Position;
                        drpRace.SelectedValue = employee.Race;
                        txtSarasotaAccess.Text = employee.SarasotaAccess;
                        txtSSN.Text = employee.SocialSecurityNumber;
                        drpState.SelectedValue = employee.State;
                        txtZip.Text = employee.Zip;
                        if (employee.IsActive.HasValue)
                        {
                            chkActive.Checked = employee.IsActive.Value;
                        }
                    }
                }
                phFileUpload.Controls.Add(_fileControl);

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
                SwnInterface.AddContactToSwnGroup(groups.ToList(), EmployeeId.ToString(), SwnServiceIdentifier, SwnSubscriptionKey, token);

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
                            ltMessage.Text = "<div class='alert alert-danger'><i class='fas fa-exclamation-circle'></i> Failed to Remove Contact from SWN Group</div>";
                            string process = string.Format("Remove Group Id:{0} from Employee Id:{1}", groupId, EmployeeId);
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

            if (DateTime.TryParse(txtBirthDate.Text, out DateTime birthDate))
            {
                t.BirthDate = birthDate;
            }
            if (DateTime.TryParse(txtHireDate.Text, out DateTime hireDate))
            {
                t.HireDate = hireDate;
            }
            if (DateTime.TryParse(txtServiceDate.Text, out DateTime serviceDate))
            {
                t.ServiceDate = serviceDate;
            }
            if (DateTime.TryParse(txtTerminationDate.Text, out DateTime terminationDate))
            {
                t.TerminationDate = terminationDate;
            }
            Int32.TryParse(drpJobClass.SelectedValue, out int classId);
            if (classId != 0)
            {
                t.ClassId = classId;
            }
            Int32.TryParse(drpCounty.SelectedValue, out int countyId);
            if (countyId != 0)
            {
                t.CountyId = countyId;
            }
            Int32.TryParse(drpDepartment.SelectedValue, out int departmentId);
            if (departmentId != 0)
            {
                t.DepartmentId = departmentId;
            }
            Int32.TryParse(drpJobGroup.SelectedValue, out int groupId);
            if (groupId != 0)
            {
                t.JobGroupId = groupId;
            }
            Int32.TryParse(drpLocation.SelectedValue, out int locationId);
            if (locationId != 0)
            {
                t.OfficeLocationId = locationId;
            }
            Int32.TryParse(drpSupervisor.SelectedValue, out int supervisorId);
            if (supervisorId != 0)
            {
                t.SupervisorId = supervisorId;
            }
            Decimal.TryParse(txtAnnualLeave.Text, out decimal annualLeave);
            if (annualLeave > 0)
            {
                t.AnnualLeaveBalance = annualLeave;
            }

            Decimal.TryParse(Helper.CleanDecimal(txtSalary.Text), out decimal salary);
            if (salary > 0)
            {
                t.Salary = salary;
            }
            Decimal.TryParse(txtSickLeave.Text, out decimal sickLeave);
            if (sickLeave > 0)
            {
                t.SickLeaveBalance = sickLeave;
            }
            DotNetNuke.Web.UI.WebControls.DnnFilePickerUploader _fileControl = phFileUpload.Controls[0] as DotNetNuke.Web.UI.WebControls.DnnFilePickerUploader;
            t.PhotoFileId = _fileControl.FileID;
            t.Address = txtAddress.Text;
            t.AgencyOfEmployment = rblWorksFor.SelectedValue;
            t.City = txtCity.Text;
            t.DesotoAccess = txtDeSotoAccess.Text;
            t.Email = txtEmail.Text;
            t.EmploymentType = drpEmploymentType.SelectedValue;
            t.FirstName = txtFirstName.Text;
            t.LastName = txtLastName.Text;
            t.Gender = rblGender.SelectedValue;
            t.JobTitle = txtTitle.Text;
            t.LastModifiedById = UserId;
            t.LastModifiedDate = DateTime.Now;
            t.ManateeAccess = chkManateeAccess.Checked;
            t.MiddleInitial = txtMiddleInitial.Text;
            t.PersonalEmail = txtPersonalEmail.Text;
            t.Position = txtPosition.Text;
            t.Race = drpRace.SelectedValue;
            t.SarasotaAccess = txtSarasotaAccess.Text;
            t.SocialSecurityNumber = txtSSN.Text.Replace("-", "");
            t.State = drpState.SelectedValue;
            t.Zip = txtZip.Text;
            t.IsActive = chkActive.Checked;
            t.IsEmployee = true;
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
        protected void upEmployee_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });

        }
        protected void upGroups_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });

        }
        #endregion

        #region Methods

        private void CheckImagePath(string imagePath)
        {
            DotNetNuke.Services.FileSystem.FolderManager objFolder = new DotNetNuke.Services.FileSystem.FolderManager();
            if (objFolder.FolderExists(PortalId, imagePath) == false)
            {
                objFolder.AddFolder(PortalId, imagePath);
            }
        }
        private void PopulateGroupLists(GroupController dCtl)
        {
            lsGroups.DataSource = dCtl.GetGroupsExcludingMembership(EmployeeId);
            lsGroups.DataBind();
            lsMembership.DataSource = dCtl.GetGroupMemberships(EmployeeId);
            lsMembership.DataBind();
        }
        private void PopulateDropDowns()
        {

            var rCtl = new RaceController();
            drpRace.DataSource = rCtl.GetRaces().OrderBy(x => x.Description);
            drpRace.DataBind();

            var lCtl = new OfficeLocationController();
            drpLocation.DataSource = lCtl.GetOfficeLocations().OrderBy(x => x.Description);
            drpLocation.DataBind();

            var dCtl = new GroupController();
            drpDepartment.DataSource = dCtl.GetGroups(0).OrderBy(x => x.GroupName);
            drpDepartment.DataBind();


            var gCtl = new JobGroupController();
            drpJobGroup.DataSource = gCtl.GetJobGroups().OrderBy(x => x.Description);
            drpJobGroup.DataBind();

            var cCtl = new JobClassController();
            drpJobClass.DataSource = cCtl.GetJobClasses().OrderBy(x => x.ClassName);
            drpJobClass.DataBind();

            var cnCtl = new Globals.CountyController();
            drpCounty.DataSource = cnCtl.GetCounties().OrderBy(x => x.CountyName);
            drpCounty.DataBind();

            var eCtl = new EmployeeController();
            drpSupervisor.DataSource = eCtl.GetEmployeeDropDown(SupervisorRole).OrderBy(x => x.DataText);
            drpSupervisor.DataBind();
            PopulateGroupLists(dCtl);
        }

        #endregion


    }
}