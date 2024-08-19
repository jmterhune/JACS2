using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.JudgeVacation.Components;

namespace tjc.Modules.JudgeVacation
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from JudgeVacationModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Holidays : JudgeVacationModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion
        #region Methods
        public Holidays()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void PopulateYears()
        {
            drpYear.DataTextField = "Years";
            drpYear.DataValueField = "Years";
            var ctl = new HolidayController();
            var years = ctl.GetYearsAvailable();
            if (years.Where(x => x.Years == DateTime.Now.Year).Count() <= 0)
            {
                years.Append(new Components.AvailableYears(DateTime.Now.Year));
            }
            drpYear.DataSource = years.OrderByDescending(x => x.Years);
            drpYear.DataBind();
        }

        private void BindData()
        {
            var ctl=new HolidayController();
            rptHolidays.DataSource = ctl.GetHolidays(CurrentYear);
            rptHolidays.DataBind();
        }

        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    cmdCancel.NavigateUrl =_navigationManager.NavigateURL();

                    PopulateYears();
                    drpYear.SelectedValue = CurrentYear.ToString();
                    BindData();
                }
                else
                {
                    CurrentYear = int.Parse(drpYear.SelectedValue);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void rptHolidays_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                int ID = int.Parse(e.CommandArgument.ToString());
                var ctl = new HolidayController();
                ctl.DeleteHoliday(ID);
                BindData();
            }
        }


        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var objHoliday = new Components.Holiday()
            {
                HolidayDate = DateTime.Parse(HolidayDatePicker.Text),
                Description = txtDescription.Text
            };
            var ctl = new HolidayController();

            ctl.CreateHoliday(objHoliday);
            Response.Redirect(EditUrl("holiday"));
        }

        protected void drpYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentYear = int.Parse(drpYear.SelectedValue);
            BindData();
        }

        #endregion
    }
}