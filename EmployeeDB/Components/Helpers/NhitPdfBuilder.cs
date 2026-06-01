using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Models;
using Font = iTextSharp.text.Font;

namespace tjc.Modules.EmployeeDB.Components.Helpers
{
    /// <summary>
    /// Renders a submitted New Hire IT Worksheet to a PDF byte array. The
    /// PDF is what gets attached to the helpdesk email and (in future)
    /// downloaded as a record copy.
    ///
    /// We use iTextSharp 5.5 — already referenced by sister modules
    /// (PretrialServices, PretrialServicesSarasota) at
    /// ..\..\Intranet\bin\itextsharp.dll. Keeping that reference path
    /// avoids dragging in a second PDF library for one new feature.
    /// </summary>
    public static class NhitPdfBuilder
    {
        // Reusable fonts. iTextSharp 5.5 lets us share these across pages.
        private static readonly Font FontTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
        private static readonly Font FontHeading = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.BLACK);
        private static readonly Font FontLabel = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.BLACK);
        private static readonly Font FontValue = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);
        private static readonly Font FontSmall = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.DARK_GRAY);

        // U+2611 / U+2610 work in the embedded fonts iText ships with — but
        // not all platforms render them, so use clear ASCII markers that
        // print cleanly everywhere.
        private const string CheckOn = "[X]";
        private const string CheckOff = "[ ]";

        public static byte[] Build(NhitRequestInfo request, IList<NhitItemInfo> activeItems, ICollection<int> checkedItemIds)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            activeItems = activeItems ?? new List<NhitItemInfo>();
            var checkedSet = new HashSet<int>(checkedItemIds ?? new List<int>());

            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.LETTER, 36f, 36f, 36f, 36f);
                var writer = PdfWriter.GetInstance(doc, ms);
                writer.CloseStream = false;
                doc.Open();

                // --- Title -------------------------------------------------
                var title = new Paragraph("New Hire IT Worksheet", FontTitle)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 8f
                };
                doc.Add(title);

                var sub = new Paragraph(
                    "Submitted: " + request.SubmittedDate.ToString("MM/dd/yyyy h:mm tt"),
                    FontSmall)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 12f
                };
                doc.Add(sub);

                // --- Employee Info ----------------------------------------
                doc.Add(SectionHeading("Employee Information"));
                doc.Add(BuildInfoTable(new[]
                {
                    Pair("Position Title",    request.PositionTitle),
                    Pair("Supervisor",        request.SupervisorName),
                    Pair("Employee Name",     request.EmployeeName),
                    Pair("AKA",               request.AKA),
                    Pair("Department / Unit / Group", request.DepartmentUnitGroup),
                    Pair("Office / Suite #",  request.OfficeSuiteNumber),
                    Pair("Desk Phone",        request.DeskPhoneNumber),
                    Pair("Today's Date",      FormatDate(request.TodaysDate)),
                    Pair("Effective Date",    FormatDate(request.EffectiveDate)),
                    Pair("Temp/Intern End",   FormatDate(request.TempInternEndDate)),
                }));

                doc.Add(SpacingPara(6f));

                // --- Building / Employee Type ----------------------------
                doc.Add(SectionHeading("Location & Employee Type"));
                doc.Add(SinglePara("Building / Location: ", request.BuildingLocation));
                doc.Add(SinglePara("Employee Type: ", request.EmployeeType));
                doc.Add(SpacingPara(6f));

                // --- Equipment Needed ------------------------------------
                doc.Add(SectionHeading("Equipment Needed"));
                doc.Add(InlineCheckboxes(new[]
                {
                    Tuple.Create("Laptop", request.EquipmentLaptop),
                    Tuple.Create("2-in-1", request.EquipmentTwoInOne),
                    Tuple.Create("Desktop", request.EquipmentDesktop),
                    Tuple.Create("Cell Phone", request.EquipmentCellPhone),
                }));
                doc.Add(SpacingPara(6f));

                // --- Keys / Access ---------------------------------------
                doc.Add(SectionHeading("Keys / Access"));
                doc.Add(BuildInfoTable(new[]
                {
                    Pair("Access card to",               request.AccessCardTo),
                    Pair("Keys needed",                  request.KeysNeeded),
                    Pair("Parking access",               request.ParkingAccess),
                    Pair("Email distribution groups",    request.EmailDistributionGroups),
                    Pair("Calendars / share calendar",   request.CalendarAccess),
                    Pair("Share drive access",           request.ShareDriveAccess),
                    Pair("Additional printer access",    request.AdditionalPrinterAccess),
                }));
                doc.Add(SpacingPara(6f));

                // --- Manager Access --------------------------------------
                doc.Add(SectionHeading("Manager Access"));
                doc.Add(InlineCheckboxes(new[]
                {
                    Tuple.Create("Manager's Blog and Manager's Guide", request.ManagerBlog),
                    Tuple.Create("Add to supervisor drop-down menu on database", request.AddToSupervisorDropdown),
                    Tuple.Create("Work cellphone set up", request.WorkCellphoneSetup),
                }));
                doc.Add(SpacingPara(6f));

                // --- Catalog sections (Software / Intranet / Judicial) ---
                AddCatalogSection(doc, "Software Applications", "Software", activeItems, checkedSet);
                AddCatalogSection(doc, "Intranet Application Access", "Intranet", activeItems, checkedSet);
                AddCatalogSection(doc, "Judicial Applications", "Judicial", activeItems, checkedSet);

                // --- Notes ------------------------------------------------
                if (!string.IsNullOrWhiteSpace(request.Notes))
                {
                    doc.Add(SectionHeading("Notes"));
                    doc.Add(new Paragraph(request.Notes, FontValue) { SpacingAfter = 6f });
                }

                doc.Close();
                writer.Close();
                return ms.ToArray();
            }
        }

        // -------- helpers --------

        private static Tuple<string, string> Pair(string label, string value)
        {
            return Tuple.Create(label, value ?? string.Empty);
        }

        private static Paragraph SectionHeading(string text)
        {
            return new Paragraph(text, FontHeading)
            {
                SpacingBefore = 8f,
                SpacingAfter = 4f
            };
        }

        private static Paragraph SinglePara(string label, string value)
        {
            var p = new Paragraph();
            p.Add(new Chunk(label, FontLabel));
            p.Add(new Chunk(string.IsNullOrEmpty(value) ? "(not specified)" : value, FontValue));
            return p;
        }

        private static Paragraph SpacingPara(float pts)
        {
            return new Paragraph(" ", FontSmall) { SpacingAfter = pts };
        }

        /// <summary>Two-column info table — labels left, values right.
        /// Uses 100% page width, no border, light separator between rows.</summary>
        private static PdfPTable BuildInfoTable(IEnumerable<Tuple<string, string>> rows)
        {
            var table = new PdfPTable(2)
            {
                WidthPercentage = 100,
                SpacingAfter = 4f
            };
            table.SetWidths(new float[] { 30f, 70f });

            foreach (var row in rows)
            {
                table.AddCell(LabelCell(row.Item1));
                table.AddCell(ValueCell(row.Item2));
            }
            return table;
        }

        private static PdfPCell LabelCell(string text)
        {
            return new PdfPCell(new Phrase(text, FontLabel))
            {
                Border = Rectangle.NO_BORDER,
                PaddingTop = 2f,
                PaddingBottom = 2f
            };
        }

        private static PdfPCell ValueCell(string text)
        {
            return new PdfPCell(new Phrase(string.IsNullOrEmpty(text) ? string.Empty : text, FontValue))
            {
                Border = Rectangle.NO_BORDER,
                PaddingTop = 2f,
                PaddingBottom = 2f
            };
        }

        /// <summary>Inline list of checkbox + label, separated by 4 spaces.</summary>
        private static Paragraph InlineCheckboxes(IEnumerable<Tuple<string, bool>> items)
        {
            var p = new Paragraph();
            bool first = true;
            foreach (var t in items)
            {
                if (!first) p.Add(new Chunk("    ", FontValue));
                p.Add(new Chunk((t.Item2 ? CheckOn : CheckOff) + " ", FontValue));
                p.Add(new Chunk(t.Item1, FontValue));
                first = false;
            }
            p.SpacingAfter = 4f;
            return p;
        }

        /// <summary>Render one of the catalog sections (Software/Intranet/Judicial)
        /// as a 2-column table of checkbox + label, with checked rows marked
        /// and unchecked rows still shown so the helpdesk sees the full
        /// catalog state (not just what's enabled).</summary>
        private static void AddCatalogSection(Document doc, string heading, string category,
            IList<NhitItemInfo> activeItems, HashSet<int> checkedSet)
        {
            var rows = activeItems
                .Where(i => string.Equals(i.Category, category, StringComparison.OrdinalIgnoreCase))
                .OrderBy(i => i.SortOrder)
                .ThenBy(i => i.Name)
                .ToList();

            doc.Add(SectionHeading(heading));

            if (rows.Count == 0)
            {
                doc.Add(new Paragraph("(no items defined)", FontSmall) { SpacingAfter = 6f });
                return;
            }

            var table = new PdfPTable(2)
            {
                WidthPercentage = 100,
                SpacingAfter = 6f
            };
            table.SetWidths(new float[] { 50f, 50f });

            foreach (var item in rows)
            {
                var checkedFlag = checkedSet.Contains(item.NhitItemId);
                var label = (checkedFlag ? CheckOn : CheckOff) + " " + item.Name;
                if (!string.IsNullOrWhiteSpace(item.Notes)) label += " (" + item.Notes + ")";
                table.AddCell(new PdfPCell(new Phrase(label, FontValue))
                {
                    Border = Rectangle.NO_BORDER,
                    PaddingTop = 1f,
                    PaddingBottom = 1f
                });
            }
            // If we have an odd number of rows the last cell is missing —
            // PdfPTable won't render the last incomplete row. Pad it.
            if (rows.Count % 2 != 0)
            {
                table.AddCell(new PdfPCell(new Phrase(" ", FontValue)) { Border = Rectangle.NO_BORDER });
            }
            doc.Add(table);
        }

        private static string FormatDate(DateTime? dt)
        {
            return dt.HasValue ? dt.Value.ToString("MM/dd/yyyy") : string.Empty;
        }
    }
}
