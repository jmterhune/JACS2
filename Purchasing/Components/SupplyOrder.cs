using System;
using System.Collections.Generic;
using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.Purchasing.Components
{
    [TableName("tjc_purchasing_supply_orders")]
    //setup the primary key for table
    [PrimaryKey("OrderID", AutoIncrement = true)]

    internal class SupplyOrder
    {
        public int OrderID { get; set; }
        public string Location { get; set; }
        public string RequestedName { get; set; }
        public string EmailAddress { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime? CompletedDate { get; set; }
        [IgnoreColumn]
        public IEnumerable<SupplyOrderItem> SupplyOrderItems
        {
            get
            {
                var ctl = new SupplyOrderController();
                return ctl.GetSupplyOrderItemsByOrder(OrderID);
            }
        }
        [IgnoreColumn]
        public IEnumerable<SupplyOrderAttachment> SupplyOrderAttachments
        {
            get
            {
                var ctl = new AttachmentController();
                return ctl.GetSupplyAttachmentsByOrderId(OrderID);
            }
        }
    }
}