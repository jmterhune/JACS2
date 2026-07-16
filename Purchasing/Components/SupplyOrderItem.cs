using System;
using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.Purchasing.Components
{
    [TableName("tjc_purchasing_supply_order_items")]
    //setup the primary key for table
    [PrimaryKey("SupplyID", AutoIncrement = true)]
    internal class SupplyOrderItem
    {
        public int SupplyID { get; set; } // int
        public string ItemNumber { get; set; } // nvarchar(200)
        public int OrderID { get; set; } // int
        public int Quantity { get; set; } // int
        public string Comments { get; set; } // nvarchar(max)
        public DateTime CreatedDate { get; set; } // datetime
        public string Description { get; set; } // nvarchar(2000)
        public string Store { get; set; } // nvarchar(250)
        public string Link { get; set; } // nvarchar(max)
        public string UnitOfMeasure { get; set; } // nvarchar(50)
        public string Recipient { get; set; } // nvarchar(100)
        [IgnoreColumn]
        public string LinkedDescription
        {
            get
            {
                if (Store != "Office Depot")
                {
                    if (string.IsNullOrEmpty(Link))
                    {
                        return Description;
                    }
                    return string.Format("<a target='_blank'  title='Opens in new tab' class='item-link' href='{0}'>{1}</a>", Link, Description);
                }
                return Description;
            }
        }
        [IgnoreColumn]
        public string ToolTipComment
        {
            get
            {
                string commentTooltip=string.Empty;
                if (!string.IsNullOrEmpty(Comments)) {
                    commentTooltip=string.Format("<i class=\"fas fa-comment-alt\" data-html=\"true\" title='{0}' data-toggle=\"tooltip\"></i>",Comments);
                }

                return commentTooltip;
            }
        }

    }
}