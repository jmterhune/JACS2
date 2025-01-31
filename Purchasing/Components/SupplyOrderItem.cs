using System;
using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.Purchasing.Components
{
    [TableName("tjc_purchasing_supply_order_items")]
    //setup the primary key for table
    [PrimaryKey("SupplyID", AutoIncrement = true)]
    internal class SupplyOrderItem
    {
        public int SupplyID { get; set; }
        public string ItemNumber { get; set; }
        public int OrderID { get; set; }
        public int Quantity { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public string Store { get; set; }
        public string Link { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Recipient { get; set; }
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