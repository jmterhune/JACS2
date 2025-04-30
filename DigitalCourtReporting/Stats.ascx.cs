/*
' Copyright (c) 2025  Joe Terhune
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
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.DigitalCourtReporting.Components;
using tjc.Modules.Globals;
using static DotNetNuke.Common.Lists.CachedCountryList;

namespace tjc.Modules.DigitalCourtReporting
{
    /// -----------------------------------------------------------------------------
    /// <summary>   
    /// The Edit class is used to manage content
    /// 
    /// Typically your edit control would be used to create new content, or edit existing content within your module.
    /// The ControlKey for this control is "Edit", and is defined in the manifest (.dnn) file.
    /// 
    /// Because the control inherits from DigitalCourtReportingModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Stats : DigitalCourtReportingModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public Stats()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        #region Methods
        private void CreateStatTable(DateTime startDate,DateTime endDate,int countyId)
        {
            List<StatsInfo> stats = new List<StatsInfo>();

            var ctl = new StatsController();
            IEnumerable<StatRecord> excludedRecords=ctl.ExcludedSum(startDate, endDate,countyId);
            if (excludedRecords.Count() > 0)
            {
                stats.Add(new StatsInfo { Heading = "Private parties or other gov't entity", MinBurned = excludedRecords.Sum(x => x.CDCount), TotalNumber = excludedRecords.Sum(g => g.TotalMinutes) });
            }
            else
            {
                stats.Add(new StatsInfo { Heading = "Private parties or other gov't entity", MinBurned = 0, TotalNumber = 0 });
            }
            IEnumerable<StatRecord> saRecords = ctl.StateAttorneySum(startDate, endDate, countyId);
            if (saRecords.Count() > 0)
            {
                stats.Add(new StatsInfo { Heading = "State Attorney", MinBurned = saRecords.Sum(x => x.CDCount), TotalNumber = saRecords.Sum(g => g.TotalMinutes) });
            }
            else
            {
                stats.Add(new StatsInfo { Heading = "State Attorney", MinBurned = 0, TotalNumber = 0 });
            }

            IEnumerable<StatRecord> pdRecords = ctl.PublicDefenderSum(startDate, endDate, countyId);
            if (pdRecords.Count() > 0)
            {
                stats.Add(new StatsInfo { Heading = "Public Defender", MinBurned = pdRecords.Sum(x => x.CDCount), TotalNumber = pdRecords.Sum(g => g.TotalMinutes) });
            }
            else
            {
                stats.Add(new StatsInfo { Heading = "Public Defender", MinBurned = 0, TotalNumber = 0 });
            }

            IEnumerable<StatRecord> caRecords = ctl.CourtAttorneySum(startDate, endDate, countyId);
            if (caRecords.Count() > 0)
            {
                stats.Add(new StatsInfo { Heading = "Court Appointed Counsel", MinBurned = caRecords.Sum(x => x.CDCount), TotalNumber = caRecords.Sum(g => g.TotalMinutes) });
            }
            else
            {
                stats.Add(new StatsInfo { Heading = "Court Appointed Counsel", MinBurned = 0, TotalNumber = 0 });
            }

            IEnumerable<StatRecord> totalRecords = ctl.TotalSum(startDate, endDate, countyId);
            if(totalRecords.Count() > 0)
            {
                lblTotal.Text=totalRecords.Sum(x => x.CDCount).ToString();
                lblMinTotal.Text = totalRecords.Sum(x => x.TotalMinutes).ToString();
            }
            else
            {
                lblTotal.Text = string.Empty;
                lblMinTotal.Text = string.Empty;
            }
            rptStats.DataSource = stats;
            rptStats.DataBind();
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Implement your edit logic for your module
                if (!Page.IsPostBack)
                {
                    try
                    {
                        var ctl = new CountyController();
                        drpCriteriaCounty.DataTextField = "CountyName";
                        drpCriteriaCounty.DataValueField = "CountyID";
                        drpCriteriaCounty.DataSource = ctl.GetCounties();
                        drpCriteriaCounty.DataBind();
                      
                    }
                    catch (Exception exc)
                    {
                        Exceptions.ProcessModuleLoadException(this, exc);
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            bool hasSearch = true;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            int countyId = -1;
            hasSearch=DateTime.TryParse(txtCriteriaStartDate.Text,out startDate);
            if (hasSearch) 
            hasSearch = DateTime.TryParse(txtCriteriaEndDate.Text, out endDate);
            if (hasSearch)
            hasSearch = Int32.TryParse(drpCriteriaCounty.SelectedValue,out countyId);
            if (hasSearch)
            {
                CreateStatTable(startDate, endDate, countyId);
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "You Request could not be completed. Check your Criteria and try again.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
            }
        }
    }
}