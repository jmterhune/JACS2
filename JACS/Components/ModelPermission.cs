using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("model_has_permissions")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("ModelPermissions", CacheItemPriority.Default, 20)]
    internal class ModelPermission
    {
        public long permission_id { get; set; }
        public string model_type { get; set; }
        public long model_id { get; set; }
    }
}