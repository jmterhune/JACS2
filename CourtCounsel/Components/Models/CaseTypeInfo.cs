using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;
namespace tjc.Modules.CourtCounsel.Components.Models
{
    [TableName("tjc_cc_case_type")]
    [PrimaryKey("caseTypeId", AutoIncrement = true)]
    [Cacheable("tjc_cc_case_type", CacheItemPriority.Default, 20)]
    public class CaseTypeInfo
    {
        public int CaseTypeId { get; set; }
        public string CaseType { get; set; }
    }
}
