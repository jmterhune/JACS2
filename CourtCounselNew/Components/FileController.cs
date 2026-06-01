/*
' Copyright (c) 2022 Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using DotNetNuke.Entities.Users;
using System.Linq;

namespace tjc.Modules.CourtCounsel.Components
{
    internal class FileController
    {
        
        private const string AuthSystemApplicationName = "Azure";

        public void CreateFile(File t, string casenumber, int portalId)
        {
            //bool fileExists = AsyncHelper.RunSync(() => FileExistsAsync(t, casenumber, portalId));
            //DriveItem driveItem = AsyncHelper.RunSync(() => UploadGraphFileAsync(t, casenumber, portalId));
            //t.Url = driveItem.WebUrl;
            //t.ItemId = driveItem.Id;

            //if (driveItem!=null)
            //    using (IDataContext ctx = DataContext.Instance())
            //    {
            //        var rep = ctx.GetRepository<File>();
            //        if (fileExists && t.FileId>0)
            //        {
            //            UserInfo user = UserController.Instance.GetCurrentUserInfo();
            //            t.ModifiedDate = DateTime.Now;
            //            t.ModifiedBy = user.Username;
            //            rep.Update(t);
            //        }
            //        else { rep.Insert(t); }

            //    }


        }

        public void DeleteFile(long fileId, int portalId)
        {
            var t = GetFile(fileId);
            //bool deleted = AsyncHelper.RunSync(() => DeleteFileAsync(t, portalId));
            DeleteFile(t);
        }

        public void DeleteFile(File t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<File>();
                rep.Delete(t);
            }
        }

        public IEnumerable<File> GetFiles()
        {
            IEnumerable<File> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<File>();
                t = rep.Get();
            }
            return t;
        }

        public IEnumerable<File> GetFilesByAssignment(long assignmentId)
        {
            IEnumerable<File> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<File>();
                t = rep.Find("Where AssignmentId = @0", assignmentId);
            }
            return t;
        }
        public File GetFilesByFileName(long assignmentId, string filename)
        {
            File t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<File>();
                t = rep.Find("Where AssignmentId = @0 And FileName=@1", assignmentId, filename).FirstOrDefault();
            }
            return t;
        }
        public File GetFile(long fileId)
        {
            File t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<File>();
                t = rep.GetById(fileId);
            }
            return t;
        }
        public void UpdateFile(File t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<File>();
                rep.Update(t);
            }
        }
        //public async Task<DriveItem> UploadGraphFileAsync(File file, string casenumber, int portalId)
        //{
        //    try
        //    {

        //        DotNetNuke.Services.Authentication.OAuth.OAuthConfigBase.ClearConfig(AuthSystemApplicationName, portalId);
        //        var config = DotNetNuke.Authentication.Azure.Components.AzureConfig.GetConfig(AuthSystemApplicationName, portalId);
        //        DotNetNuke.Authentication.Azure.Components.Graph.GraphClient graphClient = new DotNetNuke.Authentication.Azure.Components.Graph.GraphClient(config.APIKey, config.APISecret, config.TenantId);
        //        DriveItem item = await graphClient.UploadGraphFileAsync(file.DriveId, file.ParentId, file.FileName, casenumber, file.FileStream);
        //        return item;
        //    }
        //    catch (Exception exc)
        //    {
        //        DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
        //    }
        //    return null;
        //}
        //public async Task<bool> FileExistsAsync(File file, string casenumber, int portalId)
        //{
        //    try
        //    {

        //        DotNetNuke.Services.Authentication.OAuth.OAuthConfigBase.ClearConfig(AuthSystemApplicationName, portalId);
        //        var config = DotNetNuke.Authentication.Azure.Components.AzureConfig.GetConfig(AuthSystemApplicationName, portalId);
        //        DotNetNuke.Authentication.Azure.Components.Graph.GraphClient graphClient = new DotNetNuke.Authentication.Azure.Components.Graph.GraphClient(config.APIKey, config.APISecret, config.TenantId);
        //        bool exists = await graphClient.FileExistsAsync(file.DriveId, file.ParentId, file.FileName);
        //        return exists;
        //    }
        //    catch (Exception exc)
        //    {
        //        DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
        //    }
        //    return false;
        //}
        //public async Task<bool> DeleteFileAsync(File file, int portalId)
        //{
        //    try
        //    {

        //        DotNetNuke.Services.Authentication.OAuth.OAuthConfigBase.ClearConfig(AuthSystemApplicationName, portalId);
        //        var config = DotNetNuke.Authentication.Azure.Components.AzureConfig.GetConfig(AuthSystemApplicationName, portalId);
        //        DotNetNuke.Authentication.Azure.Components.Graph.GraphClient graphClient = new DotNetNuke.Authentication.Azure.Components.Graph.GraphClient(config.APIKey, config.APISecret, config.TenantId);
        //        return await graphClient.DeleteFileAsync(file.DriveId, file.ItemId);
        //    }
        //    catch (Exception exc)
        //    {
        //        DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
        //    }
        //    return false;
        //}

    }
}
