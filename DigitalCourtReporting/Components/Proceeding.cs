using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.DigitalCourtReporting.Components
{
    [TableName("tjc_dcr_proceeding")]
    [PrimaryKey("ProceedingID", AutoIncrement = true)]
    public class Proceeding : EntityBase
    {
        public int ProceedingID { get; set; }
        public string Requestor { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public int RequestorId { get; set; }
        public DateTime RequestedDate { get; set; }
        public string CDPreference { get; set; }
        public int JurisdictionID { get; set; }
        public string CaseName { get; set; }
        public string CaseNumber { get; set; }
        public string Judge { get; set; }
        public string ProceedingDate { get; set; }
        public string ProceedingTime { get; set; }
        public int CountyID { get; set; }
        public string ProceedingType { get; set; }
        public string Involvement { get; set; }
        public string Instructions { get; set; }
        public string TranscriptionList { get; set; }
        public string DeliveryMethod { get; set; }
        public bool Agency { get; set; }
        public bool CA { get; set; }
        public bool Closed { get; set; }
        public bool Paid { get; set; }
        public int? ModuleId { get; set; }
        public bool? IsInquiry { get; set; }
    }
    [TableName("tjc_dcr_proceeding_list")]
    [PrimaryKey("ProceedingID", AutoIncrement = true)]
    public class ProceedingListItem : Proceeding
    {
        public string Location { get; set; }
        public string Jurisdiction { get; set; }

        [IgnoreColumn]
        public string RequestDateFormatted
        {
            get
            {
                if (RequestedDate.AddMonths(3) < DateTime.Now)

                    return string.Format("<span class='text-danger'>{0}-{1}</span>", Location.Substring(0, 1).ToUpper(), RequestedDate.ToShortDateString());
                else
                    return string.Format("{0}-{1}", Location.Substring(0, 1).ToUpper(), RequestedDate.ToShortDateString());
            }
        }
    }
    public enum ListTypes
    {
        payment,
        notification,
        cdCreation,
        completed,
        inquiry
    }
    public enum SearchTypes
    {
        caseName,
        caseNumber,
        trackingNumber,
        requestor
    }
}