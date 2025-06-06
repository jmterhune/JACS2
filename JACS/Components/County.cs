using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("counties")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Countys", CacheItemPriority.Default, 20)]
    internal class County
    {
        public long id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
