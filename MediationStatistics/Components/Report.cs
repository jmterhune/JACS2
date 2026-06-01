/*
' Copyright (c) 2023 12th Judicial Circuit
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Data;
using System.Web.Caching;

namespace tjc.Modules.MediationStatistics.Components
{
    [TableName("tjc_med_fees_owed")]
    internal class FeesOwed
    {
        public string aFirstName
        {
            get; set;
        }

        public string aLastName
        {
            get; set;
        }

        public string Phone
        {
            get; set;
        }

        public string Extension
        {
            get; set;
        }

        public string Address
        {
            get; set;
        }

        public string City
        {
            get; set;
        }

        public string State
        {
            get; set;
        }

        public string Zip
        {
            get; set;
        }
        public string pFirstName
        {
            get; set;
        }

        public string pLastName
        {
            get; set;
        }

        public DateTime MediationDate
        {
            get; set;
        }

        public string FeeOwed
        {
            get; set;
        }

        public string CaseNumber
        {
            get; set;
        }

        public int RegionId
        {
            get; set;
        }

        public string CaseTypeGroup
        {
            get; set;
        }

        public string Region
        {
            get; set;
        }

        public bool FeeJudgement
        {
            get; set;
        }

        public bool FeeAgreement
        {
            get; set;
        }

        public bool FeeWaiver
        {
            get; set;
        }

        public bool OTS
        {
            get; set;
        }

        public bool p1_FTA
        {
            get; set;
        }

        public bool p2_FTA
        {
            get; set;
        }
        [IgnoreColumn]
        public string FormattedAddress { get {
             return   string.IsNullOrEmpty(Address) ? "" : string.Format("{0}<br />",Address);
            } }
        [IgnoreColumn]

        public string FormattedCity
        {
            get
            {
                return string.IsNullOrEmpty(City) ? "" : string.Format("{0}, ",City);
            }
        }
        [IgnoreColumn]

        public string FormattedExtension
        {
            get
            {
                return string.IsNullOrEmpty(Extension) ? "" : string.Format("ex:{0}", Extension);
            }
        }
    }

    public class StatReportGroup
    {
        public string Region
        {
            get;
            set;
        }

        public int Session_Count
        {
            get;
            set;
        }

    }

    public class StatMediatorCounts
    {
        public string Region { get; set; }
        public string MediatorName { get; set; }
        public int MediatorId { get; set; }
        public string MediatorType { get; set; }
        public int Signed { get; set; }
        public string Agreement { get; set; }
        public int Held { get; set; }
    }
    public class StatisticalReport
    {
        public string questionaire
        {
            get; set;
        }

        public string question
        {
            get; set;
        }

        public int sarasota
        {
            get; set;
        }

        public int manatee
        {
            get; set;
        }

        public int desoto
        {
            get; set;
        }

        public int southCounty
        {
            get; set;
        }

        public int northCounty
        {
            get; set;
        }

        public double sPercent
        {
            get; set;
        }

        public double mPercent
        {
            get; set;
        }

        public double dPercent
        {
            get; set;
        }
    }
    public class FeeReportCollectedOwed
    {


        public int? CountyCount
        {
            get; set;
        }

        public int? FamilyCount
        {
            get; set;
        }

        public int? CountyPaid60
        {
            get; set;
        }

        public int? FamilyPaid60
        {
            get; set;
        }

        public int? FamilyPaid120
        {
            get; set;
        }

        public int? CountyOwed
        {
            get; set;
        }

        public int? FamilyOwed60
        {
            get; set;
        }

        public int? FamilyOwed120
        {
            get; set;
        }

        public int? FamilyOwed60FTA
        {
            get; set;
        }

        public int? FamilyOwed120FTA
        {
            get; set;
        }

        public int? CountyOwedFTA
        {
            get; set;
        }

        public int? FamilyPaidIndigent
        {
            get; set;
        }

        public int? CountyPaidIndigent
        {
            get; set;
        }

        public int? FamilyOwedIndigent
        {
            get; set;
        }
        public int? CountyOwedIndigent
        {
            get; set;
        }

        public int? FamilyOwedIndigentFTA
        {
            get; set;
        }

        public int? CountyOwedIndigentFTA
        {
            get; set;
        }

        public int? FamilyOwedWaived
        {
            get; set;
        }

        public int? FamilyOwedWaivedFTA
        {
            get; set;
        }

        public int? FamilyPaidWaived
        {
            get; set;
        }

        public int? CountyOwedWaived
        {
            get; set;
        }

        public int? CountyOwedWaivedFTA
        {
            get; set;
        }

        public int? CountyPaidWaived
        {
            get; set;
        }
    }
    [TableName("tjc_med_stat_checker")]
    internal class StatChecker
    {
        public string Region
        {
            get; set;
        }

        public string CaseType
        {
            get; set;
        }

        public string CaseTypeGroup
        {
            get; set;
        }

        public string StageOfAction
        {
            get; set;
        }

        public DateTime MediationDate
        {
            get; set;
        }

        public string MediatorType
        {
            get; set;
        }
        public string MediatorName
        {
            get; set;
        }

        public DateTime ReferralDate
        {
            get; set;
        }

        public bool ArbitrationReferral
        {
            get; set;
        }

        public bool CircuitCivilReferral
        {
            get; set;
        }

        public string FeeAmount
        {
            get; set;
        }

        public bool HeldByPhone
        {
            get; set;
        }

        public char PTC_CourtOrdered
        {
            get; set;
        }

        public DateTime CreatedDate
        {
            get; set;
        }

        public bool MediationHeld
        {
            get; set;
        }

        public int SessionId
        {
            get; set;
        }

        public bool AgreementReached
        {
            get; set;
        }

        public string CaseNumber
        {
            get; set;
        }

        public string partyone
        {
            get; set;
        }

        public string partytwo
        {
            get; set;
        }

        public bool FeeJudgement
        {
            get; set;
        }

        public bool FeeAgreement
        {
            get; set;
        }

        public bool p1_FTA
        {
            get; set;
        }

        public bool p2_FTA
        {
            get; set;
        }

        public bool OTS
        {
            get; set;
        }

        public bool FeeWaiver
        {
            get; set;
        }
    }
    [TableName("tjc_med_session_counts")]
    internal class SessionCount
    {
        public string Region
        {
            get; set;
        }

        public string CaseType
        {
            get; set;
        }

        public string CaseTypeGroup
        {
            get; set;
        }

        public string StageOfAction
        {
            get; set;
        }

        public DateTime MediationDate
        {
            get; set;
        }

        public string Mediator
        {
            get; set;
        }

        public DateTime ReferralDate
        {
            get; set;
        }

        public bool ArbitrationReferral
        {
            get; set;
        }

        public bool CircuitCivilReferral
        {
            get; set;
        }

        public string FeeAmount
        {
            get; set;
        }

        public bool HeldByPhone
        {
            get; set;
        }

        public char PTC_CourtOrdered
        {
            get; set;
        }

        public DateTime CreatedDate
        {
            get; set;
        }

        public bool MediationHeld
        {
            get; set;
        }

        public int SessionId
        {
            get; set;
        }

        public bool AgreementReached
        {
            get; set;
        }

        public string CaseNumber
        {
            get; set;
        }

        public string partyone
        {
            get; set;
        }

        public string partytwo
        {
            get; set;
        }

        public bool FeeJudgement
        {
            get; set;
        }

        public bool FeeAgreement
        {
            get; set;
        }

        public bool p1_FTA
        {
            get; set;
        }

        public bool p2_FTA
        {
            get; set;
        }

        public bool OTS
        {
            get; set;
        }

        public bool FeeWaiver
        {
            get; set;
        }
    }
}
