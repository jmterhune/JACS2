using DotNetNuke.Abstractions;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
    public partial class Phones : EmployeeDBModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        #endregion
        #region Methods
        public Phones()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void UpdateSWNContact()
        {
            LoginRequest loginRequest = new LoginRequest { Password = SwnPassword, Username = SwnUsername };
            string token = SwnInterface.GetToken(SwnServiceIdentifier, SwnSubscriptionKey, loginRequest);
            var ctl = new EmployeeController();
            Employee employee = ctl.GetEmployee(EmployeeId);
            if (employee != null)
            {
                try
                {
                    SwnInterface.AddUpdateSwnContact(employee, SwnServiceIdentifier, SwnSubscriptionKey, token);
                }
                catch (Exception exc)
                {
                    ltMessage.Text = "<div class='alert alert-danger'><i class='fas fa-exclamation-circle'></i> Failed to sync contact in SWN</div>";
                    string process = string.Format("{1} {0} SWN Contact Information", employee.FullName, EmployeeId > 0 ? "Update" : "Add");
                    SwnLog swnLog = new SwnLog { CreatedBy = UserId, CreatedDate = DateTime.Now, Exception = exc.InnerException.Message, Process = process };
                    var logCtl = new SwnLogController();
                    logCtl.CreateSwnLog(swnLog);
                }
            }
        }
        private void ClearForm()
        {
            txtCascade.Text = GetMaxPhoneCascade().ToString();
            txtExtension.Text = string.Empty;
            txtNumber.Text = string.Empty;
            drpLocation.SelectedIndex = 0;
            drpType.SelectedIndex = 0;
            chkExcludeExt.Checked = false;
            chkSWNCall.Checked = false;
            chkSWNText.Checked = false;
            hdPhoneId.Value = string.Empty;

        }
        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                chkSWNCall.InputAttributes.Add("class", "form-check-input");
                chkSWNCall.LabelAttributes.Add("class", "form-check-label");
                chkSWNText.InputAttributes.Add("class", "form-check-input");
                chkSWNText.LabelAttributes.Add("class", "form-check-label");
                chkExcludeExt.InputAttributes.Add("class", "form-check-input");
                chkExcludeExt.LabelAttributes.Add("class", "form-check-label");

                //Implement your edit logic for your module
                if (!Page.IsPostBack)
                {
                    lnkDetails.NavigateUrl = DetailUrl;
                    if (DotNetNuke.Framework.AJAX.IsInstalled())
                    {
                        DotNetNuke.Framework.AJAX.RegisterScriptManager();
                    }
                    JavaScript.RequestRegistration(CommonJs.jQuery);
                    PopulateDropDowns();
                    PopulatePhoneList();
                    var ctl = new EmployeeController();
                    Employee employee = ctl.GetEmployee(EmployeeId);
                    if (employee != null)
                    {
                        txtCascade.Text = GetMaxPhoneCascade().ToString();
                        if (!employee.IsEmployee)
                        {
                            lnkDetails.NavigateUrl = ContactDetailUrl;
                            liEmergencyContacts.Visible = false;
                            liGroups.Visible = false;
                            liHistory.Visible = false;
                        }
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        private void PopulateDropDowns()
        {
            var lCtl = new OfficeLocationController();
            drpLocation.DataTextField = "Description";
            drpLocation.DataValueField = "OfficeLocationId";
            drpLocation.DataSource = lCtl.GetOfficeLocations();
            drpLocation.DataBind();
        }
        private int GetMaxPhoneCascade()
        {
            var ctl = new PhoneController();
            return ctl.GetMaxPhoneCascade(EmployeeId);
        }
        private void PopulatePhoneList()
        {
            var ctl = new PhoneController();
            var items= ctl.GetPhoneListByEmployee(EmployeeId);
            rptPhones.DataSource = items;
            rptPhones.DataBind();
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new PhoneController();
            Phone phone = new Phone();
            bool isNew = true;
            if (hdPhoneId.Value != "")
            {
                isNew = false;
                phone = ctl.GetPhone(Convert.ToInt32(hdPhoneId.Value));
            }
            phone.PhoneType = drpType.SelectedValue;
            phone.PhoneNumber = Regex.Replace(txtNumber.Text, @"[^\d]", "");
            Int32.TryParse(txtCascade.Text, out int cascade);
            phone.PhoneCascade = cascade < 0 ? 0 : cascade;
            phone.EmployeeId = EmployeeId;
            phone.Extension = txtExtension.Text;
            if (drpLocation.SelectedValue != "")
            {
                Int32.TryParse(drpLocation.SelectedValue, out int locationId);
                phone.OfficeLocationId = locationId;
            }
            phone.SwnCall = chkSWNCall.Checked;
            phone.SwnExcludeExtension = chkExcludeExt.Checked;
            phone.SwnText = chkSWNText.Checked;
            phone.LastModifiedDate = DateTime.Now;
            phone.LastModifiedById = UserId;
            if (isNew)
            {
                phone.CreatedById = UserId;
                phone.CreatedDate = DateTime.Now;
                ctl.CreatePhone(phone);
            }
            else
            {
                ctl.UpdatePhone(phone);
            }
            ClearForm();
            PopulatePhoneList();
            if (phone.SwnCall | phone.SwnText)
            {
                UpdateSWNContact();
            }
        }
        protected void pnlPhones_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        protected void rptPhones_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int phoneId = Convert.ToInt32(e.CommandArgument);
            var ctl = new PhoneController();
            if (e.CommandName == "delete")
            {
                ctl.DeletePhone(phoneId);
                UpdateSWNContact();
                PopulatePhoneList();
            }
            if (e.CommandName == "up" | e.CommandName == "down")
            {
                string direction = e.CommandName;
                ctl.MovePhoneCascade(EmployeeId, phoneId, direction);
                PopulatePhoneList();
            }
            if (e.CommandName == "edit")
            {
                Phone phone = ctl.GetPhone(phoneId);
                hdPhoneId.Value = phoneId.ToString();
                txtNumber.Text = phone.PhoneNumber;
                txtExtension.Text = phone.Extension;
                txtCascade.Text = phone.PhoneCascade.ToString();
                drpLocation.SelectedValue = phone.OfficeLocationId.ToString();
                drpType.SelectedValue = phone.PhoneType;
                chkExcludeExt.Checked = phone.SwnExcludeExtension;
                chkSWNCall.Checked = phone.SwnCall;
                chkSWNText.Checked = phone.SwnText;
                ScriptManager.RegisterStartupScript(rptPhones, rptPhones.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptPhones_ItemCreated(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
                LinkButton cmdEdit = (LinkButton)e.Item.FindControl("cmdEdit");
                LinkButton cmdDelete = (LinkButton)e.Item.FindControl("cmdDelete");
                LinkButton cmdUp = (LinkButton)e.Item.FindControl("cmdUp");
                LinkButton cmdDown = (LinkButton)e.Item.FindControl("cmdDown");

                scriptMan.RegisterAsyncPostBackControl(cmdUp);
                scriptMan.RegisterAsyncPostBackControl(cmdDown);
                scriptMan.RegisterAsyncPostBackControl(cmdDelete);
                scriptMan.RegisterAsyncPostBackControl(cmdEdit);
            }
        }
        protected void cmdFixSort_Click(object sender, EventArgs e)
        {
            var ctl = new PhoneController();
            ctl.FixPhoneSort(EmployeeId);
            PopulatePhoneList();
        }
        #endregion

        protected void rptPhones_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            LinkButton cmdUp = (LinkButton)e.Item.FindControl("cmdUp");
            LinkButton cmdDown = (LinkButton)e.Item.FindControl("cmdDown");
            if (e.Item.ItemIndex == 0)
                cmdUp.Visible = false;
            int itemCount = ((IEnumerable)rptPhones.DataSource).Cast<object>().Count();
            if (e.Item.ItemIndex == itemCount - 1)
                cmdDown.Visible = false;
        }
    }
}