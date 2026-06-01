using System;
using System.Web;
using tjc.Modules.ThreatReport.Components;

namespace tjc.Modules.ThreatReport
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class Handler1 : IHttpHandler
    {
        private AttachmentController ctl = new AttachmentController();
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                int id = Int32.Parse(context.Request.QueryString["id"].ToString());

                Attachment attachment = ctl.GetAttachment(id);
                response.ClearContent();
                response.Clear();
                response.ContentType = "application/pdf";
                response.AddHeader("Content-Disposition",
                                   "attachment; filename=" + attachment.FileName + ";");
                response.TransmitFile(System.IO.Path.Combine(attachment.Path, attachment.FileName));
                response.Flush();
                response.End();

            }
            catch (Exception exc)
            {

                context.Response.Write(" <html><body><h1>Error Processing File</h2><p>An error occurred processing the requested file.</p><div style='color:red'>");
                context.Response.Write(exc.Message);
                context.Response.Write("</div></body></html>");
                context.Response.Flush();
                context.Response.End();
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