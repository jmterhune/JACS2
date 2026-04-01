using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("motions")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Motions", CacheItemPriority.Default, 20)]
    internal class Motion
    {
        public long id { get; set; }
        public string old_id { get; set; }
        public string description { get; set; }
        public int lag { get; set; }
        public int lead { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

    [TableName("motion_clerk_xref")]
    internal class MotionClerkXref
    {
        public long motion_id { get; set; }
        public long county_id { get; set; }
        public long clerk_motion_id { get; set; }
        public string clerk_motion_name { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

    internal class MotionClerkXrefListItem : MotionClerkXref
    {
        public string county_name { get; set; } = null;
        public string motion_name { get; set; } = null;
    }
    internal class MotionXrefItem
    {
        public long EventTypeId { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
    }
}