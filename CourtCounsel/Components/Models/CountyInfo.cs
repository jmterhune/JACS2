using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;
namespace tjc.Modules.CourtCounsel.Components.Models
{
    [TableName("tjc_cc_county")]
    [PrimaryKey("countyId", AutoIncrement = true)]
    [Cacheable("tjc_cc_county", CacheItemPriority.Default, 20)]
    public class CountyInfo
    {
        public int CountyId { get; set; }
        public string County { get; set; }
    }
}
