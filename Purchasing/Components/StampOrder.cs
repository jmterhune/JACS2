using DotNetNuke.ComponentModel.DataAnnotations;
using System;

namespace tjc.Modules.Purchasing.Components
{
    [TableName("tjc_purchasing_stamp_orders")]
    //setup the primary key for table
    [PrimaryKey("OrderID", AutoIncrement = true)]
    internal class StampOrder
    {
        public int OrderID { get; set; }
        public DateTime DateCreated { get; set; }
        public string Location { get; set; }
        public string RequestedName { get; set; }
        public string ConsumerName { get; set; }
        public string Phone { get; set; }
        public string StampType { get; set; }
        public string Sample { get; set; }
        public string FontStyle { get; set; }
        public string FontSize { get; set; }
        public string InkColor { get; set; }
        public int Quantity { get; set; }
        public string Instructions { get; set; }
        public DateTime? CompletedDate { get; set; }
        public OrderStatus Status { get; set; }
        public string EmailAddress { get; set; }
    }
    public enum OrderStatus
    {
        @new = 0,
        rejected = -1,
        accepted = 1,
        completed = 2
    }
}