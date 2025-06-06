using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("mediation_case_event_payments")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("MediationCaseEventPayments", CacheItemPriority.Default, 20)]
    internal class MediationCaseEventPayment
    {
        public long id { get; set; }
        public long? p_c_id { get; set; }
        public long? p_e_id { get; set; }
        public decimal? amount_paid { get; set; }
        public string paid_by { get; set; }
        public DateTime? paid_on { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}