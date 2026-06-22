// File: View.ascx.cs
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.DocketInmateCompare.Components;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace tjc.Modules.DocketInmateCompare
{
    public partial class View : PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack && !string.IsNullOrEmpty(hfCurrentSetGuid.Value))
                {
                    var controller = new NameMatchResultController();
                    var setGuid = Guid.Parse(hfCurrentSetGuid.Value);
                    gvMatches.DataSource = controller.GetItemsBySetGuid(setGuid);
                    gvMatches.DataBind();
                    pnlFormDetails.Visible = true;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            if (fuCourtCSV.HasFile && fuJailXLSX.HasFile)
            {
                if (Path.GetExtension(fuCourtCSV.FileName).ToLower() != ".csv" ||
                    Path.GetExtension(fuJailXLSX.FileName).ToLower() != ".xlsx")
                {
                    // Handle invalid file types
                    return;
                }

                string tempPath = Path.GetTempPath();
                string courtPath = Path.Combine(tempPath, Guid.NewGuid().ToString() + ".csv");
                string jailPath = Path.Combine(tempPath, Guid.NewGuid().ToString() + ".xlsx");

                fuCourtCSV.SaveAs(courtPath);
                fuJailXLSX.SaveAs(jailPath);

                Session["CourtPath"] = courtPath;
                Session["JailPath"] = jailPath;

                var comparer = new DefendantNameComparer(0.88); // Adjust threshold as needed
                List<NameMatchResult> results = comparer.CompareFiles(courtPath, jailPath);

                var controller = new NameMatchResultController();
                Guid setGuid = Guid.NewGuid();
                hfCurrentSetGuid.Value = setGuid.ToString();

                foreach (var result in results)
                {
                    result.SetGuid = setGuid;
                    result.CreatedOnDate = DateTime.Now;
                    controller.CreateItem(result);
                }

                gvMatches.DataSource = controller.GetItemsBySetGuid(setGuid);
                gvMatches.DataBind();

                pnlFormDetails.Visible = true;
            }
        }

        protected void gvMatches_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                RadioButton rbZoom = (RadioButton)e.Row.FindControl("rbZoom");
                RadioButton rbTransport = (RadioButton)e.Row.FindControl("rbTransport");
                RadioButton rbInPerson = (RadioButton)e.Row.FindControl("rbInPerson");
                NameMatchResult result = (NameMatchResult)e.Row.DataItem;
                if (result.Mode == "Transport")
                {
                    rbTransport.Checked = true;
                }
                else if (result.Mode == "InPerson")
                {
                    rbInPerson.Checked = true;
                }
                else
                {
                    rbZoom.Checked = true;
                }

                TextBox txtStart = (TextBox)e.Row.FindControl("txtStart");
                if (DateTime.TryParseExact(txtStart.Text, "M/d/yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                {
                    txtStart.Text = dt.ToString("h:mm tt");
                }
            }
        }

        protected void gvMatches_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton btnDelete = (LinkButton)e.Row.FindControl("btnDelete");
                if (btnDelete != null)
                {
                    ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(btnDelete);
                }
            }
        }

        protected void gvMatches_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                // Update all remaining rows before delete
                var controller = new NameMatchResultController();
                for (int i = 0; i < gvMatches.Rows.Count; i++)
                {
                    GridViewRow row = gvMatches.Rows[i];
                    TextBox txtStart = (TextBox)row.FindControl("txtStart");
                    TextBox txtEventType = (TextBox)row.FindControl("txtEventType");
                    int id = Convert.ToInt32(row.Cells[0].Text);
                    var item = controller.GetItem(id);
                    item.Mode = ReadMode(row);
                    item.Start = txtStart.Text;
                    item.EventType = txtEventType.Text;
                    controller.UpdateItem(item);
                }

                int rowIndex = Convert.ToInt32(e.CommandArgument);
                int idToDelete = Convert.ToInt32(gvMatches.Rows[rowIndex].Cells[0].Text);

                controller.DeleteItem(idToDelete);

                var setGuid = Guid.Parse(hfCurrentSetGuid.Value);
                gvMatches.DataSource = controller.GetItemsBySetGuid(setGuid);
                gvMatches.DataBind();
            }
        }

        protected void gvMatches_PreRender(object sender, EventArgs e)
        {
            if (gvMatches.Rows.Count > 0)
            {
                gvMatches.UseAccessibleHeader = true;
                gvMatches.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        protected void btnGenerateWord_Click(object sender, EventArgs e)
        {
            var controller = new NameMatchResultController();
            var setGuid = Guid.Parse(hfCurrentSetGuid.Value);

            // Update modes, start times, and event types from grid
            for (int i = 0; i < gvMatches.Rows.Count; i++)
            {
                GridViewRow row = gvMatches.Rows[i];
                TextBox txtStart = (TextBox)row.FindControl("txtStart");
                TextBox txtEventType = (TextBox)row.FindControl("txtEventType");
                int id = Convert.ToInt32(row.Cells[0].Text);
                var item = controller.GetItem(id);
                item.Mode = ReadMode(row);
                item.Start = txtStart.Text;
                item.EventType = txtEventType.Text;
                controller.UpdateItem(item);
            }

            // Retrieve from database
            var results = controller.GetItemsBySetGuid(setGuid);

            // Generate Word document using OpenXML, matching the Sarasota Jail Hearings request form
            MemoryStream stream = new MemoryStream();
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new W.Document();
                W.Body body = mainPart.Document.AppendChild(new W.Body());

                string emailRelId = mainPart.AddHyperlinkRelationship(
                    new Uri("mailto:inmateappearance@sarasotasheriff.org"), true).Id;

                // Title
                body.AppendChild(CenteredParagraph("120",
                    BodyRun("SARASOTA JAIL HEARINGS", 32, bold: true, underline: true)));

                // Requesting judge / courtroom / date of hearing / submitted by (centered values)
                string dateText = DateTime.TryParse(txtDate.Text, out DateTime hearingDate)
                    ? hearingDate.ToString("dddd, MMMM d, yyyy")
                    : txtDate.Text;
                body.AppendChild(CenteredParagraph("0", BodyRun(txtJudge.Text, 24)));
                body.AppendChild(CenteredParagraph("0", BodyRun(txtCourtroom.Text, 24)));
                body.AppendChild(CenteredParagraph("0", BodyRun(dateText, 24)));
                body.AppendChild(CenteredParagraph("0", BodyRun(txtSubmittedBy.Text, 24)));

                body.AppendChild(CenteredParagraph("0"));

                // Submission deadline instructions
                body.AppendChild(CenteredParagraph("0",
                    BodyRun("This list must be sent to the Sheriff’s Office ", 24),
                    BodyRun("by 12PM of the preceding business day", 24, bold: true, underline: true),
                    BodyRun(" to ensure inmates are identified for remote appearance or transport in a timely manner.", 24)));

                // Exigent-circumstances note (highlighted)
                body.AppendChild(CenteredParagraph("0",
                    BodyRun("If exigent or emergency circumstances exist outside of the 12pm deadline, for virtual or transport appearances, JA’s are required to email the justification for the request to ", 24, bold: true, highlight: true),
                    MailHyperlink(emailRelId, 24, bold: true, highlight: true),
                    BodyRun(".", 24, bold: true, highlight: true)));

                body.AppendChild(CenteredParagraph("0"));

                // Note
                body.AppendChild(CenteredParagraph("0",
                    BodyRun("NOTE:", 24, bold: true, underline: true, highlight: true, color: "EE0000"),
                    BodyRun("  ", 24, bold: true, highlight: true),
                    BodyRun("ZOOM APPEARANCES ARE NOT CONDUCTED PAST 12PM DAILY!", 24, bold: true, underline: true, highlight: true)));

                body.AppendChild(CenteredParagraph("120",
                    BodyRun("Please email this request to ", 24),
                    MailHyperlink(emailRelId, 24),
                    BodyRun(".", 24)));

                // Hearings table
                body.AppendChild(BuildHearingsTable(results));

                // Zoom meeting information
                body.AppendChild(LeftParagraph());
                body.AppendChild(LeftParagraph(TnrRun("Zoom Meeting Information", 28, bold: true, underline: true)));
                body.AppendChild(LeftParagraph(TnrRun($"Meeting ID: {txtZoomID.Text}", 28)));
                body.AppendChild(LeftParagraph(TnrRun($"Password: {txtPassword.Text}", 28)));

                AppendFooterAndSection(mainPart, body);

                mainPart.Document.Save();
            }

            // Delete temp files
            string courtPath = Session["CourtPath"] as string;
            string jailPath = Session["JailPath"] as string;
            if (!string.IsNullOrEmpty(courtPath) && File.Exists(courtPath)) File.Delete(courtPath);
            if (!string.IsNullOrEmpty(jailPath) && File.Exists(jailPath)) File.Delete(jailPath);
            Session.Remove("CourtPath");
            Session.Remove("JailPath");

            // Delete database records
            controller.DeleteItemsBySetGuid(setGuid);
            hfCurrentSetGuid.Value = string.Empty;

            string fileName = $"InmateRequestForm_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.docx";
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
            stream.Position = 0;
            stream.CopyTo(Response.OutputStream);
            Response.End();
        }

        private static string ReadMode(GridViewRow row)
        {
            RadioButton rbTransport = (RadioButton)row.FindControl("rbTransport");
            RadioButton rbInPerson = (RadioButton)row.FindControl("rbInPerson");
            if (rbTransport != null && rbTransport.Checked) return "Transport";
            if (rbInPerson != null && rbInPerson.Checked) return "InPerson";
            return "Zoom";
        }

        private static W.Run BodyRun(string text, int sizeHalfPoints, bool bold = false, bool underline = false, bool highlight = false, string color = "242424")
        {
            return StyledRun(text, "Aptos", sizeHalfPoints, bold, underline, highlight, color);
        }

        private static W.Run TnrRun(string text, int sizeHalfPoints, bool bold = false, bool underline = false)
        {
            return StyledRun(text, "Times New Roman", sizeHalfPoints, bold, underline, false, "242424");
        }

        private static W.Run StyledRun(string text, string font, int sizeHalfPoints, bool bold, bool underline, bool highlight, string color)
        {
            var runProperties = new W.RunProperties();
            runProperties.Append(new W.RunFonts { Ascii = font, HighAnsi = font });
            if (bold) runProperties.Append(new W.Bold());
            if (!string.IsNullOrEmpty(color)) runProperties.Append(new W.Color { Val = color });
            runProperties.Append(new W.FontSize { Val = sizeHalfPoints.ToString() });
            runProperties.Append(new W.FontSizeComplexScript { Val = sizeHalfPoints.ToString() });
            if (highlight) runProperties.Append(new W.Highlight { Val = HighlightColorValues.Yellow });
            if (underline) runProperties.Append(new W.Underline { Val = UnderlineValues.Single });

            var run = new W.Run();
            run.Append(runProperties);
            run.Append(new W.Text(text ?? string.Empty) { Space = SpaceProcessingModeValues.Preserve });
            return run;
        }

        private static W.Hyperlink MailHyperlink(string relationshipId, int sizeHalfPoints, bool bold = false, bool highlight = false)
        {
            var runProperties = new W.RunProperties();
            runProperties.Append(new W.RunFonts { Ascii = "Aptos", HighAnsi = "Aptos" });
            if (bold) runProperties.Append(new W.Bold());
            runProperties.Append(new W.Color { Val = "0563C1" });
            runProperties.Append(new W.FontSize { Val = sizeHalfPoints.ToString() });
            runProperties.Append(new W.FontSizeComplexScript { Val = sizeHalfPoints.ToString() });
            if (highlight) runProperties.Append(new W.Highlight { Val = HighlightColorValues.Yellow });
            runProperties.Append(new W.Underline { Val = UnderlineValues.Single });

            var run = new W.Run(runProperties, new W.Text("inmateappearance@sarasotasheriff.org") { Space = SpaceProcessingModeValues.Preserve });
            return new W.Hyperlink(run) { Id = relationshipId, History = OnOffValue.FromBoolean(true) };
        }

        private static W.Paragraph CenteredParagraph(string spacingAfter, params OpenXmlElement[] content)
        {
            return BlockParagraph(JustificationValues.Center, spacingAfter, content);
        }

        private static W.Paragraph LeftParagraph(params OpenXmlElement[] content)
        {
            return BlockParagraph(JustificationValues.Left, "0", content);
        }

        private static W.Paragraph BlockParagraph(JustificationValues justification, string spacingAfter, params OpenXmlElement[] content)
        {
            var paragraphProperties = new W.ParagraphProperties(
                new W.SpacingBetweenLines { After = spacingAfter, Line = "276", LineRule = LineSpacingRuleValues.AtLeast },
                new W.Justification { Val = justification });

            var paragraph = new W.Paragraph();
            paragraph.Append(paragraphProperties);
            foreach (var element in content)
            {
                paragraph.Append(element);
            }
            return paragraph;
        }

        private static W.Table BuildHearingsTable(IEnumerable<NameMatchResult> results)
        {
            int[] columnWidths = { 853, 4230, 2070, 1620, 900, 900, 924 };

            var table = new W.Table();
            table.AppendChild(new W.TableProperties(
                new W.TableWidth { Width = "11497", Type = TableWidthUnitValues.Dxa },
                new W.TableIndentation { Width = -323, Type = TableWidthUnitValues.Dxa },
                new W.TableBorders(
                    new W.TopBorder { Val = BorderValues.Single, Size = 8U },
                    new W.LeftBorder { Val = BorderValues.Single, Size = 8U },
                    new W.BottomBorder { Val = BorderValues.Single, Size = 8U },
                    new W.RightBorder { Val = BorderValues.Single, Size = 8U },
                    new W.InsideHorizontalBorder { Val = BorderValues.Single, Size = 8U },
                    new W.InsideVerticalBorder { Val = BorderValues.Single, Size = 8U }),
                new W.TableLayout { Type = TableLayoutValues.Fixed }));

            var tableGrid = new W.TableGrid();
            foreach (int width in columnWidths)
            {
                tableGrid.Append(new W.GridColumn { Width = width.ToString() });
            }
            table.AppendChild(tableGrid);

            // Header row 1: the first four columns span both header rows; "Mode of Appearance" spans the three mode columns
            var headerRow1 = new W.TableRow();
            headerRow1.Append(HeaderCell(columnWidths[0], MergedCellValues.Restart, 1, "Start Time"));
            headerRow1.Append(HeaderCell(columnWidths[1], MergedCellValues.Restart, 1, "Defendant/Party Name", "(Last Name, First Name)"));
            headerRow1.Append(HeaderCell(columnWidths[2], MergedCellValues.Restart, 1, "Case Number"));
            headerRow1.Append(HeaderCell(columnWidths[3], MergedCellValues.Restart, 1, "Event Type/ Duration"));
            headerRow1.Append(HeaderCell(columnWidths[4] + columnWidths[5] + columnWidths[6], null, 3, "Mode of Appearance", "(“X” the appropriate column)"));
            table.Append(headerRow1);

            // Header row 2: continuation of the merged columns plus the three mode columns
            var headerRow2 = new W.TableRow();
            headerRow2.Append(HeaderCell(columnWidths[0], MergedCellValues.Continue, 1));
            headerRow2.Append(HeaderCell(columnWidths[1], MergedCellValues.Continue, 1));
            headerRow2.Append(HeaderCell(columnWidths[2], MergedCellValues.Continue, 1));
            headerRow2.Append(HeaderCell(columnWidths[3], MergedCellValues.Continue, 1));
            headerRow2.Append(HeaderCell(columnWidths[4], null, 1, "Remote", "Zoom"));
            headerRow2.Append(HeaderCell(columnWidths[5], null, 1, "Transport to SJC"));
            headerRow2.Append(HeaderCell(columnWidths[6], null, 1, "In-Person", "CF Court"));
            table.Append(headerRow2);

            foreach (var result in results)
            {
                var dataRow = new W.TableRow();
                dataRow.Append(DataCell(columnWidths[0], JustificationValues.Center, result.Start));
                dataRow.Append(DataCell(columnWidths[1], JustificationValues.Left, result.JailName));
                dataRow.Append(DataCell(columnWidths[2], JustificationValues.Left, result.CourtCase));
                dataRow.Append(DataCell(columnWidths[3], JustificationValues.Left, result.EventType));
                dataRow.Append(DataCell(columnWidths[4], JustificationValues.Center, result.Mode == "Zoom" ? "X" : string.Empty));
                dataRow.Append(DataCell(columnWidths[5], JustificationValues.Center, result.Mode == "Transport" ? "X" : string.Empty));
                dataRow.Append(DataCell(columnWidths[6], JustificationValues.Center, result.Mode == "InPerson" ? "X" : string.Empty));
                table.Append(dataRow);
            }

            return table;
        }

        private static W.TableCell HeaderCell(int widthDxa, MergedCellValues? verticalMerge, int gridSpan, params string[] lines)
        {
            var cellProperties = new W.TableCellProperties();
            cellProperties.Append(new W.TableCellWidth { Width = widthDxa.ToString(), Type = TableWidthUnitValues.Dxa });
            if (gridSpan > 1) cellProperties.Append(new W.GridSpan { Val = gridSpan });
            if (verticalMerge.HasValue) cellProperties.Append(new W.VerticalMerge { Val = verticalMerge.Value });
            cellProperties.Append(new W.TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });

            var cell = new W.TableCell();
            cell.Append(cellProperties);

            if (lines == null || lines.Length == 0)
            {
                cell.Append(new W.Paragraph(new W.ParagraphProperties(
                    new W.SpacingBetweenLines { After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto })));
            }
            else
            {
                foreach (string line in lines)
                {
                    cell.Append(new W.Paragraph(
                        new W.ParagraphProperties(
                            new W.SpacingBetweenLines { After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto },
                            new W.Justification { Val = JustificationValues.Center }),
                        TnrRun(line, 20)));
                }
            }
            return cell;
        }

        private static W.TableCell DataCell(int widthDxa, JustificationValues justification, string text)
        {
            var cellProperties = new W.TableCellProperties(
                new W.TableCellWidth { Width = widthDxa.ToString(), Type = TableWidthUnitValues.Dxa });

            var paragraph = new W.Paragraph(
                new W.ParagraphProperties(
                    new W.SpacingBetweenLines { After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto },
                    new W.Justification { Val = justification }),
                TnrRun(text ?? string.Empty, 20));

            return new W.TableCell(cellProperties, paragraph);
        }

        private static void AppendFooterAndSection(MainDocumentPart mainPart, W.Body body)
        {
            FooterPart footerPart = mainPart.AddNewPart<FooterPart>();
            footerPart.Footer = new W.Footer(
                new W.Paragraph(
                    new W.ParagraphProperties(new W.Justification { Val = JustificationValues.Right }),
                    new W.Run(
                        new W.RunProperties(new W.FontSize { Val = "16" }, new W.FontSizeComplexScript { Val = "16" }),
                        new W.Text(DateTime.Now.ToString("M/d/yyyy h:mm:ss tt")) { Space = SpaceProcessingModeValues.Preserve })));
            footerPart.Footer.Save();

            var sectionProperties = new W.SectionProperties(
                new W.FooterReference { Type = HeaderFooterValues.Default, Id = mainPart.GetIdOfPart(footerPart) },
                new W.PageSize { Width = 12240U, Height = 15840U },
                new W.PageMargin { Top = 720, Right = 720U, Bottom = 720, Left = 720U, Header = 720U, Footer = 343U, Gutter = 0U });
            body.Append(sectionProperties);
        }
    }
}