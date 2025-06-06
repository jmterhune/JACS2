using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("password_resets")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("PasswordResets", CacheItemPriority.Default, 20)]
    internal class PasswordReset
    {
        public string email { get; set; }
        public string token { get; set; }
        public DateTime? created_at { get; set; }
    }
}
