using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("judges")]
    [PrimaryKey("id", AutoIncrement = false)]
    [Cacheable("Judges", CacheItemPriority.Default, 20)]
    internal class Judge
    {
        public long id { get; set; }
        public string name { get; set; }
        public string old_id { get; set; }
        public string phone { get; set; }
        public long? court_id { get; set; }
        public string title { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        [IgnoreColumn]
        public string court_name { get; set; }
    }
}