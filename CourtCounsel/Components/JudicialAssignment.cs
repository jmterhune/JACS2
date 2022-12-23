using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.CourtCounsel.Components
{
    [TableName("court_counsel_judge_assignments")]
    //configure caching using PetaPoco
    [Cacheable("JudicialAssignments", CacheItemPriority.Default, 20)]
    internal class JudicialAssignment : EntityBase
    {
        public long AssignmentId { get; set; }
        public long JudgeId { get; set; }
        public DateTime DateAssigned { get; set; }
        public DateTime? DateRemoved { get;set; }
        public string Reason { get; set; }
    }
}
