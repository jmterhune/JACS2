using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using Microsoft.Extensions.DependencyInjection;
using System;
using tjc.Modules.TranscriptDatabase.Handlers;
namespace tjc.Modules.TranscriptDatabase
{
    public class TranscriptDatabaseModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public TranscriptDatabaseModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
        }
        public int DesignationId
        {
            get
            {
                var qs = Request.QueryString["did"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public string ErrorMessage
        {
            get
            {
                var qs = Request.QueryString["error"];
                if (qs != null)
                    return qs.ToString();
                return string.Empty;
            }
        }
        public bool IsAdmin
        {
            get
            {
                if (UserInfo.IsInRole(AdminRole))
                    return true;
                return false;
            }
        }
        public string AdminRole
        {
            get
            {
                if (Settings.Contains("AdminRole"))
                    return Settings["AdminRole"].ToString();

                return "Court Reporting Manager";
            }
        }
        public string CourtReporterRole
        {
            get
            {
                if (Settings.Contains("CourtReporterRole"))
                    return Settings["CourtReporterRole"].ToString();

                return "Court Reporter";
            }
        }
        public string CourtReporterIntakeRole
        {
            get
            {
                if (Settings.Contains("CourtReporterIntakeRole"))
                    return Settings["CourtReporterIntakeRole"].ToString();

                return "Court Reporter Intake";
            }
        }
        public string  MessageFormat { get{ return "<div class=\"{1} alert-dismissible\" role=\"alert\"><button aria-label=\"Close\" class=\"close\" data-dismiss=\"alert\" type=\"button\"><span aria-hidden=\"true\">&times;</span></button><i class=\"{2}\"></i> {0}</div>"; } }
        public string UploadFormFolder
        {
            get
            {
                if (Settings.Contains("UploadFormFolder"))
                    return Settings["UploadFormFolder"].ToString();

                return "Transcript-Forms";
            }
        }
        public string UploadAttachmentFolder
        {
            get
            {
                if (Settings.Contains("UploadAttachmentFolder"))
                    return Settings["UploadAttachmentFolder"].ToString();

                return "Transcript-Attachments";
            }
        }
        public DateTime CurrentDate
        {
            get
            {
                if (ViewState["CurrentDate"] != null)
                {
                    return DateTime.Parse(ViewState["CurrentDate"].ToString());
                }
                return DateTime.Now;
            }
            set
            {
                ViewState["CurrentDate"] = value;
            }

        }
        public string UploadHandler
        {
            get
            {
                return string.Format("{0}/Handlers/UploadAttachmentHandler.ashx", TemplateSourceDirectory);
            }
        }
        public string FormUploadHandler
        {
            get
            {
                return string.Format("{0}/Handlers/UploadFormHandler.ashx", TemplateSourceDirectory);
            }
        }
        public string DesignationListUrl { get { return _navigationManager.NavigateURL(); } }
        public string CalendartUrl { get { return EditUrl("calendar"); } }
        public string AttorneyListUrl { get { return EditUrl("attorney"); } }
        public string NamesListUrl { get { return EditUrl("name"); } }
        public string OfficeListUrl { get { return EditUrl("office"); } }
        public string FormListUrl { get { return EditUrl("form"); } }
        public string HearingListUrl { get { return EditUrl("hearing"); } }
        public string ReportListUrl { get { return EditUrl("report"); } }
    }
}