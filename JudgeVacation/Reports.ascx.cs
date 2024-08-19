using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
    public partial class Reports : JudgeVacationModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        private int _judgeId = 0;
        private int _vacationSum = 0;
        #endregion
        #region Methods
        public Reports()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void PopulateJudgeList()
        {
            var users = RoleController.Instance.GetUsersByRole(PortalId, "judge");
            var userList = new List<UserInfo>();
            foreach (var u in users)
                userList.Add((UserInfo)u);

            drpJudges.DataTextField = "DisplayName";
            drpJudges.DataValueField = "UserId";
            drpJudges.DataSource = userList.OrderBy(x => x.DisplayName);
            drpJudges.DataBind();

        }

        private void BindData()
        {
         bool hasStartDate=   DateTime.TryParse(StartDatePicker.Text,out DateTime startDate);
          bool hasEndDate=  DateTime.TryParse(EndDatePicker.Text, out DateTime endDate);
            var ctl = new JudgeVacationController();

            if (!hasStartDate)
                startDate = new DateTime(DateTime.Now.Year, 1, 1);
            if (!hasEndDate)
                endDate = new DateTime(DateTime.Now.Year, 12, 31);
            IEnumerable<Components.JudgeVacation> reportList = null;
            if (drpJudges.SelectedValue != "0")
            {
                reportList = ctl.GetVacationReportByJudge(startDate, endDate, int.Parse(drpJudges.SelectedValue));
            }
            else
            {
                reportList = ctl.GetVacationReport(startDate, endDate);
            }
            var hCtl = new HolidayController();
            var holidays = hCtl.GetReportHolidays(startDate.Year - 1, endDate.Year + 1);

            foreach (var r in reportList)
            {

                if (r.StartDate < startDate)
                {
                    var processDate = r.StartDate;
                    while (processDate < startDate)
                    {
                        if (!holidays.Select(h => h.HolidayDate.ToShortDateString()).Contains(processDate.ToShortDateString()))
                        {
                            if (processDate.DayOfWeek != DayOfWeek.Saturday & processDate.DayOfWeek != DayOfWeek.Sunday)
                            {
                                r.VacationDays = r.VacationDays - 1;
                            }
                        }
                        processDate = processDate.AddDays(1d);
                    }
                }
                if (r.EndDate > endDate)
                {
                    var processDate = r.EndDate;
                    while (processDate > endDate)
                    {
                        if (!holidays.Select(h => h.HolidayDate).Contains(processDate))
                        {
                            if (processDate.DayOfWeek != DayOfWeek.Saturday & processDate.DayOfWeek != DayOfWeek.Sunday)
                            {
                                r.VacationDays = r.VacationDays - 1;
                            }
                        }
                        processDate = processDate.AddDays(-1);
                    }
                }

                var maxEnddate = reportList.Where(x => x.JudgeID == r.JudgeID).Max(y => y.EndDate);
                if (r.EndDate != maxEnddate)
                {
                    r.SubTotal = 0;
                }
            }
            rptVacationDays.DataSource = reportList;
            rptVacationDays.DataBind();
            int totalVacation = 0;
            if (rptVacationDays.Items.Count == 0)
            {
                ltMessage.Text = "<div class='dnnFormMessage dnnFormValidationSummary'>No Vacations Taken During Requested Time Frame!</div>";
            }
            else
            {
                totalVacation = reportList.Select(p => p.VacationDays).Sum();
            }
            Literal ltTotal = (rptVacationDays.Controls[rptVacationDays.Controls.Count - 1].Controls[0].FindControl("ltTotal") as Literal);
            ltTotal.Text = totalVacation.ToString();
        }

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    bool IsInRole = false;
                    if (Settings["ReportingRole"] != null)
                    {
                        if (UserInfo.IsInRole(Settings["ReportingRole"].ToString()))
                        {
                            IsInRole = true;
                        }
                    }
                    StartDatePicker.Text = new DateTime(DateTime.Now.Year, 1, 1).ToShortDateString();
                    EndDatePicker.Text = new DateTime(DateTime.Now.Year, 12, 31).ToShortDateString();
                    if (!IsInRole)
                    {
                        Response.Redirect(_navigationManager.NavigateURL());
                    }
                    PopulateJudgeList();
                    cmdCancel.NavigateUrl = _navigationManager.NavigateURL();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            ltMessage.Text = "";
            BindData();
        }
        protected void rptVacationDays_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Components.JudgeVacation item = (Components.JudgeVacation)e.Item.DataItem;
                if (!(_judgeId == 0))
                {
                    if (_judgeId == item.JudgeID)
                    {
                        _vacationSum += item.VacationDays;
                    }
                    else
                    {
                        _vacationSum = item.VacationDays;
                    }
                }
            }
        }
    }
}