using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;
namespace tjc.Modules.CourtCounsel.Components.Models
{
    [TableName("tjc_cc_phase")]
    [PrimaryKey("PhaseId", AutoIncrement = true)]
    [Cacheable("tjc_cc_phase", CacheItemPriority.Default, 20)]
    public class PhaseInfo
    {
        public int PhaseId { get; set; }
        public string Phase { get; set; }
    }
}
