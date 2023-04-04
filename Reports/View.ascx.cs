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

using DotNetNuke.Services.Exceptions;
using System;
using System.Web.UI.WebControls;
using DotNetNuke.Abstractions;
using Microsoft.Extensions.DependencyInjection;

using tjc.Modules.Reports.Components;
using DotNetNuke.Framework;

namespace tjc.Modules.Reports
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from ReportsModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : ReportsModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public View()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (ReportId > 0)
                {
                    pnlReportList.Visible = false;
                    DotNetNuke.Framework.CDefault myPage = new DotNetNuke.Framework.CDefault();
                    myPage = (CDefault)this.Page;
                    
                    lnkReport.NavigateUrl = _navigationManager.NavigateURL();
                    if (ReportId == 1)
                    {
                        
                        pnlBirthdays.Visible = true;
                        hdTitle.Value= "Birthday Report";
                        myPage.Title = "Birthday Report";
                    }
                    else if (ReportId == 2)
                    {
                        pnlServiceAward.Visible = true;
                        hdTitle.Value = "Service / Employment Report";
                        myPage.Title = "Service / Employment Report";
                    }
                    else
                    {
                        pnlTerminationReport.Visible = true;
                        hdTitle.Value = "Termination Report";
                        myPage.Title = "Termination Report";
                    }
                }
                else
                {
                    lnkReport.Visible = false;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdSubmitBirthReport_Click(object sender, EventArgs e)
        {

            string county = drpCounty.SelectedValue;
            string countyTitle = "";
            int month = Convert.ToInt32(drpBirthMonth.SelectedValue);
            var ctl = new ReportController();
            if (county == "")
            {
                countyTitle = " for All Counties";
            }
            else
            {
                countyTitle = string.Format(" for {0} County", county);
            }
            ltReportTitle.Text = string.Format("{0} Birthday Report {1}", drpBirthMonth.SelectedItem.Text, countyTitle);
            grdReport.DataSource = ctl.GetBirthDates(month, county);
            grdReport.DataBind();
            grdReport.HeaderRow.TableSection = TableRowSection.TableHeader;


        }
        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (pnlBirthdays.Visible)
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {

                    for (int i = 0; i < e.Row.Controls.Count; i++)
                    {
                        var headerCell = e.Row.Controls[i] as DataControlFieldHeaderCell;
                        if (i == 0)
                        {
                            headerCell.Text = "First Name";
                        }
                        if (i == 1)
                        {
                            headerCell.Text = "Last Name";
                        }
                        if (i == 2)
                        {
                            headerCell.Text = "Birth Date";
                        }
                    }
                }
                else if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[2].Text = Convert.ToDateTime(e.Row.Cells[2].Text).ToString("MMMM dd");
                }
            }
            if (pnlServiceAward.Visible)
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;
                    for (int i = 0; i < e.Row.Controls.Count; i++)
                    {
                        var headerCell = e.Row.Controls[i] as DataControlFieldHeaderCell;
                        if (i == 0)
                        {
                            headerCell.HorizontalAlign= HorizontalAlign.Center;
                            headerCell.Text = "State<br />or<br />County";
                        }
                        if (i == 1)
                        {
                            headerCell.Text = "First Name";
                        }
                        if (i == 2)
                        {
                            headerCell.Text = "Last Name";
                        }
                        if (i == 3)
                        {
                            headerCell.Text = string.Format("{0} Date", drpReportType.SelectedValue == "1" ? "Service" : "Employment");
                        }
                        if (i == 4)
                        {
                            
                            headerCell.Text = string.Format("Years<br />of<br />{0}", drpReportType.SelectedValue == "1" ? "Service" : "Employment");
                        }
                    }
                }
                else if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[3].Text = Convert.ToDateTime(e.Row.Cells[3].Text).ToString("MM/dd/yyyy");
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
                    e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;
                }
            }
            if (pnlTerminationReport.Visible)
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    for (int i = 0; i < e.Row.Controls.Count; i++)
                    {
                        var headerCell = e.Row.Controls[i] as DataControlFieldHeaderCell;
                        if (i == 0)
                        {
                            headerCell.Text = "ID";
                        }
                        if (i == 1)
                        {
                            headerCell.Text = "First Name";
                        }
                        if (i == 2)
                        {
                            headerCell.Text = "Last Name";
                        }
                        if (i == 3)
                        {
                            headerCell.Text = "Terminated";
                        }
                        if (i == 4)
                        {
                            headerCell.Text = "Active";
                        }
                    }
                }
                else if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[3].Text = Convert.ToDateTime(e.Row.Cells[3].Text).ToString("MM/dd/yyyy");
                }
            }
        }

        protected void cmdSubmitServiceReport_Click(object sender, EventArgs e)
        {
            int reportType = Convert.ToInt32(drpReportType.SelectedValue);
            string reportTitle = "";
            int month = Convert.ToInt32(drpServiceMonth.SelectedValue);
            int year = DateTime.Now.Year;
            var ctl = new ReportController();
            if (reportType == 1)
            {
                reportTitle = "Service Award for";
            }
            else
            {
                reportTitle = "Employment for";
            }
            ltReportTitle.Text = string.Format("{0} {1}, {2}", reportTitle, drpServiceMonth.SelectedItem.Text, year.ToString());
            grdReport.DataSource = ctl.GetServiceDates(month, reportType, year);
            grdReport.DataBind();
            grdReport.HeaderRow.TableSection = TableRowSection.TableHeader;

        }

        protected void cmdTerminationReport_Click(object sender, EventArgs e)
        {
            DateTime.TryParse(txtStartDate.Text, out DateTime startDate);
            DateTime.TryParse(txtEndDate.Text, out DateTime endDate);
            string reportTitle = "Termination Report";
            var ctl = new ReportController();

            ltReportTitle.Text = reportTitle;
            grdReport.DataSource = ctl.GetTerminationDates(startDate, endDate);
            grdReport.DataBind();
            grdReport.HeaderRow.TableSection = TableRowSection.TableHeader;

        }
    }
}