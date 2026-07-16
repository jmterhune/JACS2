
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
        public int AccountID { get; set; }              // int
        public int ProceedingID { get; set; }           // int
        public string PaymentDate { get; set; }         // nvarchar(50)
        public string CheckNumber { get; set; }         // nvarchar(30)
        public decimal? Payment { get; set; }           // money
        public string ReceivedBy { get; set; }          // nvarchar(50)
        public bool? NFR { get; set; }                  // bit
        public string Notes { get; set; }               // nvarchar(750)
    }
}

