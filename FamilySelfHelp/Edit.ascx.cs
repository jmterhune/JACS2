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
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.FamilySelfHelp.Components;

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
    public partial class Edit : FamilySelfHelpModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private ModuleSecurity modSecurty;

        public Edit()
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

                    lnkCancel.NavigateUrl = _navigationManager.NavigateURL();
                    //get a list of users to assign the user to the Object
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void valOtherServiceProvided_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {

        }

        protected void valServiceProvided_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {

        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            var t = new Client();
            var tc = new ClientController();

            Response.Redirect(_navigationManager.NavigateURL());

        }
        #region Methods
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
                    rblCaseType.SelectedValue = log.CaseType;
                    rblLocation.SelectedValue = log.Location;
                    if (!string.IsNullOrEmpty(log.ServiceProvided))
                    {
                        string[] servicesProvided = log.ServiceProvided.Split('|');
                        foreach (var s in servicesProvided)
                        {
                            var value = cblServicesProvided.Items.FindByValue(s);
                            if (value != null)
                                value.Selected = true;
                        }

                    }
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
                Log objLog = ctl.GetLog(LogId);
                {
                    var withBlock = objLog;
                    hdClientId.Value = objLog.ClientId.ToString();
                    Client objClient = objLog.ClientInfo;
                    txtLastName.Text = objClient.LastName;
                    txtFirstName.Text = objClient.FirstName;
                    txtMiddleInitial.Text = objClient.MiddleInitial;
                    txtPhone.Text = objClient.Phone;
                    txtEmail.Text = objClient.Email;
                    txtCaseNumber.Text = objLog.CaseNumber;
                    drpDivision.SelectedValue = objLog.Division;
                    txtTimeSpent.Text = objLog.TimeSpent.ToString();
                    rblClientType.SelectedValue = objLog.ClientType;
                    rblLocation.SelectedValue = objLog.Location;
                    if (objLog.ServiceDate.HasValue)
                        txtServiceDate.Text = objLog.ServiceDate.Value.ToShortTimeString();
                    chkInterpreterProvided.Checked = objLog.InterpreterProvided;
                    if (objLog.CaseType != "")
                        PopulateCaseType(objLog.CaseType);
                    if (objLog.ContactMethod != "")
                        PopulateContactMethod(objLog.ContactMethod);
                    if (objLog.ServiceProvided != "")
                        PopulateServicesProvided(objLog.ServiceProvided);

                    if (objLog.HasAppointment.HasValue)
                    {
                        if (objLog.HasAppointment.Value)
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
            //else if (_newClient != "")
            //{
            //    int commaIndex = _newClient.IndexOf(",");
            //    if (commaIndex > 0)
            //    {
            //        txtLastName.Text = _newClient.Substring(0, commaIndex);
            //        txtFirstName.Text = _newClient.Substring(commaIndex + 1).Trim;
            //    }
            //    else
            //    {
            //        commaIndex = _newClient.IndexOf(" ");
            //        if (commaIndex > 0)
            //        {
            //            txtFirstName.Text = _newClient.Substring(0, commaIndex);
            //            txtLastName.Text = _newClient.Substring(commaIndex + 1).Trim;
            //        }
            //        else
            //            txtLastName.Text = _newClient;
            //    }
            //}
        }

        private string GetServicesProvided()
        {
            string serviceProvided = "";
            foreach (ListItem item in cblServicesProvided.Items)
            {
                if (item.Selected)
                {
                    if (item.Value == "O")
                        serviceProvided += txtServiceProvidedOther.Text.Trim() + "|";
                    else
                        serviceProvided += item.Value + "|";
                }
            }
            if (serviceProvided.Length > 0)
                serviceProvided = serviceProvided.TrimEnd('|');
            return serviceProvided;
        }

        private void PopulateServicesProvided(string services)
        {
            var lstServices = services.Split('|').ToList();
            if (lstServices != null)
            {
                foreach (string v in lstServices)
                {
                    ListItem item = cblServicesProvided.Items.FindByValue(v);
                    if (item == null)
                    {
                        txtServiceProvidedOther.Text = v;
                        cblServicesProvided.Items.FindByValue("O").Selected = true;
                    }
                    else
                        item.Selected = true;
                }
            }
        }

        private void PopulateCaseType(string caseType)
        {
            ListItem item = rblCaseType.Items.FindByValue(caseType);
            if (item == null)
            {
                rblCaseType.SelectedValue = "Other";
                txtCaseTypeOther.Text = caseType;
            }
            else
                item.Selected = true;
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
            Log objLog = ctl.GetLog(LogId);
            Client objClient = objLog.ClientInfo;
            objClient.LastName = txtLastName.Text.Trim();
            objClient.FirstName = txtFirstName.Text.Trim();
            objClient.Email = txtEmail.Text.Trim();
            objClient.Phone = txtPhone.Text.Trim();
            if (string.IsNullOrEmpty(txtServiceDate.Text))
                objLog.ServiceDate = DateTime.Parse(txtServiceDate.Text);
            objLog.CaseNumber = txtCaseNumber.Text;
            objLog.InterpreterProvided = chkInterpreterProvided.Checked;
            objLog.CaseNumber = txtCaseNumber.Text;
            bool hasAppointment = false;

            if (rblHasAppointment.SelectedValue == "1")
                hasAppointment = true;
            objLog.HasAppointment = hasAppointment;
            objLog.ClientType = rblClientType.SelectedValue;
            objLog.Location = rblLocation.SelectedValue;
            if (txtCaseNumber.Text.Trim() == "")
                objLog.IsNewCase = true;
            else
                objLog.IsNewCase = false;
            if (rblCaseType.SelectedValue == "Other")
                objLog.CaseType = txtCaseTypeOther.Text.Trim();
            else
                objLog.CaseType = rblCaseType.SelectedValue;
            if (rblContactMethod.SelectedValue == "Other")
                objLog.ContactMethod = txtContactMethodOther.Text.Trim();
            else
                objLog.ContactMethod = rblContactMethod.SelectedValue;
            objLog.ServiceProvided = GetServicesProvided();
            objLog.Division = drpDivision.SelectedValue;

            objLog.TimeSpent = Decimal.Parse(txtTimeSpent.Text);
            ctl.UpdateLog(objLog);
            ctlC.UpdateClient(objClient);
            Response.Redirect(_navigationManager.NavigateURL(TabId, "", "cid=" + objClient.ClientId.ToString()));
        }

        private Log GetNewLog(int clientId)
        {
            Log objLog = new Log();
            if (string.IsNullOrEmpty(txtServiceDate.Text))
                objLog.ServiceDate = DateTime.Parse(txtServiceDate.Text);
            else
                objLog.ServiceDate = DateTime.Now;
            objLog.CaseNumber = txtCaseNumber.Text;
            objLog.ClientId = clientId;
            objLog.InterpreterProvided = chkInterpreterProvided.Checked;
            objLog.CaseNumber = txtCaseNumber.Text;
            bool hasAppointment = false;

            if (rblHasAppointment.SelectedValue == "1")
                hasAppointment = true;
            objLog.HasAppointment = hasAppointment;

            if (txtCaseNumber.Text.Trim() == "")
                objLog.IsNewCase = true;
            else
                objLog.IsNewCase = false;
            if (rblCaseType.SelectedValue == "Other")
                objLog.CaseType = txtCaseTypeOther.Text.Trim();
            else
                objLog.CaseType = rblCaseType.SelectedValue;
            objLog.ClientType = rblClientType.SelectedValue;
            objLog.Location = rblLocation.SelectedValue;
            if (rblContactMethod.SelectedValue == "Other")
                objLog.ContactMethod = txtContactMethodOther.Text.Trim();
            else
                objLog.ContactMethod = rblContactMethod.SelectedValue;
            objLog.ServiceProvided = GetServicesProvided();
            objLog.Division = drpDivision.SelectedValue;

            objLog.TimeSpent = Decimal.Parse(txtTimeSpent.Text);
            return objLog;
        }
        #endregion
    }
}