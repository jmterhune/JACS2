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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.FamilySelfHelp.Components;
using Service = tjc.Modules.FamilySelfHelp.Components.Service;

namespace tjc.Modules.FamilySelfHelp
{
    /// -----------------------------------------------------------------------------
    /// <summary>   
    /// The Edit class is used to manage content
    /// 
    /// Typically your edit control would be used to create new content, or edit existing content within your module.
    /// The ControlKey for this control is "Edit", and is defined in the manifest (.dnn) file.
    /// 
    /// Because the control inherits from FamilySelfHelpModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class EditLog : FamilySelfHelpModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private ModuleSecurity modSecurty;

        public EditLog()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Implement your edit logic for your module
                if (!Page.IsPostBack)
                {
                    modSecurty = new ModuleSecurity(this.ModuleConfiguration);
                    BindForm();
                    chkInterpreterProvided.InputAttributes.Add("class", "form-check-input");
                    chkInterpreterProvided.LabelAttributes.Add("class", "form-check-label");
                    lnkSearch.NavigateUrl = _navigationManager.NavigateURL();
                    lnkCancel.NavigateUrl = _navigationManager.NavigateURL();
                    lnkDataEntry.NavigateUrl = EditUrl("log");
                    lnkMerge.NavigateUrl = EditUrl("merge");
                    lnkReports.NavigateUrl = EditUrl("report");
                    if (IsAdmin)
                    {
                        lnkMerge.Visible = true;
                        lnkReports.Visible = true;
                    }
                    if (modSecurty.HasReportPermission)
                        lnkReports.Visible = true;
                    if (modSecurty.HasMergePermission)
                        lnkMerge.Visible = true;
                    if (LogId > 0)
                    {
                        var ctl = new Components.LogController();
                        var cCtl = new ClientController();
                        Log log = ctl.GetLog(LogId);
                        Client client = cCtl.GetClient(log.ClientId);
                        txtLastName.Text = client.LastName;
                        txtFirstName.Text = client.FirstName;
                        txtMiddleInitial.Text = client.MiddleInitial;
                        txtPhone.Text = client.Phone;
                        txtEmail.Text = client.Email;
                        txtCaseNumber.Text = log.CaseNumber;
                        drpDivision.SelectedValue = log.Division;
                        txtTimeSpent.Text = log.TimeSpent.ToString("#.##");
                        rblClientType.SelectedValue = log.ClientType;
                        rblLocation.SelectedValue = log.Location;
                        if (log.ServiceDate.HasValue)
                            txtServiceDate.Text = log.ServiceDate.Value.ToShortDateString();
                        chkInterpreterProvided.Checked = log.InterpreterProvided;
                        IEnumerable<CaseType> caseTypes = log.CaseTypes;
                        IEnumerable<Service> services = log.Services;
                        if (caseTypes.Count() > 0)
                            PopulateCaseType(caseTypes);
                        if (!string.IsNullOrEmpty(log.ContactMethod))
                            PopulateContactMethod(log.ContactMethod);
                        if (services.Count() > 0)
                            PopulateServicesProvided(services);
                        if (log.HasAppointment.HasValue && log.HasAppointment.Value == true)
                        {
                            rblHasAppointment.SelectedValue = "1";
                        }
                        else { rblHasAppointment.SelectedValue = "0"; }
                    }
                    else if (ClientId > 0)
                    {
                        var cCtl = new ClientController();
                        Client client = cCtl.GetClient(ClientId);
                        txtLastName.Text = client.LastName;
                        txtFirstName.Text = client.FirstName;
                        txtMiddleInitial.Text = client.MiddleInitial;
                        txtPhone.Text = client.Phone;
                        txtEmail.Text = client.Email;
                        hdClientId.Value = client.ClientId.ToString();
                    }

                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdUpdateExisting_Click(object sender, EventArgs e)
        {
            long.TryParse(hdClientId.Value, out long clientId);
            var ctl = new Components.LogController();
            var log = ctl.CreateLog(GetNewLog(clientId));
            SetCaseTypes(log.LogId);
            SetServicesProvided(log.LogId);
            Response.Redirect(_navigationManager.NavigateURL(TabId, "", "cid=" + clientId.ToString()));

        }
        protected void cmdChangeName_Click(object sender, EventArgs e)
        {
            pnlExistingClient.Visible = false;
            pnlForm.Enabled = true;
        }
        protected void valOtherServiceProvided_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = false;
            bool hasOther = false;
            foreach (ListItem item in cblServicesProvided.Items)
            {
                if (item.Selected)
                {
                    if (item.Value == "0")
                        hasOther = true; break;
                }
            }
            if (!hasOther)
                args.IsValid = true;
            else if (!string.IsNullOrEmpty(txtServiceProvidedOther.Text))
                args.IsValid = true;
        }

