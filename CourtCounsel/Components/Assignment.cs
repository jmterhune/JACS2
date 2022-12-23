using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Caching;
namespace tjc.Modules.CourtCounsel.Components
{
    [TableName("court_counsel_assignments")]
    //setup the primary key for table
    [PrimaryKey("AssignmentId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Assignments", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class Assignment : EntityBase
    {
        public long AssignmentId { get; set; }
        public long LogId { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? MotionFiled { get; set; }
        public int CaseTypeId { get; set; }
        public int CurrentJudiciaryId { get; set; }

        public int CurrentAttorneyId { get; set; }

        public int PhaseId { get; set; }

        public int ActionId { get; set; }

        public DateTime? DateCompleted { get; set; }

        public int TimeSpanId { get; set; }

        public string Comments { get; set; }
        public int StatusTypeId { get; set; }
        public bool PreventReassignment { get; set; }

        [IgnoreColumn]
        [EnumDataType(typeof(StatusTypes))]
        public StatusTypes StatusType
        {
            get
            {
                return (StatusTypes)this.StatusTypeId;
            }
            set
            {
                this.StatusTypeId = (int)value;
            }
        }
        [IgnoreColumn]
        public LogEntry logEntry
        {
            get
            {
                var ctl = new LogEntryController();

                return ctl.GetLogEntry(LogId);
            }
        }
    }
    public enum StatusTypes
    {
        active,
        pending,
        closed
    }
}
