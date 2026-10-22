using DotNetNuke.Abstractions.Application;
using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using DotNetNuke.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace tjc.Modules.Purchasing.Components
{
    [TableName("tjc_purchasing_stamp_orders")]
    //setup the primary key for table
    [PrimaryKey("OrderID", AutoIncrement = true)]
    internal class StampOrder
    {
        public int OrderID { get; set; } // int
        public DateTime DateCreated { get; set; } // datetime
        public string Location { get; set; } // nvarchar(50)
        public string RequestedName { get; set; } // nvarchar(100)
        public string ConsumerName { get; set; } // nvarchar(100)
        public string Phone { get; set; } // nvarchar(20)
        public string StampType { get; set; } // nvarchar(20)
        public string Sample { get; set; } // nvarchar(max)
        public string FontStyle { get; set; } // nvarchar(20)
        public string FontSize { get; set; } // nvarchar(20)
        public string InkColor { get; set; } // nvarchar(20)
        public int Quantity { get; set; } // int
        public string Instructions { get; set; } // nvarchar(max)
        public DateTime? CompletedDate { get; set; } // datetime
        public OrderStatus Status { get; set; } // int
        public string EmailAddress { get; set; } // nvarchar(250)
        [IgnoreColumn]
        public IEnumerable<StampOrderAttachment> StampOrderAttachments
        {
            get
            {
                // This is a PetaPoco entity mapped by reflection (needs a parameterless constructor),
                // so IHostSettings can't be constructor-injected here the way the controllers get it.
                var hostSettings = System.Web.HttpContext.Current.GetScope().ServiceProvider.GetRequiredService<IHostSettings>();
                var ctl = new AttachmentController(hostSettings);
                return ctl.GetStampAttachmentsByOrderId(OrderID);
            }
        }
    }
    public enum OrderStatus
    {
        @new = 0,
        rejected = -1,
        accepted = 1,
        completed = 2
    }
}