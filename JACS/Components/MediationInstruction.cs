using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("mediation_instructions")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("MediationInstructions", CacheItemPriority.Default, 20)]
    internal class MediationInstruction
    {
        public long id { get; set; }
        public long county_id { get; set; }
        public string instruction { get; set; }
        public long? location_type_id { get; set; }
        public string case_type { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}