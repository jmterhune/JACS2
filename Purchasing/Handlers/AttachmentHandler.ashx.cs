using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using System;
using System.Collections;
using System.Net.Mail;
using System.Web;
using tjc.Modules.Purchasing.Components;

namespace tjc.Modules.Purchasing.Handlers
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class AttachmentHandler : IHttpHandler
    {
        private int _moduleId;
        private int _portalId;
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.Files.Count > 0)
            {
                HttpFileCollection files = context.Request.Files;
                HttpPostedFile file = files[0];
                int fileId = 0;
                string moduleIdString = context.Request.Params["mid"];
                _moduleId = Convert.ToInt32(moduleIdString);
                try
                {
                    fileId = InsertAttachment(file);
                }
                catch (Exception ex)
                {
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                }
                context.Response.ContentType = "text/plain";
                context.Response.Write(fileId.ToString());
            }
            else
            {
                string fileIdParameter = context.Request.Params["fid"];
                if (!string.IsNullOrEmpty(fileIdParameter))
                {
                    try
                    {
                        int fileId = System.Convert.ToInt32(fileIdParameter);
                        DotNetNuke.Services.FileSystem.FileManager objFile = new DotNetNuke.Services.FileSystem.FileManager();
                        objFile.DeleteFile(objFile.GetFile(fileId));
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("");
                    }
                    catch (Exception ex)
                    {
                        context.Response.ContentType = "text/plain";
                        context.Response.Write("Error Attempting Delete");
                        DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                    }
                }
            }
        }
        private int InsertAttachment(HttpPostedFile file)
        {
            ModuleController moduleController = new ModuleController();
            ModuleInfo modCtl = moduleController.GetModule(_moduleId);
            Hashtable setting = modCtl.TabModuleSettings;
            string attachmentFolder = "Purchasing-Attachments";
            if (setting.Contains("AttachmentFolderName"))
            {
                attachmentFolder = setting["AttachmentFolderName"].ToString();
            }
            DotNetNuke.Services.FileSystem.FolderManager objFolder = new DotNetNuke.Services.FileSystem.FolderManager();
            DotNetNuke.Services.FileSystem.FileManager objFile = new DotNetNuke.Services.FileSystem.FileManager();
            DotNetNuke.Services.FileSystem.IFolderInfo folderInfo = null;
            if (objFolder.FolderExists(_portalId, attachmentFolder) == false)
            {
                folderInfo = objFolder.AddFolder(_portalId, attachmentFolder);
            }
            else
            {
                folderInfo=objFolder.GetFolder(_portalId, attachmentFolder);
            }
            string fileName= string.Format("{0}-{1}",DateTime.Now.ToString("MM-dd-yy-HH"), file.FileName);
            DotNetNuke.Services.FileSystem.IFileInfo fileInfo = objFile.AddFile(folderInfo, fileName, file.InputStream);
            return fileInfo.FileId;
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