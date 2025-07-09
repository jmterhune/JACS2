using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.jacs.Components
{
    [TableName("court_templates")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("CourtTemplates", CacheItemPriority.Default, 20)]
    internal class CourtTemplate
    {
        public long id { get; set; }
        public long court_id { get; set; }
        public string name { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        [IgnoreColumn]
        public string court_description { get; set; }
        [IgnoreColumn]
        public string judge_name { get; set; }
    }
}