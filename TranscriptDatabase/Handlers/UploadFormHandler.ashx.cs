using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using System;
using System.Collections;
using System.Web;

namespace tjc.Modules.TranscriptDatabase.Handlers
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class UploadFormHandler : IHttpHandler
    {
        private int _moduleId;
        private int _portalId = PortalSettings.Current.PortalId;
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
                    fileId = InsertFile(file);
                }
                catch (Exception ex)
                {
                    DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                }
                context.Response.ContentType = "text/plain";
                context.Response.Write(fileId.ToString());
            }
        }
        private int InsertFile(HttpPostedFile file)
        {
            ModuleController moduleController = new ModuleController();
            ModuleInfo modCtl = moduleController.GetModule(_moduleId);
            Hashtable setting = modCtl.ModuleSettings;
            string uploadFolder = "Transcript-Forms";
            if (setting.Contains("UploadFormFolder"))
            {
                uploadFolder = setting["UploadFormFolder"].ToString();
            }
            DotNetNuke.Services.FileSystem.FolderManager objFolder = new DotNetNuke.Services.FileSystem.FolderManager();
            DotNetNuke.Services.FileSystem.FileManager objFile = new DotNetNuke.Services.FileSystem.FileManager();
            DotNetNuke.Services.FileSystem.IFolderInfo folderInfo = null;
            if (objFolder.FolderExists(_portalId, uploadFolder) == false)
            {
                folderInfo = objFolder.AddFolder(_portalId, uploadFolder);
            }
            else
            {
                folderInfo=objFolder.GetFolder(_portalId, uploadFolder);
            }
            DotNetNuke.Services.FileSystem.IFileInfo fileInfo = objFile.AddFile(folderInfo, file.FileName, file.InputStream);
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