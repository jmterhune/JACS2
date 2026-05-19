using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    [TableName("court_motions")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("CourtMotions", CacheItemPriority.Default, 20)]
    internal class CourtMotion
    {
        public long id { get; set; }
        public long court_id { get; set; }
        public long motion_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool allowed { get; set; }
        [IgnoreColumn]
        public string motion_description { get; set; }
        [IgnoreColumn]
        public string court_description { get; set; }
    }
    internal class CourtMotionSearchResult
    {
        public List<CourtMotionViewModel> data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int draw { get; set; }
        public string error { get; set; }
    }

    internal class CourtMotionResult
    {
        public CourtMotionViewModel data { get; set; }
        public string error { get; set; }
    }


}