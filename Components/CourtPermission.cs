using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("court_permissions")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("CourtPermissions", CacheItemPriority.Default, 20)]
    internal class CourtPermission
    {
        public long id { get; set; }
        public long user_id { get; set; }
        public long judge_id { get; set; }
        public bool editable { get; set; }
        public bool active { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
