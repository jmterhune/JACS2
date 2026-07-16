
using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;
namespace tjc.Modules.MediationStatistics.Components
{
    [TableName("tjc_med_sessions")]
    [PrimaryKey("SessionId", AutoIncrement = true)]
    [Cacheable("Sessions", CacheItemPriority.Default, 20)]
    internal class Session : EntityBase
    {
        public int SessionId { get; set; }  // int (identity PK)

        public int? CaseId { get; set; }  // int

        public int? PrimaryCaseType { get; set; }  // int

        public int? StageOfAction { get; set; }  // int

        public int? p1_AttorneyId { get; set; }  // int

        public bool p1_ProSe { get; set; }  // bit

        public string p1_FeesPaid { get; set; }  // nvarchar(50)

        public string p1_FeesOwed { get; set; }  // nvarchar(50)

        public int? p2_AttorneyId { get; set; }  // int

        public bool p2_ProSe { get; set; }  // bit

        public string p2_FeesPaid { get; set; }  // nvarchar(50)

        public string p2_FeesOwed { get; set; }  // nvarchar(50)

        public bool? p1_FTA { get; set; }  // bit

        public bool? p2_FTA { get; set; }  // bit

        public DateTime? MediationDate { get; set; }  // datetime

        public DateTime? ReferralDate { get; set; }  // datetime

        public string ProgramReferralSource { get; set; }  // nvarchar(50)

        public bool ArbitrationReferral { get; set; }  // bit

        public bool CircuitCivilReferral { get; set; }  // bit

        public string FeeAmount { get; set; }  // nvarchar(50)

        public bool? FeeJudgement { get; set; }  // bit

        public bool? FeeAgreement { get; set; }  // bit

        public string PTC_CourtOrdered { get; set; }  // char(1)

        public int? ChildrenInvolved { get; set; }  // int

        public int? ParentsInvolved { get; set; }  // int

        public bool HeldByPhone { get; set; }  // bit

        public string Comment { get; set; }  // nvarchar(4000)

        public bool? OTS { get; set; }  // bit

        public bool? FeeWaiver { get; set; }  // bit

        public bool? Inmate { get; set; }  // bit

        public bool? Interpreter { get; set; }  // bit
        [IgnoreColumn]
        public IEnumerable<Event> SessionEvents { get { var ctl = new EventController();
                return ctl.GetEventsBySession(SessionId);
            } }
        [IgnoreColumn]
        public IEnumerable<Issue> SessionIssues
        {
            get
            {
                var ctl = new IssueController();
                return ctl.GetIssuesBySession(SessionId);
            }
        }
    }
}