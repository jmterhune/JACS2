using System;
using DotNetNuke.ComponentModel.DataAnnotations;
namespace tjc.Modules.Purchasing.Components
{
    [TableName("tjc_purchasing_form_order_items")]
    //setup the primary key for table
    [PrimaryKey("FormID", AutoIncrement = true)]
    public class FormOrderItem
    {
        public int FormID { get; set; } // int
        public string FormNumber { get; set; } // nvarchar(200)
        public string FormName { get; set; } // nvarchar(200)
        public int OrderID { get; set; } // int
        public int Quantity { get; set; } // int
        public string Comments { get; set; } // nvarchar(max)
        public DateTime CreatedDate { get; set; } // datetime
        public string Description { get; set; } // nvarchar(2000)
        public string Recipient { get; set; } // nvarchar(100)
        public int NumberSets { get; set; } // int
        public int NumberParts { get; set; } // int
        public string PageType { get; set; } // nvarchar(20)
    }
}