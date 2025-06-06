using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("events")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Events", CacheItemPriority.Default, 20)]
    internal class Event
    {
        public long id { get; set; }
        public string case_num { get; set; }
        public string notes { get; set; }
        public string plaintiff { get; set; }
        public string defendant { get; set; }
        public long? motion_id { get; set; }
        public long? attorney_id { get; set; }
        public long? type_id { get; set; }
        public long? status_id { get; set; }
        public bool reminder { get; set; }
        public long? opp_attorney_id { get; set; }
        public long? owner_id { get; set; }
        public string owner_type { get; set; }
        public bool? addon { get; set; }
        public string plaintiff_email { get; set; }
        public string defendant_email { get; set; }
        public string cancellation_reason { get; set; }
        public string template { get; set; }
        public string telephone { get; set; }
        public string custom_motion { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}