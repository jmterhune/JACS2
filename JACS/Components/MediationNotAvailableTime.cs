using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("mediation_not_avail_times")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("MediationNotAvailableTimes", CacheItemPriority.Default, 20)]
    internal class MediationNotAvailableTime
    {
        public long id { get; set; }
        public long? Dd_med { get; set; }
        public string Dd_time { get; set; }
        public DateTime? Tb_sdate { get; set; }
        public DateTime? Tb_edate { get; set; }
        public short? at_weekday { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}