using DotNetNuke.Abstractions.Application;
using System;
using System.Collections.Generic;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

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
                // This is a PetaPoco entity mapped by reflection (needs a parameterless constructor),
                // so IHostSettings can't be constructor-injected here the way the controllers get it.
                var hostSettings = System.Web.HttpContext.Current.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();
                var ctl = new SupplyOrderController(hostSettings);
                return ctl.GetSupplyOrderItemsByOrder(OrderID);
            }
        }
        [IgnoreColumn]
        public IEnumerable<SupplyOrderAttachment> SupplyOrderAttachments
        {
            get
            {
                var hostSettings = System.Web.HttpContext.Current.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();
                var ctl = new AttachmentController(hostSettings);
                return ctl.GetSupplyAttachmentsByOrderId(OrderID);
            }
        }
    }
}