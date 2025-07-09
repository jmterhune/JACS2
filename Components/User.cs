using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("users")]
    [PrimaryKey("id", AutoIncrement = false)]
    [Cacheable("Users", CacheItemPriority.Default, 20)]
    internal class User
    {
        public long id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public DateTime? email_verified_at { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}