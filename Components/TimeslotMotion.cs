using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("timeslot_motions")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("TimeslotMotions", CacheItemPriority.Default, 20)]
    internal class TimeslotMotion : ICloneable
    {
        public long id { get; set; }
        public string timeslotable_type { get; set; }
        public long timeslotable_id { get; set; }
        public long motion_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public object Clone()
        {
            return new TimeslotMotion
            {
                timeslotable_type = this.timeslotable_type,
                timeslotable_id = this.timeslotable_id,
                motion_id = this.motion_id,
            };
        }
    }
}