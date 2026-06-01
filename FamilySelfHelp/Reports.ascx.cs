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
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.FamilySelfHelp.Components;

namespace tjc.Modules.FamilySelfHelp
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from FamilySelfHelpModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Reports : FamilySelfHelpModuleBase
    {
        private readonly INavigationManager _navigationManager;
        private ModuleSecurity modSecurty;

        public Reports()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                JavaScript.RequestRegistration(CommonJs.DnnPlugins);

                if (!IsPostBack)
                {
                    modSecurty = new ModuleSecurity(this.ModuleConfiguration);

                    lnkDataEntry.NavigateUrl = EditUrl("log");
                    lnkMerge.NavigateUrl = EditUrl("merge");
                    lnkReports.NavigateUrl = EditUrl("report");
                    lnkSearch.NavigateUrl = _navigationManager.NavigateURL();
                    if (IsAdmin)
                    {
                        lnkMerge.Visible = true;
                        lnkReports.Visible = true;
                    }
                    if (modSecurty.HasReportPermission)
                        lnkReports.Visible = true;
                    if (modSecurty.HasMergePermission)
                        lnkMerge.Visible = true;

                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdReport_Click(object sender, EventArgs e)
        {
            DateTime startdate = DateTime.Now.AddDays(-1);
            DateTime enddate = DateTime.Now;
            string division = drpDivisions.SelectedValue;

            DateTime.TryParse(txtStartDate.Text, out startdate);
            DateTime.TryParse(txtEndDate.Text, out enddate);

            var ctl = new Components.LogController();

            IEnumerable<Log> lstLog;
            IEnumerable<Report> lstCaseTypeReport;
            IEnumerable<Report> lstServiceReport;

            if (string.IsNullOrEmpty(division) || division == "All")
            {
                lstLog = ctl.GetReport(startdate, enddate);
                lstCaseTypeReport = ctl.GetCaseTypeReport(startdate, enddate);
                lstServiceReport = ctl.GetServiceReport(startdate, enddate);
            }
            else
            {
                lstLog = ctl.GetReport(startdate, enddate, division);
                lstCaseTypeReport = ctl.GetCaseTypeReport(startdate, enddate, division);
                lstServiceReport = ctl.GetServiceReport(startdate, enddate, division);
            }

            if (!lstLog.Any())
            {
                ltMessage.Text = "No Records returned";
                pnlReport.Visible = false;
                return;
            }

            // Group by Location
            var locationGroups = lstLog
                .GroupBy(x => string.IsNullOrWhiteSpace(x.Location) ? "Unknown" : x.Location.Trim())
                .OrderBy(g => g.Key)
                .ToList();

            var reportData = new List<LocationReportViewModel>();

            foreach (var group in locationGroups)
            {
                var logs = group.ToList();
                string currentLocation = group.Key;

                var vm = new LocationReportViewModel
                {
                    Location = currentLocation,

                    ClientTypes = logs.GroupBy(x => x.ClientType ?? "None")
                        .Select(g => new CountItem { Name = g.Key, Count = g.Count() })
                        .OrderBy(x => x.Name),

                    ContactMethods = logs.GroupBy(x => x.ContactMethod ?? "None")
                        .Select(g => new CountItem { Name = g.Key, Count = g.Count() })
                        .OrderBy(x => x.Name),

                    CaseTypes = lstCaseTypeReport
                        .Where(r => logs.Any(l => l.LogId == r.LogId))
                        .GroupBy(x => x.Name ?? "None")
                        .Select(g => new CountItem { Name = g.Key, Count = g.Count() })
                        .OrderBy(x => x.Name),

                    Services = lstServiceReport
                        .Where(r => logs.Any(l => l.LogId == r.LogId))
                        .GroupBy(x => x.Name ?? "None")
                        .Select(g => new CountItem { Name = g.Key, Count = g.Count() })
                        .OrderBy(x => x.Name),

                    Divisions = logs.GroupBy(x => x.Division ?? "None")
                        .Select(g => new CountItem { Name = g.Key, Count = g.Count() })
                        .OrderBy(x => x.Name),

                    InterpreterRequested = logs.Count(x => x.InterpreterProvided == true),
                    NewCases = logs.Count(x => x.IsNewCase == true),
                    TotalTime = logs.Sum(x => x.TimeSpent),
                    AverageTime = logs.Any() ? logs.Average(x => x.TimeSpent) : 0m,
                    UniqueCustomers = logs.Select(x => x.ClientId).Distinct().Count()
                };

                reportData.Add(vm);
            }

            rptLocations.DataSource = reportData;
            rptLocations.DataBind();

            pnlReport.Visible = true;
            ltMessage.Text = "";
        }
    }
}