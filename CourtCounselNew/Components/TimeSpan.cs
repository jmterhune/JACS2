using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Modules.CourtCounsel.Components
{
    [TableName("court_counsel_timespans")]
    //setup the primary key for table
    [PrimaryKey("TimeSpanId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("TimeSpans", CacheItemPriority.Default, 20)]
    internal class TimeSpan : EntityBase
    {
        public int TimeSpanId { get; set; }
        public string TimeSpanName { get; set; }
        public bool Active { get; set; }
    }
}
