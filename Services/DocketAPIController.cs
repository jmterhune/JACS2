// Judicial Automated Calendaring System 2.0
// 12th Judicial Circuit - Manatee County
// Docket
// JUDGE HEATHER DOYLE
// Thursday, October 30, 2025

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
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

                var courtCtl = new CourtController();
                var court = courtCtl.GetCourt(courtId);

                if (court == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Court not found." });
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
                                    { "motion", evt.motion_id == 221 ? CleanString(evt.custom_motion) : CleanString(evt.Motion?.description) },
                                    { "hearing_type", evt.EventType?.name ?? null },
                                    { "plaintiff", CleanString(evt.plaintiff) },
                                    { "defendant", CleanString(evt.defendant) },
                                    { "plaintiff_attorney", evt.Attorney?.name ?? "Pro Se" },
                                    { "defendant_attorney", evt.opposing_attorney?.name ?? "Pro Se" },
                                    { "plaintiff_attorney_phone", evt.Attorney?.phone ?? null },
                                    { "defendant_attorney_phone", evt.opposing_attorney?.phone ?? null },
                                    { "category", ts.Category?.description ?? null },
                                    { "notes", CleanString(evt.notes) },
                                    { "user_defined_fields", evt.template ?? null },
                                    { "addon", evt.addon.Value ? "Add-On" : null }
                                });
                            }
                        }
                    }

                    hearings[date] = dayHearings;
                }

                using (var ms = new MemoryStream())
                {
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = mainPart.Document.AppendChild(new Body());

                        var sectionIndex = 1;
                        var totalSections = hearings.Count;

                        var judgeCtl = new JudgeController();
                        var judge = judgeCtl.GetJudgeByCourt(courtId);
                        string judgeName = judge?.name?.ToUpper() ?? "";

                        foreach (var day in hearings)
                        {
                            var date = day.Key;
                            var items = day.Value;

                            if (!items.Any()) continue;

                            // Header with single spacing
                            body.Append(CreateParagraph("Judicial Automated Calendaring System 2.0", true, JustificationValues.Center, 18));
                            body.Append(CreateCircuitParagraph(countyName));
                            body.Append(CreateParagraph("Docket", true, JustificationValues.Center, 14));
                            body.Append(CreateParagraph($"JUDGE {judgeName}", true, JustificationValues.Center, 14));
                            body.Append(CreateParagraph(date.ToString("dddd, MMMM d, yyyy"), true, JustificationValues.Center, 14));

                            // Horizontal line
                            Paragraph hrPara = new Paragraph(new Run(new Text("__________________________________________________________________________________________")));
                            body.Append(hrPara);

                            foreach (var item in items)
                            {
                                if (Convert.ToBoolean(item["blocked"]))
                                {
                                    body.Append(CreateParagraph(item["start_time"].ToString() + "      " + item["duration"].ToString(), false, JustificationValues.Left));
                                    body.Append(CreateParagraph("", false));
                                    body.Append(CreateParagraph("  *** " + item["block_description"].ToString() + " *** ", true, JustificationValues.Left));
                                    body.Append(CreateParagraph("", false));
                                }
                                else
                                {
                                    body.Append(CreateParagraph(item["start_time"].ToString() + "      " + item["duration"].ToString(), false, JustificationValues.Left));
                                    body.Append(CreateParagraph("Case: " + item["case_num"].ToString() + "  ", false, JustificationValues.Left));
                                    body.Append(CreateParagraph("Motion: " + item["motion"].ToString(), false, JustificationValues.Left));

                                    if (item["hearing_type"] != null)
                                    {
                                        body.Append(CreateParagraph("Hearing Type: " + item["hearing_type"].ToString(), false, JustificationValues.Left));
                                    }

                                    if (item["addon"] != null)
                                    {
                                        body.Append(CreateParagraph(item["addon"].ToString(), true, JustificationValues.Left));
                                    }

                                    body.Append(CreateParagraph("", false));

                                    body.Append(CreateParagraph(item["plaintiff"].ToString(), true, JustificationValues.Left));
                                    body.Append(CreateParagraph(item["plaintiff_attorney"].ToString(), false, JustificationValues.Left));
                                    if (item["plaintiff_attorney_phone"] != null)
                                    {
                                        body.Append(CreateParagraph(FormatPhone(item["plaintiff_attorney_phone"].ToString()), false, JustificationValues.Left));
                                    }

                                    body.Append(CreateParagraphWithTab("vs.", TabStopValues.Center, 4320));

                                    body.Append(CreateParagraphWithTab(item["defendant"].ToString(), TabStopValues.Left, 5760));
                                    body.Append(CreateParagraphWithTab(item["defendant_attorney"].ToString(), TabStopValues.Left, 5760));
                                    if (item["defendant_attorney_phone"] != null)
                                    {
                                        body.Append(CreateParagraphWithTab(FormatPhone(item["defendant_attorney_phone"].ToString()), TabStopValues.Left, 5760));
                                    }

                                    body.Append(CreateParagraph("", false));

                                    body.Append(CreateParagraphWithTab(item["category"].ToString(), TabStopValues.Center, 4320));

                                    // User Defined Fields
                                    if (item["user_defined_fields"] is string udfJson && !string.IsNullOrEmpty(udfJson))
                                    {
                                        var udfData = JObject.Parse(udfJson);
                                        var udfCtl = new UserDefinedFieldController();
                                        var udfModels = udfCtl.GetUserDefinedFieldsByCourtId(courtId).ToList();

                                        foreach (var prop in udfData.Properties())
                                        {
                                            var udfModel = udfModels.FirstOrDefault(udf => udf.id.ToString() == prop.Name.Split('_')[0]);

                                            if (udfModel != null && udfModel.display_on_docket==1)
                                            {
                                                string udfName = Regex.Replace(prop.Name.Split(new[] { "_|" }, StringSplitOptions.None)[0], @"\d+$", "");
                                                var value = prop.Value.ToString();
                                                if (udfModel.field_type == "DATE" && DateTime.TryParse(value, out DateTime dt))
                                                {
                                                    value = dt.ToString("MM-dd-yyyy");
                                                }
                                                value = CleanString(value);

                                                body.Append(CreateParagraph(udfName + ":", true, JustificationValues.Left));

                                                if (udfName.ToLower().Contains("defendant") || udfName.ToLower().Contains(court.defendant?.ToLower() ?? "defendant"))
                                                {
                                                    body.Append(CreateParagraphWithTab(value, TabStopValues.Left, 5760));
                                                }
                                                else
                                                {
                                                    body.Append(CreateParagraph(value, false, JustificationValues.Left));
                                                }
                                            }
                                        }
                                    }


                                    // Notes
                                    if (item["notes"] is string notes && !string.IsNullOrEmpty(notes))
                                    {
                                        body.Append(CreateParagraph("Notes:", true, JustificationValues.Left));
                                        body.Append(CreateParagraph(CleanString(notes), false, JustificationValues.Left));
                                    }

                                    body.Append(CreateParagraph("", false));
                                }
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

        private Paragraph CreateParagraphWithTab(string text, TabStopValues val, int pos, bool bold = false, double fontSize = 11)
        {
            Paragraph para = new Paragraph();
            ParagraphProperties paraProps = new ParagraphProperties(
                new Tabs(new TabStop { Val = val, Position = pos }),
                new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" }
            );
            para.Append(paraProps);

            para.Append(new Run(new TabChar()));

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