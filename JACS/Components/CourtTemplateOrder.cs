using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("court_template_order")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("CourtTemplateOrders", CacheItemPriority.Default, 20)]
    internal class CourtTemplateOrder
    {
        public long id { get; set; }
        public long court_id { get; set; }
        public int? order { get; set; }
        public DateTime? date { get; set; }
        public long? template_id { get; set; }
        public bool auto { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
