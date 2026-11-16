using DotNetNuke.ComponentModel.DataAnnotations;
using System;


namespace tjc.Modules.JCID.Components
{
    [TableName("tjc_jcid_payments")]
    //setup the primary key for table
    [PrimaryKey("TransactionID", AutoIncrement = true)]
    //configure caching using PetaPoco
    public class Transaction
    {
        public int TransactionID { get; set; }
        public string CustomerName { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public int RequestID { get; set; }
        public string EmailAddress { get; set; }
        public bool Success { get; set; }
        public string OrderId { get; set; }
        public decimal Fee { get; set; }
        public decimal Total { get; set; }
        public string PaymentType { get; set; }
    }
}
