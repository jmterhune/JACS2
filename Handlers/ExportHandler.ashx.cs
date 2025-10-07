using System;
using System.Web;
using DotNetNuke.Security;
using tjc.Modules.jacs.Components; // For controllers, models, etc.
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Net;

namespace tjc.Modules.jacs.Handlers
{
    /// <summary>
    /// Summary description for ExportHandler
    /// </summary>
    public class ExportHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                // Parse parameters
                long courtId;
                DateTime startDate, endDate;
                if (!long.TryParse(context.Request.QueryString["courtId"], out courtId) ||
                    !DateTime.TryParse(context.Request.QueryString["fromDate"], out startDate) ||
                    !DateTime.TryParse(context.Request.QueryString["toDate"], out endDate))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.Write("Invalid parameters");
                    return;
                }

                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(courtId);
                if (court == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.Write("Court not found");
                    return;
                }

                DateTime startTime = startDate.Date.AddDays(1 - startDate.Day); // Start of month
                DateTime endTime = startTime.AddMonths(1).AddTicks(-1); // End of month

                var hearingCounts = new List<MonthlySummaryItem>();

                for (DateTime date = startTime; date <= endTime; date = date.AddDays(1))
                {
                    var tsCtl = new TimeslotController();
                    var timeslots = tsCtl.GetTimeslotsByCourtAndDate(courtId, date).ToList();

                    int count = timeslots.Count(t => t.available && !t.blocked);
                    int courtRes = timeslots.Count;

                    string title = null;
                    if (court.auto_extension)
                    {
                        var ctoCtl = new CourtTemplateOrderController();
                        var templates = ctoCtl.GetCourtTemplateOrdersByCourtId(courtId, true).ToList();
                        if (templates.Any())
                        {
                            int weekOfMonth = GetWeekOfMonth(date);
                            try
                            {
                                title = templates[weekOfMonth - 1].template.name;
                            }
                            catch 
                            {
                                if (weekOfMonth > templates.Count)
                                {
                                    title = templates[(weekOfMonth - 1) % templates.Count].template.name;
                                }
                                else
                                {
                                    title = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        var ctoCtl = new CourtTemplateOrderController();
                        var templates = ctoCtl.GetCourtTemplateOrdersByCourtId(courtId, false).ToList();
                        if (templates.Any())
                        {
                            title = templates[0].template.name;
                        }
                    }

                    if (IsWeekday(date) && courtRes != 0)
                    {
                        hearingCounts.Add(new MonthlySummaryItem
                        {
                            start = date,
                            end = date.AddDays(1).AddTicks(-1),
                            allDay = true,
                            title = count + " Free Timeslots",
                            tCount = count,
                            timeslotDescription = title
                        });
                    }
                }

                var hearings = hearingCounts;

                var weekGp = hearings.GroupBy(item =>
                {
                    DateTime d = item.start;
                    int weekNo = (int)((d - startTime).TotalDays / 7) + 1;
                    DateTime weekStart = StartOfWeek(d);
                    DateTime weekEnd = weekStart.AddDays(6);
                    string label = $"Week {weekNo} ({weekStart.ToString("MM/dd/yyyy")} - {weekEnd.ToString("MM/dd/yyyy")})";
                    return new { WeekNo = weekNo, Label = label };
                }).OrderBy(g => g.Key.WeekNo);

                string pdfTitle = "Monthly Calendar for " + startTime.ToString("MMMM yyyy");
                string courtName = court.description ?? "";

                // Generate PDF
                using (MemoryStream ms = new MemoryStream())
                {
                    Document doc = new Document(PageSize.A4, 36, 36, 36, 36); // Margins: left, right, top, bottom
                    PdfWriter.GetInstance(doc, ms);
                    doc.Open();

                    Font fontTitle = FontFactory.GetFont("Verdana", 14, Font.BOLD);
                    Font fontWeek = FontFactory.GetFont("Verdana", 12, Font.BOLD);
                    Font fontDay = FontFactory.GetFont("Verdana", 10, Font.NORMAL);

                    Paragraph paraTitle = new Paragraph(pdfTitle, fontTitle);
                    paraTitle.Alignment = Element.ALIGN_LEFT;
                    doc.Add(paraTitle);

                    Paragraph paraCourt = new Paragraph("Court: " + courtName, fontTitle);
                    paraCourt.Alignment = Element.ALIGN_LEFT;
                    doc.Add(paraCourt);

                    doc.Add(new Paragraph(" ")); // Spacer

                    foreach (var group in weekGp)
                    {
                        Paragraph paraWeek = new Paragraph(group.Key.Label, fontWeek);
                        paraWeek.Alignment = Element.ALIGN_LEFT;
                        doc.Add(paraWeek);

                        foreach (var item in group.OrderBy(i => i.start))
                        {
                            string line = $"{item.start.ToString("MM/dd/yyyy")}: {item.title} ({item.timeslotDescription ?? ""})";
                            Paragraph paraDay = new Paragraph(line, fontDay);
                            paraDay.IndentationLeft = 20; // Indent for days
                            paraDay.Alignment = Element.ALIGN_LEFT;
                            doc.Add(paraDay);
                        }

                        doc.Add(new Paragraph(" ")); // Spacer between weeks
                    }

                    doc.Close();

                    byte[] pdfBytes = ms.ToArray();

                    context.Response.ContentType = "application/pdf";
                    context.Response.AddHeader("Content-Disposition", "attachment; filename=pdfcal.pdf");
                    context.Response.BinaryWrite(pdfBytes);
                    context.Response.Flush();
                }
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Write(ex.Message);
            }
        }

        private int GetWeekOfMonth(DateTime date)
        {
            return ((date.Day - 1) / 7) + 1;
        }

        private bool IsWeekday(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }

        private DateTime StartOfWeek(DateTime dt)
        {
            int diff = ((int)dt.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            return dt.AddDays(-diff).Date;
        }

        public bool IsReusable { get { return false; } }
    }   
}