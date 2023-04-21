using DotNetNuke.Abstractions;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
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
    public partial class Phones : EmployeeDBModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public Phones()
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
                    PopulateDropDowns();
                    PopulatePhoneList();
                    chkSWNCall.InputAttributes.Add("class", "form-check-input");
                    chkSWNCall.LabelAttributes.Add("class", "form-check-label");
                    chkSWNText.InputAttributes.Add("class", "form-check-input");
                    chkSWNText.LabelAttributes.Add("class", "form-check-label");
                    chkExcludeExt.InputAttributes.Add("class", "form-check-input");
                    chkExcludeExt.LabelAttributes.Add("class", "form-check-label");
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
        private void PopulatePhoneList()
        {
            var ctl = new PhoneController();
            rptPhones.DataSource = ctl.GetPhonesByEmployee(EmployeeId);
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
            phone.PhoneNumber = txtNumber.Text;
            phone.PhoneCascade = txtCascade.Text;
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
            hdPhoneId.Value = "";
            PopulatePhoneList();
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
                PopulatePhoneList();
            }
            if (e.CommandName == "edit")
            {
                Phone phone = ctl.GetPhone(phoneId);
                hdPhoneId.Value = phoneId.ToString();
                txtNumber.Text = phone.PhoneNumber;
                txtExtension.Text = phone.Extension;
                txtCascade.Text = phone.PhoneCascade;
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
                scriptMan.RegisterAsyncPostBackControl(cmdDelete);
                scriptMan.RegisterAsyncPostBackControl(cmdEdit);
            }
        }
    }
}