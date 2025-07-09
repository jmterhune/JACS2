using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("user_defined_fields")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("UserDefinedFields", CacheItemPriority.Default, 20)]
    internal class UserDefinedField
    {
        public long id { get; set; }
        public long court_id { get; set; }
        public string field_name { get; set; }
        public string field_type { get; set; }
        public string alignment { get; set; }
        public string default_value { get; set; }
        public int required { get; set; }
        public int yes_answer_required { get; set; }
        public int display_on_docket { get; set; }
        public int display_on_schedule { get; set; }
        public int use_in_attorany_scheduling { get; set; }
        public string old_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}