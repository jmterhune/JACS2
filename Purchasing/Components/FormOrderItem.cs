using System;
using DotNetNuke.ComponentModel.DataAnnotations;
namespace tjc.Modules.Purchasing.Components
{
    [TableName("tjc_purchasing_form_order_items")]
    //setup the primary key for table
    [PrimaryKey("FormID", AutoIncrement = true)]
    public class FormOrderItem
    {
        public int FormID { get; set; }
        public string FormNumber { get; set; }
        public string FormName { get; set; }
        public int OrderID { get; set; }
        public int Quantity { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public string Recipient { get; set; }
        public int NumberSets { get; set; }
        public int NumberParts { get; set; }
        public string PageType { get; set; }
    }
}