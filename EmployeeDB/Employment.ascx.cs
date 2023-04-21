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
    public partial class Employment : EmployeeDBModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private EmployeeController ctl = new EmployeeController();

        public Employment()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        public string SSN
        {
            get
            {
                object o = ViewState["SSN"];
                return (o == null) ? String.Empty : (string)o;
            }

            set
            {
                ViewState["SSN"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Implement your edit logic for your module
                if (!Page.IsPostBack)
                {
                    Employee employee = ctl.GetEmployee(EmployeeId);
                    if (employee != null)
                    {
                        SSN = employee.SocialSecurityNumber;
                    }
                    if (DotNetNuke.Framework.AJAX.IsInstalled())
                    {
                        DotNetNuke.Framework.AJAX.RegisterScriptManager();
                    }
                    JavaScript.RequestRegistration(CommonJs.jQuery);
                    PopulatePositionList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PopulatePositionList()
        {
            var ctl = new PositionHistoryController();
            rptPositionHistory.DataSource = ctl.GetPositionHistoriesByEmployee(SSN);
            rptPositionHistory.DataBind();
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new PositionHistoryController();
            PositionHistory positionHistory = new PositionHistory();
            bool isNew = true;
            if (hdPositionHistoryId.Value != "")
            {
                isNew = false;
                positionHistory = ctl.GetPositionHistory(Convert.ToInt32(hdPositionHistoryId.Value));
            }
            positionHistory.PositionHistoryType = drpType.SelectedValue;
            positionHistory.PositionHistoryNumber = txtNumber.Text;
            positionHistory.PositionHistoryCascade = txtCascade.Text;
            positionHistory.EmployeeId = EmployeeId;
            positionHistory.Extension = txtExtension.Text;
            if (drpLocation.SelectedValue != "")
            {
                Int32.TryParse(drpLocation.SelectedValue, out int locationId);
                positionHistory.OfficeLocationId = locationId;
            }
            positionHistory.SwnCall = chkSWNCall.Checked;
            positionHistory.SwnExcludeExtension = chkExcludeExt.Checked;
            positionHistory.SwnText = chkSWNText.Checked;
            positionHistory.LastModifiedDate = DateTime.Now;
            positionHistory.LastModifiedById = UserId;
            if (isNew)
            {
                positionHistory.CreatedById = UserId;
                positionHistory.CreatedDate = DateTime.Now;
                ctl.CreatePositionHistory(positionHistory);
            }
            else
            {
                ctl.UpdatePositionHistory(positionHistory);
            }
            hdPositionHistoryId.Value = "";
            PopulatePositionList();
        }

        protected void pnlPositionHistory_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptPositionHistory_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int positionHistoryId = Convert.ToInt32(e.CommandArgument);
            var ctl = new PositionHistoryController();
            if (e.CommandName == "delete")
            {
                ctl.DeletePositionHistory(positionHistoryId);
                PopulatePositionList();
            }
            if (e.CommandName == "edit")
            {
                PositionHistory positionHistory = ctl.GetPositionHistory(positionHistoryId);
                hdPositionHistoryId.Value = positionHistoryId.ToString();
                txtNumber.Text = positionHistory.PositionHistoryNumber;
                txtExtension.Text = positionHistory.Extension;
                txtCascade.Text = positionHistory.PositionHistoryCascade;
                drpLocation.SelectedValue = positionHistory.OfficeLocationId.ToString();
                drpType.SelectedValue = positionHistory.PositionHistoryType;
                chkExcludeExt.Checked = positionHistory.SwnExcludeExtension;
                chkSWNCall.Checked = positionHistory.SwnCall;
                chkSWNText.Checked = positionHistory.SwnText;
                ScriptManager.RegisterStartupScript(rptPositionHistorys, rptPositionHistorys.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }

        protected void rptPositionHistory_ItemCreated(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
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