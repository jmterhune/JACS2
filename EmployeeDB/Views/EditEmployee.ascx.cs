using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
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
        // _groupController stays — used for the Department dropdown bind +
        // the change-notification email body's Department lookup.
        private readonly GroupController _groupController = new GroupController();
        // _phoneController is only used by the helpdesk-notification path to
        // snapshot + diff phones around a Save. The Phones tab itself is
        // driven by the Web API (Components/Api/PhonesController.cs).
        private readonly PhoneController _phoneController = new PhoneController();

        /// <summary>Phone types HR is willing to share with the helpdesk in the
        /// employee-add / employee-update notification email. Anything outside
        /// this set (Personal, Mobile, Home, etc.) is considered PII and is
        /// excluded from the email body.</summary>
        private static readonly HashSet<string> HelpdeskPhoneTypes =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "Work", "Work Cell", "Judicial Office" };

        /// <summary>ViewState key for the phones-at-page-load snapshot. Phones
        /// are persisted by their own Web API mid-edit (out-of-band from this
        /// page's postbacks), so we capture the pre-edit state when the page
        /// first loads and diff against the current DB state at Save time.</summary>
        private const string PhonesSnapshotKey = "EditEmp_PhonesSnapshot";

        // The Phones / Positions / Services / Contacts / Groups membership /
        // Photo tabs are now driven entirely by the Web API + JS
        // (Components/Api/*Controller.cs + Scripts/empdb-edit.js).
        // No server-side controller fields are needed for them on this page.

        /// <summary>SSN of the currently-loaded employee, exposed to markup so the
        /// JS layer can pass it through on Position / Service POSTs (the server
        /// stores those keyed by SSN, not EmployeeId). Empty for new employees.</summary>
        protected string EmployeeSsn { get; private set; }

        /// <summary>URL to the currently-saved photo, if any. Used by the Photo
        /// tab's initial render — afterwards the JS replaces this src as the
        /// user drops in a new file.</summary>
        protected string EmployeePhotoUrl { get; private set; } = "";

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

                // Emit the DNN ServicesFramework AntiForgery token so the JS
                // layer can post to the Web API. This adds a hidden
                // __RequestVerificationToken input to the form.
                DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

                cmdCancel.NavigateUrl = HomeUrl;
                // Top-of-form Back to List link points at the EmployeeList
                // page (same TabId, no ctl parameter) so the user lands back
                // on the list with their previous DataTables state restored.
                cmdBackToList.NavigateUrl = BuildListUrl();

                if (!IsPostBack)
                {
                    BindRaces();
                    BindCounties();
                    BindLocations();
                    BindSupervisors();
                    BindDepartments();
                    BindJobGroups();
                    BindClasses();
                    // Phones / Groups / Photo / Position History / Service History /
                    // Emergency Contacts tabs are all loaded client-side via the API.
                    // The Phone modal's Location <select> options are rendered
                    // inline in markup via GetPhoneLocationOptions().

                    if (EmployeeId > 0)
                    {
                        LoadEmployee();
                        cmdDelete.Visible = true;
                    }
                    else
                    {
                        lblEmployeeName.Text = "(New Employee)";
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

        /// <summary>Renders the &lt;option&gt; tags for the modal's Location
        /// &lt;select&gt;. Called inline from the markup — keeps the dropdown
        /// as plain HTML (no asp:DropDownList) so the JS's
        /// [name="OfficeLocationId"] selector matches.</summary>
        protected string GetPhoneLocationOptions()
        {
            var sb = new StringBuilder();
            foreach (var l in _locationController.GetAll().OrderBy(x => x.Description))
            {
                sb.Append("<option value=\"")
                  .Append(l.OfficeLocationId)
                  .Append("\">")
                  .Append(Server.HtmlEncode(l.Description ?? ""))
                  .Append("</option>");
            }
            return sb.ToString();
        }

        private void BindSupervisors()
        {
            // Pull the roster from tjc_supervisor (managed via the
            // Supervisors admin tab on EmployeeList). Each option is tagged
            // with data-supgroup="active" or "inactive"; empdb-edit.js runs
            // on page load and wraps each group in a native <optgroup
            // label="Active|Inactive"> so the section headers render as
            // proper grouped options. Inactive options get a disabled
            // attribute too — they stay visible for already-assigned
            // employees but can't be picked for new saves.
            drpSupervisor.Items.Clear();
            drpSupervisor.Items.Add(new ListItem("", ""));

            var roster = _employeeController.GetSupervisors().ToList();
            var active   = roster.Where(s =>  s.IsActive).OrderBy(s => s.LastName).ThenBy(s => s.FirstName);
            var inactive = roster.Where(s => !s.IsActive).OrderBy(s => s.LastName).ThenBy(s => s.FirstName);

            foreach (var s in active)
            {
                var li = new ListItem(s.LastName + ", " + s.FirstName, s.EmployeeId.ToString());
                li.Attributes["data-supgroup"] = "active";
                drpSupervisor.Items.Add(li);
            }
            foreach (var s in inactive)
            {
                var li = new ListItem(s.LastName + ", " + s.FirstName, s.EmployeeId.ToString());
                li.Attributes["data-supgroup"] = "inactive";
                li.Attributes["disabled"]      = "disabled";
                li.Attributes["class"]         = "text-muted";
                drpSupervisor.Items.Add(li);
            }
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

        // Group membership is loaded/saved client-side via the Web API
        // (Components/Api/MembershipsController.cs + empdb-edit.js#groups).

        // Phones / Position History / Service History / Emergency Contacts
        // tabs are all driven by the Web API now (Components/Api/*Controller.cs
        // + Scripts/empdb-edit.js). No server-side bind methods are needed.

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

        /// <summary>Selects the matching item in a RadioButtonList by value;
        /// silently no-ops if the value isn't present (e.g. legacy data).</summary>
        private void SelectListItem(RadioButtonList rbl, string value)
        {
            if (rbl == null || string.IsNullOrEmpty(value)) return;
            var item = rbl.Items.FindByValue(value);
            if (item != null)
            {
                rbl.ClearSelection();
                item.Selected = true;
            }
        }

        /// <summary>Splits the single Address column into Line 1 / Line 2 for the
        /// form. Anything past the first newline goes into Line 2 (preserves
        /// further blank lines via Environment.NewLine join).</summary>
        private static Tuple<string, string> SplitAddressLines(string address)
        {
            if (string.IsNullOrEmpty(address)) return Tuple.Create(string.Empty, string.Empty);
            var idx = address.IndexOf('\n');
            if (idx < 0) return Tuple.Create(address, string.Empty);
            var line1 = address.Substring(0, idx).TrimEnd('\r');
            var line2 = address.Substring(idx + 1);
            return Tuple.Create(line1, line2);
        }

        /// <summary>Joins Line 1 + Line 2 for storage in the single Address column.
        /// Empty Line 2 collapses to just Line 1 (no trailing newline).</summary>
        private static string JoinAddressLines(string line1, string line2)
        {
            line1 = (line1 ?? string.Empty).Trim();
            line2 = (line2 ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(line2)) return line1;
            if (string.IsNullOrEmpty(line1)) return line2;
            return line1 + "\n" + line2;
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
            EmployeeSsn = emp.SocialSecurityNumber ?? string.Empty;
            if (emp.BirthDate.HasValue)
                txtBirthDate.Text = emp.BirthDate.Value.ToString("yyyy-MM-dd");

            SelectItemByValue(drpRace, emp.Race);
            SelectListItem(rblGender, emp.Gender);
            SelectListItem(rblAgency, emp.AgencyOfEmployment);

            // The DB stores both address lines in a single Address column,
            // joined by a newline. Split on the first newline to repopulate the
            // separate Line 1 / Line 2 inputs.
            var addrParts = SplitAddressLines(emp.Address);
            txtAddressLine1.Text = addrParts.Item1;
            txtAddressLine2.Text = addrParts.Item2;

            txtCity.Text = emp.City ?? string.Empty;
            SelectItemByValue(drpState, emp.State);
            txtZip.Text = emp.Zip ?? string.Empty;
            SelectItemByValue(drpCounty, emp.CountyId?.ToString());

            txtEmail.Text = emp.Email ?? string.Empty;
            txtPersonalEmail.Text = emp.PersonalEmail ?? string.Empty;

            SelectItemByValue(drpOfficeLocation, emp.OfficeLocationId?.ToString());
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

            // DROP / Certification dates (Employee Reports module reads these).
            txtDropEntryDate.Text = emp.DropEntryDate.HasValue
                ? emp.DropEntryDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            txtDropExitDate.Text = emp.DropExitDate.HasValue
                ? emp.DropExitDate.Value.ToString("yyyy-MM-dd") : string.Empty;
            txtDropLeavePayout.Text = emp.DropLeavePayout.HasValue
                ? emp.DropLeavePayout.Value.ToString("0.##") : string.Empty;
            txtCertificationDate.Text = emp.CertificationDate.HasValue
                ? emp.CertificationDate.Value.ToString("yyyy-MM-dd") : string.Empty;

            chkIsActive.Checked = emp.IsActive.GetValueOrDefault();
            
            chkManateeAccess.Checked = emp.ManateeAccess.GetValueOrDefault();
            txtSarasotaAccess.Text = emp.SarasotaAccess ?? string.Empty;
            txtDesotoAccess.Text = emp.DesotoAccess ?? string.Empty;

            // Photo URL is rendered into the markup via EmployeePhotoUrl —
            // the Photo tab's drag-drop control handles upload/replace via the API.
            if (emp.FileId.HasValue && emp.FileId.Value > 0)
            {
                var fi = FileManager.Instance.GetFile(emp.FileId.Value);
                if (fi != null)
                    EmployeePhotoUrl = FileManager.Instance.GetUrl(fi);
            }

            // Groups membership, Phones, Position / Service History, and
            // Emergency Contacts are all fetched client-side via the Web API
            // on tab load — see Scripts/empdb-edit.js.

            // Snapshot the helpdesk-visible phones so the change-notification
            // email at Save time can diff phones that were added / removed /
            // modified through the Phones tab during this edit session.
            ViewState[PhonesSnapshotKey] = SnapshotHelpdeskPhones(EmployeeId);
        }

        #endregion

        #region Save / Delete

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid) return;

                EmployeeInfo emp;
                EmployeeInfo before = null;
                if (EmployeeId > 0)
                {
                    emp = _employeeController.GetEmployee(EmployeeId);
                    if (emp == null) emp = new EmployeeInfo();
                    else before = CloneEmployee(emp);
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
                emp.Gender = rblGender.SelectedValue;
                emp.AgencyOfEmployment = rblAgency.SelectedValue;

                emp.Address = JoinAddressLines(txtAddressLine1.Text, txtAddressLine2.Text);
                emp.City = txtCity.Text.Trim();
                emp.State = drpState.SelectedValue;
                emp.Zip = txtZip.Text.Trim();
                emp.CountyId = ParseIntOrNull(drpCounty.SelectedValue);

                emp.Email = txtEmail.Text.Trim();
                emp.PersonalEmail = txtPersonalEmail.Text.Trim();

                emp.OfficeLocationId = ParseIntOrNull(drpOfficeLocation.SelectedValue);
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

                emp.DropEntryDate = ParseDate(txtDropEntryDate.Text);
                emp.DropExitDate = ParseDate(txtDropExitDate.Text);
                emp.DropLeavePayout = ParseDecimalOrNull(txtDropLeavePayout.Text);
                emp.CertificationDate = ParseDate(txtCertificationDate.Text);

                emp.IsActive = chkIsActive.Checked;
                emp.IsEmployee = true;
                emp.ManateeAccess = chkManateeAccess.Checked;
                emp.SarasotaAccess = txtSarasotaAccess.Text.Trim();
                emp.DesotoAccess = txtDesotoAccess.Text.Trim();

                // Photo upload is handled by the Photo tab's drag-drop API
                // (Components/Api/PhotosController.cs), so the Details Save
                // doesn't touch FileId. Anything that was already there stays.

                int savedId;
                // Capture whether this Save was for a brand-new hire BEFORE
                // we mutate the row — the post-save redirect for new hires
                // sends the user straight to the New Hire IT Worksheet view
                // so HR can immediately fill out the IT setup ticket.
                bool isNewHire = EmployeeId <= 0;
                if (EmployeeId > 0)
                {
                    _employeeController.UpdateEmployee(emp, UserId);
                    savedId = EmployeeId;
                }
                else
                {
                    savedId = _employeeController.CreateEmployee(emp, UserId);
                }

                // Group membership is saved by the Groups tab's own API call,
                // not by the Details-tab postback.

                // Helpdesk-notify on add/update (opt-in via Settings).
                if (NotifyOnSave)
                {
                    try
                    {
                        // Phones are stored via a separate Web API and may have
                        // been added/edited/deleted mid-session. Diff against the
                        // snapshot taken at page load (LoadEmployee) to surface
                        // those out-of-band changes in the helpdesk email.
                        var beforePhones = (ViewState[PhonesSnapshotKey] as List<string>)
                                           ?? new List<string>();
                        var afterPhones  = SnapshotHelpdeskPhones(savedId);

                        SendChangeNotification(before, emp, isNew: before == null,
                                               beforePhones, afterPhones);

                        // Refresh the snapshot so a follow-up Save in the same
                        // page lifecycle doesn't re-report the same phone diff.
                        ViewState[PhonesSnapshotKey] = afterPhones;
                    }
                    catch (Exception mailEx)
                    {
                        // Don't fail the save just because the email pipeline is down —
                        // log it through DNN's exception system and move on.
                        DotNetNuke.Services.Exceptions.Exceptions.LogException(mailEx);
                    }
                }

                if (isNewHire)
                {
                    // New hire: send the user straight to the New Hire IT
                    // Worksheet view (same module, NewHireIT controlKey) with
                    // the freshly-saved EmployeeId in the query string. The
                    // worksheet's code-behind reads it and pre-populates the
                    // form fields from the new employee's record.
                    var nhitUrl = _navigationManager.NavigateURL(
                        TabId,
                        "NewHireIT",
                        "mid=" + ModuleId,
                        "EmployeeId=" + savedId);
                    Response.Redirect(nhitUrl, false);
                }
                else
                {
                    // Existing employee: back to the list. ?empSaved=1 tells
                    // the list page to flash a success banner; ?empId=N tells
                    // it which row to highlight; DataTables stateSave restores
                    // their previous page/sort/filter automatically.
                    Response.Redirect(BuildListUrl("empSaved=1&empId=" + savedId), false);
                }
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        /// <summary>Show a transient success banner in the form's message area.</summary>
        private void ShowSavedMessage(string text)
        {
            ltMessage.Text = "<div class=\"alert alert-success\">" + Server.HtmlEncode(text) + "</div>";
        }

        /// <summary>No-op now that the form lives on its own page. Kept so the
        /// per-tab Add handlers don't need to be touched.</summary>
        private void NotifyParentSaved() { }

        /// <summary>URL of the EmployeeList page with optional query string tail
        /// (e.g. "empSaved=1"). Used when the form redirects after Save/Delete.</summary>
        private string BuildListUrl(string queryTail = null)
        {
            var url = _navigationManager.NavigateURL(TabId, "", "mid=" + ModuleId);
            if (string.IsNullOrEmpty(queryTail)) return url;
            var separator = url.IndexOf('?') >= 0 ? "&" : "?";
            return url + separator + queryTail;
        }

        #region Save-notification email

        /// <summary>Field-by-field shallow copy used to snapshot the BEFORE state
        /// so the Save handler can diff it against the post-mutation values.
        /// Only the fields the form touches are copied — anything we don't show
        /// in the Edit page can't have changed here.</summary>
        private static EmployeeInfo CloneEmployee(EmployeeInfo src)
        {
            if (src == null) return null;
            return new EmployeeInfo
            {
                EmployeeId = src.EmployeeId,
                FirstName = src.FirstName,
                LastName = src.LastName,
                MiddleInitial = src.MiddleInitial,
                JobTitle = src.JobTitle,
                SocialSecurityNumber = src.SocialSecurityNumber,
                BirthDate = src.BirthDate,
                Race = src.Race,
                Gender = src.Gender,
                AgencyOfEmployment = src.AgencyOfEmployment,
                Address = src.Address,
                City = src.City,
                State = src.State,
                Zip = src.Zip,
                CountyId = src.CountyId,
                Email = src.Email,
                PersonalEmail = src.PersonalEmail,
                OfficeLocationId = src.OfficeLocationId,
                SupervisorId = src.SupervisorId,
                DepartmentId = src.DepartmentId,
                JobGroupId = src.JobGroupId,
                ClassId = src.ClassId,
                Position = src.Position,
                EmploymentType = src.EmploymentType,
                Salary = src.Salary,
                HireDate = src.HireDate,
                ServiceDate = src.ServiceDate,
                TerminationDate = src.TerminationDate,
                AnnualLeaveBalance = src.AnnualLeaveBalance,
                SickLeaveBalance = src.SickLeaveBalance,
                BadgeNumber = src.BadgeNumber,
                IsActive = src.IsActive,
                IsEmployee = src.IsEmployee,
                ManateeAccess = src.ManateeAccess,
                SarasotaAccess = src.SarasotaAccess,
                DesotoAccess = src.DesotoAccess,
                FileId = src.FileId,
            };
        }

        private void SendChangeNotification(EmployeeInfo before, EmployeeInfo after, bool isNew,
                                            List<string> beforePhones, List<string> afterPhones)
        {
            var body = BuildChangeBody(before, after, isNew, beforePhones, afterPhones);
            if (string.IsNullOrEmpty(body)) return; // nothing actually changed

            var subject = isNew
                ? "Employee Added: " + after.DisplayName
                : "Employee Updated: " + after.DisplayName;

            // DotNetNuke.Services.Mail.Mail.SendEmail(fromAddress, toAddress,
            // subject, body). Returns void in this DNN version; failures are
            // logged into the EventLog by DNN itself.
            Mail.SendEmail(NotifyFromEmail, NotifyToEmail, subject, body);
        }

        /// <summary>Builds the plaintext email body listing changes between
        /// <paramref name="before"/> (null on Add) and <paramref name="after"/>.
        ///
        /// Only the fields HR has approved for the helpdesk are emitted —
        /// First/Last Name, Job Title, Agency of Employment, County, Work
        /// Email, Office Location, Supervisor, Department, Position,
        /// Employment Type, Hire Date, Active, the three Access fields,
        /// plus phones of the helpdesk-visible types (Work, Work Cell,
        /// Judicial Office). Everything else (SSN, BirthDate, home Address,
        /// Salary, leave balances, badge number, etc.) is intentionally
        /// excluded so this email stays free of PII.
        ///
        /// Special-case rules:
        ///   - Manatee Access (bool): on a new hire, emitted only when
        ///     explicitly granted (true) — a default-false grant is noise.
        ///     On update, emitted on any change.
        ///   - Sarasota / DeSoto Access (strings): standard non-empty-on-new,
        ///     changed-on-update rule via the Diff helper.
        ///
        /// Returns empty string when nothing actually changed (so the caller
        /// can skip the send).</summary>
        private string BuildChangeBody(EmployeeInfo before, EmployeeInfo after, bool isNew,
                                       List<string> beforePhones, List<string> afterPhones)
        {
            var sb = new StringBuilder();
            sb.AppendLine(isNew ? "**** New Employee ****" : "**** Employee Updated: " + after.DisplayName + " ****");
            sb.AppendLine();

            // Resolve foreign keys to human-readable names so the recipient
            // doesn't have to translate IDs in their head.
            string deptName(int? id) => id.HasValue ? _groupController.GetById(id.Value)?.GroupName : null;
            string locName(int? id) => id.HasValue ? _locationController.GetById(id.Value)?.Description : null;
            string countyName(int? id) => id.HasValue ? _countyController.GetById(id.Value)?.CountyName : null;
            string supervisorName(int? id) => id.HasValue ? _employeeController.GetEmployee(id.Value)?.DisplayName : null;

            int changes = 0;
            void Diff(string label, object oldValue, object newValue)
            {
                if (!isNew && Equals(NormalizeForCompare(oldValue), NormalizeForCompare(newValue))) return;
                if (isNew && (newValue == null || string.IsNullOrEmpty(newValue.ToString()))) return;
                sb.AppendLine(label + ": " + (newValue ?? "(blank)"));
                changes++;
            }

            // HR-approved fields. Anything not on this list is PII (SSN,
            // birth date, salary, home address, leave balances, etc.) and
            // stays out of the helpdesk inbox.
            Diff("First Name",           before?.FirstName,                          after.FirstName);
            Diff("Last Name",            before?.LastName,                           after.LastName);
            Diff("Job Title",            before?.JobTitle,                           after.JobTitle);
            Diff("Agency of Employment", before?.AgencyOfEmployment,                 after.AgencyOfEmployment);
            Diff("County",               countyName(before?.CountyId),               countyName(after.CountyId));
            Diff("Work Email",           before?.Email,                              after.Email);
            Diff("Office Location",      locName(before?.OfficeLocationId),          locName(after.OfficeLocationId));
            Diff("Supervisor",           supervisorName(before?.SupervisorId),       supervisorName(after.SupervisorId));
            Diff("Department",           deptName(before?.DepartmentId),             deptName(after.DepartmentId));
            Diff("Position",             before?.Position,                           after.Position);
            Diff("Employment Type",      before?.EmploymentType,                     after.EmploymentType);
            Diff("Hire Date",            FormatDate(before?.HireDate),               FormatDate(after.HireDate));
            Diff("Active",               before?.IsActive,                           after.IsActive);

            // Manatee Access is a checkbox (nullable bool). For a brand-new
            // hire we only emit it when explicitly granted (true) — emitting
            // "Manatee Access: False" by default would be helpdesk noise.
            // On update we emit on any change (true->false flips matter).
            var manateeNew = after.ManateeAccess.GetValueOrDefault();
            var manateeOld = before?.ManateeAccess.GetValueOrDefault() ?? false;
            if (isNew)
            {
                if (manateeNew) { sb.AppendLine("Manatee Access: True"); changes++; }
            }
            else if (manateeNew != manateeOld)
            {
                sb.AppendLine("Manatee Access: " + manateeNew);
                changes++;
            }

            // Sarasota / DeSoto Access are free-form strings (card #s,
            // notes); standard Diff handles "non-blank for new, changed for
            // update" exactly as the user requested.
            Diff("Sarasota Access", before?.SarasotaAccess, after.SarasotaAccess);
            Diff("DeSoto Access",   before?.DesotoAccess,   after.DesotoAccess);

            // Phones (Work / Work Cell / Judicial Office only). For a new
            // hire this is the full list; for an update it's a +/-/Δ diff
            // against the snapshot captured at page load.
            var phoneLines = BuildPhoneSection(beforePhones ?? new List<string>(),
                                               afterPhones  ?? new List<string>(),
                                               isNew);
            if (phoneLines.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine(isNew ? "Phones:" : "Phone changes:");
                foreach (var line in phoneLines) sb.AppendLine("  " + line);
            }

            // No scalar field changed AND no phone changed -> skip the email.
            if (changes == 0 && phoneLines.Count == 0) return string.Empty;

            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine("Saved by: " + (UserInfo?.DisplayName ?? "(unknown)") + " (UserId " + UserId + ")");
            return sb.ToString();
        }

        // ---------- Phone snapshot / diff helpers ----------------------------

        /// <summary>Returns a serializable snapshot of the helpdesk-visible
        /// phones for the given employee. Each entry is encoded as
        /// "PhoneId|PhoneType|PhoneNumber|Extension" so it survives ViewState
        /// round-trips. Returns an empty list for new (unsaved) employees.</summary>
        private List<string> SnapshotHelpdeskPhones(int employeeId)
        {
            if (employeeId <= 0) return new List<string>();
            return _phoneController.GetForEmployee(employeeId)
                .Where(p => HelpdeskPhoneTypes.Contains(p.PhoneType ?? string.Empty))
                .OrderBy(p => p.PhoneId)
                .Select(EncodePhone)
                .ToList();
        }

        private static string EncodePhone(PhoneInfo p) =>
            string.Join("|", new[] {
                p.PhoneId.ToString(),
                p.PhoneType  ?? string.Empty,
                p.PhoneNumber ?? string.Empty,
                p.Extension  ?? string.Empty,
            });

        /// <summary>For a new hire, returns each current phone as a plain
        /// list entry. For an update, returns a +/-/Δ diff keyed by PhoneId:
        ///   + Type: number ext (added)
        ///   - Type: number ext (removed)
        ///   Δ Type: oldNumber -> newNumber (modified)
        /// </summary>
        private static List<string> BuildPhoneSection(
            List<string> beforePhones, List<string> afterPhones, bool isNew)
        {
            if (isNew)
            {
                return afterPhones.Select(FormatPhone).ToList();
            }

            var beforeById = ToById(beforePhones);
            var afterById  = ToById(afterPhones);
            var lines = new List<string>();

            // Added: in after but not before.
            foreach (var kv in afterById)
                if (!beforeById.ContainsKey(kv.Key))
                    lines.Add("+ " + FormatPhone(kv.Value));

            // Removed: in before but not after.
            foreach (var kv in beforeById)
                if (!afterById.ContainsKey(kv.Key))
                    lines.Add("- " + FormatPhone(kv.Value));

            // Modified: same PhoneId, different encoded value.
            foreach (var kv in afterById)
                if (beforeById.TryGetValue(kv.Key, out var oldEncoded)
                    && !string.Equals(oldEncoded, kv.Value, StringComparison.Ordinal))
                {
                    lines.Add("Δ " + FormatPhone(oldEncoded) + " -> " + FormatPhone(kv.Value));
                }

            return lines;
        }

        private static Dictionary<long, string> ToById(List<string> encoded)
        {
            var d = new Dictionary<long, string>();
            foreach (var s in encoded)
            {
                var pipe = s.IndexOf('|');
                if (pipe <= 0) continue;
                if (long.TryParse(s.Substring(0, pipe), out var id)) d[id] = s;
            }
            return d;
        }

        /// <summary>Renders "Type: number" or "Type: number ext NNN" from the
        /// pipe-encoded form stored in ViewState.</summary>
        private static string FormatPhone(string encoded)
        {
            // "PhoneId|Type|Number|Extension"
            var parts = encoded.Split('|');
            if (parts.Length < 4) return encoded;
            var type = parts[1];
            var num  = parts[2];
            var ext  = parts[3];
            return string.IsNullOrWhiteSpace(ext)
                ? type + ": " + num
                : type + ": " + num + " ext " + ext;
        }

        private static string FormatDate(DateTime? d)
        {
            return d.HasValue ? d.Value.ToString("yyyy-MM-dd") : null;
        }

        /// <summary>Treat null and empty string as the same value so a fresh
        /// "" out of an unfilled textbox doesn't look like a change vs a NULL
        /// in the database.</summary>
        private static object NormalizeForCompare(object v)
        {
            if (v == null) return null;
            if (v is string s) return string.IsNullOrEmpty(s) ? null : (object)s.Trim();
            return v;
        }

        #endregion

        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (EmployeeId > 0)
                {
                    _employeeController.DeleteEmployee(EmployeeId);
                }

                Response.Redirect(BuildListUrl("empDeleted=1"), false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        #endregion

        // Groups membership tab is driven by the Web API now
        // (Components/Api/MembershipsController.cs + Scripts/empdb-edit.js#groups).
        // The Photo tab uses Components/Api/PhotosController.cs.

        // Position / Service / Phones / Emergency Contacts tabs are all driven
        // by the Web API now (Components/Api/*Controller.cs + Scripts/empdb-edit.js).
        // No server-side click handlers are needed for them on this page anymore.

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
