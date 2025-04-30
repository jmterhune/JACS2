
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Web.Caching;
namespace tjc.Modules.DigitalCourtReporting.Components
{
    [TableName("tjc_dcr_accounting")]
    [PrimaryKey("AccountID", AutoIncrement = true)]
    [Cacheable("Accounts", CacheItemPriority.Default, 20)]
    internal class Account : EntityBase
    {
        public int AccountID { get; set; }
        public int ProceedingID { get; set; }
        public string PaymentDate { get; set; }
        public string CheckNumber { get; set; }
        public decimal? Payment { get; set; }
        public string ReceivedBy { get; set; }
        public bool? NFR { get; set; }
        public string Notes { get; set; }
    }
}

