using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using System;
using System.Collections;
using System.Net.Mail;
using System.Web;

namespace tjc.Modules.TranscriptDatabase.Handlers
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class UploadAttachmentHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.Files.Count > 0)
            {
                HttpFileCollection files = context.Request.Files;
                HttpPostedFile file = files[0];
                int fileId = 0;
                string moduleIdString = context.Request.Params["mid"];
                string designationString = context.Request.Params["did"];
                string description = context.Request.Params["des"];
                int moduleId = Convert.ToInt32(moduleIdString);
                int designationId = Convert.ToInt32(designationString);
                int portalId = PortalSettings.Current.PortalId;
                try
                {
                    fileId = InsertFile(file,moduleId,portalId,designationId,description);
                }
                catch (Exception exc)
                {
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
                }
                context.Response.ContentType = "text/plain";
                context.Response.Write(fileId.ToString());
            }
        }
        private int InsertFile(HttpPostedFile file,int moduleId,int portalId,int designationId,string description)
        {
            try
            {
                UserInfo currentUser = UserController.Instance.GetCurrentUserInfo();
                ModuleController moduleController = new ModuleController();
                ModuleInfo modCtl = moduleController.GetModule(moduleId);
                Hashtable setting = modCtl.ModuleSettings;
                string uploadFolder = "Transcript-Attachments";
                if (setting.Contains("UploadAttachmentFolder"))
                {
                    uploadFolder = setting["UploadAttachmentFolder"].ToString();
                }
                DotNetNuke.Services.FileSystem.FolderManager objFolder = new DotNetNuke.Services.FileSystem.FolderManager();
                DotNetNuke.Services.FileSystem.FileManager objFile = new DotNetNuke.Services.FileSystem.FileManager();
                DotNetNuke.Services.FileSystem.IFolderInfo folderInfo = null;
                if (objFolder.FolderExists(portalId, uploadFolder) == false)
                {
                    objFolder.AddFolder(portalId, uploadFolder);
                }
                uploadFolder = string.Format("{0}/{1}", uploadFolder, designationId);
                if (objFolder.FolderExists(portalId, uploadFolder) == false)
                {
                    folderInfo = objFolder.AddFolder(portalId, uploadFolder);
                }
                else
                {
                    folderInfo = objFolder.GetFolder(portalId, uploadFolder);
                }
                DotNetNuke.Services.FileSystem.IFileInfo fileInfo = objFile.AddFile(folderInfo, file.FileName.Replace(";","-").Replace("&", "-").Replace("/", "-").Replace("\\", "-"), file.InputStream);
                
                return fileInfo.FileId;
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
                return -1;
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