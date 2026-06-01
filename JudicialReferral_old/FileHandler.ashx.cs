using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using tjc.Modules.JudicialReferral.Components;

namespace tjc.Modules.JudicialReferral
{
    /// <summary>
    /// Summary description for FileManager
    /// </summary>
    public class FileManager : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<string> errorMessage = new List<string>();
            var portalsettings = DotNetNuke.Common.Globals.GetPortalSettings();

            string aid = context.Request.QueryString["aid"];

            // Check if 'aid' is present and valid
            if (!string.IsNullOrEmpty(aid))
            {
                try
                {
                    int.TryParse(aid, out int id);
                    DeleteAttachment(id);
                }
                catch (Exception ex)
                {
                    errorMessage.Add("Unable to delete file");
                    Exceptions.LogException(ex);
                }
                context.Response.ContentType = "application/json";
                System.Web.Script.Serialization.JavaScriptSerializer jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var returnValues = jsonSerializer.Serialize(
                      new
                      {
                          idList = aid,
                          errorList = errorMessage,
                      });
                context.Response.Write(returnValues);
            }
            else if (context.Request.Files.Count > 0)
            {
                int moduleId = System.Convert.ToInt32(context.Request.Params["mid"]);
                int tabId = System.Convert.ToInt32(context.Request.Params["tid"]);
                var module = DotNetNuke.Entities.Modules.ModuleController.Instance.GetModule(moduleId, tabId, true);
                string targetFolder = "Judicial-Referral-Attachments";
                if (module.ModuleSettings.ContainsKey("FolderName"))
                {
                    targetFolder = module.ModuleSettings["FolderName"].ToString();
                }

                HttpFileCollection files = context.Request.Files;

                List<int> attachmentIds = new List<int>();
                foreach (string key in files)
                {
                    HttpPostedFile file = files[key];
                    try
                    {
                        int attachmentId = InsertAttachment(file, portalsettings.PortalId, targetFolder);
                        attachmentIds.Add(attachmentId);
                    }
                    catch (Exception ex)
                    {
                        errorMessage.Add(string.Format("Upload of {0} failed", file.FileName));
                        Exceptions.LogException(ex);
                    }
                }

                context.Response.ContentType = "application/json";
                context.Response.Write(GetJsonReturnValue(attachmentIds, errorMessage));

            }
            else
            {
                context.Response.ContentType = "application/json";
                context.Response.Write(GetJsonReturnValue(null, errorMessage));
            }
        }
        public int InsertAttachment(HttpPostedFile file, int portalId, string targetFolder)
        {
            var ctl = new AttachmentController();
            int attachmentId = 0;
            string extension = Path.GetExtension(file.FileName);
            var extensionList = new List<string> { ".docx", ".doc", ".pdf", ".xls", ".xlsx" };
            bool validExtension = extensionList.Contains(extension, StringComparer.OrdinalIgnoreCase);
            if (validExtension)
            {
                try
                {

                    var folder = DotNetNuke.Services.FileSystem.FolderManager.Instance.GetFolder(portalId, targetFolder);
                    var fileInfo = DotNetNuke.Services.FileSystem.FileManager.Instance.AddFile(folder, file.FileName, file.InputStream);
                    Attachment uf = new Attachment
                    {
                        FileName = file.FileName,
                        FileID = fileInfo.FileId,
                        Path = DotNetNuke.Services.FileSystem.FileManager.Instance.GetUrl(fileInfo)
                    };
                    ctl.CreateAttachment(uf);
                    attachmentId = uf.AttachmentID;
                }
                catch (Exception ex)
                {
                    Exceptions.LogException(ex);
                }
            }

            return attachmentId;
        }
        public void DeleteAttachment(int attachmentId)
        {
            var ctl = new AttachmentController();
            Attachment attachment = ctl.GetAttachment(attachmentId);
            if (attachment != null)
            {
                var file = DotNetNuke.Services.FileSystem.FileManager.Instance.GetFile(attachment.FileID);
                if (file != null)
                {
                    DotNetNuke.Services.FileSystem.FileManager.Instance.DeleteFile(file);
                }
                ctl.DeleteAttachment(attachmentId);
            }
        }
        private string GetJsonReturnValue(List<int> attachmentIds, List<string> errorMessage)
        {
            System.Web.Script.Serialization.JavaScriptSerializer jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            return jsonSerializer.Serialize(
                  new
                  {
                      idList = attachmentIds,
                      errorList = errorMessage,
                  });

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