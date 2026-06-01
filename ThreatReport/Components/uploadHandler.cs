using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
namespace tjc.Modules.ThreatReport.Components
{
    public class UploadHandler : IHttpHandler
    {

        /// <summary>
        /// You will need to configure this handler in the Web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: https://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var ctl = new AttachmentController();
            if (context.Request.Files.Count > 0)
            {
                var portalsettings = DotNetNuke.Common.Globals.GetPortalSettings();
                string username = context.User.Identity.Name;
                if (username == "")
                {
                    context.Response.ContentType = "application/json";
                    context.Response.Write(GetJsonReturnValue(0, "File Rejected. You must be Logged in to Upload Files"));
                    return;
                }
                UserInfo objuser = UserController.GetUserByName(portalsettings.PortalId, context.User.Identity.Name);

                if (objuser != null && objuser.UserID > 0)
                {
                    HttpFileCollection files = context.Request.Files;
                    HttpPostedFile file = files[0];
                    string filename = file.FileName;
                    int fileId = 0;
                    int uploadedByUserId = objuser.UserID;
                    int incidentId = System.Convert.ToInt32(context.Request.Params["incidentId"]);
                    int moduleId = System.Convert.ToInt32(context.Request.Params["moduleId"]);
                    try
                    {
                        fileId = InsertAttachment(file, filename, incidentId, moduleId);
                        if (fileId == 0)
                        {
                            context.Response.ContentType = "application/json";
                            context.Response.Write(GetJsonReturnValue(fileId, "File Rejected. Please make sure the file is in one of the expected file formats"));

                        }
                    }
                    catch (Exception ex)
                    {
                        Exceptions.LogException(ex);
                    }
                    System.Web.Script.Serialization.JavaScriptSerializer jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    context.Response.ContentType = "application/json";
                    context.Response.Write(GetJsonReturnValue(fileId, ""));
                }
                else
                {
                    context.Response.ContentType = "application/json";
                    context.Response.Write(GetJsonReturnValue(0, "File Rejected. You must be a Registered User to Upload Files"));
                }
            }
            else
            {
                string fileIdParameter = context.Request.Params["fileId"];
                if (fileIdParameter.Length > 0)
                {
                    try
                    {
                        int fileId = System.Convert.ToInt32(fileIdParameter);
                        var objUploadedFile = ctl.GetAttachment(fileId);
                        bool deleted = DeleteFile(objUploadedFile);
                        ctl.DeleteAttachment(fileId);
                        context.Response.ContentType = "application/json";
                        context.Response.Write(GetJsonReturnValue(fileId, "File Deleted"));
                    }
                    catch (Exception ex)
                    {
                        Exceptions.LogException(ex);
                        context.Response.ContentType = "application/json";
                        context.Response.Write(GetJsonReturnValue(0, "Unexpected error deleting file."));
                    }
                }
            }
        }
        public int InsertAttachment(HttpPostedFile file, string filename, int incidentId, int moduleId)
        {
            var ctl = new AttachmentController();
            ModuleInfo module = new ModuleController().GetModule(moduleId, DotNetNuke.Common.Utilities.Null.NullInteger);
            string rootDirectory = "C:\\websites\\Threats\\Attachments";
            if (module.TabModuleSettings.Contains("AttachmentDirectory"))
            {
                rootDirectory = module.TabModuleSettings["AttachmentDirectory"].ToString();
                if(!rootDirectory.EndsWith("\\"))
                    rootDirectory+="\\";
            }
            int fileId = 0;
            string extension = Path.GetExtension(filename);
            string fullPath = "";
            var strings = new List<string> { ".pdf", ".doc", ".docx", ".txt", ".wpd", ".jpg", ".jpeg" };
            bool validExtension = strings.Contains(extension, StringComparer.OrdinalIgnoreCase);
            if (validExtension)
            {
                try
                {
                    Attachment uf = new Attachment
                    {
                        UploadedDate = DateTime.Now,
                        IncidentID = incidentId,
                        Path = rootDirectory,

                    };
                    ctl.CreateAttachment(uf);
                    fileId = uf.AttachmentID;
                    uf.FileName = incidentId.ToString() + "_" + fileId.ToString() + "_" + filename;
                    ctl.UpdateAttachment(uf);
                    fullPath = rootDirectory + incidentId.ToString() + "_" + fileId.ToString() + "_" + filename.Replace(" ", "_");
                    file.SaveAs(fullPath);
                }
                catch (Exception ex)
                {
                    Exceptions.LogException(ex);
                    if (fileId > 0)
                    {
                        ctl.DeleteAttachment(fileId);
                    }
                }
            }

            return fileId;
        }

        private string GetJsonReturnValue(int fileId, string errorMessage)
        {
            System.Web.Script.Serialization.JavaScriptSerializer jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            return jsonSerializer.Serialize(
                  new
                  {
                      fileId = fileId,
                      error = errorMessage,
                  });

        }
        private bool DeleteFile(Attachment uploadedFile)
        {
            string fileName = uploadedFile.Path;
            try
            {
                if (File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return false;
            }
        }

        #endregion
    }
}
