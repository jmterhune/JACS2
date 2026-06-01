using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;
namespace tjc.Modules.CourtCounsel.Components.Models
{
    [TableName("tjc_cc_requestor")]
    [PrimaryKey("RequestorId", AutoIncrement = true)]
    [Cacheable("tjc_cc_requestor", CacheItemPriority.Default, 20)]
    public class RequestorInfo
    {
        public int RequestorId { get; set; }
        public string RequestorName { get; set; }
        public bool? IsActive { get; set; }
    }
}
