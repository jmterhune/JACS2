using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Web;
using tjc.Modules.ProcessServer.Components;

namespace tjc.Modules.ProcessServer
{
    /// <summary>
    /// Summary description for FileViewer
    /// </summary>
    public class FileViewer : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var hostSettings = context.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();
                var portalsettings = DotNetNuke.Common.Globals.GetPortalSettings(hostSettings);
                UserInfo objuser = UserController.GetUserByName(portalsettings.PortalId, context.User.Identity.Name);

                if (objuser != null && objuser.UserID > 0)
                {
                    string aid = context.Request.QueryString["aid"];
                    Int32.TryParse(aid, out int attachmentId);
                    var ctl = new FileUploadController(hostSettings);
                    UploadedFile attachment = ctl.GetFileUpload(attachmentId);
                    string filename = GetFileName(attachment.FileName);
                    context.Response.Clear();
                    context.Response.ContentType = GetContentType(filename);
                    context.Response.AddHeader("content-disposition", "inline; filename=" + filename);
                    if (attachment.FileData != null)
                        context.Response.BinaryWrite(attachment.FileData);
                    //Response.End() raises a ThreadAbortException by design, which the catch below
                    //then logged on every file view; CompleteRequest ends the request instead,
                    //without aborting the thread
                    context.Response.Flush();
                    context.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception exc)
            {

                Exceptions.LogException( exc);
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        private string GetExtension(string path)
        {
            return System.IO.Path.GetExtension(path);
        }

        private string GetFileName(string path)
        {
            return System.IO.Path.GetFileName(path);
        }

        private string GetContentType(string path)
        {
            string extension = GetExtension(path);
            switch (extension)
            {
                case ".pdf":
                    {
                        return "application/pdf";
                    }

                case ".jpeg":
                    {
                        return "image/jpeg";
                    }

                case ".jpg":
                    {
                        return "image/jpg";
                    }


                default:
                    {
                        return "text/plain";
                    }
            }
        }
    }
}