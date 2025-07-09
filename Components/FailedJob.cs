using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("failed_jobs")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("FailedJobs", CacheItemPriority.Default, 20)]
    internal class FailedJob
    {
        public long id { get; set; }
        public string uuid { get; set; }
        public string connection { get; set; }
        public string queue { get; set; }
        public string payload { get; set; }
        public string exception { get; set; }
        public DateTime failed_at { get; set; }
    }
}
