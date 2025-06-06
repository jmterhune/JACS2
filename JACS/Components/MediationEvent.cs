using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("mediation_events")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("MediationEvents", CacheItemPriority.Default, 20)]
    internal class MediationEvent
    {
        public long id { get; set; }
        public long? e_c_id { get; set; }
        public long? e_m_id { get; set; }
        public bool? e_def_failedtoap { get; set; }
        public bool? e_pltf_failedtoap { get; set; }
        public long? e_outcome_id { get; set; }
        public DateTime? e_sch_datetime { get; set; }
        public decimal? e_sch_length { get; set; }
        public decimal? e_med_fee { get; set; }
        public decimal? e_pltf_chg { get; set; }
        public decimal? e_def_chg { get; set; }
        public string e_subject { get; set; }
        public string e_notes { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public double? e_med_per_hr { get; set; }
    }
}