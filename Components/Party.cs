using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("parties")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Partys", CacheItemPriority.Default, 20)]
    internal class Party
    {
        public long id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public long? attorney_id { get; set; }
        public string address { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public long mediation_case_id { get; set; }
    }
}
