using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.CourtCounsel.Components
{
    public class UrlQueryParameters
    {
        const int maxPageSize = 50;
        private int _pageSize = 20;
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
        public bool IncludeCount { get; set; } = false;
    }

    public class SearchQueryParameters
    {

        public string UserName { get; set; } = null;
        public string CaseName { get; set; } = null;
        public string CaseNumber { get; set; } = null;
        public string FileName { get; set; } = null;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ReportQueryParameters
    {
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.Today;
        public int Status { get; set; } = -1;
        public int County { get; set; } = 0;
        public int Requestor { get; set; } = 0;
        public int Phase { get; set; } = 0;
        public int Details { get; set; } = 0;
        public string AttorneyList { get; set; }
    }
}
