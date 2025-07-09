using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("model_has_roles")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("ModelHasRoles", CacheItemPriority.Default, 20)]
    internal class ModelHasRole
    {
        public long role_id { get; set; }
        public string model_type { get; set; }
        public long model_id { get; set; }
    }
}