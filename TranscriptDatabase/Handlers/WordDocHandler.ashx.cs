using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DotNetNuke.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using tjc.Modules.TranscriptDatabase.Components;
namespace tjc.Modules.TranscriptDatabase.Handlers
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class WordDocHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            Designation designation;
            DesignationController ctl = new DesignationController();
            DotNetNuke.Entities.Users.UserInfo currUser = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
            DocumentTypes formType = 0;
            string designationString = context.Request.Params["did"];
            string extensionDateString = context.Request.Params["date"];
            string reason = context.Request.Params["reason"];
            string formTypeString = context.Request.Params["type"];

            if (!string.IsNullOrEmpty(designationString))
            {
                if (Int32.TryParse(formTypeString, out int formTypeId))
                    formType = (DocumentTypes)formTypeId;
                int designationId = Int32.Parse(designationString);
                designation = ctl.GetDesignation(designationId);
                if (designation != null)
                {
                    DateTime extensionDate = Null.NullDate;
                    if (!string.IsNullOrEmpty(extensionDateString))
                        DateTime.TryParse(extensionDateString, out extensionDate);
                    var aCtl = new AttorneyController();
                    var fCtl = new FormController();
                    IEnumerable<Attorney> attorneys = aCtl.GetAttorneysByDesignation(designation.DesignationID);
                    DocumentDataExport documentDataExport = new DocumentDataExport();
                    Components.Form documentForm = fCtl.GetFormByType(formType);
                    string attorneyNames = "";
                    if (attorneys != null && attorneys.Count() > 0)
                    {
                        attorneyNames = String.Join(", ", attorneys.Select(x => x.AttorneyName));
                        documentDataExport.MailType = GetDeliveryType(attorneys.FirstOrDefault().OfficeID);
                    }
                    else
                    {
                        documentDataExport.MailType = "Unknown. No Attorney Selected";
                    }
                    int days = (extensionDate - designation.DueDate.Value).Days;
                    string daysText = "";
                    documentDataExport.CaseNumber = designation.LowerTribunalCaseNumber;
                    documentDataExport.DCACaseNumber = designation.AppellateCaseNumber;
                    documentDataExport.County = designation.County.ToUpper();
                    documentDataExport.CircuitCounty = GetCircuitCounty(designation.LowerTribunalCaseNumber);
                    documentDataExport.CourtReporter = currUser.FirstName + " " + currUser.LastName;
                    documentDataExport.CreatedDate = DateTime.Today.ToShortDateString();
                    documentDataExport.DateReceived = designation.ReceiptDate.Value.ToShortDateString();
                    documentDataExport.DaysDesignated = designation.TrialHearingDays.ToString();
                    documentDataExport.Defendant = designation.DisplayName;
                    documentDataExport.DesignatingAttorney = attorneyNames;
                    documentDataExport.EstimatedPages = designation.EstimatedPages(0).ToString();
                    if (days > 0)
                        daysText = "The Court Reporter requests an extension of " + days.ToString() + " days";
                    else
                        daysText = "The Court Reporter requests no extension";
                    documentDataExport.ExtensionDays = daysText;
                    documentDataExport.TranscriptFiled = extensionDate.ToShortDateString();
                    byte[] documentBytes = GenerateWordDocument(HttpContext.Current.Server.MapPath(documentForm.FilePath), documentDataExport);
                    string fileName = documentForm.FileName;
                    context.Response.Clear();
                    context.Response.ContentType =
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    context.Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
                    context.Response.BinaryWrite(documentBytes);
                    context.Response.End();
                }
                else
                    context.Response.Write("<h2>Invalid Event Record</h2><p>Return to the previous page and try again</p>");
            }
        }
        private byte[] GenerateWordDocument(string templatePath, DocumentDataExport data)
        {
            byte[] templateBytes = File.ReadAllBytes(templatePath);

            using (var memStream = new MemoryStream())
            {
                memStream.Write(templateBytes, 0, templateBytes.Length);

                using (var wordDoc = WordprocessingDocument.Open(memStream, true))
                {
                    var body = wordDoc.MainDocumentPart.Document.Body;
                    var bookmarks = body.Descendants<BookmarkStart>();

                    var props = typeof(DocumentDataExport).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var bookmark in bookmarks)
                    {
                        string name = bookmark.Name;
                        var prop = props.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                        if (prop == null) continue;

                        string value = prop.GetValue(data)?.ToString() ?? "";

                        // Remove existing text after the bookmark if it exists
                        var current = bookmark.NextSibling<Run>();
                        if (current != null)
                        {
                            current.Remove();
                        }

                        // Create RunProperties for Times New Roman, 14pt (28 half-points)
                        RunProperties runProps = new RunProperties(
                            new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                            new FontSize { Val = "28" },                   // 14 pt
                            new FontSizeComplexScript { Val = "28" }       // For non-ASCII scripts if needed
                        );

                        // Create a new run with the value and styling
                        Run run = new Run();
                        run.Append(runProps);
                        run.Append(new Text(value) { Space = SpaceProcessingModeValues.Preserve });

                        // Insert new run
                        bookmark.Parent.InsertAfter(run, bookmark);
                    }

                    wordDoc.MainDocumentPart.Document.Save();
                }

                return memStream.ToArray();
            }
        }
        private string GetCircuitCounty(string caseNumber)
        {
            if (caseNumber.ToUpper().Contains("CF") | caseNumber.ToUpper().Contains("DP") | caseNumber.ToUpper().Contains("CJ"))
                return "CIRCUIT";
            else if (caseNumber.ToUpper().Contains("MM") | caseNumber.ToUpper().Contains("CT"))
                return "COUNTY";
            else
                return "CIRCUIT";
        }
        private string GetDeliveryType(int officeId)
        {
            var ctl = new OfficeController();
            Components.Office office = ctl.GetOffice(officeId) ?? new Office();
            switch (office.DeliveryType)
            {
                case DeliveryTypes.Interoffice:
                    {
                        return "Interoffice";
                    }

                case DeliveryTypes.UsPostage:
                    {
                        return "U.S. Postage";
                    }

                default:
                    {
                        return "";
                    }
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}