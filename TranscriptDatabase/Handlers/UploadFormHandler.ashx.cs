using DotNetNuke.Common.Utilities;
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
            // ModuleController.Instance returns IModuleController, which only exposes the
            // (moduleId, tabId, ignoreCache) overload of GetModule. The concrete ModuleController's
            // single-arg GetModule(moduleId) is just a thin wrapper around that overload using
            // Null.NullInteger for tabId, so this reproduces the exact same lookup.
            ModuleInfo modCtl = ModuleController.Instance.GetModule(_moduleId, Null.NullInteger, false);
            Hashtable setting = modCtl.ModuleSettings;
            string uploadFolder = "Transcript-Forms";
            if (setting.Contains("UploadFormFolder"))
            {
                uploadFolder = setting["UploadFormFolder"].ToString();
            }
            DotNetNuke.Services.FileSystem.IFolderManager objFolder = DotNetNuke.Services.FileSystem.FolderManager.Instance;
            DotNetNuke.Services.FileSystem.IFileManager objFile = DotNetNuke.Services.FileSystem.FileManager.Instance;
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