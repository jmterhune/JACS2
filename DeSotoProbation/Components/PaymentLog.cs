using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;

namespace tjc.Modules.DeSoto.Probation.Components
{
    [TableName("tjc_desoto_probation_payments")]
    //setup the primary key for table
    [PrimaryKey("TransactionID", AutoIncrement = true)]
    // Deliberately not [Cacheable]. The public jud12 site's DeSotoCaseManagement
    // module inserts, updates and deletes rows in this same table, and its writes
    // cannot invalidate a cache held by this application. A cached collection here
    // went stale for up to twenty minutes, and GetById returned null for payments
    // that Find() could still see.
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