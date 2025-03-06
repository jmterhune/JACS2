using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Web;
using tjc.Modules.TranscriptDatabase.Components;
namespace tjc.Modules.TranscriptDatabase.Handlers
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class WordDocHandler : IHttpHandler
    {
        private int _moduleId;
        private int _portalId = PortalSettings.Current.PortalId;
        private DocumentTypes docType;
        private Designation designation;
        private DesignationController ctl = new DesignationController();
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.Files.Count > 0)
            {
                DotNetNuke.Entities.Users.UserInfo currUser = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
                DocumentTypes formType = 0;
                string designationString = context.Request.Params["did"];
                string extensionDateString = context.Request.Params["date"];
                string reason = context.Request.Params["reason"];
                string formTypeString = context.Request.Params["type"];
                if (!string.IsNullOrEmpty(designationString))
                {
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
                        string attorneyNames = String.Join(", ", attorneys.Select(x => x.AttorneyName));
                        DocumentDataExport documentDataExport = new DocumentDataExport();
                        Components.Form documentForm = fCtl.GetFormByType(formType);
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
                        documentDataExport.MailType = GetDeliveryType(attorneys.FirstOrDefault().OfficeID);
                        documentDataExport.TranscriptFiled = extensionDate.ToShortDateString();
                        CreateForm(documentDataExport, documentForm);
                    }
                    else
                        context.Response.Write("<h2>Invalid Event Record</h2><p>Return to the previous page and try again</p>");
                }
            }
        }


        private void CreateForm(DocumentDataExport documentDataExport, Components.Form documentForm)
        {
            // Instantiate the OOXml Class
            OfficeOpenXml helper = new OfficeOpenXml();

            // Create the target package
            Package targetPackage = helper.CreateOpenPackage();

            // Set the tempate file path
            // Dim path As String = Request.PhysicalApplicationPath & Resources.Resource.WordTemplate
            string path = documentForm.FilePath;
            // Create the template package
            Package templatePackage = helper.CreateTemplatePackage(path);

            // Copy the template to the target package
            helper.CopyTemplate(targetPackage,ref templatePackage);

            // ***Replace Bookmark Code Start***

            // Create a string array containing the bookmarks in the document
            string[] bookmarks = new string[] { "DCACaseNumber", "CaseNumber", "CircuitCounty", "County", "CourtReporter", "CreatedDate", "DateReceived", "DaysDesignated", "Defendant", "DesignatingAttorney", "EstimatedPages", "ExtensionDays", "MailType", "TranscriptFiled" };

            // Create a string array containing all of the values we need to place in the bookmark fields
            string[] values = GetValues(documentDataExport);

            // Loop through Each Bookmark and add the value to the document
            for (int i = 0; i <= bookmarks.Length - 1; i++)
                helper.ReplaceBookMark(ref targetPackage, bookmarks[i], values[i]);

            // ***Replace Bookmark Code End***

            // Close the package
            helper.ClosePackage(ref targetPackage);

            // Stream the file to the client
            helper.DisplayFile();
        }

        private string[] GetValues(DocumentDataExport objData)
        {
            string[] values = new string[] { objData.DCACaseNumber, objData.CaseNumber, objData.CircuitCounty, objData.County, objData.CourtReporter, objData.CreatedDate, objData.DateReceived, objData.DaysDesignated, objData.Defendant, objData.DesignatingAttorney, objData.EstimatedPages, objData.ExtensionDays, objData.MailType, objData.TranscriptFiled };

            return values;
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

                case  DeliveryTypes.UsPostage:
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