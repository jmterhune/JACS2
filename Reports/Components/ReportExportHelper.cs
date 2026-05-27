/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace tjc.Modules.Reports.Components
{
    /// <summary>
    /// Tiny helper shared by every Employee Reports view that needs an
    /// &quot;Export to Excel&quot; button. We render the rows ourselves
    /// as a plain HTML table and tag the response with the Excel content
    /// type — Excel opens it as a .xls cleanly (with one mild "file is
    /// in a different format" warning the first time).
    ///
    /// We deliberately do NOT call GridView.RenderControl: Page's
    /// VerifyRenderingInServerForm guards against rendering a server
    /// control outside the &lt;form&gt;, and UserControls can't override
    /// that. Iterating the already-bound GridView rows / cells lets us
    /// produce the same output without involving WebForms' rendering
    /// pipeline.
    /// </summary>
    internal static class ReportExportHelper
    {
        /// <summary>
        /// Stream the supplied GridView's already-rendered cells to the
        /// response as an Excel-readable HTML table. Headers come from
        /// the live <c>grid.HeaderRow</c>; rows come from <c>grid.Rows</c>.
        /// </summary>
        /// <param name="grid">Already-bound GridView (call DataBind() first).</param>
        /// <param name="response">Page.Response.</param>
        /// <param name="filename">Base filename, ".xls" is appended.</param>
        public static void ExportGridToExcel(GridView grid, HttpResponse response, string filename)
        {
            response.Clear();
            response.Buffer = true;
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xls");
            response.Charset = string.Empty;
            response.ContentEncoding = Encoding.UTF8;
            // UTF-8 BOM so Excel renders accented characters (e.g. José) correctly.
            response.Write("\xEF\xBB\xBF");

            var sb = new StringBuilder(8192);
            // Excel's HTML importer respects <style> in the document head;
            // a tiny stylesheet keeps the imported table readable.
            sb.AppendLine("<html><head><meta charset=\"utf-8\"/>");
            sb.AppendLine("<style>table{border-collapse:collapse} td,th{border:1px solid #999;padding:4px 6px;font-family:Calibri,Arial,sans-serif;font-size:11pt} th{background:#f2f2f2;text-align:left}</style>");
            sb.AppendLine("</head><body>");
            sb.AppendLine("<table>");

            // Header row -- use the live grid header (after sorting / paging
            // has been applied) so the export reflects what the user sees.
            if (grid.HeaderRow != null)
            {
                sb.Append("<tr>");
                foreach (TableCell cell in grid.HeaderRow.Cells)
                {
                    sb.Append("<th>").Append(CellText(cell)).Append("</th>");
                }
                sb.AppendLine("</tr>");
            }

            // Data rows.
            foreach (GridViewRow row in grid.Rows)
            {
                sb.Append("<tr>");
                foreach (TableCell cell in row.Cells)
                {
                    sb.Append("<td>").Append(CellText(cell)).Append("</td>");
                }
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table></body></html>");
            response.Write(sb.ToString());
            response.Flush();
            response.End();
        }

        /// <summary>Extract the displayable text from a GridView cell.
        /// BoundField cells expose their value via Text; TemplateField
        /// cells have rendered Controls (LiteralControl) — we concat their
        /// markup. The result is plain HTML good enough for Excel.</summary>
        private static string CellText(TableCell cell)
        {
            if (!string.IsNullOrEmpty(cell.Text))
            {
                return cell.Text;
            }
            if (cell.Controls.Count == 0) return string.Empty;
            var sb = new StringBuilder();
            foreach (System.Web.UI.Control c in cell.Controls)
            {
                if (c is System.Web.UI.LiteralControl lit && lit.Text != null)
                    sb.Append(lit.Text);
                else if (c is System.Web.UI.WebControls.Literal asL && asL.Text != null)
                    sb.Append(asL.Text);
                else
                {
                    // Last-resort: try to read the text via property.
                    var prop = c.GetType().GetProperty("Text");
                    var v = prop?.GetValue(c, null);
                    if (v != null) sb.Append(v.ToString());
                }
            }
            return sb.ToString().Trim();
        }
    }
}
