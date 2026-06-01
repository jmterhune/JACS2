using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;
using System.Web.Caching;

namespace tjc.Modules.DeSoto.Probation.Components
{
    [TableName("tjc_desoto_probation_payments")]
    //setup the primary key for table
    [PrimaryKey("TransactionID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Transactions", CacheItemPriority.Default, 20)]
    internal class TransactionLog
    {
        public int TransactionID { get; set; }
        public string CustomerName { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string CaseNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool? Success { get; set; }
        public string OrderId { get; set; }
        public decimal? Fee { get; set; }
        public decimal? Total { get; set; }
        public string PaymentType { get; set; }
    }
}