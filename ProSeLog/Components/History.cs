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
        public int HistoryID { get; set; } // int

        public string Month { get; set; } // nvarchar(10)

        public string MonthNumber { get; set; } // nvarchar(2)

        public int? Year { get; set; } // int

        public string Petitioner { get; set; } // nvarchar(50)

        public string Respondent { get; set; } // nvarchar(50)

        public string CaseName { get; set; } // nvarchar(50)

        public string CaseNumber { get; set; } // nvarchar(50)

        public string Phone { get; set; } // nvarchar(50)

        public DateTime? ReceivedDate { get; set; } // smalldatetime

        public int CountyID { get; set; } // int

        public int CaseTypeID { get; set; } // int

        public int ContactID { get; set; } // int

        public bool NeedsLetter { get; set; } // bit

        public bool ProvidedForms { get; set; } // bit

        public bool AssistedForms { get; set; } // bit

        public bool AssistedProcedures { get; set; } // bit

        public bool SetFinalHearing { get; set; } // bit

        public bool SetOtherHearing { get; set; } // bit

        public bool ReferralOther { get; set; } // bit

        public bool ReferralGmMag { get; set; } // bit

        public bool PreparedOrder { get; set; } // bit

        public bool Other { get; set; } // bit

        public bool AppointedPro { get; set; } // bit

        public DateTime? ResolutionDate { get; set; } // smalldatetime

        public DateTime CreatedDate { get; set; } // datetime

        public int CreatedByID { get; set; } // int

        public DateTime LastModifiedDate { get; set; } // datetime

        public int LastModifiedByID { get; set; } // int
        [IgnoreColumn]
        public bool Resolution { get{ return ResolutionDate.HasValue; } }

    }
    [TableName("tjc_prose_history_list")]
    internal class HistoryListItem : History
    {
        public string CaseTypeName { get; set; } // nvarchar(50)
        public string ContactName { get; set; } // nvarchar(100)
        public string LastModifiedByName { get; set; } // nvarchar(128)
        public string CountyName { get; set; } // nvarchar(50)
    }
}