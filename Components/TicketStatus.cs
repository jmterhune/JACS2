using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("tickets_statuses")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("TicketStatuss", CacheItemPriority.Default, 20)]
    internal class TicketStatus
    {
        public long id { get; set; }
        public string name { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
