using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("mediation_outcome")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("MediationOutcomes", CacheItemPriority.Default, 20)]
    internal class MediationOutcome
    {
        public long id { get; set; }
        public string o_outcome { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}