        protected void valServiceProvided_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = false;
            foreach (ListItem item in cblServicesProvided.Items)
            {
                if (item.Selected)
                    args.IsValid = true;
                break;
            }
        }
        protected void valCaseType_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;
            foreach (ListItem item in cblCaseType.Items)
            {
                if (item.Selected)
                    args.IsValid = true;
                break;
            }
        }
        protected void valCaseTypeOther_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;
            bool hasOther = false;
            foreach (ListItem item in cblCaseType.Items)
            {
                if (item.Selected)
                {
                    if (item.Value == "0")
                        hasOther = true; break;
                }
            }
            if (!hasOther)
                args.IsValid = true;
            else if (!string.IsNullOrEmpty(txtCaseTypeOther.Text))
                args.IsValid = true;
        }
        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            if (LogId > 0)
            {
                UpdateLog();
            }
            else if (ClientId > 0)
            {
                var ctl = new Components.LogController();
                Log log = GetNewLog(ClientId);
                ctl.CreateLog(log);
                Response.Redirect(_navigationManager.NavigateURL(TabId, "", "cid=" + ClientId.ToString()));
            }
            else
            {
                CheckClient();
            }
        }
        #region Methods
        private void CheckClient()
        {
            var ctl = new ClientController();
            var lCtl = new Components.LogController();
            IEnumerable<Client> clients = ctl.GetExistingClient(txtLastName.Text.Trim(), txtFirstName.Text.Trim());
            if (clients.Count() > 0)
            {
                hdClientId.Value = clients.FirstOrDefault().ClientId.ToString();
                pnlExistingClient.Visible = true;
                pnlForm.Enabled = false;
            }
            else
            {
                Client client = new Client { LastName = txtLastName.Text.Trim(), FirstName = txtFirstName.Text.Trim(), MiddleInitial = txtMiddleInitial.Text.Trim(), CreatedDate = DateTime.Now, CreatedById = UserId, LastModifiedById = UserId, LastModifiedDate = DateTime.Now };
                ctl.CreateClient(client);
                var log = lCtl.CreateLog(GetNewLog(client.ClientId));
                SetCaseTypes(log.LogId);
                SetServicesProvided(log.LogId);
                Response.Redirect(_navigationManager.NavigateURL(TabId, "", "cid=" + client.ClientId.ToString()));
            }
        }
        private void BindForm()
        {
            var ctl = new Components.LogController();
            var ctlC = new ClientController();
            if (ClientId > 0)
            {
                Client client = ctlC.GetClient(ClientId);
                if (client != null)
                {
                    txtLastName.Text = client.LastName;
                    txtFirstName.Text = client.FirstName;
                    txtMiddleInitial.Text = client.MiddleInitial;
                    txtPhone.Text = client.Phone;
                    txtEmail.Text = client.Email;
                }
            }
            if (LogId > 0)
            {
                Log log = ctl.GetLog(LogId);
                if (log != null)
                {
                    Client client = log.ClientInfo;
                    if (client != null)
                    {
                        txtLastName.Text = client.LastName;
                        txtFirstName.Text = client.FirstName;
                        txtMiddleInitial.Text = client.MiddleInitial;
                        txtPhone.Text = client.Phone;
                        txtEmail.Text = client.Email;
                    }
                    hdClientId.Value = log.ClientId.ToString();
                    if (log.ServiceDate.HasValue)
                        txtServiceDate.Text = log.ServiceDate.Value.ToShortDateString();
                    drpDivision.SelectedValue = log.Division;
                    txtTimeSpent.Text = log.TimeSpent.ToString();
                    if (log.HasAppointment.HasValue)
                    {
                        if (log.HasAppointment.Value)
                        {
                        }
                        rblHasAppointment.SelectedValue = "1";
                    }
                    else
                    {
                        rblHasAppointment.SelectedValue = "0";
                    }
                    rblClientType.SelectedValue = log.ClientType;
                    txtCaseNumber.Text = log.CaseNumber;
                    rblContactMethod.SelectedValue = log.ContactMethod;
                    IEnumerable<CaseType> caseTypes = log.CaseTypes;
                    IEnumerable<Service> services = log.Services;
                    PopulateCaseType(caseTypes);
                    PopulateServicesProvided(services);
                    rblLocation.SelectedValue = log.Location;
                    chkInterpreterProvided.Checked = log.InterpreterProvided;
                }
            }
            if (modSecurty.HasMergePermission)
            {
                lnkMerge.Visible = true;
                lnkMerge.NavigateUrl = EditUrl("merge");
            }
            if (modSecurty.HasReportPermission)
            {
                lnkReports.Visible = true;
                lnkReports.NavigateUrl = EditUrl("reports");
            }
            if (LogId != Null.NullInteger)
            {
                Log log = ctl.GetLog(LogId);
                {
                    var withBlock = log;
                    hdClientId.Value = log.ClientId.ToString();
                    Client client = log.ClientInfo;
                    txtLastName.Text = client.LastName;
                    txtFirstName.Text = client.FirstName;
                    txtMiddleInitial.Text = client.MiddleInitial;
                    txtPhone.Text = client.Phone;
                    txtEmail.Text = client.Email;
                    txtCaseNumber.Text = log.CaseNumber;
                    drpDivision.SelectedValue = log.Division;
                    txtTimeSpent.Text = log.TimeSpent.ToString();
                    rblClientType.SelectedValue = log.ClientType;
                    rblLocation.SelectedValue = log.Location;
                    if (log.ServiceDate.HasValue)
                        txtServiceDate.Text = log.ServiceDate.Value.ToShortTimeString();
                    chkInterpreterProvided.Checked = log.InterpreterProvided;
                    IEnumerable<CaseType> caseTypes = log.CaseTypes;
                    IEnumerable<Components.Service> services = log.Services;
                    PopulateCaseType(caseTypes);
                    PopulateServicesProvided(services);
                    if (log.ContactMethod != "")
                        PopulateContactMethod(log.ContactMethod);
                    if (log.HasAppointment.HasValue)
                    {
                        if (log.HasAppointment.Value)
                        {
                            rblHasAppointment.SelectedValue = "1";
                        }
                        else
                        {
                            rblHasAppointment.SelectedValue = "0";
                        }
                    }
                }
            }
        }
        private void SetServicesProvided(long logId)
        {
            List<Components.Service> services = new List<Components.Service>();
            foreach (ListItem item in cblServicesProvided.Items)
            {
                Components.Service service = new Components.Service { LogID = logId };
                if (item.Selected)
                {
                    if (item.Value == "O")
                        service.ServiceName = txtServiceProvidedOther.Text.Trim();
                    else
                        service.ServiceName = item.Value;
                    services.Add(service);
                }

            }
            var ctl = new Components.LogController();
            ctl.CreateServicesByLog(services, logId);
        }
        private void SetCaseTypes(long logId)
        {
            List<Components.CaseType> caseTypes = new List<Components.CaseType>();
            foreach (ListItem item in cblCaseType.Items)
            {
                Components.CaseType caseType = new Components.CaseType { LogID = logId };
                if (item.Selected)
                {
                    if (item.Value == "O")
                        caseType.CaseTypeName = txtCaseTypeOther.Text.Trim();
                    else
                        caseType.CaseTypeName = item.Value;
                    caseTypes.Add(caseType);
                }

            }
            var ctl = new Components.LogController();
            ctl.CreateCaseTypesByLog(caseTypes, logId);
        }
        private void PopulateServicesProvided(IEnumerable<Components.Service> services)
        {
            foreach (Service service in services)
            {
                ListItem item = cblServicesProvided.Items.FindByValue(service.ServiceName);
                if (item == null)
                {
                    txtServiceProvidedOther.Text = service.ServiceName;
                    cblServicesProvided.Items.FindByValue("O").Selected = true;
                }
                else
                    item.Selected = true;
            }
        }
        private void PopulateCaseType(IEnumerable<CaseType> caseTypes)
        {
            foreach (CaseType caseType in caseTypes)
            {
                ListItem item = cblCaseType.Items.FindByValue(caseType.CaseTypeName);
                if (item == null)
                {
                    cblCaseType.SelectedValue = "O";
                    txtCaseTypeOther.Text = caseType.CaseTypeName;
                }
                else
                    item.Selected = true;
            }
        }
        private void PopulateContactMethod(string method)
        {
            ListItem item = rblContactMethod.Items.FindByValue(method);
            if (item == null)
            {
                rblContactMethod.SelectedValue = "Other";
                txtContactMethodOther.Text = method;
            }
            else
                item.Selected = true;
        }
        private void UpdateLog()
        {
            var ctl = new Components.LogController();
            var ctlC = new Components.ClientController();
            Log log = ctl.GetLog(LogId);
            Client client = log.ClientInfo;
            client.LastName = txtLastName.Text.Trim();
            client.FirstName = txtFirstName.Text.Trim();
            client.Email = txtEmail.Text.Trim();
            client.Phone = txtPhone.Text.Trim();
            if (!string.IsNullOrEmpty(txtServiceDate.Text))
                log.ServiceDate = DateTime.Parse(txtServiceDate.Text);
            log.CaseNumber = txtCaseNumber.Text;
            log.InterpreterProvided = chkInterpreterProvided.Checked;
            log.CaseNumber = txtCaseNumber.Text;
            bool hasAppointment = false;
            if (rblHasAppointment.SelectedValue == "1")
                hasAppointment = true;
            log.HasAppointment = hasAppointment;
            log.ClientType = rblClientType.SelectedValue;
            log.Location = rblLocation.SelectedValue;
            if (txtCaseNumber.Text.Trim() == "")
                log.IsNewCase = true;
            else
                log.IsNewCase = false;
            if (rblContactMethod.SelectedValue == "Other")
                log.ContactMethod = txtContactMethodOther.Text.Trim();
            else
                log.ContactMethod = rblContactMethod.SelectedValue;
            log.Division = drpDivision.SelectedValue;
            log.TimeSpent = Decimal.Parse(txtTimeSpent.Text);
            log.LastModifiedDate = DateTime.Now;
            log.LastModifiedById = UserId;
            ctl.UpdateLog(log);
            SetServicesProvided(log.LogId);
            SetCaseTypes(log.LogId);
            client.LastModifiedById = UserId;
            client.LastModifiedDate = DateTime.Now;
            ctlC.UpdateClient(client);
            Response.Redirect(_navigationManager.NavigateURL(TabId, "", "cid=" + client.ClientId.ToString()));
        }
        private Log GetNewLog(long clientId)
        {
            Log log = new Log();
            if (!string.IsNullOrEmpty(txtServiceDate.Text))
                log.ServiceDate = DateTime.Parse(txtServiceDate.Text);
            else
                log.ServiceDate = DateTime.Now;
            log.CaseNumber = txtCaseNumber.Text;
            log.ClientId = clientId;
            log.InterpreterProvided = chkInterpreterProvided.Checked;
            log.CaseNumber = txtCaseNumber.Text;
            bool hasAppointment = false;

            if (rblHasAppointment.SelectedValue == "1")
                hasAppointment = true;
            log.HasAppointment = hasAppointment;
            if (txtCaseNumber.Text.Trim() == "")
                log.IsNewCase = true;
            else
                log.IsNewCase = false;
            log.ClientType = rblClientType.SelectedValue;
            log.Location = rblLocation.SelectedValue;
            if (rblContactMethod.SelectedValue == "Other")
                log.ContactMethod = txtContactMethodOther.Text.Trim();
            else
                log.ContactMethod = rblContactMethod.SelectedValue;
            log.Division = drpDivision.SelectedValue;
            log.TimeSpent = Decimal.Parse(txtTimeSpent.Text);
            log.CreatedById = UserId;
            log.CreatedDate = DateTime.Now;
            log.LastModifiedDate = DateTime.Now;
            log.LastModifiedById = UserId;
            return log;
        }


        #endregion

    }
}