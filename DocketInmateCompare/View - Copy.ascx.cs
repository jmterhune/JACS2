// File: View.ascx.cs
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
                if (!IsPostBack && Session["CurrentSetGuid"] != null)
                {
                    var controller = new NameMatchResultController();
                    var setGuid = (Guid)Session["CurrentSetGuid"];
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
                Session["CurrentSetGuid"] = setGuid;

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
                DropDownList ddlMode = (DropDownList)e.Row.FindControl("ddlMode");
                NameMatchResult result = (NameMatchResult)e.Row.DataItem;
                ddlMode.SelectedValue = result.Mode;

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
                    DropDownList ddlMode = (DropDownList)row.FindControl("ddlMode");
                    TextBox txtStart = (TextBox)row.FindControl("txtStart");
                    TextBox txtEventType = (TextBox)row.FindControl("txtEventType");
                    int id = Convert.ToInt32(row.Cells[0].Text);
                    var item = controller.GetItem(id);
                    item.Mode = ddlMode.SelectedValue;
                    item.Start = txtStart.Text;
                    item.EventType = txtEventType.Text;
                    controller.UpdateItem(item);
                }

                int rowIndex = Convert.ToInt32(e.CommandArgument);
                int idToDelete = Convert.ToInt32(gvMatches.Rows[rowIndex].Cells[0].Text);

                controller.DeleteItem(idToDelete);

                var setGuid = (Guid)Session["CurrentSetGuid"];
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

        protected void btnGeneratePDF_Click(object sender, EventArgs e)
        {
            var controller = new NameMatchResultController();
            var setGuid = (Guid)Session["CurrentSetGuid"];

            // Update modes, start times, and event types from grid
            for (int i = 0; i < gvMatches.Rows.Count; i++)
            {
                GridViewRow row = gvMatches.Rows[i];
                DropDownList ddlMode = (DropDownList)row.FindControl("ddlMode");
                TextBox txtStart = (TextBox)row.FindControl("txtStart");
                TextBox txtEventType = (TextBox)row.FindControl("txtEventType");
                int id = Convert.ToInt32(row.Cells[0].Text);
                var item = controller.GetItem(id);
                item.Mode = ddlMode.SelectedValue;
                item.Start = txtStart.Text;
                item.EventType = txtEventType.Text;
                controller.UpdateItem(item);
            }

            // Retrieve from database
            var results = controller.GetItemsBySetGuid(setGuid);

            // Generate PDF using iTextSharp
            MemoryStream stream = new MemoryStream();
            Document document = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.NORMAL);
            Font fontBig = FontFactory.GetFont(FontFactory.HELVETICA, 12, Font.NORMAL);
            Font fontHeading = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, Font.NORMAL);
            Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);

            // Title
            Paragraph title = new Paragraph("SARASOTA JAIL HEARINGS", fontHeading);
            title.SpacingAfter = 20f;
            document.Add(title);

            // Form details
            document.Add(new Paragraph($"Requesting Judge: {txtJudge.Text}", fontBig));
            document.Add(new Paragraph($"Courtroom: {txtCourtroom.Text}", fontBig));
            document.Add(new Paragraph($"Date of Hearing: {txtDate.Text}", fontBig));
            document.Add(new Paragraph($"Submitted By: {txtSubmittedBy.Text}", fontBig));

            Paragraph spacer = new Paragraph(" ");
            spacer.SpacingAfter = 20f;
            document.Add(spacer);

            // Table
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            float[] columnWidths = new float[] { 11f, 30f, 21f, 28f, 10f };
            table.SetWidths(columnWidths);

            table.AddCell(new PdfPCell(new Phrase("Start Time", boldFont)) { Border = Rectangle.NO_BORDER });
            table.AddCell(new PdfPCell(new Phrase("Defendant/Party Name", boldFont)) { Border = Rectangle.NO_BORDER });
            table.AddCell(new PdfPCell(new Phrase("Case Number", boldFont)) { Border = Rectangle.NO_BORDER });
            table.AddCell(new PdfPCell(new Phrase("Event Type/Duration", boldFont)) { Border = Rectangle.NO_BORDER });
            table.AddCell(new PdfPCell(new Phrase("Type", boldFont)) { Border = Rectangle.NO_BORDER });

            foreach (var result in results)
            {
                table.AddCell(new PdfPCell(new Phrase(result.Start, font)) { Border = Rectangle.NO_BORDER });
                table.AddCell(new PdfPCell(new Phrase(result.JailName, font)) { Border = Rectangle.NO_BORDER });
                table.AddCell(new PdfPCell(new Phrase(result.CourtCase, font)) { Border = Rectangle.NO_BORDER });
                table.AddCell(new PdfPCell(new Phrase(result.EventType, font)) { Border = Rectangle.NO_BORDER });
                string hearingType = result.Mode == "Remote" ? "Zoom" : "Transport";
                table.AddCell(new PdfPCell(new Phrase(hearingType, font)) { Border = Rectangle.NO_BORDER });
            }

            document.Add(table);

            document.Add(spacer);

            // Zoom info
            document.Add(new Paragraph("Zoom Meeting Information", boldFont));
            document.Add(new Paragraph($"Meeting ID: {txtZoomID.Text}", font));
            document.Add(new Paragraph($"Password: {txtPassword.Text}", font));

            document.Close();

            // Delete temp files
            string courtPath = Session["CourtPath"] as string;
            string jailPath = Session["JailPath"] as string;
            if (!string.IsNullOrEmpty(courtPath) && File.Exists(courtPath)) File.Delete(courtPath);
            if (!string.IsNullOrEmpty(jailPath) && File.Exists(jailPath)) File.Delete(jailPath);
            Session.Remove("CourtPath");
            Session.Remove("JailPath");

            // Delete database records
            controller.DeleteItemsBySetGuid(setGuid);
            Session.Remove("CurrentSetGuid");

            string fileName = $"InmateRequestForm_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf";
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
            Response.BinaryWrite(stream.ToArray());
            Response.End();
        }
    }
}