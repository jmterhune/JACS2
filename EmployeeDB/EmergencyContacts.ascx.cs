using DotNetNuke.Abstractions;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.EmployeeDB.Components;

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
    public partial class EmergencyContacts : EmployeeDBModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private EmergencyContactController ctl = new EmergencyContactController();

        public EmergencyContacts()
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
                    if (DotNetNuke.Framework.AJAX.IsInstalled())
                    {
                        DotNetNuke.Framework.AJAX.RegisterScriptManager();
                    }
                    JavaScript.RequestRegistration(CommonJs.jQuery);
                    PopulateList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PopulateList()
        {
            rptEmergencyContact.DataSource = ctl.GetEmergencyContactsByEmployee(EmployeeId);
            rptEmergencyContact.DataBind();
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            EmergencyContact emergencyContact = new EmergencyContact();
            bool isNew = true;
            if (hdEmergencyContactId.Value != "")
            {
                isNew = false;
                emergencyContact = ctl.GetEmergencyContact(Convert.ToInt32(hdEmergencyContactId.Value));
            }
            emergencyContact.EmployeeId = EmployeeId;
            emergencyContact.FirstName = txtFirstName.Text;
            emergencyContact.LastName = txtLastName.Text;
            emergencyContact.Relationship = txtRelationship.Text;
            emergencyContact.PhoneWork = Regex.Replace(txtWorkPhone.Text, @"[^\d]", "");
            emergencyContact.PhoneHome = Regex.Replace(txtHomePhone.Text, @"[^\d]", "");
            emergencyContact.PhoneMobile = Regex.Replace(txtMobilePhone.Text, @"[^\d]", ""); 
            emergencyContact.CallOrder = 1;
            emergencyContact.LastModifiedDate = DateTime.Now;
            emergencyContact.LastModifiedByID = UserId;
            if (isNew)
            {
                emergencyContact.CreatedByID = UserId;
                emergencyContact.CreatedDate = DateTime.Now;
                ctl.CreateEmergencyContact(emergencyContact);
            }
            else
            {
                ctl.UpdateEmergencyContact(emergencyContact);
            }
            ClearForm();
            PopulateList();
        }

        private void ClearForm()
        {
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtRelationship.Text = string.Empty;
            txtHomePhone.Text = string.Empty;
            txtWorkPhone.Text = string.Empty;
            txtMobilePhone.Text = string.Empty;
        }


        protected void pnlEmergencyContact_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });

        }

        protected void rptEmergencyContact_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int contactId = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "delete")
            {
                ctl.DeleteEmergencyContact(contactId);
            }
            if (e.CommandName == "edit")
            {
                EmergencyContact emergencyContact = ctl.GetEmergencyContact(contactId);
                hdEmergencyContactId.Value = contactId.ToString();
                txtFirstName.Text = emergencyContact.FirstName;
                txtLastName.Text = emergencyContact.LastName;
                txtHomePhone.Text = emergencyContact.PhoneHome;
                txtRelationship.Text = emergencyContact.Relationship;
                txtMobilePhone.Text = emergencyContact.PhoneMobile;
                txtWorkPhone.Text = emergencyContact.PhoneWork;
                ScriptManager.RegisterStartupScript(rptEmergencyContact, rptEmergencyContact.GetType(), "ToggleForm", "ToggleForm(true)", true);
            }
            PopulateList();

        }

        protected void rptEmergencyContact_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
                LinkButton cmdEdit = (LinkButton)e.Item.FindControl("cmdEdit");
                LinkButton cmdDelete = (LinkButton)e.Item.FindControl("cmdDelete");
                scriptMan.RegisterAsyncPostBackControl(cmdDelete);
                scriptMan.RegisterAsyncPostBackControl(cmdEdit);
            }

        }
    }
}