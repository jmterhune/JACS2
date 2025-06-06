using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("mediation_mediators")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Mediators", CacheItemPriority.Default, 20)]
    internal class Mediator
    {
        public long id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public short? type { get; set; }
        public string county { get; set; }
        public string phone { get; set; }
        public bool? active { get; set; }
        public string address { get; set; }
        public bool? contract { get; set; }
        public string teams_link { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}