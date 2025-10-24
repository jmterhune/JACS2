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
                long categoryId = p1["category"]?.ToObject<long>() ?? 0;
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
                bool categoryPrint = p1["category_print"]?.ToObject<bool>() ?? true;

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
                    if (court.category_print != categoryPrint)
                    {
                        court.category_print = categoryPrint;
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

                    if (categoryId != 0)
                    {
                        timeslots = timeslots.Where(ts => ts.category_id == categoryId).ToList();
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
                                    { "attorney", evt.attorney_id.HasValue ? evt.attorney_name : "Unknown" },
                                    { "opp_attorney", evt.opp_attorney_id.HasValue ? evt.opp_attorney_name : "Unknown" },
                                    { "attorney_phone", evt.attorney_id.HasValue ? attorneyCtl.GetAttorney(evt.attorney_id.Value)?.phone ?? "" : "" },
                                    { "opp_attorney_phone", evt.opp_attorney_id.HasValue ? attorneyCtl.GetAttorney(evt.opp_attorney_id.Value)?.phone ?? "" : "" },
                                    { "plaintiff", evt.plaintiff },
                                    { "defendant", evt.defendant },
                                    { "notes", evt.notes },
                                    { "category", ts.Category?.description ?? "Unknown" },
                                    { "template", evt.template ?? "" }
                                });
                            }
                        }
                    }
                    if (dayHearings.Count > 0)
                        hearings[date] = dayHearings;
                }

                // Generate Word document
                using (MemoryStream ms = new MemoryStream())
                {
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = mainPart.Document.AppendChild(new Body());

                        // Add footer
                        FooterPart footerPart = mainPart.AddNewPart<FooterPart>();
                        string footerPartId = mainPart.GetIdOfPart(footerPart);
                        SectionProperties sectionPropsGlobal = new SectionProperties(
                            new PageMargin { Top = 720, Bottom = 720, Left = 720, Right = 720, Header = 360, Footer = 360 }
                        );
                        sectionPropsGlobal.Append(new FooterReference { Id = footerPartId, Type = HeaderFooterValues.Default });
                        body.Append(sectionPropsGlobal);
                        GenerateFooterPartContent(footerPart);

                        // Sort hearings by date
                        var sortedHearings = hearings.OrderBy(h => h.Key).ToDictionary(h => h.Key, h => h.Value);

                        int sectionIndex = 1;
                        int totalSections = sortedHearings.Count;
                        var judgeCtl = new JudgeController();
                        var judge = judgeCtl.GetJudgeByCourt(courtId);
                        string judgeName = judge?.name?.ToUpper() ?? "";


                        foreach (var kvp in sortedHearings)
                        {
                            DateTime date = kvp.Key;
                            var dayHearings = kvp.Value;
                            if (sectionIndex > 1)
                            {
                                //add a page break before new date section
                                Paragraph pageBreakParagraph = new Paragraph(new Run(new Break { Type = BreakValues.Page }));
                                body.Append(pageBreakParagraph);
                            }
                            // Circuit paragraph
                            body.Append(CreateParagraph("Judicial Automated Calendaring System 2.0", true, JustificationValues.Center, 18));
                            body.Append(CreateCircuitParagraph(countyName));
                            body.Append(CreateParagraph("Docket", true, JustificationValues.Center, 14));
                            body.Append(CreateParagraph($"JUDGE {judgeName}", true, JustificationValues.Center, 14));
                            body.Append(CreateParagraph(date.ToString("dddd, MMMM d, yyyy"), true, JustificationValues.Center, 14));

                            var categories = dayHearings.Select(h => h.ContainsKey("category") ? h["category"].ToString() : "Unknown").Distinct().ToList();

                            if (categoryPrint && categories.Count > 1)
                            {
                                foreach (var cat in categories.OrderBy(c => c))
                                {
                                    body.Append(CreateParagraph(cat, true, null, 14));
                                    var catHearings = dayHearings.Where(h => h.ContainsKey("category") && h["category"].ToString() == cat).ToList();
                                    AddHearingsTable(body, catHearings,courtId);
                                }
                            }
                            else
                            {
                                AddHearingsTable(body, dayHearings,courtId);
                            }

                            SectionProperties sectionProps = new SectionProperties();
                            if (sectionIndex < totalSections)
                            {
                                sectionProps.Append(new SectionType() { Val = SectionMarkValues.NextPage });
                            }
                            body.Append(sectionProps);
                            sectionIndex++;
                        }

                        mainPart.Document.Save();
                    }

                    byte[] docBytes = ms.ToArray();

                    context.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    context.Response.AddHeader("Content-Disposition", $"attachment; filename={court.description.Replace("/", "-")}-{DateTime.Now:yyyy-MM-dd}.docx");
                    context.Response.BinaryWrite(docBytes);
                    context.Response.Flush();
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Write(ex.Message);
            }
        }

        private void AddHearingsTable(Body body, List<Dictionary<string, object>> hearings,long courtId)
        {
           var userDefinedFieldsCtl = new UserDefinedFieldController();
            Table outerTable = new Table();
            TableProperties outerProps = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 12 },
                    new BottomBorder { Val = BorderValues.Single, Size = 12 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 12 },
                    new LeftBorder { Val = BorderValues.None },
                    new RightBorder { Val = BorderValues.None },
                    new InsideVerticalBorder { Val = BorderValues.None }
                ),
                new TableWidth { Type = TableWidthUnitValues.Pct, Width = "5000" } // 100%
            );
            outerTable.Append(outerProps);

            foreach (var h in hearings)
            {
                TableRow outerRow = new TableRow();
                TableCell outerCell = new TableCell();
                outerCell.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "5000" }));

                if ((bool)h["blocked"])
                {
                    string startTime = h["start_time"].ToString();
                    string endTime = h["end_time"].ToString();
                    outerCell.Append(CreateParagraph(startTime + " - " + endTime, true));
                    outerCell.Append(CreateParagraph(h["block_description"].ToString()));
                }
                else
                {
                    Table innerTable = new Table();
                    TableProperties innerProps = new TableProperties(
                        new TableWidth { Type = TableWidthUnitValues.Pct, Width = "5000" },
                        new TableLayout { Type = TableLayoutValues.Fixed },
                        new TableBorders(
                            new TopBorder { Val = BorderValues.None },
                            new BottomBorder { Val = BorderValues.None },
                            new LeftBorder { Val = BorderValues.None },
                            new RightBorder { Val = BorderValues.None },
                            new InsideHorizontalBorder { Val = BorderValues.None },
                            new InsideVerticalBorder { Val = BorderValues.None }
                        )
                    );
                    innerTable.Append(innerProps);

                    TableGrid innerGrid = new TableGrid(
                        new GridColumn { Width = "2000" },
                        new GridColumn { Width = "2000" },
                        new GridColumn { Width = "2000" }
                    );
                    innerTable.Append(innerGrid);

                    // Row 1
                    string timeStr = h["start_time"].ToString() + " - " + h["end_time"].ToString() + " (" + h["duration"].ToString() + ")";
                    TableRow row1 = new TableRow();
                    row1.Append(new TableCell(CreateParagraph(timeStr)));
                    row1.Append(new TableCell(CreateLabeledParagraph("Case", CleanString(h["case_num"].ToString()), JustificationValues.Left)));
                    row1.Append(new TableCell(CreateLabeledParagraph("Motion", CleanString(h["motion"].ToString()), JustificationValues.Left)));
                    innerTable.Append(row1);

                    // Row 2
                    TableRow row2 = new TableRow();
                    row2.Append(new TableCell(CreateParagraph(CleanString(h["plaintiff"].ToString()))));
                    row2.Append(new TableCell(CreateParagraph("vs.", false, JustificationValues.Center)));
                    row2.Append(new TableCell(CreateParagraph(CleanString(h["defendant"].ToString()))));
                    innerTable.Append(row2);

                    // Row 3
                    string attorney = h["attorney"].ToString();
                    string attorneyLastFirst = "Unknown";
                    if (attorney != "Unknown")
                    {
                        var parts = attorney.Split(' ');
                        if (parts.Length >= 2)
                        {
                            attorneyLastFirst = parts.Last() + ", " + parts.First();
                        }
                        else
                        {
                            attorneyLastFirst = attorney;
                        }
                    }
                    string oppAttorney = h["opp_attorney"].ToString();
                    string oppLastFirst = "Unknown";
                    if (oppAttorney != "Unknown")
                    {
                        var parts = oppAttorney.Split(' ');
                        if (parts.Length >= 2)
                        {
                            oppLastFirst = parts.Last() + ", " + parts.First();
                        }
                        else
                        {
                            oppLastFirst = oppAttorney;
                        }
                    }
                    TableRow row3 = new TableRow();
                    row3.Append(new TableCell(CreateParagraph(attorneyLastFirst)));
                    row3.Append(new TableCell(CreateParagraph("")));
                    row3.Append(new TableCell(CreateParagraph(oppLastFirst)));
                    innerTable.Append(row3);

                    // Row 4
                    string attPhone = FormatPhone(h["attorney_phone"].ToString());
                    string oppPhone = FormatPhone(h["opp_attorney_phone"].ToString());
                    TableRow row4 = new TableRow();
                    row4.Append(new TableCell(CreateParagraph(attPhone)));
                    row4.Append(new TableCell(CreateParagraph("")));
                    row4.Append(new TableCell(CreateParagraph(oppPhone)));
                    innerTable.Append(row4);

                    // Row 5
                    string hearingType = h["category"].ToString();
                    TableRow row5 = new TableRow();
                    TableCell mergedCell = new TableCell(CreateParagraph(hearingType));
                    mergedCell.Append(new TableCellProperties(new GridSpan { Val = 3 }));
                    row5.Append(mergedCell);
                    innerTable.Append(row5);

                    // UDF
                    string template = h["template"].ToString();
                    if (!string.IsNullOrEmpty(template))
                    {
                        JObject json = JObject.Parse(template);
                        int i = 0;
                        var udfs = json.Properties()
                            .Where(p => !string.IsNullOrEmpty(p.Name))
                            .Select(p => new { Label = CleanLabel(p.Name), Value = p.Value.ToString(), OriginalName = p.Name })
                            .ToList();

                        foreach (var udf in udfs)
                        {
                            string field_name = udf.Label;
                            var display_on_docket = userDefinedFieldsCtl.GetUserDefinedFields()
                                .Where(u => u.court_id == courtId)
                                .Where(u => new[] { field_name, Regex.Replace(field_name, @"\d+$", "") }.Contains(u.field_name))
                                .Select(u => new { u.display_on_docket, u.field_type })
                                .FirstOrDefault();

                            if (display_on_docket != null && display_on_docket.display_on_docket == 1)
                            {
                                string cleaned_key = CleanLabel(udf.OriginalName);
                                cleaned_key = Regex.Replace(cleaned_key, @"[^\w\s:,\']", "");
                                if (!string.IsNullOrEmpty(cleaned_key) && !string.IsNullOrEmpty(udf.Value))
                                {
                                    if (i % 3 == 0)
                                    {
                                        TableRow udfRow = new TableRow();
                                        innerTable.Append(udfRow);
                                    }

                                    string defined_data_value = display_on_docket.field_type == "DATE" &&
                                        DateTime.TryParse(udf.Value, out DateTime dateValue)
                                        ? dateValue.ToString("MM-dd-yyyy")
                                        : udf.Value;

                                    TableRow currentRow = innerTable.Descendants<TableRow>().Last();
                                    currentRow.Append(new TableCell(CreateLabeledParagraph(cleaned_key, defined_data_value, JustificationValues.Left)));

                                    i++;
                                    if (i % 3 == 0 && i < udfs.Count(p =>
                                    {
                                        var field = CleanLabel(p.OriginalName);
                                        var check = userDefinedFieldsCtl.GetUserDefinedFields()
                                            .Where(u => u.court_id == courtId)
                                            .Where(u => new[] { field, Regex.Replace(field, @"\d+$", "") }.Contains(u.field_name))
                                            .Select(u => u.display_on_docket)
                                            .FirstOrDefault();
                                        return  check == 1;
                                    }))
                                    {
                                        TableRow blankRow = new TableRow();
                                        TableCell blankMerged = new TableCell(CreateParagraph(""));
                                        blankMerged.Append(new TableCellProperties(new GridSpan { Val = 3 }));
                                        blankRow.Append(blankMerged);
                                        innerTable.Append(blankRow);
                                    }
                                }
                            }
                        }
                    }
                    // Notes (optional, if needed)
                    if (!string.IsNullOrEmpty(h["notes"].ToString()))
                    {
                        TableRow notesRow = new TableRow();
                        TableCell notesCell = new TableCell(CreateLabeledParagraph("Notes", CleanString(h["notes"].ToString()), JustificationValues.Left));
                        notesCell.Append(new TableCellProperties(new GridSpan { Val = 3 }));
                        notesRow.Append(notesCell);
                        innerTable.Append(notesRow);
                    }

                    outerCell.Append(innerTable);
                }

                outerRow.Append(outerCell);
                outerTable.Append(outerRow);
            }

            body.Append(outerTable);
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