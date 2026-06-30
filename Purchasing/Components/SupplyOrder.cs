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
        public int OrderID { get; set; } // int
        public string Location { get; set; } // nvarchar(150)
        public string RequestedName { get; set; } // nvarchar(100)
        public string EmailAddress { get; set; } // nvarchar(250)
        public DateTime DateRequested { get; set; } // datetime
        public DateTime? CompletedDate { get; set; } // datetime
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