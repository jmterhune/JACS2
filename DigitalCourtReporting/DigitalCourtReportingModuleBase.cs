using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace tjc.Modules.DigitalCourtReporting
{
    public class DigitalCourtReportingModuleBase : PortalModuleBase
    {
        #region Properties
        private readonly INavigationManager _navigationManager;
        public DigitalCourtReportingModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        public int RequestId
        {
            get
            {
                var qs = Request.QueryString["rid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }

        }
        public int ProceedingId
        {
            get
            {
                var qs = Request.QueryString["pid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }

        }
        public Components.SearchTypes SearchType
        {
            get
            {
                var qs = Request.QueryString["searchType"];
                if (qs != null)
                    return (Components.SearchTypes)Convert.ToInt32(qs);
                return Components.SearchTypes.caseName;
            }

        }
        public Components.ListTypes ListType
        {
            get
            {
                var qs = Request.QueryString["listType"];
                if (qs != null)
                    return (Components.ListTypes)Convert.ToInt32(qs);
                return Components.ListTypes.payment;
            }

        }
        public int CountyId
        {
            get
            {
                var qs = Request.QueryString["cid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return 0;
            }
        }
        public bool IsInquiry
        {
            get
            {
                var qs = Request.QueryString["inquiry"];
                if (qs != null)
                    return true;
                return false;
            }
        }
        public string SearchText
        {
            get
            {
                var qs = Request.QueryString["searchText"];
                if (qs != null)
                    if (qs.ToString() != "null")
                        return qs.ToString();
                return "null";
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
        public string DeSotoReporterEmail
        {
            get
            {
                if (Settings.Contains("DeSotoReportingEmail"))
                    return Settings["DeSotoReportingEmail"].ToString();

                return "dcrgrpman@jud12.flcourts.org";
            }
        }
        public string ManateeReporterEmail
        {
            get
            {
                if (Settings.Contains("ManateeReportingEmail"))
                    return Settings["ManateeReportingEmail"].ToString();

                return "dcrgrpman@jud12.flcourts.org";
            }
        }
        public string SarasotaReporterEmail
        {
            get
            {
                if (Settings.Contains("SarasotaReportingEmail"))
                    return Settings["SarasotaReportingEmail"].ToString();

                return "dcrgrpsar@jud12.flcourts.org";
            }
        }
        #endregion

        #region NavigationURLs
        public string AccountingUrl { get { return _navigationManager.NavigateURL(); } }
        public string NotificationUrl { get { return _navigationManager.NavigateURL("", "listtype=1"); } }
        public string InquiryUrl { get { return _navigationManager.NavigateURL("", "listtype=4", "inquiry=1"); } }
        public string InquiryDeSotoUrl { get { return _navigationManager.NavigateURL("", "listtype=4", "cid=1"); } }
        public string InquiryManateeUrl { get { return _navigationManager.NavigateURL("", "listtype=4", "cid=3"); } }
        public string InquirySarasotaUrl { get { return _navigationManager.NavigateURL("", "listtype=4", "cid=2"); } }
        public string DCRUrl { get { return _navigationManager.NavigateURL("", "listtype=2"); } }
        public string DCRDeSotoUrl { get { return _navigationManager.NavigateURL("", "listtype=2", "cid=1"); } }
        public string DCRManateeUrl { get { return _navigationManager.NavigateURL("", "listtype=2", "cid=3"); } }
        public string DCRSarasotaUrl { get { return _navigationManager.NavigateURL("", "listtype=2", "cid=2"); } }
        public string CompleteUrl { get { return _navigationManager.NavigateURL("", "listtype=3"); } }
        public string CompleteDeSotoUrl { get { return _navigationManager.NavigateURL("", "listtype=3", "cid=1"); } }
        public string CompleteManateeUrl { get { return _navigationManager.NavigateURL("", "listtype=3", "cid=3"); } }
        public string CompleteSarasotaUrl { get { return _navigationManager.NavigateURL("", "listtype=3", "cid=2"); } }
        public string SearchUrl { get { return EditUrl("search"); } }
        public string StatsUrl { get { return EditUrl("stats"); } }
        #endregion
    }
}