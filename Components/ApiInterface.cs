using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("api_interfaces")]
    [PrimaryKey("id", AutoIncrement = false)]
    [Cacheable("apis", CacheItemPriority.Default, 20)]
    internal class ApiInterface
    {
        public long id { get; set; }
        public long county_id { get; set; }
        public string end_point_url { get; set; }
        public ApiInterfaceType type { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
    public enum ApiInterfaceType
    {
        GetCase = 1,
        AddEvent = 2,
        RescheduleEvent = 3,
        UpdateEvent = 4,
        CancelEvent = 5,
        GetEvent = 6,
    }
}