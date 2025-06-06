using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("emails")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Emails", CacheItemPriority.Default, 20)]
    internal class Email
    {
        public long id { get; set; }
        public string email { get; set; }
        public string emailable_type { get; set; }
        public long emailable_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}