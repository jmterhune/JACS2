/*
' Copyright (c) 2023  Joe Terhune
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
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Mail;
using DotNetNuke.UI.Skins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using tjc.Modules.EmployeeDB.Components;
using tjc.Modules.EmployeeDB.Components.Services;

namespace tjc.Modules.EmployeeDB
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from EmployeeDBModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : EmployeeDBModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        private bool ShowActive
        {
            get
            {
                if (ViewState["ShowActive"] != null) { return Convert.ToBoolean(ViewState["ShowActive"]); }
                return true;
            }
            set { ViewState["ShowActive"] = value; }
        }
        public string DepartmentFilterHtml
        {
            get
            {
                if (ViewState["DepartmentFilterHtml"] != null) { return ViewState["DepartmentFilterHtml"].ToString(); }
                return "";
            }
            set { ViewState["DepartmentFilterHtml"] = value; }
        }
        public List<string> ContactIds
        {
            get
            {
                if (ViewState["ContactIds"] != null) { return (List<string>)ViewState["ContactIds"]; }
                return null;
            }
            set { ViewState["ContactIds"] = value; }
        }
        #endregion

        #region Methods
        public View()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void PopulateEmployeeList()
        {
            var ctl = new EmployeeController();
            rptEmployees.DataSource = ctl.GetEmployeeListItems(ShowActive, true);
            rptEmployees.DataBind();
        }
        private string GetDepartmentFilterHtml()
        {
            string filterHtml = "";
            filterHtml = "<label class='me-2'>Filter by Department<select id='drpfilter' class='form-control form-control-sm' aria-controls='employees'><option value='-1'>All</option>";
            var ctl = new GroupController();

            IEnumerable<Group> departments = ctl.GetGroups().Where(x => x.GroupType == Convert.ToInt32(Group.GroupTypes.Internal));
            foreach (Group department in departments)
            {
                filterHtml += "<option value='" + department.GroupID.ToString() + "'>" + department.GroupName + "</option>";
            }
            filterHtml += "</select></label>";
            return filterHtml;
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    PopulateEmployeeList();
                    DepartmentFilterHtml = GetDepartmentFilterHtml();
                    var eCtl = new EmployeeController();
                    drpOldSupervisor.DataSource = eCtl.GetEmployeeDropDown(SupervisorRole).OrderBy(x => x.DataText);
                    drpOldSupervisor.DataBind();
                    drpNewSupervisor.DataSource = eCtl.GetEmployeeDropDown(SupervisorRole).OrderBy(x => x.DataText);
                    drpNewSupervisor.DataBind();
                }
                chkInactiveEmployees.InputAttributes.Add("class", "form-check-input");
                chkInactiveEmployees.LabelAttributes.Add("class", "form-check-label");
                lnkCancel.NavigateUrl = _navigationManager.NavigateURL();
                lnkEeoReport.NavigateUrl = EEOUrl;
                lnkSwnList.NavigateUrl = string.Format("{0}/SwnList.aspx", TemplateSourceDirectory);

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void chkInactiveEmployees_CheckedChanged(object sender, EventArgs e)
        {
            ltMessage.Text = "";
            if (ShowActive)
            {
                ShowActive = false;
                chkInactiveEmployees.Text = "Inactive Employees";
            }
            else
            {
                ShowActive = true;
                chkInactiveEmployees.Text = "Active Employees";
            }
            PopulateEmployeeList();

        }
        protected void cmdAddContacts_Click(object sender, EventArgs e)
        {
            ltMessage.Text = "";

            var ctl = new EmployeeController();
            LoginRequest loginRequest = new LoginRequest { Password = SwnPassword, Username = SwnUsername };
            string token = SwnInterface.GetToken(SwnServiceIdentifier, SwnSubscriptionKey, loginRequest);
            SwgGroupMemberResponse contactIds = SwnInterface.GetSwnContactIds(SwnServiceIdentifier, SwnSubscriptionKey, token);

            if (cmdAddContacts.Text.Contains("Show Missing SWN Contacts"))
            {
                if (contactIds.contacts.Count > 0)
                {
                    IEnumerable<EmployeeListItem> missingContacts = ctl.GetContactListItems(ShowActive).Where(x => !contactIds.contacts.Contains(x.EmployeeId.ToString()));
                    if (missingContacts.Count() > 0)
                    {
                        ContactIds = contactIds.contacts;
                        cmdAddContacts.Text = "<i class=\"fas fa-address-book\"></i> Add Users to SWN Contacts";
                        lnkEeoReport.Enabled = false;
                        lnkSwnList.Enabled = false;
                        cmdSyncAll.Enabled = false;
                        lnkCancel.Visible = true;
                        chkInactiveEmployees.Enabled = false;
                        rptEmployees.DataSource = missingContacts;
                        rptEmployees.DataBind();
                        ltMessage.Text = "<div class='alert alert-warning'><i class='fa fa-warning'></i> <strong>Please Note:</strong> This list includes all Contacts, including Non Employees</div>";

                    }
                    else
                    {
                        ltMessage.Text = "<div class='alert alert-warning'><i class='fa fa-warning'></i> All Active Employees & Contacts Exist in SWN</div>";
                    }
                }
                else
                {
                    ltMessage.Text = "<div class='alert alert-danger'><i class='fas fa-exclamation-circle'></i> Unable to retrieve contacts from SWN</div>";
                }
            }
            else
            {
                IEnumerable<Employee> missingEmployees = ctl.GetActiveContacts().Where(x => !contactIds.contacts.Contains(x.EmployeeId.ToString()));
                cmdAddContacts.Text = "<i class=\"fas fa-address-book\"></i> Show Missing SWN Contacts";
                lnkEeoReport.Enabled = true;
                lnkSwnList.Enabled = true;
                cmdSyncAll.Enabled = true;
                lnkCancel.Visible = false;
                chkInactiveEmployees.Enabled = true;
                List<string> successList = new List<string>();
                List<string> messageList = new List<string>();
                if (missingEmployees != null)
                {
                    foreach (Employee employee in missingEmployees)
                    {
                        try
                        {
                            SwnInterface.AddUpdateSwnContact(employee, SwnServiceIdentifier, SwnSubscriptionKey, token);
                            successList.Add(string.Format("<li>{0}</li>", employee.FullName));

                        }
                        catch (Exception exc)
                        {
                            messageList.Add(string.Format("<li>{0}</li>", employee.FullName));
                            string process = string.Format("Add {0} as SWN Contact ", employee.FullName);
                            SwnLog swnLog = new SwnLog { CreatedBy = UserId, CreatedDate = DateTime.Now, Exception = exc.InnerException.Message, Process = process };
                            var logCtl = new SwnLogController();
                            logCtl.CreateSwnLog(swnLog);
                        }
                    }
                    ltMessage.Text += string.Format("<div class='alert alert-success'><h5><i class='far fa-thumbs-up'></i> The following contacts were successfully added</h5><ul>{0}</ul></div>", string.Join("", successList));
                }
                if (messageList.Count > 0)
                {
                    ltMessage.Text += string.Format("<div class='alert alert-danger'><h5 class='alert-danger'><i class='fas fa-exclamation-circle'></i> The following contacts where not added</h5><ul>{0}</ul></div>", string.Join("", messageList));
                }
            }
        }
        protected void cmdSyncAll_Click(object sender, EventArgs e)
        {
            ltMessage.Text = "";
            var ctl = new EmployeeController();
            LoginRequest loginRequest = new LoginRequest { Password = SwnPassword, Username = SwnUsername };
            string token = SwnInterface.GetToken(SwnServiceIdentifier, SwnSubscriptionKey, loginRequest);
            IEnumerable<Employee> activeContacts = ctl.GetActiveContacts();
            List<string> succeses = new List<string>();
            List<string> message = new List<string>();
            if (activeContacts != null)
            {
                foreach (Employee employee in activeContacts)
                {
                    try
                    {
                        SwnInterface.AddUpdateSwnContact(employee, SwnServiceIdentifier, SwnSubscriptionKey, token);
                        succeses.Add(string.Format("<li>{0}</li>", employee.FullName));
                    }
                    catch (Exception exc)
                    {
                        message.Add(string.Format("<li>{0}</li>", employee.FullName));
                        string process = string.Format("Update {0} SWN Contact Information", employee.FullName);
                        SwnLog swnLog = new SwnLog { CreatedBy = UserId, CreatedDate = DateTime.Now, Exception = exc.InnerException.Message, Process = process };
                        var logCtl = new SwnLogController();
                        logCtl.CreateSwnLog(swnLog);
                    }
                }
                ltMessage.Text += string.Format("<div class='alert alert-success'><h5><i class='far fa-thumbs-up'></i> The following contacts were successfully updated</h5><ul>{0}</ul></div>", succeses.ToString());
            }
            if (message.Count > 0)
            {
                ltMessage.Text += string.Format("<div class='alert alert-danger'><h5 class='alert-danger'><i class='fas fa-exclamation-circle'></i> The following contacts where not updated</h5><ul>{0}</ul></div>", message.ToString());
            }

        }
        protected void pnlEmployees_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        protected void cmdSwithSupervisor_Click(object sender, EventArgs e)
        {
            var ctl = new EmployeeController();
            Int32.TryParse(drpOldSupervisor.SelectedValue, out int oldsup);
            Int32.TryParse(drpNewSupervisor.SelectedValue, out int newsup);
            IEnumerable<Employee> employees = ctl.SwitchSupervisorBulk(oldsup, newsup);
            string employeeNames = "";
            foreach (Employee employee in employees) {
                employeeNames += string.Format("<li>{0}</li>", employee.FullName); 
            }
            Skin.AddModuleMessage(this, string.Format("Employees reporting to {0} are now reporting to {1}", drpOldSupervisor.SelectedItem.Text, drpNewSupervisor.SelectedItem.Text), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
            string body = string.Format("The supervisor for the following employees has changed to {0}:{1}",drpNewSupervisor.SelectedItem.Text, string.Format("<ul>{0}</ul>", employeeNames));
            Mail.SendEmail("hr@jud12.flcourts.org", "helpdesk@ud12.flcourts.org", "Employee Supervisor Updated", body);
        }

        #endregion

    }
}