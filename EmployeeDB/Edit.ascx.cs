using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using System.Web.UI;
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
    public partial class Edit : EmployeeDBModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Implement your edit logic for your module
                if (!Page.IsPostBack)
                {
                    chkActive.InputAttributes.Add("class", "custom-control-input");
                    chkActive.LabelAttributes.Add("class", "custom-control-label");
                    chkManateeAccess.InputAttributes.Add("class", "custom-control-input");
                    chkManateeAccess.LabelAttributes.Add("class", "custom-control-label");
                    JavaScript.RequestRegistration(CommonJs.jQuery);
                    PopulateDropDowns();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PopulateDropDowns()
        {
            var rCtl=new RaceController();
            drpRace.DataSource = rCtl.GetRaces().OrderBy(x=>x.Description);
            drpRace.DataBind();

            var lCtl=new OfficeLocationController();
            drpLocation.DataSource=lCtl.GetOfficeLocations().OrderBy(x=>x.Description);
            drpLocation.DataBind();

            var dCtl = new GroupController();
            drpDepartment.DataSource = dCtl.GetGroups(0).OrderBy(x=>x.GroupName);
            drpDepartment.DataBind();

            var gCtl = new JobGroupController();
            drpJobGroup.DataSource = gCtl.GetJobGroups().OrderBy(x=>x.Description);
            drpJobGroup.DataBind();

            var cCtl = new JobClassController();
            drpJobClass.DataSource = cCtl.GetJobClasses().OrderBy(x=>x.ClassName);
            drpJobClass.DataBind();

            var cnCtl = new Globals.CountyController();
            drpCounty.DataSource=cnCtl.GetCounties().OrderBy(x=>x.CountyName);
            drpCounty.DataBind();

            var eCtl = new EmployeeController();
            drpSupervisor.DataSource = eCtl.GetEmployeeDropDown(SupervisorRole).OrderBy(x=>x.DataText);
            drpSupervisor.DataBind();
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var t = new Employee();
            var tc = new EmployeeController();

            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL());
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL());
        }
    }
}