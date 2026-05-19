using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    [TableName("motions")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Motions", CacheItemPriority.Default, 20)]
    internal class Motion
    {
        public long id { get; set; }
        public string old_id { get; set; }
        public string description { get; set; }
        public int lag { get; set; }
        public int lead { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

    internal class MotionSearchResult
    {
        public List<MotionViewModel> data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int draw { get; set; }
        public string error { get; set; }
    }

    internal class MotionResult
    {
        public Motion data { get; set; }
        public string error { get; set; }
    }

}