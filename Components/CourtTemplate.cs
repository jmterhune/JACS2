using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;

namespace tjc.Modules.jacs.Components
{
    [TableName("court_templates")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("CourtTemplates", CacheItemPriority.Default, 20)]
    internal class CourtTemplate : ICloneable
    {
        public long id { get; set; }
        public long court_id { get; set; }
        public string name { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        [IgnoreColumn]
        public string court_description { get; set; }
        [IgnoreColumn]
        public string judge_name { get; set; }
        [IgnoreColumn]
        public IEnumerable<TemplateTimeslot> template_timeslots
        {
            get
            {
                var ctl = new TemplateTimeslotController();
                return ctl.GetTemplateTimeslotsByTemplateId(id);
            }
        }
        public object Clone()
        {
            return new CourtTemplate
            {
                id = this.id,
                court_id = this.court_id,
                name = this.name,
                created_at = this.created_at,
                updated_at = this.updated_at,
                deleted_at = this.deleted_at,
                court_description = this.court_description,
                judge_name = this.judge_name
            };
        }
    }
}
