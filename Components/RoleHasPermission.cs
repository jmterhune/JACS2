using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("role_has_permissions")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("RoleHasPermissions", CacheItemPriority.Default, 20)]
    internal class RoleHasPermission
    {
        public long permission_id { get; set; }
        public long role_id { get; set; }
    }
}