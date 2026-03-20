using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("courtrooms")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Courtrooms", CacheItemPriority.Default, 20)]
    internal class Courtroom
    {
        public long id { get; set; }
        public string old_id { get; set; }
        public string description { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

    [TableName("courtroom_clerk_xref")]
    internal class CourtroomClerkXref
    {
        public long courtroom_id { get; set; }
        public long county_id { get; set; }
        public long clerk_courtroom_id { get; set; }
        public string clerk_courtroom_name { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
    internal class CourtroomClerkXrefListItem: CourtroomClerkXref
    {
        public string county_name { get; set; } = null;
        public string courtroom_name { get; set; } = null;
    }
}
