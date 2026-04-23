using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Views
{
    public partial class EditEmployee : EmployeeDBModuleBase
    {
        private readonly EmployeeController _employeeController = new EmployeeController();
        private readonly RaceController _raceController = new RaceController();
        private readonly OfficeLocationController _locationController = new OfficeLocationController();
        private readonly CountyController _countyController = new CountyController();
        private readonly JobGroupController _jobGroupController = new JobGroupController();
        private readonly JobClassController _jobClassController = new JobClassController();
        private readonly GroupController _groupController = new GroupController();
        private readonly GroupMembershipController _membershipController = new GroupMembershipController();
        private readonly PhoneController _phoneController = new PhoneController();
        private readonly EmergencyContactController _contactController = new EmergencyContactController();
        private readonly PositionHistoryController _positionController = new PositionHistoryController();
        private readonly ServiceHistoryController _serviceController = new ServiceHistoryController();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsHrAdmin)
                {
                    Response.Redirect(HomeUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                cmdCancel.NavigateUrl = HomeUrl;

                if (!IsPostBack)
                {
                    BindRaces();
                    BindCounties();
                    BindLocations();
                    BindSupervisors();
                    BindDepartments();
                    BindJobGroups();
                    BindClasses();
                    BindGroupsList();

                    if (EmployeeId > 0)
                    {
                        LoadEmployee();
                        cmdDelete.Visible = true;
                    }
                    else
                    {
                        lblEmployeeName.Text = "(New Employee)";
                        chkIsEmployee.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        #region Binding Helpers

        private void BindRaces()
        {
            drpRace.Items.Clear();
            drpRace.Items.Add(new ListItem("", ""));
            foreach (var r in _raceController.GetAll())
                drpRace.Items.Add(new ListItem(r.Description, r.RaceCode));
        }

        private void BindCounties()
        {
            drpCounty.Items.Clear();
            drpCounty.Items.Add(new ListItem("", ""));
            foreach (var c in _countyController.GetAll().OrderBy(x => x.CountyName))
                drpCounty.Items.Add(new ListItem(c.CountyName, c.CountyId.ToString()));
        }

        private void BindLocations()
        {
            drpOfficeLocation.Items.Clear();
            drpOfficeLocation.Items.Add(new ListItem("", ""));
            foreach (var l in _locationController.GetAll().OrderBy(x => x.Description))
                drpOfficeLocation.Items.Add(new ListItem(l.Description, l.OfficeLocationId.ToString()));
        }

        private void BindSupervisors()
        {
            drpSupervisor.Items.Clear();
            drpSupervisor.Items.Add(new ListItem("", ""));
            foreach (var s in _employeeController.GetSupervisors())
                drpSupervisor.Items.Add(new ListItem(s.DisplayName, s.EmployeeId.ToString()));
        }

        private void BindDepartments()
        {
            drpDepartment.Items.Clear();
            drpDepartment.Items.Add(new ListItem("", ""));
            foreach (var g in _groupController.GetAll().OrderBy(x => x.GroupName))
                drpDepartment.Items.Add(new ListItem(g.GroupName, g.GroupID.ToString()));
        }

        private void BindJobGroups()
        {
            drpJobGroup.Items.Clear();
            drpJobGroup.Items.Add(new ListItem("", ""));
            foreach (var g in _jobGroupController.GetAll().OrderBy(x => x.Description))
                drpJobGroup.Items.Add(new ListItem(g.Description, g.JobGroupId.ToString()));
        }

        private void BindClasses()
        {
            drpClass.Items.Clear();
            drpClass.Items.Add(new ListItem("", ""));
            foreach (var c in _jobClassController.GetAll().OrderBy(x => x.ClassName))
                drpClass.Items.Add(new ListItem(c.ClassName, c.ClassId.ToString()));
        }

        private void BindGroupsList()
        {
            lstGroups.DataSource = _groupController.GetAll().OrderBy(x => x.GroupName);
            lstGroups.DataBind();
        }

        private void BindGroupMembership()
        {
            if (EmployeeId <= 0) return;
            var current = _membershipController.GetForEmployee(EmployeeId)
                                               .Select(m => m.GroupId.ToString())
                                               .ToList();
            foreach (ListItem li in lstGroups.Items)
                li.Selected = current.Contains(li.Value);
        }

        private void BindPositionHistory(string ssn)
        {
            rptPositionHistory.DataSource = string.IsNullOrEmpty(ssn)
                ? null
                : _positionController.GetForSsn(ssn);
            rptPositionHistory.DataBind();
        }

        private void BindServiceHistory(string ssn)
        {
            rptServiceHistory.DataSource = string.IsNullOrEmpty(ssn)
                ? null
                : _serviceController.GetForSsn(ssn);
            rptServiceHistory.DataBind();
        }

        private void BindPhones()
        {
            rptPhones.DataSource = EmployeeId > 0 ? _phoneController.GetForEmployee(EmployeeId) : null;
            rptPhones.DataBind();
        }

        private void BindContacts()
        {
            rptContacts.DataSource = EmployeeId > 0 ? _contactController.GetForEmployee(EmployeeId) : null;
            rptContacts.DataBind();
        }

        private void SelectItemByValue(DropDownList ddl, string value)
        {
            if (ddl == null || value == null) return;
            var item = ddl.Items.FindByValue(value);
            if (item != null)
            {
                ddl.ClearSelection();
                item.Selected = true;
            }
        }

        private void SelectItemByText(DropDownList ddl, string text)
        {
            if (ddl == null || text == null) return;
            var item = ddl.Items.FindByText(text);
            if (item != null)
            {
                ddl.ClearSelection();
                item.Selected = true;
            }
        }

        #endregion

        #region Load Employee

        private void LoadEmployee()
        {
            var emp = _employeeController.GetEmployee(EmployeeId);
            if (emp == null)
            {
                lblEmployeeName.Text = "(Not Found)";
                return;
            }

            lblEmployeeName.Text = emp.DisplayName;

            txtFirstName.Text = emp.FirstName ?? string.Empty;
            txtLastName.Text = emp.LastName ?? string.Empty;
            txtMiddleInitial.Text = emp.MiddleInitial ?? string.Empty;
            txtJobTitle.Text = emp.JobTitle ?? string.Empty;
            txtSsn.Text = emp.SocialSecurityNumber ?? string.Empty;
            if (emp.BirthDate.HasValue)
                txtBirthDate.Text = emp.BirthDate.Value.ToString("yyyy-MM-dd");

            SelectItemByValue(drpRace, emp.Race);
            SelectItemByValue(drpGender, emp.Gender);
            SelectItemByValue(drpAgency, emp.AgencyOfEmployment);

            txtAddress.Text = emp.Address ?? string.Empty;
            txtCity.Text = emp.City ?? string.Empty;
            txtState.Text = emp.State ?? string.Empty;
            txtZip.Text = emp.Zip ?? string.Empty;
            SelectItemByValue(drpCounty, emp.CountyId?.ToString());

            txtEmail.Text = emp.Email ?? string.Empty;
            txtPersonalEmail.Text = emp.PersonalEmail ?? string.Empty;

            SelectItemByValue(drpOfficeLocation, emp.OfficeLocationId?.ToString());
            txtLocationName.Text = emp.LocationName ?? string.Empty;
            SelectItemByValue(drpSupervisor, emp.SupervisorId?.ToString());
            SelectItemByValue(drpDepartment, emp.DepartmentId?.ToString());
            SelectItemByValue(drpJobGroup, emp.JobGroupId?.ToString());
            SelectItemByValue(drpClass, emp.ClassId?.ToString());

            txtPosition.Text = emp.Position ?? string.Empty;
            SelectItemByValue(drpEmploymentType, emp.EmploymentType);
            txtSalary.Text = emp.Salary.HasValue ? emp.Salary.Value.ToString("0.##") : string.Empty;

            if (emp.HireDate.HasValue)
                txtHireDate.Text = emp.HireDate.Value.ToString("yyyy-MM-dd");
            if (emp.ServiceDate.HasValue)
                txtServiceDate.Text = emp.ServiceDate.Value.ToString("yyyy-MM-dd");
            if (emp.TerminationDate.HasValue)
                txtTerminationDate.Text = emp.TerminationDate.Value.ToString("yyyy-MM-dd");

            txtAnnualLeave.Text = emp.AnnualLeaveBalance.HasValue
                ? emp.AnnualLeaveBalance.Value.ToString("0.##") : string.Empty;
            txtSickLeave.Text = emp.SickLeaveBalance.HasValue
                ? emp.SickLeaveBalance.Value.ToString("0.##") : string.Empty;
            txtBadgeNumber.Text = emp.BadgeNumber ?? string.Empty;
            txtSwnGroupId.Text = emp.SwnGroupId ?? string.Empty;

            lblUserId.Text = emp.UserId.HasValue && emp.UserId.Value > 0
                ? emp.UserId.Value.ToString()
                : "(not associated)";

            chkIsActive.Checked = emp.IsActive.GetValueOrDefault();
            chkIsEmployee.Checked = emp.IsEmployee;
            chkManateeAccess.Checked = emp.ManateeAccess.GetValueOrDefault();
            txtSarasotaAccess.Text = emp.SarasotaAccess ?? string.Empty;
            txtDesotoAccess.Text = emp.DesotoAccess ?? string.Empty;

            hdnPhotoFileId.Value = emp.PhotoFileId.HasValue ? emp.PhotoFileId.Value.ToString() : string.Empty;
            if (emp.PhotoFileId.HasValue && emp.PhotoFileId.Value > 0)
            {
                var fi = FileManager.Instance.GetFile(emp.PhotoFileId.Value);
                if (fi != null)
                    imgPhoto.ImageUrl = FileManager.Instance.GetUrl(fi);
            }

            BindGroupMembership();
            BindPositionHistory(emp.SocialSecurityNumber);
            BindServiceHistory(emp.SocialSecurityNumber);
            BindPhones();
            BindContacts();
        }

        #endregion

        #region Save / Delete

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid) return;

                EmployeeInfo emp;
                if (EmployeeId > 0)
                {
                    emp = _employeeController.GetEmployee(EmployeeId);
                    if (emp == null) emp = new EmployeeInfo();
                }
                else
                {
                    emp = new EmployeeInfo();
                }

                emp.FirstName = txtFirstName.Text.Trim();
                emp.LastName = txtLastName.Text.Trim();
                emp.MiddleInitial = string.IsNullOrWhiteSpace(txtMiddleInitial.Text) ? null : txtMiddleInitial.Text.Trim();
                emp.JobTitle = txtJobTitle.Text.Trim();
                emp.SocialSecurityNumber = txtSsn.Text.Trim();
                emp.BirthDate = ParseDate(txtBirthDate.Text);
                emp.Race = drpRace.SelectedValue;
                emp.Gender = drpGender.SelectedValue;
                emp.AgencyOfEmployment = drpAgency.SelectedValue;

                emp.Address = txtAddress.Text.Trim();
                emp.City = txtCity.Text.Trim();
                emp.State = txtState.Text.Trim();
                emp.Zip = txtZip.Text.Trim();
                emp.CountyId = ParseIntOrNull(drpCounty.SelectedValue);

                emp.Email = txtEmail.Text.Trim();
                emp.PersonalEmail = txtPersonalEmail.Text.Trim();

                emp.OfficeLocationId = ParseIntOrNull(drpOfficeLocation.SelectedValue);
                emp.LocationName = txtLocationName.Text.Trim();
                emp.SupervisorId = ParseIntOrNull(drpSupervisor.SelectedValue);
                emp.DepartmentId = ParseIntOrNull(drpDepartment.SelectedValue);
                emp.JobGroupId = ParseIntOrNull(drpJobGroup.SelectedValue);
                emp.ClassId = ParseIntOrNull(drpClass.SelectedValue);

                emp.Position = txtPosition.Text.Trim();
                emp.EmploymentType = drpEmploymentType.SelectedValue;
                emp.Salary = ParseDecimalOrNull(txtSalary.Text);

                emp.HireDate = ParseDate(txtHireDate.Text);
                emp.ServiceDate = ParseDate(txtServiceDate.Text);
                emp.TerminationDate = ParseDate(txtTerminationDate.Text);

                emp.AnnualLeaveBalance = ParseDecimalOrNull(txtAnnualLeave.Text);
                emp.SickLeaveBalance = ParseDecimalOrNull(txtSickLeave.Text);
                emp.BadgeNumber = txtBadgeNumber.Text.Trim();
                emp.SwnGroupId = txtSwnGroupId.Text.Trim();

                emp.IsActive = chkIsActive.Checked;
                emp.IsEmployee = chkIsEmployee.Checked;
                emp.ManateeAccess = chkManateeAccess.Checked;
                emp.SarasotaAccess = txtSarasotaAccess.Text.Trim();
                emp.DesotoAccess = txtDesotoAccess.Text.Trim();

                // Photo upload
                if (fuPhoto.HasFile)
                {
                    var folder = FolderManager.Instance.GetFolder(PortalId, "Employee-Photos")
                                 ?? FolderManager.Instance.AddFolder(PortalId, "Employee-Photos");
                    var file = FileManager.Instance.AddFile(folder, fuPhoto.FileName, fuPhoto.PostedFile.InputStream, true);
                    if (file != null)
                    {
                        emp.PhotoFileId = file.FileId;
                        hdnPhotoFileId.Value = file.FileId.ToString();
                    }
                }
                else if (!string.IsNullOrEmpty(hdnPhotoFileId.Value) && int.TryParse(hdnPhotoFileId.Value, out int existingFileId))
                {
                    emp.PhotoFileId = existingFileId;
                }

                int savedId;
                if (EmployeeId > 0)
                {
                    _employeeController.UpdateEmployee(emp, UserId);
                    savedId = EmployeeId;
                }
                else
                {
                    savedId = _employeeController.CreateEmployee(emp, UserId);
                }

                SaveGroupMembership(savedId);

                Response.Redirect(_navigationManager.NavigateURL(TabId, "", "EmployeeId=" + savedId), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        private void SaveGroupMembership(int employeeId)
        {
            if (employeeId <= 0) return;

            var existing = _membershipController.GetForEmployee(employeeId).ToList();
            foreach (var m in existing)
                _membershipController.DeleteMembership(m.GroupId, employeeId);

            foreach (ListItem li in lstGroups.Items)
            {
                if (li.Selected && int.TryParse(li.Value, out int gid))
                    _membershipController.AddMembership(gid, employeeId, UserId);
            }
        }

        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (EmployeeId > 0)
                {
                    _employeeController.DeleteEmployee(EmployeeId);
                }
                Response.Redirect(HomeUrl, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        #endregion

        #region Groups Tab

        protected void cmdSaveGroups_Click(object sender, EventArgs e)
        {
            try
            {
                if (EmployeeId <= 0)
                {
                    ltMessage.Text = "<div class=\"alert alert-warning\">Save the employee first before assigning groups.</div>";
                    return;
                }
                SaveGroupMembership(EmployeeId);
                ltMessage.Text = "<div class=\"alert alert-success\">Group membership saved.</div>";
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        #endregion

        #region Position / Service History

        protected void cmdAddPosition_Click(object sender, EventArgs e)
        {
            try
            {
                if (EmployeeId <= 0 || string.IsNullOrWhiteSpace(txtSsn.Text))
                {
                    ltMessage.Text = "<div class=\"alert alert-warning\">Save the employee (with an SSN) first before adding position history.</div>";
                    return;
                }

                var item = new PositionHistoryInfo
                {
                    SocialSecurityNumber = txtSsn.Text.Trim(),
                    StartDate = ParseDate(txtPosStartDate.Text),
                    EndDate = ParseDate(txtPosEndDate.Text),
                    Description = txtPosDescription.Text.Trim(),
                    EntryType = txtPosEntryType.Text.Trim(),
                    IsInternal = chkPosInternal.Checked
                };
                _positionController.Create(item, UserId);

                txtPosStartDate.Text = string.Empty;
                txtPosEndDate.Text = string.Empty;
                txtPosDescription.Text = string.Empty;
                txtPosEntryType.Text = string.Empty;
                chkPosInternal.Checked = false;

                BindPositionHistory(txtSsn.Text.Trim());
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected void rptPositionHistory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete" && int.TryParse(e.CommandArgument?.ToString(), out int id))
            {
                _positionController.Delete(id);
                BindPositionHistory(txtSsn.Text.Trim());
            }
        }

        protected void cmdAddService_Click(object sender, EventArgs e)
        {
            try
            {
                if (EmployeeId <= 0 || string.IsNullOrWhiteSpace(txtSsn.Text))
                {
                    ltMessage.Text = "<div class=\"alert alert-warning\">Save the employee (with an SSN) first before adding service history.</div>";
                    return;
                }

                var item = new ServiceHistoryInfo
                {
                    SocialSecurityNumber = txtSsn.Text.Trim(),
                    CompanyName = txtSvcCompanyName.Text.Trim(),
                    HireDate = ParseDate(txtSvcHireDate.Text),
                    TerminationDate = ParseDate(txtSvcTerminationDate.Text),
                    LastPayRate = ParseDecimalOrNull(txtSvcLastPayRate.Text)
                };
                _serviceController.Create(item, UserId);

                txtSvcCompanyName.Text = string.Empty;
                txtSvcHireDate.Text = string.Empty;
                txtSvcTerminationDate.Text = string.Empty;
                txtSvcLastPayRate.Text = string.Empty;

                BindServiceHistory(txtSsn.Text.Trim());
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected void rptServiceHistory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete" && int.TryParse(e.CommandArgument?.ToString(), out int id))
            {
                _serviceController.Delete(id);
                BindServiceHistory(txtSsn.Text.Trim());
            }
        }

        #endregion

        #region Phones

        protected void cmdAddPhone_Click(object sender, EventArgs e)
        {
            try
            {
                if (EmployeeId <= 0)
                {
                    ltMessage.Text = "<div class=\"alert alert-warning\">Save the employee first before adding phones.</div>";
                    return;
                }

                var item = new PhoneInfo
                {
                    EmployeeId = EmployeeId,
                    PhoneType = drpPhoneType.SelectedValue,
                    PhoneNumber = txtPhoneNumber.Text.Trim(),
                    Extension = txtPhoneExtension.Text.Trim(),
                    IsMain = chkPhoneIsMain.Checked,
                    SwnCall = chkPhoneSwnCall.Checked,
                    SwnText = chkPhoneSwnText.Checked,
                    SwnExcludeExtension = chkPhoneSwnExcludeExt.Checked
                };
                _phoneController.Create(item, UserId);

                txtPhoneNumber.Text = string.Empty;
                txtPhoneExtension.Text = string.Empty;
                chkPhoneIsMain.Checked = false;
                chkPhoneSwnCall.Checked = false;
                chkPhoneSwnText.Checked = false;
                chkPhoneSwnExcludeExt.Checked = false;

                BindPhones();
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected void rptPhones_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete" && long.TryParse(e.CommandArgument?.ToString(), out long id))
            {
                _phoneController.Delete(id);
                BindPhones();
            }
        }

        #endregion

        #region Emergency Contacts

        protected void cmdAddContact_Click(object sender, EventArgs e)
        {
            try
            {
                if (EmployeeId <= 0)
                {
                    ltMessage.Text = "<div class=\"alert alert-warning\">Save the employee first before adding contacts.</div>";
                    return;
                }

                var item = new EmergencyContactInfo
                {
                    EmployeeId = EmployeeId,
                    FirstName = txtContactFirstName.Text.Trim(),
                    LastName = txtContactLastName.Text.Trim(),
                    Relationship = txtContactRelationship.Text.Trim(),
                    PhoneHome = txtContactPhoneHome.Text.Trim(),
                    PhoneWork = txtContactPhoneWork.Text.Trim(),
                    PhoneMobile = txtContactPhoneMobile.Text.Trim(),
                    CallOrder = int.TryParse(txtContactCallOrder.Text, out int co) ? co : (int?)null
                };
                _contactController.Create(item, UserId);

                txtContactFirstName.Text = string.Empty;
                txtContactLastName.Text = string.Empty;
                txtContactRelationship.Text = string.Empty;
                txtContactPhoneHome.Text = string.Empty;
                txtContactPhoneWork.Text = string.Empty;
                txtContactPhoneMobile.Text = string.Empty;
                txtContactCallOrder.Text = string.Empty;

                BindContacts();
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected void rptContacts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete" && int.TryParse(e.CommandArgument?.ToString(), out int id))
            {
                _contactController.Delete(id);
                BindContacts();
            }
        }

        #endregion

        #region DNN User Association

        protected void cmdAssociateUser_Click(object sender, EventArgs e)
        {
            pnlSelectUser.Visible = true;
        }

        protected void cmdSelectUserSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (EmployeeId <= 0)
                {
                    ltMessage.Text = "<div class=\"alert alert-warning\">Save the employee first before associating a user.</div>";
                    pnlSelectUser.Visible = false;
                    return;
                }
                if (int.TryParse(txtSelectUserId.Text, out int uid) && uid > 0)
                {
                    _employeeController.SetUserId(EmployeeId, uid, UserId);
                    lblUserId.Text = uid.ToString();
                }
                pnlSelectUser.Visible = false;
                txtSelectUserId.Text = string.Empty;
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected void cmdSelectUserCancel_Click(object sender, EventArgs e)
        {
            pnlSelectUser.Visible = false;
            txtSelectUserId.Text = string.Empty;
        }

        #endregion

        #region Utility

        private DateTime? ParseDate(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            return DateTime.TryParse(text, out DateTime dt) ? dt : (DateTime?)null;
        }

        private int? ParseIntOrNull(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            return int.TryParse(text, out int i) ? i : (int?)null;
        }

        private decimal? ParseDecimalOrNull(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            return decimal.TryParse(text, out decimal d) ? d : (decimal?)null;
        }

        #endregion
    }
}
