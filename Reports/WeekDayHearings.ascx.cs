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
using System.Collections.Generic;

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
    public partial class WeekDayHearings : ReportsModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public WeekDayHearings()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    

                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
                if (e.Row.RowType == DataControlRowType.Header)
                {

                    for (int i = 0; i < e.Row.Controls.Count; i++)
                    {
                        var headerCell = e.Row.Controls[i] as DataControlFieldHeaderCell;
                        if (i == 0)
                        {
                            headerCell.Text = "Day of Week";
                        }
                        if (i == 1)
                        {
                            headerCell.Text = "Number of Hearings";
                        }
                    }
                }
        }
        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            if (DateTime.TryParse(txtStartDate.Text.Trim(), out DateTime startDate) && DateTime.TryParse(txtEndDate.Text.Trim(), out DateTime endDate) && drpCounty.SelectedIndex>0 && drpJudges.SelectedIndex>0)
            {

                var ctl = new ReportController();
                IEnumerable<WeekdayHearing> weekdayHearings = ctl.GetWeekdayHearingCounts(drpCounty.SelectedValue, startDate, endDate, drpJudges.SelectedValue);
                grdReport.DataSource = weekdayHearings;
                grdReport.DataBind();
            }
            else
            {
                ltMessage.Text = string.Format(ltMessage.Text, "danger", "exclamation", "Please complete all report criteria fields.");
                ltMessage.Visible = true;
            }
        }

        protected void drpCounty_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ctl = new ReportController();
            drpJudges.DataSource = ctl.GetJacsJudges(drpCounty.SelectedValue);
            drpJudges.DataBind();
            drpJudges.Items.Insert(0, new ListItem("< Select Judge>", ""));
            if (drpJudges.Items.Count > 1)
            {
                drpJudges.Enabled = true;
            }
        }
    }
}