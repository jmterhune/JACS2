/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Skins.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using tjc.Modules.CourtRegistry.Components;

namespace tjc.Modules.CourtRegistry
{
    public partial class CodeComparison : CourtRegistryModuleBase
    {
        private readonly IHostSettings _hostSettings;

        public CodeComparison()
        {
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    lnkCancel.NavigateUrl = _navigationManager.NavigateURL();
                    var aCtl = new AttorneyController(_hostSettings);
                    var attorneys = aCtl.GetAttornies()
                        .OrderBy(a => a.LastName).ThenBy(a => a.FirstName)
                        .Select(a => new { Display = a.LastName + ", " + a.FirstName, a.AttorneyID })
                        .ToList();
                    drpAttorney.DataTextField = "Display";
                    drpAttorney.DataValueField = "AttorneyID";
                    drpAttorney.DataSource = attorneys;
                    drpAttorney.DataBind();
                    drpAttorney.Items.Insert(0, new ListItem("Select Attorney", ""));

                    var appCtl = new ApplicationController(_hostSettings);
                    int maxYear = appCtl.GetMaxApplicationYear();
                    if (maxYear > 0)
                    {
                        for (int i = maxYear - 3; i <= maxYear; i++)
                        {
                            drpYear.Items.Add(new ListItem(i.ToString()));
                            drpYear2.Items.Add(new ListItem(i.ToString()));
                        }
                    }
                    drpYear.Items.Insert(0, new ListItem("Select Year", "-1"));

                    // When launched from an application row (?aid=N) preselect
                    // that application's attorney and default the comparison to
                    // its fiscal year vs. the prior year, then run it.
                    PreselectFromApplication();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PreselectFromApplication()
        {
            var qs = Request.QueryString["aid"];
            if (string.IsNullOrEmpty(qs) || !int.TryParse(qs, out int aid) || aid <= 0)
                return;

            var appCtl = new ApplicationController(_hostSettings);
            var application = appCtl.GetApplication(aid);
            if (application == null)
                return;

            int year = application.Year;
            SelectYear(drpYear, year);

            drpYear2.Enabled = true;
            foreach (ListItem item in drpYear2.Items)
                item.Enabled = item.Value != year.ToString();
            SelectYear(drpYear2, year - 1);

            var attyValue = application.AttorneyID.ToString();
            if (drpAttorney.Items.FindByValue(attyValue) != null)
                drpAttorney.SelectedValue = attyValue;

            RunComparison();
        }

        /// <summary>Select <paramref name="year"/> in the dropdown, adding it
        /// as an item first if the standard maxYear-3..maxYear range didn't
        /// include it (e.g. an older application).</summary>
        private void SelectYear(DropDownList dd, int year)
        {
            string val = year.ToString();
            if (dd.Items.FindByValue(val) == null)
                dd.Items.Add(new ListItem(val, val));
            dd.SelectedValue = val;
        }

        protected void drpYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpYear2.Enabled = true;
            string selected = drpYear.SelectedValue;
            foreach (ListItem item in drpYear2.Items)
            {
                item.Enabled = true;
                if (item.Value == selected)
                    item.Enabled = false;
            }
        }

        protected void cmdCompare_Click(object sender, EventArgs e)
        {
            RunComparison();
        }

        private void RunComparison()
        {
            ltCompareTable.Text = string.Empty;
            ltCompareTableHeader.Text = string.Empty;

            if (!int.TryParse(drpYear.SelectedValue, out int year1) || year1 <= 0)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                    "new Noty({ text: '" + System.Web.HttpUtility.JavaScriptStringEncode("Please select a Year.") + "', type: 'warning', timeout: 4500, layout: 'topRight', theme: 'mint' }).show();", true);
                return;
            }
            if (!int.TryParse(drpYear2.SelectedValue, out int year2) || year2 <= 0)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                    "new Noty({ text: '" + System.Web.HttpUtility.JavaScriptStringEncode("Please select a Year to Compare.") + "', type: 'warning', timeout: 4500, layout: 'topRight', theme: 'mint' }).show();", true);
                return;
            }
            int.TryParse(drpAttorney.SelectedValue, out int attorneyId);

            var appCtl = new ApplicationController(_hostSettings);
            var year1Codes = appCtl.GetJacCodesByYear(year1, attorneyId).ToList();
            var year2Codes = appCtl.GetJacCodesByYear(year2, attorneyId).ToList();

            if (year1Codes.Count == 0)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                    "new Noty({ text: '" + System.Web.HttpUtility.JavaScriptStringEncode("Sorry no data available for Year " + year1) + "', type: 'warning', timeout: 4500, layout: 'topRight', theme: 'mint' }).show();", true);
                return;
            }
            if (year2Codes.Count == 0)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                    "new Noty({ text: '" + System.Web.HttpUtility.JavaScriptStringEncode("Sorry no data available for Year " + year2) + "', type: 'warning', timeout: 4500, layout: 'topRight', theme: 'mint' }).show();", true);
                return;
            }

            var locations = year1Codes.Select(c => c.LocationName)
                .Union(year2Codes.Select(c => c.LocationName))
                .Distinct().OrderBy(n => n).ToList();

            var byLocation = locations.ToDictionary(l => l, l => BuildLocationList(year1Codes, year2Codes, l));
            int maxCount = byLocation.Values.Max(list => list.Count);

            var sb = new StringBuilder();
            for (int i = 0; i < maxCount; i++)
            {
                sb.Append("<tr>");
                foreach (var l in locations)
                {
                    var list = byLocation[l];
                    if (i < list.Count)
                    {
                        sb.AppendFormat("<td>{0}</td><td>{1}</td>",
                            list[i].Year1 == 0 ? "&nbsp;" : list[i].Year1.ToString(),
                            list[i].Year2 == 0 ? "&nbsp;" : list[i].Year2.ToString());
                    }
                    else
                    {
                        sb.Append("<td></td><td></td>");
                    }
                }
                sb.Append("</tr>");
            }
            ltCompareTable.Text = sb.ToString();

            var hb = new StringBuilder();
            hb.Append("<tr>");
            foreach (var l in locations)
                hb.AppendFormat("<th colspan='2'>{0}</th>", l);
            hb.Append("</tr><tr>");
            foreach (var l in locations)
                hb.AppendFormat("<th>{0}</th><th>{1}</th>", year1, year2);
            hb.Append("</tr>");
            ltCompareTableHeader.Text = hb.ToString();
        }

        private List<JacCodePair> BuildLocationList(List<JacCodeYearLocation> year1, List<JacCodeYearLocation> year2, string location)
        {
            var y1 = year1.Where(c => c.LocationName == location).Select(c => c.JacCodeID).Distinct().OrderBy(c => c).ToList();
            var y2 = year2.Where(c => c.LocationName == location).Select(c => c.JacCodeID).Distinct().OrderBy(c => c).ToList();
            var combo = y1.Union(y2).Distinct().OrderBy(c => c).ToList();
            var result = new List<JacCodePair>();
            foreach (var code in combo)
            {
                if (y1.Contains(code) && y2.Contains(code))
                    result.Add(new JacCodePair(code, code));
                else if (y1.Contains(code))
                    result.Add(new JacCodePair(code, 0));
                else if (y2.Contains(code))
                    result.Add(new JacCodePair(0, code));
            }
            return result;
        }

        private class JacCodePair
        {
            public int Year1 { get; set; }
            public int Year2 { get; set; }
            public JacCodePair(int y1, int y2) { Year1 = y1; Year2 = y2; }
        }
    }
}
