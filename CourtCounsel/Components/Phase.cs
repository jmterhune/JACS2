using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Modules.CourtCounsel.Components
{
    [TableName("court_counsel_phases")]
    //setup the primary key for table
    [PrimaryKey("PhaseId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Phases", CacheItemPriority.Default, 20)]
    internal class Phase : EntityBase
    {
        public int PhaseId { get; set; }
        public string PhaseName { get; set; }
        public string GroupName { get; set; }
        public int GroupIndex { get; set; }
        public bool IsPending { get; set; }
        public bool Active { get; set; }
    }
}
