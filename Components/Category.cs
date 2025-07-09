using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("categories")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Categorys", CacheItemPriority.Default, 20)]
    internal class Category
    {
        public long id { get; set; }
        public string old_id { get; set; }
        public string description { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
