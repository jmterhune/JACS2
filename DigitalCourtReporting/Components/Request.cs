using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.DigitalCourtReporting.Components
{
    [TableName("tjc_dcr_request")]
    [PrimaryKey("RequestID", AutoIncrement = true)]
    [Cacheable("Requests", CacheItemPriority.Default, 20)]
    internal class Request : EntityBase
    {
        public int RequestID { get; set; }
        public int? ProcessingID { get; set; }
        public string Juvenile { get; set; }
        public string Indigence { get; set; }
        public string CDType { get; set; }
        public string Employee { get; set; }
        public string Tracking { get; set; }
        public DateTime? CDBurnDate { get; set; }
        public DateTime? DateotCA { get; set; }
        public int? TotalMinutes { get; set; }
        public int? CDCount { get; set; }
        public string Notes { get; set; }
        public bool? UTP { get; set; }
    }
    public enum PaymentType
    {
        [System.ComponentModel.Description("Credit Card/Debit Card/e-Check")]
        card = 1,
        [System.ComponentModel.Description("Check")]
        check = 2,
        [System.ComponentModel.Description("Money Order")]
        moneyOrder = 3
    }

    public enum OrderStatus
    {
        submitted = 0,
        reviewed = 1,
        paid = 2,
        mediaCreated = 3,
        notified = 4,
        completed = 5,
        paymentRejected = 6,
        cancelled = 7,
        resubmitted = 8,
        repopened = 9
    }
}