using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace tjc.Modules.CourtCounsel.Components
{
    [TableName("tjc_cc_case_type_counts")]
    //setup the primary key for table
    internal class CaseTypeCount
    {
        public string CaseTypeName { get; set; }
        public int CaseTypeId { get; set; }
        public int Count { get; set; }
        public IEnumerable<CaseDetail> CaseDetails { get; set; }
    }
    [TableName("tjc_cc_case_details")]
    internal class CaseDetail
    {
        public string CaseTypeName { get; set; }
        public int CaseTypeId { get; set; }

        public DateTime? MotionFiled { get; set; }
        public DateTime? DateReceived { get; set; }
        public string CaseName { get; set; }
        public string CaseNumber { get; set; }
        public string Responsible { get; set; }
        public string PhaseName { get; set; }
    }
}
