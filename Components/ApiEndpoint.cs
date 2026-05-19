using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Caching;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    [TableName("api_endpoints")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("apis", CacheItemPriority.Default, 20)]
    internal class ApiEndpoint
    {
        public long id { get; set; }
        public long county_id { get; set; }
        public string end_point_url { get; set; }
        public ApiEndpointType type { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        [IgnoreColumn]
        public string type_desc => type.GetDescription();
        [IgnoreColumn]
        public string county_name { get { 
            var county = new CountyController().GetCounty(county_id);
            return county?.name ?? string.Empty;
            } }
    }
    public enum ApiEndpointType
    {
       
        [Description("Create Event")]
        AddEvent = 2,
        [Description("Reschedule Event")]
        RescheduleEvent = 3,
        [Description("Update Event")]
        UpdateEvent = 4,
        [Description("Cancel Event")]
        CancelEvent = 5,
        [Description("Get Case Information")]
        GetCase = 1,
        [Description("Get Event Information")]
        GetEvent = 6,
        [Description("Get Clerk Judges")]
        GetClerkJudges = 7,
        [Description("Get Clerk Courtrooms")]
        GetClerkCourtrooms = 8,
        // others as needed
    }
    internal class ApiEndpointSearchResult
    {
        public List<ApiEndpointViewModel> data { get; set; }
        public string error { get; set; }
    }

    internal class ApiEndpointResult
    {
        public ApiEndpoint data { get; set; }
        public string error { get; set; }
    }

}