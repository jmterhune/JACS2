using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("court_restricted_motions")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("CourtRestrictedMotions", CacheItemPriority.Default, 20)]
    internal class CourtRestrictedMotion
    {
        public long id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
