// Filename: DocketAPIController.cs
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web.Http;
using tjc.Modules.jacs.Components;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class DocketAPIController : DnnApiController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage GenerateDocketReport(JObject p1)
        {
            try
            {
                long courtId = p1["court"].ToObject<long>();
                long categoryId = p1["category"]?.ToObject<long>() ?? 0;
                if (!DateTime.TryParse(p1["from"]?.ToString(), out DateTime fromDate))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid from date." });
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
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court not found." });
                }
                else
                {
                    if (court.category_print != categoryPrint)
                    {
                        court.category_print = categoryPrint;
                        courtCtl.UpdateCourt(court);
                    }
                }
                var hearings = new Dictionary<DateTime, List<Dictionary<string, object>>>();
                var period = toDate.HasValue ? GetDateRange(fromDate, toDate.Value) : new List<DateTime> { fromDate };
                var holidayCtl = new HolidayController();
                var timeslotCtl = new TimeslotController();
                var eventCtl = new EventController();
                var courtTimeslotCtl = new CourtTimeslotController();

                foreach (var date in period.Where(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday))
                {
                    var timeslots = courtTimeslotCtl.GetCourtTimeslotsByCourtId(courtId)
                        .Select(ct => ct.Timeslot)
                        .Where(ts => ts.start.Date == date.Date)
                        .ToList();

                    if (categoryId != 0)
                    {
                        timeslots = timeslots.Where(ts => ts.category_id == categoryId).ToList();
                    }

                    timeslots = timeslots.OrderBy(ts => ts.start).ToList();

                    var holiday = holidayCtl.GetHolidays().FirstOrDefault(h => h.date.Date == date.Date);

                    var dayHearings = new List<Dictionary<string, object>>();

                    if (!timeslots.Any())
                    {
                        var desc = holiday is Holiday hol ? hol.name : "Not Available";
                        dayHearings.Add(new Dictionary<string, object>
                        {
                            { "start_time", date.ToString("h:mm tt") },
                            { "end_time", date.ToString("h:mm tt") },
                            { "duration", "0 min" },
                            { "description", desc },
                            { "blocked", true },
                            { "public_block", true },
                            { "block_description", desc }
                        });
                    }
                    else
                    {
                        foreach (var ts in timeslots)
                        {
                            var events = eventCtl.GetEventsByTimeslot(ts.id);
                            switch (hearing)
                            {
                                case "addon":
                                    events = events.Where(e => e.addon.Value).ToList();
                                    break;
                                case "noaddon":
                                    events = events.Where(e => !e.addon.Value).ToList();
                                    break;
                            }

                            if (!events.Any())
                            {
                                var desc = holiday is Holiday hol ? hol.name : ts.description ?? "Not Available";
                                dayHearings.Add(new Dictionary<string, object>
                                {
                                    { "start_time", ts.start.ToString("h:mm tt") },
                                    { "end_time", ts.end.ToString("h:mm tt") },
                                    { "duration", ts.duration + " min" },
                                    { "description", desc },
                                    { "blocked", ts.blocked },
                                    { "public_block", ts.public_block },
                                    { "block_description", ts.description }
                                });
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
                                        { "motion", evt.motion_id == 221 ? CleanString(evt.custom_motion) : CleanString(evt.Motion?.description) },
                                        { "hearing_type", evt.EventType?.name ?? null },
                                        { "plaintiff", CleanString(evt.plaintiff) },
                                        { "defendant", CleanString(evt.defendant) },
                                        { "plaintiff_attorney", evt.Attorney?.name ?? null },
                                        { "defendant_attorney", evt.OppAttorney?.name ?? null },
                                        { "plaintiff_attorney_phone", evt.Attorney?.phone ?? null },
                                        { "defendant_attorney_phone", evt.OppAttorney?.phone ?? null },
                                        { "category", ts.Category?.description ?? null },
                                        { "notes", CleanString(evt.notes) },
                                        { "user_defined_fields", evt.template ?? null }
                                    });
                                }
                            }
                        }
                    }

                    if (dayHearings.Any())
                    {
                        hearings[date] = dayHearings;
                    }
                }

                using (var ms = new MemoryStream())
                {
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = mainPart.Document.AppendChild(new Body());

                        int sectionIndex = 0;
                        foreach (var kv in hearings)
                        {
                            var day = kv.Key;
                            var dayHearings = kv.Value;

                            // Create section properties for each "section" (per day)
                            SectionProperties sectionProps = new SectionProperties(
                                new PageSize() { Width = 12240U, Height = 15840U }
                            );

                            // Header
                            string headerRelId = "header" + sectionIndex;
                            HeaderPart headerPart = mainPart.AddNewPart<HeaderPart>(headerRelId);
                            Header header = new Header();
                            var headerLines = court.custom_header?.Split(new[] { "\n" }, StringSplitOptions.None) ?? new string[0];
                            foreach (var line in headerLines)
                            {
                                header.Append(CreateParagraph(CleanString(line), true, JustificationValues.Center, 12));
                            }
                            header.Append(CreateParagraph(court.judge_name, true, JustificationValues.Center, 14));
                            header.Append(CreateParagraph(day.ToString("dddd, MMMM d, yyyy"), true, JustificationValues.Center, 14));
                            headerPart.Header = header;
                            sectionProps.Append(new HeaderReference() { Type = HeaderFooterValues.Default, Id = headerRelId });

                            // Footer
                            string footerRelId = "footer" + sectionIndex;
                            FooterPart footerPart = mainPart.AddNewPart<FooterPart>(footerRelId);
                            GenerateFooterPartContent(footerPart);
                            //Footer footer = new Footer(
                            //    CreateParagraph("Page ", false, JustificationValues.Center, 10)
                            //);
                            //footer.LastChild.AppendChild(new SimpleField() { Instruction = "PAGE" });
                            //footer.Append(CreateParagraph(" of ", false, JustificationValues.Center, 10));
                            //footer.LastChild.AppendChild(new SimpleField() { Instruction = "NUMPAGES" });
                            //footerPart.Footer = footer;
                            sectionProps.Append(new FooterReference() { Type = HeaderFooterValues.Default, Id = footerRelId });

                            // Add tables for hearings
                            foreach (var item in dayHearings)
                            {
                                Table table = new Table();

                                // Table properties (bottom border only)
                                TableProperties tblProps = new TableProperties(
                                    new TableBorders(
                                        new BottomBorder() { Val = BorderValues.Single, Size = 4, Color = "38C172" } // Approx #38C172 for rgb(56,193,114)
                                    ),
                                    new TableGrid(
                                        new GridColumn() { Width = "3120" }, // Approx 33% of content width (9360 twips / 3)
                                        new GridColumn() { Width = "3120" },
                                        new GridColumn() { Width = "3120" }
                                    )
                                );
                                table.Append(tblProps);

                                // First row: Time | Case | Motion
                                TableRow firstRow = new TableRow();
                                TableCell timeCell = new TableCell();
                                Paragraph timePara = CreateParagraph(item["start_time"].ToString(), true);
                                timeCell.Append(timePara);
                                timePara.AppendChild(new Run(new Text($" ({item["duration"]})")));
                                if (item.TryGetValue("hearing_type", out var ht) && ht != null)
                                {
                                    timePara.AppendChild(new Run(new Text(ht.ToString())));
                                }

                                if (!item.ContainsKey("case_num"))
                                {
                                    // Blocked/description row
                                    timeCell.Append(new TableCellProperties(new GridSpan() { Val = 3 }));
                                    firstRow.Append(timeCell);
                                    table.Append(firstRow);

                                    TableRow descRow = new TableRow();
                                    TableCell descCell = new TableCell(CreateParagraph(item["description"].ToString(), true, JustificationValues.Center));
                                    descCell.Append(new TableCellProperties(new GridSpan() { Val = 3 }));
                                    descRow.Append(descCell);
                                    table.Append(descRow);
                                }
                                else
                                {
                                    if ((bool)item["public_block"])
                                    {
                                        Run blockRun = new Run(new Text(item["block_description"].ToString()));
                                        blockRun.RunProperties = new RunProperties(new Bold());
                                        timePara.AppendChild(blockRun);
                                    }

                                    firstRow.Append(timeCell);

                                    TableCell caseCell = new TableCell(CreateParagraph("CASE", true));
                                    caseCell.Append(CreateParagraph(item["case_num"].ToString()));
                                    firstRow.Append(caseCell);

                                    TableCell motionCell = new TableCell(CreateParagraph("MOTION", true));
                                    motionCell.Append(CreateParagraph(item["motion"].ToString()));
                                    firstRow.Append(motionCell);

                                    table.Append(firstRow);

                                    // Party row: Plaintiff | vs. | Defendant
                                    TableRow partyRow = new TableRow();
                                    TableCell plaintiffCell = new TableCell();
                                    plaintiffCell.Append(CreateParagraph(item["plaintiff"]?.ToString() ?? ""));
                                    plaintiffCell.Append(CreateParagraph(item["plaintiff_attorney"]?.ToString() ?? ""));
                                    plaintiffCell.Append(CreateParagraph(FormatPhone(item["plaintiff_attorney_phone"]?.ToString())));
                                    partyRow.Append(plaintiffCell);

                                    TableCell vsCell = new TableCell(CreateParagraph("vs.", false, JustificationValues.Center));
                                    partyRow.Append(vsCell);

                                    TableCell defendantCell = new TableCell();
                                    defendantCell.Append(CreateParagraph(item["defendant"]?.ToString() ?? ""));
                                    defendantCell.Append(CreateParagraph(item["defendant_attorney"]?.ToString() ?? ""));
                                    defendantCell.Append(CreateParagraph(FormatPhone(item["defendant_attorney_phone"]?.ToString())));
                                    partyRow.Append(defendantCell);

                                    table.Append(partyRow);

                                    // User-defined fields (3 per row)
                                    if (item["user_defined_fields"] is string udfJson && !string.IsNullOrEmpty(udfJson))
                                    {
                                        var udf = JObject.Parse(udfJson);
                                        int i = 0;
                                        var udfCtl = new UserDefinedFieldController();
                                        var udfs = udfCtl.GetUserDefinedFields().Where(u => u.court_id == courtId).ToList();

                                        TableRow udfRow = null;
                                        foreach (var prop in udf.Properties())
                                        {
                                            var fieldName = Regex.Replace(prop.Name.Split(new[] { "_|" }, StringSplitOptions.None)[0], @"\d+$", "");
                                            // Fix for CS0165: Use of unassigned local variable 'udfModel'
                                            if (udfs.FirstOrDefault(u => u.field_name == fieldName || Regex.Replace(u.field_name, @"\d+$", "") == fieldName) is UserDefinedField udfModel && udfModel.display_on_docket == 1)
                                            {
                                                if (i % 3 == 0)
                                                {
                                                    udfRow = new TableRow();
                                                    table.Append(udfRow);
                                                }
                                                TableCell udfCell = new TableCell();
                                                Paragraph udfPara = CreateParagraph(Regex.Replace(prop.Name.Split(new[] { "_|" }, StringSplitOptions.None)[0], @"\d+$", ""), true);
                                                udfCell.Append(udfPara);
                                                var value = prop.Value.ToString();
                                                if (udfModel.field_type == "DATE" && DateTime.TryParse(value, out DateTime dt))
                                                {
                                                    value = dt.ToString("MM-dd-yyyy");
                                                }
                                                udfPara.AppendChild(new Run(new Text(CleanString(value))));
                                                udfRow.Append(udfCell);
                                                i++;
                                            }
                                        }
                                        // Fill empty cells if needed
                                        while (i % 3 != 0)
                                        {
                                            udfRow.Append(new TableCell());
                                            i++;
                                        }
                                    }

                                    // Category
                                    if (categoryPrint && item["category"] != null)
                                    {
                                        TableRow catRow = new TableRow();
                                        TableCell catCell = new TableCell();
                                        catCell.Append(CreateParagraph("CATEGORY", true));
                                        catCell.Append(CreateParagraph(item["category"].ToString()));
                                        catCell.Append(new TableCellProperties(new GridSpan() { Val = 3 }));
                                        catRow.Append(catCell);
                                        table.Append(catRow);
                                    }

                                    // Notes
                                    if (item["notes"] is string notes && !string.IsNullOrEmpty(notes))
                                    {
                                        TableRow notesRow = new TableRow();
                                        TableCell notesCell = new TableCell();
                                        notesCell.Append(CreateParagraph("NOTES", true));
                                        notesCell.Append(CreateParagraph(CleanString(notes)));
                                        notesCell.Append(new TableCellProperties(new GridSpan() { Val = 3 }));
                                        notesRow.Append(notesCell);
                                        table.Append(notesRow);
                                    }
                                }

                                body.Append(table);
                            }

                            body.Append(sectionProps);
                            sectionIndex++;
                        }

                        mainPart.Document.Save();
                    }

                    ms.Position = 0;

                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(ms.ToArray())
                    };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = $"{court.description.Replace("/", "-")}-{DateTime.Now:yyyy-MM-dd}.docx"
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        private Paragraph CreateParagraph(string text, bool bold = false, JustificationValues? alignment = null, double fontSize = 11)
        {
            // Replace the coalescing assignment with a standard null check and assignment
            if (!alignment.HasValue)
            {
                alignment = JustificationValues.Left; // Assign default value
            }

            Paragraph para = new Paragraph();
            ParagraphProperties paraProps = new ParagraphProperties(new Justification() { Val = alignment });
            para.Append(paraProps);

            Run run = new Run(new Text(text));
            if (bold || fontSize != 11)
            {
                RunProperties runProps = new RunProperties();
                if (bold) runProps.Append(new Bold());
                if (fontSize != 11) runProps.Append(new FontSize() { Val = (fontSize * 2).ToString() }); // Half-points
                run.PrependChild(runProps);
            }
            para.Append(run);

            return para;
        }

        private string CleanString(string input)
        {
            return string.IsNullOrEmpty(input) ? input : Regex.Replace(input, @"[^A-Za-z0-9\-\.\@\/ ]", "");
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
    }
}