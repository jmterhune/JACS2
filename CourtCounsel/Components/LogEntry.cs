using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace tjc.Modules.CourtCounsel.Components
{
    [TableName("court_counsel_log_entries")]
    //setup the primary key for table
    [PrimaryKey("LogId", AutoIncrement = true)]
    internal class LogEntry : EntityBase
    {
        public long LogId { get; set; }
        public string CaseNumber { get; set; }
        public string Description { get; set; }
        public bool IsCase { get; set; }
        public int CountyId { get; set; }
    }

    [TableName("court_counsel_entry_list")]
    //setup the primary key for table
    internal class LogEntryListItem 
    {
        public long LogId { get; set; }
        public string CaseNumber { get; set; }
        public string Description { get; set; }
        public string CaseTypeName { get; set; }
        public long AssignmentId { get; set; }
        public string ActionName { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? MotionFiled { get; set; }
        public DateTime? DateCompleted { get; set; }
        public string AttorneyName { get; set; }
        public string PhaseName { get; set; }
        public string Email { get; set; }
        public string JudgeEmail { get; set; }
        public int StatusTypeId { get; set; }
        public int CountyId { get; set; }
        public int PhaseId { get; set; }
        [IgnoreColumn]
        [EnumDataType(typeof(StatusTypes))]
        public string StatusType
        {
            get
            {
                StatusTypes status = (StatusTypes)this.StatusTypeId;
                switch (status)
                {
                    case StatusTypes.active:
                        return "Active";
                    case StatusTypes.pending:
                        return "Pending";
                    case StatusTypes.closed:
                        return "Closed";
                    default:
                        break;
                }
                return "";
            }
            
        }
    }
}

public enum SearchType
{
    recent,caseName,caseNumber,attorney
}
