
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
        public int SessionId { get; set; }

        public int? CaseId { get; set; }

        public int? PrimaryCaseType { get; set; }

        public int? StageOfAction { get; set; }

        public int? p1_AttorneyId { get; set; }

        public bool p1_ProSe { get; set; }

        public string p1_FeesPaid { get; set; }

        public string p1_FeesOwed { get; set; }

        public int? p2_AttorneyId { get; set; }

        public bool p2_ProSe { get; set; }

        public string p2_FeesPaid { get; set; }

        public string p2_FeesOwed { get; set; }

        public bool? p1_FTA { get; set; }

        public bool? p2_FTA { get; set; }

        public DateTime? MediationDate { get; set; }

        public string Mediator { get; set; }

        public DateTime? ReferralDate { get; set; }

        public string ProgramReferralSource { get; set; }

        public bool ArbitrationReferral { get; set; }

        public bool CircuitCivilReferral { get; set; }

        public string FeeAmount { get; set; }

        public bool? FeeJudgement { get; set; }

        public bool? FeeAgreement { get; set; }

        public string PTC_CourtOrdered { get; set; }

        public int? ChildrenInvolved { get; set; }

        public int? ParentsInvolved { get; set; }

        public bool HeldByPhone { get; set; }

        public string Comment { get; set; }

        public bool? OTS { get; set; }

        public bool? FeeWaiver { get; set; }

        public bool? Inmate { get; set; }

        public bool? Interpreter { get; set; }
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