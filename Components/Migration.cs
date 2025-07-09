using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("migrations")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Migrations", CacheItemPriority.Default, 20)]
    internal class Migration
    {
        public int id { get; set; }
        public string migration { get; set; }
        public int batch { get; set; }
    }
}