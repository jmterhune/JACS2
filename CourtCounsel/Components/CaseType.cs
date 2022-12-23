using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Modules.CourtCounsel.Components
{
    [TableName("court_counsel_case_types")]
    //setup the primary key for table
    [PrimaryKey("CaseTypeId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("CaseTypes", CacheItemPriority.Default, 20)]
    internal class CaseType : EntityBase
    {
        public int CaseTypeId { get; set; }
        public string CaseTypeName { get; set; }
        public bool Active { get; set; }
    }
}
