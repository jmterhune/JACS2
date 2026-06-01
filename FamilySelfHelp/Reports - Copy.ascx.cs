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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
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
            IEnumerable<Log> lstLog = Enumerable.Empty<Log>();
            IEnumerable<Report> lstCaseTypeReport = Enumerable.Empty<Report>();
            IEnumerable<Report> lstServiceReport = Enumerable.Empty<Report>();
            var ctl = new Components.LogController();
            DateTime.TryParse(txtStartDate.Text, out startdate);
            DateTime.TryParse(txtEndDate.Text, out  enddate);
            if(drpDivisions.SelectedValue != "all")
            {
                lstLog = ctl.GetReport(startdate, enddate);
                lstCaseTypeReport=ctl.GetCaseTypeReport(startdate, enddate);
                lstServiceReport=ctl.GetServiceReport(startdate,enddate);
            }
            else
            {
                lstLog = ctl.GetReport(startdate, enddate,division);
                lstCaseTypeReport= ctl.GetCaseTypeReport(startdate,enddate,division);
                lstServiceReport= ctl.GetServiceReport(startdate,enddate, division);
            }
            if (lstLog.Count() > 0)
            {
                var clientTypeList = lstLog.GroupBy(x => x.ClientType, (field, itemCount) => new { Client = field, ClientCount = itemCount.Count() });
                var contactMethodlist = lstLog.GroupBy(x => x.ContactMethod, (field, itemCount) => new { Method = field, MethodCount = itemCount.Count() });
                var caseTypeList = lstCaseTypeReport.GroupBy(x => x.Name, (field, itemCount) => new { CaseType = field, CaseTypeCount = itemCount.Count() });
                var serviceProvidedList = lstServiceReport.GroupBy(x => x.Name, (field, itemCount) => new { Service = field, ServiceCount = itemCount.Count() });
                var divisionlist = lstLog.GroupBy(x => x.Division, (field, itemCount) => new { Division = field, DivisionCount = itemCount.Count() });
                var totalCustomers = lstLog.GroupBy(x => x.ClientId, (field, itemCount) => new { ClientId = field, ClientCount = itemCount.Count() }).Count();
                int interpreterCount = lstLog.Where(x => x.InterpreterProvided == true).Count();
                decimal totalTime = lstLog.Sum(x => x.TimeSpent);
                decimal averageTime = lstLog.Average(x => x.TimeSpent);
                int newCase = lstLog.Where(x => x.IsNewCase == true).Count();
                rptCaseType.DataSource = caseTypeList;
                rptCaseType.DataBind();
                rptClientTypes.DataSource = clientTypeList;
                rptClientTypes.DataBind();
                rptContactMethod.DataSource=contactMethodlist;
                rptContactMethod.DataBind();
                rptServiceProvided.DataSource=serviceProvidedList;
                rptServiceProvided.DataBind();
                rptDivision.DataSource=divisionlist;
                rptDivision.DataBind();
                ltCustomerTotal.Text=totalCustomers.ToString();
                ltAverage.Text=averageTime.ToString("0.00");
                ltInterpreter.Text=interpreterCount.ToString();
                ltTotal.Text=totalTime.ToString("0.00");
                ltCase.Text=newCase.ToString();
                pnlReport.Visible = true;
            }
            else
            {
                ltMessage.Text = "No Records returned";
                pnlReport.Visible = false;

            }
        }
    }
}