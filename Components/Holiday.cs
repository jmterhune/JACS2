using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("holidays")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Holidays", CacheItemPriority.Default, 20)]
    internal class Holiday
    {
        public long id { get; set; }
        public string name { get; set; }
        public DateTime date { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}