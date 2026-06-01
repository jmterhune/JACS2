using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;
namespace tjc.Modules.CourtCounsel.Components.Models
{
    [TableName("tjc_cc_time_spent")]
    [PrimaryKey("TimeSpanId", AutoIncrement = true)]
    [Cacheable("tjc_cc_time_spent", CacheItemPriority.Default, 20)]
    public class TimeSpentInfo
    {
        public int TimeSpanId { get; set; }
        public string TimeSpan { get; set; }
        public bool IsActive { get; set; }
    }
}
