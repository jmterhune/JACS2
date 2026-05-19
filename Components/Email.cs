using DotNetNuke.ComponentModel.DataAnnotations;
using System;
namespace tjc.Modules.jacs.Components
{
    [TableName("emails")]
    [PrimaryKey("id", AutoIncrement = true)]
    // No [Cacheable] — emails are written from multiple applications; see Attorney.cs.
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