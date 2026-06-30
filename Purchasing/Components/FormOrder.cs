using System;
using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.Purchasing.Components
{
    [TableName("tjc_purchasing_form_orders")]
    //setup the primary key for table
    [PrimaryKey("OrderID", AutoIncrement = true)]
    internal class FormOrder
    {
        public int OrderID{ get; set; } // int
        public string Location { get; set; } // nvarchar(150)
        public string RequestedName { get; set; } // nvarchar(100)
        public DateTime DateRequested { get; set; } // datetime
        public DateTime? CompletedDate { get; set; } // datetime
        public string EmailAddress { get; set; } // nvarchar(250)
       
    }

}