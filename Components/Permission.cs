using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("permissions")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Permissions", CacheItemPriority.Default, 20)]
    internal class Permission
    {
        public long id { get; set; }
        public string name { get; set; }
        public string guard_name { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}