using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("mediation_avail_times")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("MediationTimes", CacheItemPriority.Default, 20)]
    internal class MediationTime
    {
        public long id { get; set; }
        public long? at_m_id { get; set; }
        public string at_time { get; set; }
        public DateTime? at_begin { get; set; }
        public DateTime? at_end { get; set; }
        public bool? at_available { get; set; }
        public short? at_weekday { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
