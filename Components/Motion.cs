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
}