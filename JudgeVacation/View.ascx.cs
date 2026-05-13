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
    public partial class View : JudgeVacationModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion
        #region Methods
        public View()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void PopulateYears()
        {
            drpYear.DataTextField = "Years";
            drpYear.DataValueField = "Years";
            var ctl = new HolidayController();
            var years = ctl.GetYearsAvailable(UserId);
            if (years.Where(x => x.Years == DateTime.Now.Year).Count() <= 0)
            {
                years.Append(new AvailableYears(DateTime.Now.Year));
            }
            drpYear.DataSource = years.OrderByDescending(x => x.Years);
            drpYear.DataBind();
        }

        private void BindData()
        {
            var ctl = new JudgeVacationController();
            rptVacationDays.DataSource = ctl.GetJudgeVacations(UserId, CurrentYear);
            rptVacationDays.DataBind();
        }
        private void SendEmails(string action, string startDate, string endDate, string pStartDate, string pEndDate)
        {
            string subject = "Judicial Vacation Tracker Notice";
            string body = "";

            switch (action ?? "")
            {
                case "s":
                    {
                        body = string.Format("{0} Added Vacation Dates {1} - {2}", UserInfo.DisplayName, startDate, endDate);
                        break;
                    }
                case "u":
                    {
                        body = string.Format("{0} Updated Vacation Dates from {1} - {2} to {3} - {4}", UserInfo.DisplayName, pStartDate, pEndDate, startDate, endDate);
                        break;
                    }
                case "d":
                    {
                        body = string.Format("{0} Deleted Vacation Dates {1} - {2}", UserInfo.DisplayName, startDate, endDate);
                        break;
                    }
            }
            string emails = EmailTo;
            foreach (var email in emails.Split(','))
                DotNetNuke.Services.Mail.Mail.SendEmail("noreply.vt@jud12.flcourts.org", email.Trim(), subject, body);
        }

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (UserInfo.IsInRole(ReportingRole))
                    {
                        lnkReports.Visible = true;
                        lnkReports.NavigateUrl = EditUrl("reports");
                    }
                    if (UserId > 0 && UserInfo.IsAdmin)
                    {
                        lnkHolidays.Visible = true;
                        lnkHolidays.NavigateUrl = EditUrl("holiday");
                    }
                    if (CalenderID > 0)
                    {
                        cmdSave.Visible = false;
                        cmdUpdate.Visible = true;
                        pnlRecords.Visible = false;
                        var ctl = new JudgeVacationController();
                        var objJC = ctl.GetJudgeVacation(CalenderID);
                        if (objJC != null)
                        {
                            StartDatePicker.Text = objJC.StartDate.ToShortDateString();
                            EndDatePicker.Text = objJC.EndDate.ToShortDateString();
                        }
                    }
                    else
                    {
                        PopulateYears();
                        drpYear.SelectedValue = CurrentYear.ToString();
                        BindData();
                    }
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

        protected void rptVacationDays_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                int mycalendarId = int.Parse(e.CommandArgument.ToString());
                var ctl = new JudgeVacationController();
                var objJC = ctl.GetJudgeVacation(mycalendarId);
                ctl.DeleteJudgeVacation(mycalendarId);
                SendEmails("d", objJC.StartDate.ToShortDateString(), objJC.EndDate.ToShortDateString(), "", "");
                BindData();
            }
        }

        protected void rptVacationDays_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Components.JudgeVacation item = (Components.JudgeVacation)e.Item.DataItem;
                HyperLink lnkEdit = (HyperLink)e.Item.FindControl("lnkEdit");
                EditUrl();
                lnkEdit.NavigateUrl = _navigationManager.NavigateURL() + "?calId=" + item.CalendarID;
            }
        }
        protected void drpYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentYear = int.Parse(drpYear.SelectedValue);
            BindData();
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            DateTime.TryParse(StartDatePicker.Text, out DateTime startDate);
            DateTime.TryParse(EndDatePicker.Text, out DateTime endDate);
            if (!string.IsNullOrWhiteSpace(StartDatePicker.Text) & !string.IsNullOrWhiteSpace(EndDatePicker.Text))
            {
                var ctl = new JudgeVacationController();
                var hCtl = new HolidayController();
                var objJC = new Components.JudgeVacation()
                {
                    StartDate = startDate,
                    EndDate = endDate
                };
                objJC.VacationDays = hCtl.GetActualVacationDays(objJC.StartDate, objJC.EndDate);
                objJC.JudgeID = UserId;
                ctl.CreateJudgeVacation(objJC);
                SendEmails("s", objJC.StartDate.ToShortDateString(), objJC.EndDate.ToShortDateString(), "", "");
                ModuleController.SynchronizeModule(ModuleId);
                Response.Redirect(_navigationManager.NavigateURL());
            }
            else
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                    "new Noty({ text: '" + System.Web.HttpUtility.JavaScriptStringEncode("Please Check the Start and End Dates") + "', type: 'error', timeout: 4500, layout: 'topRight', theme: 'mint' }).show();", true);
            }
        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            DateTime.TryParse(StartDatePicker.Text, out DateTime startDate);
            DateTime.TryParse(EndDatePicker.Text, out DateTime endDate);
            var ctl = new JudgeVacationController();
            var hCtl = new HolidayController();
            var objJC = ctl.GetJudgeVacation(CalenderID);
            var previousStartDate = objJC.StartDate;
            var previousEndDate = objJC.EndDate;
            objJC.StartDate = startDate;
            objJC.EndDate = endDate;
            objJC.VacationDays = hCtl.GetActualVacationDays(objJC.StartDate, objJC.EndDate);
            ctl.UpdateJudgeVacation(objJC);
            SendEmails("u", objJC.StartDate.ToShortDateString(), objJC.EndDate.ToShortDateString(), previousStartDate.ToShortDateString(), previousEndDate.ToShortDateString());
            ModuleController.SynchronizeModule(ModuleId);
            Response.Redirect(_navigationManager.NavigateURL());
        }
    }
}