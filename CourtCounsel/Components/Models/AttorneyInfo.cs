using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;
namespace tjc.Modules.CourtCounsel.Components.Models
{
    [TableName("tjc_cc_attorney")]
    [PrimaryKey("AttorneyId", AutoIncrement = true)]
    [Cacheable("tjc_cc_attorney", CacheItemPriority.Default, 20)]
    public class AttorneyInfo
    {
        public int AttorneyId { get; set; }
        public string AttorneyName { get; set; }
        public bool? IsActive { get; set; }
    }
}
