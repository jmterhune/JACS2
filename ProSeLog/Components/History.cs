using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace tjc.Modules.ProSeLog.Components
{
    [TableName("tjc_prose_history")]
    [PrimaryKey("HistoryID", AutoIncrement = true)]
    internal class History
    {
        public int HistoryID { get; set; }

        public string Month { get; set; }

        public string MonthNumber { get; set; }

        public int? Year { get; set; }

        public string Petitioner { get; set; }

        public string Respondent { get; set; }

        public string CaseName { get; set; }

        public string CaseNumber { get; set; }

        public string Phone { get; set; }

        public DateTime? ReceivedDate { get; set; }

        public int CountyID { get; set; }

        public int CaseTypeID { get; set; }

        public int ContactID { get; set; }

        public bool NeedsLetter { get; set; }

        public bool ProvidedForms { get; set; }

        public bool AssistedForms { get; set; }

        public bool AssistedProcedures { get; set; }

        public bool SetFinalHearing { get; set; }

        public bool SetOtherHearing { get; set; }

        public bool ReferralOther { get; set; }

        public bool ReferralGmMag { get; set; }

        public bool PreparedOrder { get; set; }

        public bool Other { get; set; }

        public bool AppointedPro { get; set; }

        public DateTime? ResolutionDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedByID { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public int LastModifiedByID { get; set; }
        [IgnoreColumn]
        public bool Resolution { get{ return ResolutionDate.HasValue; } }

    }
    [TableName("tjc_prose_history_list")]
    internal class HistoryListItem : History
    {
        public string CaseTypeName { get; set; }
        public string ContactName { get; set; }
        public string LastModifiedByName { get; set; }
        public string CountyName { get; set; }
    }
}