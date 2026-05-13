<%@ WebHandler Language="C#" Class="tjc.Modules.CourtRegistry.ExportExcel" %>

using System;
using System.Linq;
using System.Text;
using System.Web;
using tjc.Modules.CourtRegistry.Components;

namespace tjc.Modules.CourtRegistry
{
    public class ExportExcel : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int year = 0;
            var qs = context.Request.QueryString["yr"];
            if (!string.IsNullOrEmpty(qs))
                int.TryParse(qs, out year);

            var ctl = new ApplicationController();
            var rows = ctl.GetJacExport(year).ToList();

            var sb = new StringBuilder();
            sb.Append("<html><head><meta charset='utf-8'></head><body>");
            sb.Append("<table border='1'><thead><tr>");
            string[] headers = { "Bar ID", "First Name", "Last Name", "Case Type Code", "Case Type Description",
                                 "Effective Date", "Termination Date", "Circuit", "County Code", "Email Address" };
            foreach (var h in headers)
                sb.AppendFormat("<th>{0}</th>", h);
            sb.Append("</tr></thead><tbody>");

            string effectiveDate = string.Format("7/1/{0}", year - 1);
            string terminationDate = string.Format("6/30/{0}", year);
            var processedAppIds = new System.Collections.Generic.HashSet<int>();

            foreach (var r in rows)
            {
                if (r.BarNumber <= 0) continue;
                if (!string.IsNullOrEmpty(r.GuardianSignature) && (r.JacCodeID == 860 || r.JacCodeID == 865))
                    continue;
                sb.Append("<tr>");
                sb.AppendFormat("<td>{0}</td>", r.BarNumber);
                sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(r.FirstName));
                sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(r.LastName));
                sb.AppendFormat("<td>{0}</td>", r.JacCodeID);
                sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(r.Category));
                sb.AppendFormat("<td>{0}</td>", effectiveDate);
                sb.AppendFormat("<td>{0}</td>", terminationDate);
                sb.Append("<td>12</td>");
                sb.AppendFormat("<td>{0}</td>", r.CountyNumber);
                sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(r.Email));
                sb.Append("</tr>");
                processedAppIds.Add(r.ApplicationID);
            }
            sb.Append("</tbody></table></body></html>");

            var now = DateTime.Now;
            foreach (var appId in processedAppIds)
                ctl.MarkApplicationExported(appId, now);

            context.Response.Clear();
            context.Response.ContentType = "application/vnd.ms-excel";
            context.Response.AddHeader("Content-Disposition", "attachment;filename=JAC Report.xls");
            context.Response.Write(sb.ToString());
            context.Response.End();
        }

        public bool IsReusable { get { return false; } }
    }
}
