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
                NameMatchResult result = (NameMatchResult)e.Row.DataItem;
                if (result.Mode == "Zoom")
                {
                    rbZoom.Checked = true;
                }
                else
                {
                    rbTransport.Checked = true;
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
                    RadioButton rbZoom = (RadioButton)row.FindControl("rbZoom");
                    RadioButton rbTransport = (RadioButton)row.FindControl("rbTransport");
                    TextBox txtStart = (TextBox)row.FindControl("txtStart");
                    TextBox txtEventType = (TextBox)row.FindControl("txtEventType");
                    int id = Convert.ToInt32(row.Cells[0].Text);
                    var item = controller.GetItem(id);
                    item.Mode = rbZoom.Checked ? "Zoom" : "Transport";
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
                RadioButton rbZoom = (RadioButton)row.FindControl("rbZoom");
                RadioButton rbTransport = (RadioButton)row.FindControl("rbTransport");
                TextBox txtStart = (TextBox)row.FindControl("txtStart");
                TextBox txtEventType = (TextBox)row.FindControl("txtEventType");
                int id = Convert.ToInt32(row.Cells[0].Text);
                var item = controller.GetItem(id);
                item.Mode = rbZoom.Checked ? "Zoom" : "Transport";
                item.Start = txtStart.Text;
                item.EventType = txtEventType.Text;
                controller.UpdateItem(item);
            }

            // Retrieve from database
            var results = controller.GetItemsBySetGuid(setGuid);

            // Generate Word document using OpenXML
            MemoryStream stream = new MemoryStream();
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Title
                Paragraph titleParagraph = body.AppendChild(new Paragraph());
                Run titleRun = titleParagraph.AppendChild(new Run(new RunProperties(new DocumentFormat.OpenXml.Wordprocessing.Bold(), new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "28" }), new Text("SARASOTA JAIL HEARINGS")));
                DateTime.TryParse(txtDate.Text, out DateTime hearingDate);
                // Form details
                AddParagraph(body, $"Requesting Judge: {txtJudge.Text}", true);
                AddParagraph(body, $"Courtroom: {txtCourtroom.Text}", true);
                AddParagraph(body, $"Date of Hearing: {hearingDate.ToShortDateString()}", true);
                AddParagraph(body, $"Submitted By: {txtSubmittedBy.Text}", true);

                // Spacer
                AddParagraph(body, " ");

                // Table
                DocumentFormat.OpenXml.Wordprocessing.Table table = new DocumentFormat.OpenXml.Wordprocessing.Table();

                TableProperties tableProperties = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single) },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single) },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single) },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single) },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single) },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single) }
                    )
                );
                table.AppendChild(tableProperties);

                // Header row
                DocumentFormat.OpenXml.Wordprocessing.TableRow headerRow = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
                headerRow.Append(AddCell("Start Time", true));
                headerRow.Append(AddCell("Defendant/Party Name", true));
                headerRow.Append(AddCell("Case Number", true));
                headerRow.Append(AddCell("Event Type/Duration", true));
                headerRow.Append(AddCell("Type", true));
                table.Append(headerRow);

                // Data rows
                foreach (var result in results)
                {
                    DocumentFormat.OpenXml.Wordprocessing.TableRow dataRow = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
                    dataRow.Append(AddCell(result.Start));
                    dataRow.Append(AddCell(result.JailName));
                    dataRow.Append(AddCell(result.CourtCase));
                    dataRow.Append(AddCell(result.EventType));
                    string hearingType = result.Mode;
                    dataRow.Append(AddCell(hearingType));
                    table.Append(dataRow);
                }

                body.Append(table);

                // Spacer
                AddParagraph(body, " ");

                // Zoom info
                AddParagraph(body, "Zoom Meeting Information", true);
                AddParagraph(body, $"Meeting ID: {txtZoomID.Text}");
                AddParagraph(body, $"Password: {txtPassword.Text}");

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

        private void AddParagraph(Body body, string text, bool bold = false)
        {
            Paragraph paragraph = body.AppendChild(new Paragraph());
            Run run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text(text));
            if (bold)
            {
                run.RunProperties = new RunProperties(new DocumentFormat.OpenXml.Wordprocessing.Bold());
            }
        }

        private DocumentFormat.OpenXml.Wordprocessing.TableCell AddCell(string text, bool bold = false)
        {
            DocumentFormat.OpenXml.Wordprocessing.TableCell cell = new DocumentFormat.OpenXml.Wordprocessing.TableCell();
            Paragraph paragraph = cell.AppendChild(new Paragraph());
            Run run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text(text));
            if (bold)
            {
                run.RunProperties = new RunProperties(new DocumentFormat.OpenXml.Wordprocessing.Bold());
            }
            return cell;
        }
    }
}