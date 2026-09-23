/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Web;
using tjc.Modules.Arbitrators.Components;

namespace tjc.Modules.Arbitrators.Handlers
{
    /// <summary>
    /// Streams an arbitrator application attachment (stored as a DB blob in
    /// tjc_arbitrators_attachments, on the jud12.flcourts.org database) to a
    /// logged-in reviewer on this intranet site. Same
    /// authenticated-user-only pattern as the public Arbitrators module's
    /// counterpart handler (Process-Server/FileViewer.ashx.cs) in tjc.Modules.
    /// </summary>
    public class AttachmentViewer : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                if (!context.Request.IsAuthenticated)
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                int attachmentId;
                if (!int.TryParse(context.Request.QueryString["id"], out attachmentId) || attachmentId <= 0)
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                IHostSettings hostSettings = context.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();
                var ctl = new ArbitratorAttachmentController(hostSettings);
                ArbitratorAttachment attachment = ctl.GetAttachment(attachmentId);
                if (attachment == null || attachment.FileData == null)
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                context.Response.Clear();
                context.Response.ContentType = string.IsNullOrEmpty(attachment.ContentType)
                    ? "application/octet-stream"
                    : attachment.ContentType;
                context.Response.AddHeader("content-disposition", "inline; filename=" + Uri.EscapeDataString(attachment.FileName ?? "attachment"));
                context.Response.BinaryWrite(attachment.FileData);
                context.Response.Flush();
                context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
                context.Response.StatusCode = 500;
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
