using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("revisions")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Revisions", CacheItemPriority.Default, 20)]
    internal class Revision
    {
        public long id { get; set; }
        public string revisionable_type { get; set; }
        public long revisionable_id { get; set; }
        public long? user_id { get; set; }
        public string user_type { get; set; }
        public string key { get; set; }
        public string old_value { get; set; }
        public string new_value { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}