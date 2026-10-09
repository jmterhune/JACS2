using System.IO;
using System.Net;
using System.Text;
using CourtCounsel.Desktop.Models;

namespace CourtCounsel.Desktop.Services;

// Mirrors DataSheet.ascx.cs's cmdExport_Click: an Excel-flavored HTML table
// (Office namespace hints, mso-number-format so Case Number stays text and
// dates format as m/d/yyyy) saved with a .xls extension — the same trick
// the web module uses, so files opened here behave identically in Excel.
public static class ExcelExportService
{
    public static void ExportDataSheet(IEnumerable<HistoryInfo> rows, string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<html xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\" xmlns=\"http://www.w3.org/TR/REC-html40\">");
        sb.AppendLine("<head><meta charset=\"UTF-8\">");
        sb.AppendLine("<!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>");
        sb.AppendLine("<x:Name>Data Sheet</x:Name><x:WorksheetOptions></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]-->");
        sb.AppendLine("<style>.text{mso-number-format:'\\@';} .date{mso-number-format:'m/d/yyyy';}</style>");
        sb.AppendLine("</head><body><table border='1'>");
        sb.AppendLine("<tr>" +
            "<th>Case Name</th><th>Case Type</th><th>Case Number</th><th>Date Received</th>" +
            "<th>Motion Filed</th><th>Requested By</th><th>Responsible</th><th>Action</th>" +
            "<th>Completed</th><th>Status</th></tr>");

        foreach (var h in rows)
        {
            sb.AppendLine("<tr>" +
                $"<td>{Enc(h.PartyName)}</td>" +
                $"<td>{Enc(h.CaseType)}</td>" +
                $"<td class='text'>{Enc(h.CaseNumber)}</td>" +
                $"<td class='date'>{FormatDate(h.DateReceived)}</td>" +
                $"<td class='date'>{FormatDate(h.MotionFiled)}</td>" +
                $"<td>{Enc(h.RequestedBy)}</td>" +
                $"<td>{Enc(h.Responsible)}</td>" +
                $"<td>{Enc(h.Action)}</td>" +
                $"<td class='date'>{FormatDate(h.DateCompleted)}</td>" +
                $"<td>{Enc(h.StatusName)}</td>" +
                "</tr>");
        }

        sb.AppendLine("</table></body></html>");

        File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }

    private static string Enc(string? value) => WebUtility.HtmlEncode(value ?? "");

    private static string FormatDate(DateTime? value) => value.HasValue ? value.Value.ToString("M/d/yyyy") : "";
}
