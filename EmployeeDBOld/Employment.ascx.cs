using DotNetNuke.Abstractions;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
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
                        if(string.IsNullOrEmpty(SSN))
                        {
                            ltMessage.Text = string.Format(ltMessage.Text, "The User must have a Social Security Number in order to add work history. Return to the Details tab and enter a Social Security Number and update the record.");
                            pnlPositionHistory.Attributes.Add("disabled","true") ;
                            pnlServiceHistory.Attributes.Add("disabled", "true");
                        }
                    }
                    if (DotNetNuke.Framework.AJAX.IsInstalled())
                    {
                        DotNetNuke.Framework.AJAX.RegisterScriptManager();
                    }
                    JavaScript.RequestRegistration(CommonJs.jQuery);
                    PopulatePositionList();
                    PopulateServiceList();
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
        private void PopulateServiceList()
        {
            var ctl = new ServiceHistoryController();
            rptServiceHistory.DataSource = ctl.GetServiceHistoriesByEmployee(SSN);
            rptServiceHistory.DataBind();
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
            positionHistory.EntryType = drpType.SelectedValue;
            positionHistory.Description = txtPosition.Text;
            positionHistory.IsInternal = drpExternal.SelectedValue=="Internal";
            positionHistory.SocialSecurityNumber = SSN;
            if(DateTime.TryParse(txtStartDate.Text, out DateTime startDate))
            positionHistory.StartDate = startDate;
            if (DateTime.TryParse(txtEndDate.Text, out DateTime endDate))
            positionHistory.EndDate = endDate;
            positionHistory.LastModifiedDate = DateTime.Now;
            positionHistory.LastModifiedByID = UserId;
            if (isNew)
            {
                positionHistory.CreatedByID = UserId;
                positionHistory.CreatedDate = DateTime.Now;
                ctl.CreatePositionHistory(positionHistory);
            }
            else
            {
                ctl.UpdatePositionHistory(positionHistory);
            }
            ClearPositionHistoryForm();
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
            }
            if (e.CommandName == "edit")
            {
                PositionHistory positionHistory = ctl.GetPositionHistory(positionHistoryId);
                hdPositionHistoryId.Value = positionHistoryId.ToString();
                txtPosition.Text = positionHistory.Description;
                drpExternal.SelectedValue = "Internal";
                if (!positionHistory.IsInternal)
                    drpExternal.SelectedValue = "External";
                if (positionHistory.StartDate.HasValue)
                txtStartDate.Text = positionHistory.StartDate.Value.ToShortDateString();
                if(positionHistory.EndDate.HasValue)
                txtEndDate.Text = positionHistory.EndDate.Value.ToShortDateString();
                drpType.SelectedValue = positionHistory.EntryType;
                ScriptManager.RegisterStartupScript(rptPositionHistory, rptPositionHistory.GetType(), "ToggleForm", "TogglePositionForm(true)", true);
            }
            PopulatePositionList();

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

        protected void rptServiceHistory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int serviceHistoryId = Convert.ToInt32(e.CommandArgument);
            var ctl = new ServiceHistoryController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteServiceHistory(serviceHistoryId);
            }
            if (e.CommandName == "edit")
            {
                ServiceHistory serviceHistory = ctl.GetServiceHistory(serviceHistoryId);
                hdServiceHistoryId.Value = serviceHistoryId.ToString();
                txtCompany.Text= serviceHistory.CompanyName;
                txtLastPayRate.Text = serviceHistory.LastPayRate.Value.ToString("C", CultureInfo.CurrentCulture);
                if (serviceHistory.HireDate.HasValue)
                    txtHireDate.Text = serviceHistory.HireDate.Value.ToShortDateString();
                if (serviceHistory.TerminationDate.HasValue)
                    txtTerminationDate.Text = serviceHistory.TerminationDate.Value.ToShortDateString();
                ScriptManager.RegisterStartupScript(rptServiceHistory, rptServiceHistory.GetType(), "ToggleForm", "ToggleServiceForm(true)", true);
            }
            PopulateServiceList();
        }

        protected void rptServiceHistory_ItemCreated(object sender, RepeaterItemEventArgs e)
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

        protected void cmdSaveService_Click(object sender, EventArgs e)
        {
            var ctl = new ServiceHistoryController();
            ServiceHistory serviceHistory = new ServiceHistory();
            bool isNew = true;
            if (hdServiceHistoryId.Value != "")
            {
                isNew = false;
                serviceHistory = ctl.GetServiceHistory(Convert.ToInt32(hdServiceHistoryId.Value));
            }
            serviceHistory.CompanyName = txtCompany.Text;
            Decimal.TryParse(Helper.CleanDecimal(txtLastPayRate.Text), out decimal payRate);
            if (payRate > 0)
            {
                serviceHistory.LastPayRate = payRate;
            }
            serviceHistory.SocialSecurityNumber = SSN;
            if (DateTime.TryParse(txtHireDate.Text, out DateTime hireDate))
                serviceHistory.HireDate = hireDate;
            if (DateTime.TryParse(txtTerminationDate.Text, out DateTime terminationDate))
                serviceHistory.TerminationDate = terminationDate;
            serviceHistory.LastModifiedDate = DateTime.Now;
            serviceHistory.LastModifiedByID = UserId;
            if (isNew)
            {
                serviceHistory.CreatedByID = UserId;
                serviceHistory.CreatedDate = DateTime.Now;
                ctl.CreateServiceHistory(serviceHistory);
            }
            else
            {
                ctl.UpdateServiceHistory(serviceHistory);
            }
            ClearPositionHistoryForm();
            PopulateServiceList();
        }
        private void ClearPositionHistoryForm()
        {
            drpType.SelectedIndex = 0;
            txtPosition.Text = string.Empty;
            drpExternal.SelectedIndex = 0;
            txtStartDate.Text = string.Empty;
            txtEndDate.Text = string.Empty;
            hdPositionHistoryId.Value=string.Empty;
            hdServiceHistoryId.Value=string.Empty;
            txtCompany.Text = string.Empty;
            txtHireDate.Text = string.Empty;
            txtTerminationDate.Text = string.Empty;
            txtLastPayRate.Text = string.Empty;
        }

        protected void pnlServiceHistory_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
    }
}