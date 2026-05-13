/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Common;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tjc.Modules.CourtRegistry.Components;

namespace tjc.Modules.CourtRegistry
{
    public partial class CodeCounts : CourtRegistryModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                int year = 0;
                var qs = Request.QueryString["yr"];
                if (!string.IsNullOrEmpty(qs))
                    int.TryParse(qs, out year);

                lnkCancel.NavigateUrl = Globals.NavigateURL();
                ltHeader.Text = string.Format("JAC Code Application Counts for {0}-{1}", year - 1, year);

                var appCtl = new ApplicationController();
                var locCtl = new LocationController();
                var counts = appCtl.GetJacCodeCounts(year).ToList();
                var locations = locCtl.GetLocations().OrderBy(l => l.LocationName).Select(l => l.LocationName).ToList();
                if (locations.Count == 0)
                    locations = counts.Select(c => c.LocationName).Distinct().OrderBy(n => n).ToList();

                var sb = new StringBuilder();
                sb.Append("<table id='jcounts' class='table-bordered'>");
                sb.Append("<thead><tr><th>Category</th><th class='secStart'>Code</th>");
                foreach (var l in locations)
                    sb.AppendFormat("<th colspan='3' class='secStart'>{0} (N,A,R)</th>", l);
                sb.Append("</tr></thead><tbody>");

                var jacRows = counts
                    .GroupBy(c => new { c.CaseTypeName, c.JacCodeID, c.Category })
                    .OrderBy(g => g.Key.CaseTypeName).ThenBy(g => g.Key.Category).ThenBy(g => g.Key.JacCodeID)
                    .ToList();

                string lastCaseType = null;
                int columnCount = 2 + (locations.Count * 3);
                foreach (var row in jacRows)
                {
                    if (row.Key.CaseTypeName != lastCaseType)
                    {
                        sb.AppendFormat("<tr class='header'><td class='casetype' colspan='{0}'>{1}</td></tr>", columnCount, row.Key.CaseTypeName);
                        lastCaseType = row.Key.CaseTypeName;
                    }
                    sb.Append("<tr class='tableRow'>");
                    sb.AppendFormat("<td>{0}</td>", row.Key.Category);
                    sb.AppendFormat("<td class='secStart'>{0}</td>", row.Key.JacCodeID);
                    foreach (var l in locations)
                    {
                        sb.AppendFormat("<td class='secStart'>{0}</td>", GetCount(row, l, CodeStatus.New));
                        sb.AppendFormat("<td>{0}</td>", GetCount(row, l, CodeStatus.Approved) + GetCount(row, l, CodeStatus.Locked));
                        sb.AppendFormat("<td>{0}</td>", GetCount(row, l, CodeStatus.Rejected));
                    }
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table>");
                ltBody.Text = sb.ToString();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private int GetCount(IEnumerable<JacCodeCount> row, string location, CodeStatus status)
        {
            var match = row.FirstOrDefault(c => c.LocationName == location && c.Status == (int)status);
            return match != null ? match.Cnt : 0;
        }
    }
}
