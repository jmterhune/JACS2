using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("court_types")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("CourtTypes", CacheItemPriority.Default, 20)]
    internal class CourtType
    {
        public long id { get; set; }
        public string old_id { get; set; }
        public string description { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}