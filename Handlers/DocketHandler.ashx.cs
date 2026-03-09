using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DotNetNuke.Services.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs.Handlers
{
    /// <summary>
    /// Summary description for DocketHandler
    /// </summary>
    public class DocketHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                // Read the JSON body from the POST request
                string jsonBody = new StreamReader(context.Request.InputStream).ReadToEnd();
                JObject p1 = JObject.Parse(jsonBody);

                long courtId = p1["court"].ToObject<long>();
                long courtroomId = p1["courtroom"]?.ToObject<long>() ?? 0;
                if (!DateTime.TryParse(p1["from"]?.ToString(), out DateTime fromDate))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.Write("Invalid from date.");
                    return;
                }
                DateTime? toDate = null;
                if (p1["to"] != null && DateTime.TryParse(p1["to"].ToString(), out DateTime parsedToDate))
                {
                    toDate = parsedToDate;
                }
                string hearing = p1["hearing"]?.ToString() ?? "all";
                bool courtroomPrint = p1["courtroom_print"]?.ToObject<bool>() ?? true;

                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(courtId);

                if (court == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.Write("Court not found.");
                    return;
                }
                else
                {
                    if (court.courtroom_print != courtroomPrint)
                    {
                        court.courtroom_print = courtroomPrint;
                        courtCtl.UpdateCourt(court);
                    }
                }

                var countyCtl = new CountyController();
                var county = countyCtl.GetCounty(court.county_id);
                string countyName = county?.name ?? "Unknown";

                var hearings = new Dictionary<DateTime, List<Dictionary<string, object>>>();
                var period = toDate.HasValue ? GetDateRange(fromDate, toDate.Value) : new List<DateTime> { fromDate };
                var holidayCtl = new HolidayController();
                var timeslotCtl = new TimeslotController();
                var eventCtl = new EventController();
                var courtTimeslotCtl = new CourtTimeslotController();
                var attorneyCtl = new AttorneyController();

                foreach (var date in period.Where(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday))
                {
                    var timeslots = courtTimeslotCtl.GetCourtTimeslotsByCourtId(courtId)
                        .Select(ct => ct.Timeslot ?? new Timeslot())
                        .Where(ts => ts.start.Date == date.Date)
                        .ToList();

                    if (courtroomId != 0)
                    {
                        timeslots = timeslots.Where(ts => ts.courtroom_id == courtroomId).ToList();
                    }

                    timeslots = timeslots.OrderBy(ts => ts.start).ToList();

                    var holiday = holidayCtl.GetHolidays().FirstOrDefault(h => h.date.Date == date.Date);

                    var dayHearings = new List<Dictionary<string, object>>();

                    foreach (var ts in timeslots)
                    {
                        var events = eventCtl.GetEventsByTimeslot(ts.id);
                        if (!events.Any())
                        {
                            if (ts.blocked)
                            {
                                var desc = holiday is Holiday hol ? hol.name : ts.description ?? "Blocked";
                                dayHearings.Add(new Dictionary<string, object>
                                {
                                    { "start_time", ts.start.ToString("h:mm tt") },
                                    { "end_time", ts.end.ToString("h:mm tt") },
                                    { "duration", "" },
                                    { "description", desc },
                                    { "blocked", true },
                                    { "public_block", ts.public_block },
                                    { "block_description", desc }
                                });
                            }
                            else
                            {
                                // Skip empty timeslots
                                continue;
                            }
                        }
                        else
                        {
                            foreach (var evt in events)
                            {
                                // ──────────────────────────────────────────────────────────────
                                // Improved name formatting: Last, First [Middle] [Suffix]
                                string attorneyFormatted = "Unknown";
                                if (evt.attorney_id.HasValue && !string.IsNullOrWhiteSpace(evt.attorney_name))
                                {
                                    attorneyFormatted = FormatNameLastFirst(evt.attorney_name);
                                }

                                string oppAttorneyFormatted = "Unknown";
                                if (evt.opp_attorney_id.HasValue && !string.IsNullOrWhiteSpace(evt.opp_attorney_name))
                                {
                                    oppAttorneyFormatted = FormatNameLastFirst(evt.opp_attorney_name);
                                }
                                // ──────────────────────────────────────────────────────────────

                                dayHearings.Add(new Dictionary<string, object>
                                {
                                    { "start_time", ts.start.ToString("h:mm tt") },
                                    { "end_time", ts.end.ToString("h:mm tt") },
                                    { "blocked", ts.blocked },
                                    { "public_block", ts.public_block },
                                    { "block_description", ts.description },
                                    { "duration", ts.duration + " min" },
                                    { "case_num", evt.case_num },
                                    { "motion", evt.motion_id == 221 ? evt.custom_motion : evt.Motion?.description ?? "Unknown" },
                                    { "attorney", attorneyFormatted },
                                    { "opp_attorney", oppAttorneyFormatted },
                                    { "attorney_phone", evt.attorney_id.HasValue ? attorneyCtl.GetAttorney(evt.attorney_id.Value)?.phone ?? "" : "" },
                                    { "opp_attorney_phone", evt.opp_attorney_id.HasValue ? attorneyCtl.GetAttorney(evt.opp_attorney_id.Value)?.phone ?? "" : "" },
                                    { "plaintiff", evt.plaintiff },
                                    { "defendant", evt.defendant },
                                    { "notes", evt.notes }
                                });
                            }
                        }
                    }

                    if (dayHearings.Any())
                    {
                        hearings[date] = dayHearings;
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // Circuit header
                    body.Append(CreateCircuitParagraph(countyName));
                    body.Append(new Paragraph(new Run(new Text(" "))) { ParagraphProperties = new ParagraphProperties(new SpacingBetweenLines { After = "240" }) });

                    // Create main table for the docket
                    Table outerTable = new Table();
                    TableProperties outerTableProps = new TableProperties(
                        new TableWidth { Width = "5000", Type = TableWidthUnitValues.Auto },
                        new TableBorders(
                            new TopBorder { Val = BorderValues.Single, Size = 4 },
                            new BottomBorder { Val = BorderValues.Single, Size = 4 },
                            new LeftBorder { Val = BorderValues.Single, Size = 4 },
                            new RightBorder { Val = BorderValues.Single, Size = 4 },
                            new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                            new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                        )
                    );
                    outerTable.Append(outerTableProps);

                    foreach (var kvp in hearings.OrderBy(h => h.Key))
                    {
                        DateTime hearingDate = kvp.Key;
                        var dayEvents = kvp.Value;

                        // Date row
                        TableRow dateRow = new TableRow();
                        TableCell dateCell = new TableCell();
                        dateCell.Append(new TableCellProperties(new TableCellWidth { Width = "5000", Type = TableWidthUnitValues.Dxa }));
                        dateCell.Append(CreateParagraph(hearingDate.ToString("dddd, MMMM dd, yyyy"), bold: true, alignment: JustificationValues.Center, fontSize: 14));
                        dateRow.Append(dateCell);
                        outerTable.Append(dateRow);

                        // Events table inside cell
                        Table innerTable = new Table();
                        // ... (your original inner table structure, borders, etc. would continue here)

                        // Example continuation – adjust to match your exact original layout
                        foreach (var evt in dayEvents)
                        {
                            TableRow innerRow = new TableRow();
                            // Add cells for time, case, motion, attorney, etc.
                            // Use evt["start_time"], evt["attorney"], evt["opp_attorney"], etc.
                            // This is where your original truncated code would be placed
                        }

                        TableCell outerCell = new TableCell();
                        outerCell.Append(innerTable);

                        TableRow outerRow = new TableRow();
                        outerRow.Append(outerCell);
                        outerTable.Append(outerRow);
                    }

                    body.Append(outerTable);

                    // Add footer
                    FooterPart footerPart = mainPart.AddNewPart<FooterPart>();
                    GenerateFooterPartContent(footerPart);

                    // Link footer to section
                    SectionProperties sectionProps = body.AppendChild(new SectionProperties());
                    FooterReference footerRef = new FooterReference { Type = HeaderFooterValues.Default, Id = mainPart.GetIdOfPart(footerPart) };
                    sectionProps.Append(footerRef);

                    mainPart.Document.Save();

                    // Send the document as response
                    context.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    context.Response.AddHeader("Content-Disposition", "attachment; filename=Docket_" + DateTime.Now.ToString("yyyyMMdd") + ".docx");
                    stream.Position = 0;
                    stream.CopyTo(context.Response.OutputStream);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Write("An error occurred while generating the docket.");
            }
        }

        private string FormatNameLastFirst(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "Unknown";

            string trimmed = fullName.Trim();
            string[] parts = trimmed.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length <= 1)
                return trimmed;

            // Common generational suffixes (case-insensitive)
            var suffixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "jr", "jr.", "jnr",
                "sr", "sr.", "snr",
                "ii", "iii", "iv", "v", "vi",
                "esq", "esq."
            };

            string lastName;
            string suffix = "";
            string firstAndMiddle;

            string potentialSuffix = parts[parts.Length - 1];
            bool hasSuffix = suffixes.Contains(potentialSuffix) ||
                             (potentialSuffix.EndsWith(".", StringComparison.OrdinalIgnoreCase) &&
                              suffixes.Contains(potentialSuffix.TrimEnd('.')));

            if (hasSuffix)
            {
                suffix = " " + potentialSuffix;
                lastName = parts[parts.Length - 2];
                firstAndMiddle = string.Join(" ", parts, 0, parts.Length - 2);
            }
            else
            {
                lastName = parts[parts.Length - 1];
                firstAndMiddle = string.Join(" ", parts, 0, parts.Length - 1);
            }

            // Optional: preserve common titles at the beginning
            string title = "";
            string[] commonTitles = { "Dr.", "Mr.", "Mrs.", "Ms.", "Hon.", "Judge", "The Honorable" };

            if (firstAndMiddle.Length > 0)
            {
                string[] firstParts = firstAndMiddle.Split(new char[] { ' ' }, 2);
                string firstWord = firstParts[0];

                if (commonTitles.Any(t => string.Equals(t, firstWord, StringComparison.OrdinalIgnoreCase)))
                {
                    title = firstWord + " ";
                    firstAndMiddle = firstParts.Length > 1 ? firstParts[1] : "";
                }
            }

            string result = lastName + "," + suffix + " " + title + firstAndMiddle;
            return result.Trim();
        }

        private Paragraph CreateParagraph(string text, bool bold = false, JustificationValues? alignment = null, double fontSize = 11)
        {
            if (!alignment.HasValue)
            {
                alignment = JustificationValues.Left;
            }

            Paragraph para = new Paragraph();
            ParagraphProperties paraProps = new ParagraphProperties(
                new Justification() { Val = alignment },
                new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" }
            );
            para.Append(paraProps);

            Run run = new Run(new Text(text));
            if (bold || fontSize != 11)
            {
                RunProperties runProps = new RunProperties();
                if (bold) runProps.Append(new Bold());
                if (fontSize != 11) runProps.Append(new FontSize() { Val = (fontSize * 2).ToString() });
                run.PrependChild(runProps);
            }
            para.Append(run);

            return para;
        }

        private Paragraph CreateLabeledParagraph(string label, string value, JustificationValues? alignment, double fontSize = 11)
        {
            if (!alignment.HasValue) alignment = JustificationValues.Left;

            Paragraph para = new Paragraph();
            ParagraphProperties paraProps = new ParagraphProperties(
                new Justification { Val = alignment },
                new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" }
            );
            para.Append(paraProps);

            Run labelRun = new Run(new Text(label + ": "));
            labelRun.PrependChild(new RunProperties(new Bold()));
            para.Append(labelRun);

            Run valueRun = new Run(new Text(" " + value));
            if (fontSize != 11)
            {
                RunProperties valueProps = new RunProperties(new FontSize { Val = (fontSize * 2).ToString() });
                valueRun.PrependChild(valueProps);
            }
            para.Append(valueRun);

            return para;
        }

        private Paragraph CreateCircuitParagraph(string countyName)
        {
            Paragraph para = new Paragraph();
            ParagraphProperties paraProps = new ParagraphProperties(
                new Justification() { Val = JustificationValues.Center },
                new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" }
            );
            para.Append(paraProps);

            // "12"
            Run run12 = new Run(new Text("12"));
            RunProperties rp12 = new RunProperties(
                new Bold(),
                new FontSize() { Val = "32" }
            );
            run12.PrependChild(rp12);
            para.Append(run12);

            // "th"
            Run runTh = new Run(new Text("th"));
            RunProperties rpTh = new RunProperties(
                new Bold(),
                new FontSize() { Val = "32" },
                new VerticalTextAlignment() { Val = VerticalPositionValues.Superscript }
            );
            runTh.PrependChild(rpTh);
            para.Append(runTh);

            // " Judicial Circuit - {CountyName} County"
            Run runRest = new Run(new Text($" Judicial Circuit - {countyName} County"));
            RunProperties rpRest = new RunProperties(
                new Bold(),
                new FontSize() { Val = "32" }
            );
            runRest.PrependChild(rpRest);
            para.Append(runRest);

            return para;
        }

        private string CleanString(string input)
        {
            return string.IsNullOrEmpty(input) ? input : Regex.Replace(input, @"[^A-Za-z0-9\-\.\@\/ ]", "");
        }

        private string CleanLabel(string label)
        {
            int underscoreIndex = label.IndexOf('_');
            string cleanedKey = underscoreIndex >= 0 ? label.Substring(0, underscoreIndex) : label;
            return cleanedKey;
        }

        private string FormatPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return phone;
            var match = Regex.Match(phone, @"(\d{3})[^\d]{0,7}(\d{3})[^\d]{0,7}(\d{4})");
            return match.Success ? $"({match.Groups[1]}) {match.Groups[2]}-{match.Groups[3]}" : phone;
        }

        private IEnumerable<DateTime> GetDateRange(DateTime start, DateTime end)
        {
            for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
                yield return date;
        }

        private void GenerateFooterPartContent(FooterPart footerPart)
        {
            Footer footer = new Footer();
            Paragraph paragraph = new Paragraph();

            // Set paragraph properties for center alignment
            ParagraphProperties paragraphProperties = new ParagraphProperties
            {
                Justification = new Justification() { Val = JustificationValues.Center }
            };
            paragraph.Append(paragraphProperties);

            // Add page number field
            Run run1 = new Run();
            SimpleField pageField = new SimpleField() { Instruction = "PAGE" };
            run1.Append(pageField);
            paragraph.Append(run1);

            // Add " of " text
            Run run2 = new Run();
            Text textOf = new Text(" of ") { Space = SpaceProcessingModeValues.Preserve };
            run2.Append(textOf);
            paragraph.Append(run2);

            // Add total pages field
            Run run3 = new Run();
            SimpleField numPagesField = new SimpleField() { Instruction = "NUMPAGES" };
            run3.Append(numPagesField);
            paragraph.Append(run3);

            footer.Append(paragraph);
            footerPart.Footer = footer;
        }

        public bool IsReusable { get { return false; } }
    }
}