using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("tickets")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Tickets", CacheItemPriority.Default, 20)]
    internal class Ticket
    {
        public int id { get; set; }
        public string ticket_number { get; set; }
        public string subject { get; set; }
        public string issue { get; set; }
        public long created_by { get; set; }
        public long priority_id { get; set; }
        public string created_user_type { get; set; }
        public string comment { get; set; }
        public long status_id { get; set; }
        public string file { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
